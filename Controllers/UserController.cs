using Microsoft.AspNetCore.Mvc;
using web1.Models;
using web1.Helpers;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

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
                return View(khachhang);
            }

            var existingUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == khachhang.TaiKhoanKh);
            if (existingUser != null)
            {
                ModelState.AddModelError("TaiKhoanKh", "Tên đăng nhập đã tồn tại.");
                return View(khachhang);
            }

            _db.Khachhangs.Add(khachhang);
            _db.SaveChanges();

            return RedirectToAction("DangNhap", "User"); 
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
            if (taikhoan.Contains("admin")) 
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, taikhoan),
                    new Claim("IsAdmin", "true") 
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
            }
            else
            {
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
                    return RedirectToAction("Index", "Home"); 
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }

            return View();
        }


        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("DangNhap", "User");
        }

        public IActionResult HoSo()
        {
            if (User.Identity.IsAuthenticated)
            {
                string taiKhoan = User.Identity.Name; 
                var khachhang = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == taiKhoan);

                if (khachhang != null)
                {
                    return View(khachhang);
                }
            }

            ViewBag.ErrorMessage = "Không tìm thấy thông tin khách hàng.";
            return View(null); 
        }

        public IActionResult DoiMatKhau()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DoiMatKhau(string matKhauCu, string matKhauMoi, string xacNhanMatKhauMoi)
        {
            if (User.Identity.IsAuthenticated)
            {
                string taiKhoan = User.Identity.Name;
                var khachhang = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == taiKhoan);

                if (khachhang != null)
                {
                    if (khachhang.MatKhau == matKhauCu)
                    {
                        if (matKhauMoi == xacNhanMatKhauMoi)
                        {
                            khachhang.MatKhau = matKhauMoi;
                            _db.SaveChanges();

                            ViewBag.SuccessMessage = "Đổi mật khẩu thành công.";
                        }
                        else
                        {
                            ModelState.AddModelError("", "Mật khẩu mới không khớp.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mật khẩu cũ không chính xác.");
                    }
                }
            }

            return View();
        }

        public IActionResult QuenMatKhau() {
            return View();
        }

    }
}
