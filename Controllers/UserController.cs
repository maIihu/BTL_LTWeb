using Microsoft.AspNetCore.Mvc;
using web1.Models;
using web1.Helpers;
using System.Diagnostics;

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

        // GET: Display registration form
        public IActionResult DangKy()
        {
            return View();
        }

        // POST: Handle registration form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKy(Khachhang khachhang)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tài khoản đã tồn tại hay chưa
                var existingUser = _db.Khachhangs.FirstOrDefault(kh => kh.TaiKhoanKh == khachhang.TaiKhoanKh);
                if (existingUser != null)
                {
                    ModelState.AddModelError("TaiKhoanKh", "Tên đăng nhập đã tồn tại.");
                    _logger.LogWarning("Tài khoản {TaiKhoanKh} đã tồn tại.", khachhang.TaiKhoanKh);
                    return View(khachhang);
                }

                _db.Khachhangs.Add(khachhang);
                _db.SaveChanges();
                _logger.LogInformation("Đăng ký thành công cho tài khoản {TaiKhoanKh}.", khachhang.TaiKhoanKh);

                return RedirectToAction("DangNhap");
            }

            // Trả về view với model nếu có lỗi
            _logger.LogWarning("Lỗi khi đăng ký: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return View(khachhang);
        }

        // GET: Display login form
        public IActionResult DangNhap()
        {
            return View();
        }

        // POST: Handle login form submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhap(Khachhang model)
        {
            _logger.LogInformation("Received login request for account: {TaiKhoanKh}", model.TaiKhoanKh);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState errors during login: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return View(model);
            }

            var user = _db.Khachhangs
                .FirstOrDefault(u => u.TaiKhoanKh == model.TaiKhoanKh && u.MatKhau == model.MatKhau);

            if (user != null)
            {
                // Lưu thông tin người dùng vào session
                HttpContext.Session.SetString("UserID", user.MaKh.ToString());
                _logger.LogInformation("Đăng nhập thành công cho tài khoản {TaiKhoanKh}, chuyển hướng tới trang chủ.", model.TaiKhoanKh);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Thêm thông báo lỗi nếu đăng nhập không thành công
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                _logger.LogWarning("Đăng nhập thất bại cho tài khoản {TaiKhoanKh}: Tài khoản hoặc mật khẩu không đúng.", model.TaiKhoanKh);
            }

            // Trả về view với model nếu không thành công
            return View(model);
        }
    }
}
