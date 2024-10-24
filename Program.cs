using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using web1.Helpers;
using web1.Models;
using web1.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailSender, EmailSender>();

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
// Cấu hình dịch vụ authentication với cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = "/User/DangNhap";  // Đường dẫn đến trang đăng nhập
    options.LogoutPath = "/User/DangXuat";   // Đường dẫn để đăng xuất
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

app.UseAuthentication();
app.UseAuthorization();




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
