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
using tl121pet.Middlwares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("TeamLead_Db"), o => o.MigrationsAssembly("tl121pet")));

//auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(opts => { opts.LoginPath = "/auth/Login"; })
    .AddJwtBearer(opts => {
        opts.RequireHttpsMetadata = false;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
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
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseCors(x => x.AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true) // allow any origin
    .WithOrigins(builder.Configuration.GetSection("CorsAllowedHosts").ToString())); // Allow only this origin can also have multiple origins separated with comma
    //.AllowCredentials());
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
//app.UseStatusCodePages(async context =>
//{
//    var response = context.HttpContext.Response;
//    var request = context.HttpContext.Request;
//    bool isAnauthorized = response.StatusCode == (int)HttpStatusCode.Unauthorized;
//    bool isForbidden = response.StatusCode == (int)HttpStatusCode.Forbidden;

//    //дл€ что бы новый фронт не получал ответ в виде разметки со страниццей запрета доступа, 
//    //необходимо проверить Headers.Origin. ¬ случае внешнего фронтенда там будет заполнен хост,
//    //в случае MVC - будет пустота
//    //временное решение, пока не будет полностью выпелен старый фронтенд
//    bool isMVC = context.HttpContext.Request.Headers.Origin.ToString() == "";

//    if ((isAnauthorized || isForbidden) && isMVC)
//        response.Redirect("/auth/AccessDenied");
//});
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
