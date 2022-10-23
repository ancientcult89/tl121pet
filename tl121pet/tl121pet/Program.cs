using Microsoft.EntityFrameworkCore;
using tl121pet.DAL;
using tl121pet.DAL.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("TeamLead_Db"), x => x.MigrationsAssembly("tl121pet")));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(context);

app.Run();
