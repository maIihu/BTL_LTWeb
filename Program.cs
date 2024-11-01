using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using web1.Helpers;
using web1.Models;
using web1.Repository;

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ vào container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailSender, EmailSender>();

// Cấu hình logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Hoặc thêm các logger khác theo nhu cầu

// Lấy chuỗi kết nối từ cấu hình
var connectionString = builder.Configuration.GetConnectionString("ShopGiayContext");
builder.Services.AddDbContext<ShopGiayContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddScoped<IThuongHieuRepository, ThuongHieuRepository>();

// Cấu hình cache
builder.Services.AddDistributedMemoryCache();

// Cấu hình dịch vụ session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Cấu hình dịch vụ authentication với cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = "/User/DangNhap";  // Đường dẫn đến trang đăng nhập
    options.LogoutPath = "/User/DangXuat";   // Đường dẫn để đăng xuất
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Cấu hình pipeline cho HTTP request.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Giá trị HSTS mặc định là 30 ngày. Bạn có thể thay đổi điều này cho các tình huống sản xuất.
    app.UseHsts();
}

// Đặt middleware theo thứ tự thích hợp
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Kích hoạt session
app.UseSession();

// Kích hoạt xác thực và cấp quyền
app.UseAuthentication();
app.UseAuthorization();

// Định nghĩa route cho controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
