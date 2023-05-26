using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaWeb_PortalTrabajo.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<portalTrabajoDbContext>(opt =>
        opt.UseSqlServer(
            builder.Configuration.GetConnectionString("portalTrabajoDbConnection"))
        );



var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
