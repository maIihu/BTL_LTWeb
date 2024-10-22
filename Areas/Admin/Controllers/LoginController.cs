using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web1.Models;

namespace web1.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/login")]
    public class LoginController : Controller
    {
        ShopGiayContext _db = new ShopGiayContext();

        [Route("")]
        [Route("dangnhap")]
        public IActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        [Route("dangnhap")]
        public IActionResult DangNhap(string taiKhoan, string matKhau)
        {
            var quanLy = _db.Quanlies
                .FirstOrDefault(q => q.TaiKhoanQl == taiKhoan && q.MatKhau == matKhau);

            if (quanLy != null)
            {
                // Lưu mã quản lý vào cookie (giả sử thời hạn là 30 phút)
                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),
                    HttpOnly = true // Đảm bảo cookie chỉ truy cập được qua HTTP
                };
                HttpContext.Response.Cookies.Append("MaQl", quanLy.MaQl.ToString(), options);

                // Chuyển hướng đến HomeAdminController trong Admin area
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
            }

            // Trả về thông báo lỗi nếu đăng nhập thất bại
            ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng.";
            return View();
        }
    }
}
