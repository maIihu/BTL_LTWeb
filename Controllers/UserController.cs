using Microsoft.AspNetCore.Mvc;
using web1.Models;
using web1.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using web1.Enums;

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
        #region DangKy
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
        #endregion

        #region DangNhap
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
            var adminAccount = _db.Quanlies.SingleOrDefault(a=>a.TaiKhoanQl == taikhoan && a.MatKhau == matkhau);
            var userAccount = _db.Khachhangs.SingleOrDefault(a => a.TaiKhoanKh == taikhoan && a.MatKhau == matkhau);

            if (adminAccount != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, adminAccount.TaiKhoanQl),
                    new Claim("UserType", UserType.Admin.ToString())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return Json(new { success = true, redirectUrl = Url.Action("Index", "HomeAdmin", new { area = "Admin" }) });
            }
            else if(userAccount != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userAccount.TaiKhoanKh),
                    new Claim("UserType", UserType.Customer.ToString())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                }
            else
            {
                return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không đúng." });
            }


        }


        #endregion

        #region DangXuat
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("DangNhap", "User");
        }
        #endregion

        #region HoSo
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
        [HttpPost]
        public IActionResult HoSo(Khachhang updatedKhachhang)
        {
            if (User.Identity.IsAuthenticated)
            {
                string taiKhoan = User.Identity.Name;
                var khachhang = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == taiKhoan);

                if (khachhang != null)
                {
                    // Update only the editable fields
                    khachhang.HoTen = updatedKhachhang.HoTen;
                    khachhang.DiaChiKh = updatedKhachhang.DiaChiKh;
                    khachhang.DienThoaiKh = updatedKhachhang.DienThoaiKh;
                    khachhang.NgaySinh = updatedKhachhang.NgaySinh;

                    _db.SaveChanges();

                    // Return JSON success response
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        #endregion

        #region DoiMatKhau
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
        #endregion

        #region QuenMatKhau
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
        #endregion

        #region DonHang
        public IActionResult DonHang()
        {
            if (User.Identity.IsAuthenticated)
            {
                string tk = User.Identity.Name;
                var kh = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == tk);
                if (kh != null) { 
                    var dsDonHang = _db.Donhangs.Where(dh => dh.MaKh == kh.MaKh).Include(dh => dh.CtDonhangs).ToList();
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return PartialView("_DonHangPartial", dsDonHang);
                    }
                    return View(dsDonHang);
                }
                
            }
            return RedirectToAction("DangNhap");
        }
        public IActionResult ChiTietDonHang(int id)
        {
            var donHang = _db.Donhangs.Include(dh => dh.CtDonhangs).ThenInclude(ct => ct.MaGiayNavigation)
                             .FirstOrDefault(dh => dh.MaDonHang == id);
            if (donHang == null)
            {
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ChiTietDonHangPartial", donHang);
            }

            return View(donHang);
        }

        #endregion
    }
}
