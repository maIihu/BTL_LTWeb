using Microsoft.AspNetCore.Mvc;
using web1.Models;
using web1.Helpers;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace web1.Controllers
{
    public class UserController : Controller
    {
        private readonly ShopGiayContext _db;
        private readonly ILogger<UserController> _logger;

        public UserController(ShopGiayContext db, ILogger<UserController> logger)
        {
            _db = db;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangKy(Khachhang khachhang)
        {
            if (!ModelState.IsValid)
            {
                return View(khachhang); // Trả về view nếu dữ liệu không hợp lệ
            }

            // Kiểm tra xem tên đăng nhập đã tồn tại chưa
            var existingUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == khachhang.TaiKhoanKh);
            if (existingUser != null)
            {
                ModelState.AddModelError("TaiKhoanKh", "Tên đăng nhập đã tồn tại.");
                return View(khachhang);
            }

            // Thêm người dùng mới vào database
            _db.Khachhangs.Add(khachhang);
            _db.SaveChanges();

            return RedirectToAction("DangNhap", "User"); // Chuyển hướng đến trang đăng nhập
        }

        public IActionResult DangNhap()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(string taikhoan, string matkhau)
        {
            // Kiểm tra xem tài khoản có chứa "admin" không
            if (taikhoan.Contains("admin")) // Thay "admin" nếu cần
            {
                // Tạo danh sách claim cho admin
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, taikhoan),
                    new Claim("IsAdmin", "true") // Thêm claim để xác định là admin
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" }); // Chuyển hướng đến trang Admin
            }
            else
            {
                // Kiểm tra tên đăng nhập và mật khẩu cho người dùng thông thường
                var user = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == taikhoan && u.MatKhau == matkhau);

                if (user != null)
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.TaiKhoanKh)
            };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    return RedirectToAction("Index", "Home"); // Chuyển hướng đến trang người dùng
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }

            return View(); // Trả về view nếu có lỗi
        }


        public async Task<IActionResult> Logout()
        {
            // Đăng xuất và xóa cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("DangNhap", "User");
        }

    }
}
