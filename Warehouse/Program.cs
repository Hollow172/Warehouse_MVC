using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Warehouse.Models;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WarehouseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("WarehouseContext") ?? throw new InvalidOperationException("Connection string 'WarehouseContext' not found.")));


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Access/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddSingleton(singleton => SingletonCurrentlyLoggedInUser.GetInstance());
builder.Services.AddSingleton(singletonSummarySum => SingletonSum.GetInstance());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var singletonSum = services.GetRequiredService<SingletonSum>();
    var context = services.GetRequiredService<WarehouseContext>();
    singletonSum.setSums(context);
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Access}/{action=Login}/{id?}");

app.Run();
