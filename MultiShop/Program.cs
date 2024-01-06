using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.UseRouting();
app.UseStaticFiles();


app.MapControllerRoute(
    "default",
    "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
