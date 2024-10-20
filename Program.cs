using Microsoft.EntityFrameworkCore;
using web1.Models;
using web1.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Hoặc thêm các logger khác theo nhu cầu

var connectionString = builder.Configuration.GetConnectionString("ShopGiayContext");
builder.Services.AddDbContext<ShopGiayContext>(x=>x.UseSqlServer(connectionString));
builder.Services.AddScoped<IThuongHieuRepository, ThuongHieuRepository>();

builder.Services.AddDistributedMemoryCache();
// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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

app.UseSession();

app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
