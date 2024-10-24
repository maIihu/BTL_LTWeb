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
        private readonly IEmailSender _emailSender;
        public UserController(ShopGiayContext db, ILogger<UserController> logger, IEmailSender emailSender)
        {
            _db = db;
            _logger = logger;
            _emailSender = emailSender;
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


        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            // Lấy email từ session nếu có
            var email = HttpContext.Session.GetString("Email");
            ViewBag.Email = email; // Truyền email vào ViewBag

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(string email, string verificationCode, bool sendCode)
        {
            if (sendCode) // Nếu người dùng nhấn nút "Gửi mã xác nhận"
            {
                if (string.IsNullOrEmpty(email))
                {
                    ModelState.AddModelError("", "Email không được để trống.");
                    return View();
                }

                var generatedCode = new Random().Next(100000, 999999).ToString();
                HttpContext.Session.SetString("VerificationCode", generatedCode);
                HttpContext.Session.SetString("Email", email);
                HttpContext.Session.SetString("CodeCreationTime", DateTime.UtcNow.ToString()); // Lưu thời gian tạo mã

                var subject = "Mã xác nhận của bạn";
                var message = $"Mã xác nhận của bạn là {generatedCode}";

                await _emailSender.SendEmailAsync(email, subject, message);
                ViewBag.Message = "Mã xác nhận đã được gửi đến email.";
            }
            else // Nếu người dùng nhấn nút "Xác nhận mã"
            {
                var savedCode = HttpContext.Session.GetString("VerificationCode");
                var codeCreationTime = HttpContext.Session.GetString("CodeCreationTime");

                if (savedCode == null || codeCreationTime == null)
                {
                    ViewBag.Message = "Mã xác nhận đã hết hiệu lực.";
                    return View();
                }

                var creationTime = DateTime.Parse(codeCreationTime);
                if (DateTime.UtcNow > creationTime.AddMinutes(1)) 
                {
                    ViewBag.Message = "Mã xác nhận đã hết hiệu lực.";
                    return View();
                }

                if (savedCode == verificationCode)
                {
                    return RedirectToAction("XacNhanMatKhau"); // Điều hướng tới trang nhập mật khẩu mới
                }
                else
                {
                    ViewBag.Message = "Mã xác nhận không đúng.";
                }
            }

            ViewBag.Email = email; // Đảm bảo truyền lại email vào ViewBag khi post dữ liệu
            return View();
        }



        [HttpGet]
        public IActionResult XacNhanMatKhau()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> XacNhanMatKhau(string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
                return View();
            }

            var email = HttpContext.Session.GetString("Email");
            if (email == null)
            {
                return RedirectToAction("QuenMatKhau"); // Điều hướng lại nếu không có email trong session
            }

            var user = await _db.Khachhangs.FirstOrDefaultAsync(u => u.EmailKh == email);
            if (user != null)
            {
                user.MatKhau = newPassword;
                await _db.SaveChangesAsync();
                return RedirectToAction("DangNhap", "User");
            }

            ModelState.AddModelError("", "Không thể đổi mật khẩu.");
            return View();
        }


    }
}
