using Microsoft.EntityFrameworkCore;
using tl121pet.DAL.Data;
using tl121pet.DAL.Interfaces;
using tl121pet.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DataContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("TeamLead_Db"), o => o.MigrationsAssembly("tl121pet")));
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IPeopleRepository, PeopleRepository>();
builder.Services.AddScoped<IProjectTeamRepository, ProjectTeamRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();

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
