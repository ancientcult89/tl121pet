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
using System.Net;
using tl121pet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("TeamLead_Db"), o => o.MigrationsAssembly("tl121pet")));

//auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts => {
    opts.RequireHttpsMetadata = false;
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.ConfigureApplicationCookie(opts => {
    opts.LoginPath = "/Auth/Login";
});


builder.Services.AddControllersWithViews().AddDataAnnotationsLocalization(opts => {
    opts.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedListResource));
    opts.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedEditFormResource));
});
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

builder.Services.AddScoped<IOneToOneService, OneToOneService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddSingleton<IAutomapperMini, AutomapperMini>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.UseRouting();
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
app.UseStatusCodePages(async context => { 
    var responce = context.HttpContext.Response;
    if (responce.StatusCode == (int)HttpStatusCode.Unauthorized || responce.StatusCode == (int)HttpStatusCode.Forbidden)
        responce.Redirect("/auth/AccessDenied");
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
SeedData.SeedDatabase(context);


app.Run();
