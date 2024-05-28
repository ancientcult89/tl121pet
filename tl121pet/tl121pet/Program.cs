using Microsoft.EntityFrameworkCore;
using tl121pet.Services.Interfaces;
using tl121pet.Services.Services;
using tl121pet.DAL.Data;
using tl121pet.Entities.Infrastructure;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using tl121pet.Middlwares;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
bool useInMemoryFlag = false;
if (builder.Configuration.GetSection("AppSettings:UseInMemory").Value != "false")
{
    builder.Services.AddDbContext<DataContext>(o => o.UseInMemoryDatabase("testDB"));
    useInMemoryFlag = true;
}
else
{ 
    builder.Services.AddDbContext<DataContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("TeamLead_Db"), o => o.MigrationsAssembly("tl121pet")));
}

//auth
string secret = builder.Configuration.GetSection("AppSettings:TokenSecret").Value;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(opts => { opts.LoginPath = "/auth/Login"; })
    .AddJwtBearer(opts => {
        opts.RequireHttpsMetadata = false;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
        };
    });


builder.Services.AddControllers();
builder.Services.AddSession();

builder.Services.AddSwaggerGen(opts => {
    opts.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standart Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    opts.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddLocalization(opts =>  opts.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options => {
    List<CultureInfo> supportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
        new CultureInfo("ru-RU")
    };
    options.DefaultRequestCulture = new RequestCulture("ru-RU");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);
builder.Services.AddHttpClient();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddScoped<IOneToOneApplication, OneToOneApplication>();
builder.Services.AddTransient<ITlMailService, TlMailService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddSingleton(secret);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseCors(x => x.AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin());

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.UseRouting();
app.UseMiddleware<ExceptionHandlingMiddleware>();
var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options.Value);

#region auth
app.UseSession();
app.Use(async (context, next) =>
{
    var token = context.Session.GetString("Token");
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers.Add("Authorization", "Bearer " + token);
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
#endregion auth

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
var authService = app.Services.CreateScope().ServiceProvider.GetRequiredService<IAuthService>();
CreatePasswordDelegate createPasswordDelegate = new CreatePasswordDelegate(authService.CreatePasswordHash);
SeedData.SeedDatabase(context, useInMemoryFlag, createPasswordDelegate);

app.Run();
