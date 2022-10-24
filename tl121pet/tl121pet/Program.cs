using Microsoft.EntityFrameworkCore;
using tl121pet.Data;
using tl121pet.Services.Interfaces;
using tl121pet.Services.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("TeamLead_Db")));
builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<IDataRepository, EFDataRepository>();
//builder.Services.AddScoped<IPeopleService, PeopleService>();
//builder.Services.AddScoped<IProjectTeamService, ProjectTeamService>();
//builder.Services.AddScoped<IGradeService, GradeService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(context);

app.Run();
