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
    [Route("api/[controller]")]
    [ApiController]
    public class UserControllerAPI : ControllerBase
    {
        private readonly ShopGiayContext _db;
        private readonly ILogger<UserControllerAPI> _logger;
        private readonly IEmailSender _emailSender;

        public UserControllerAPI(ShopGiayContext db, ILogger<UserControllerAPI> logger, IEmailSender emailSender)
        {
            _db = db;
            _logger = logger;
            _emailSender = emailSender;
        }

        #region Đăng ký người dùng
        [HttpPost("register")]
        public IActionResult Register([FromBody] Khachhang khachhang)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == khachhang.TaiKhoanKh);
            if (existingUser != null)
            {
                return Conflict("Tên đăng nhập đã tồn tại.");
            }

            _db.Khachhangs.Add(khachhang);
            _db.SaveChanges();

            return CreatedAtAction(nameof(Login), new { taikhoan = khachhang.TaiKhoanKh }, khachhang);
        }
        #endregion

        #region Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var adminAccount = _db.Quanlies.SingleOrDefault(a => a.TaiKhoanQl == request.TaiKhoan && a.MatKhau == request.MatKhau);
            var userAccount = _db.Khachhangs.SingleOrDefault(a => a.TaiKhoanKh == request.TaiKhoan && a.MatKhau == request.MatKhau);

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

                return Ok(new { success = true, redirectUrl = Url.Action("Index", "HomeAdmin", new { area = "Admin" }) });
            }
            else if (userAccount != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userAccount.TaiKhoanKh),
                    new Claim("UserType", UserType.Customer.ToString())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Ok(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            }
            else
            {
                return Unauthorized("Tên đăng nhập hoặc mật khẩu không đúng.");
            }
        }
        #endregion

        #region Đăng xuất
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { success = true });
        }
        #endregion

        #region Lấy thông tin người dùng
        [HttpGet("userinfo")]
        public IActionResult GetUserInfo()
        {
            if (User.Identity.IsAuthenticated)
            {
                string taiKhoan = User.Identity.Name;
                var khachhang = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == taiKhoan);

                if (khachhang != null)
                {
                    return Ok(new
                    {
                        hoTen = khachhang.HoTen,
                        email = khachhang.EmailKh,
                        soDienThoai = khachhang.DienThoaiKh,
                        diaChi = khachhang.DiaChiKh,
                    });
                }
            }

            return NotFound(); // Trả về 404 nếu không tìm thấy thông tin người dùng
        }
        #endregion

        #region Đổi mật khẩu
        [HttpPut("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("Bạn cần đăng nhập để thực hiện hành động này.");
            }

            string taiKhoan = User.Identity.Name;
            var khachhang = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == taiKhoan);

            if (khachhang == null || khachhang.MatKhau != request.MatKhauCu)
            {
                return BadRequest("Mật khẩu cũ không chính xác.");
            }

            if (request.MatKhauMoi != request.XacNhanMatKhauMoi)
            {
                return BadRequest("Mật khẩu mới không khớp.");
            }

            khachhang.MatKhau = request.MatKhauMoi;
            _db.SaveChanges();

            return Ok(new { success = true, message = "Đổi mật khẩu thành công." });
        }
        #endregion
    }

    public class LoginRequest
    {
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string MatKhauCu { get; set; }
        public string MatKhauMoi { get; set; }
        public string XacNhanMatKhauMoi { get; set; }
    }
}
