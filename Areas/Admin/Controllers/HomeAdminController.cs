using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web1.Models;

namespace web1.Areas.Admin.Controllers
{
    //[Authorize] // đảm bảo đăng nhập mới vào được
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        private readonly ShopGiayContext _db;

        public HomeAdminController(ShopGiayContext db)
        {
            _db = db;
        }
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("hoso")]
        public IActionResult HoSo()
        {
            if (User.Identity.IsAuthenticated && User.HasClaim("IsAdmin", "true"))
            {
                string adminAccount = User.Identity.Name;
                var admin = _db.Quanlies.FirstOrDefault(a => a.TaiKhoanQl == adminAccount);

                if (admin != null)
                {
                    return View(admin);
                }
            }

            ViewBag.ErrorMessage = "Không tìm thấy thông tin admin.";
            return View(null);
        }



        [HttpPost]
        [Route("xoa-don-hang/{id}")]
        public IActionResult XoaDonHang(int id)
        {
            var donHang = _db.Donhangs.Include(dh => dh.CtDonhangs)
                                       .FirstOrDefault(dh => dh.MaDonHang == id);
            if (donHang == null)
            {
                return NotFound();
            }

            // Xóa chi tiết đơn hàng trước
            _db.CtDonhangs.RemoveRange(donHang.CtDonhangs);

            // Sau đó xóa đơn hàng
            _db.Donhangs.Remove(donHang);
            _db.SaveChanges();

            // Chuyển hướng về danh sách đơn hàng với thông báo
            TempData["SuccessMessage"] = "Đơn hàng đã được xóa thành công.";
            return RedirectToAction("DonHang");
        }

    }
}
