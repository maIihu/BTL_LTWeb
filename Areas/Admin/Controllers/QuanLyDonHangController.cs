using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using web1.Models;

namespace web1.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/quanlydonhang")]
    public class QuanLyDonHang : Controller
    {
        private readonly ShopGiayContext _db;
        private readonly ILogger<QuanLyDonHang> _logger;
        public QuanLyDonHang(ShopGiayContext db, ILogger<QuanLyDonHang> logger)
        {
            _db = db;
            _logger = logger;
        }

        #region DonHang
        [Route("donhang")]
        public IActionResult DonHang()
        {
            var donHangs = _db.Donhangs
                .Include(dh => dh.CtDonhangs)
                .Include(dh => dh.MaKhNavigation) 
                .ToList();

            return View(donHangs);
        }

        [HttpPost]
        [Route("xoadonhang/{id}")]
        public IActionResult XoaDonHang(int id)
        {
            var donHang = _db.Donhangs.Include(dh => dh.CtDonhangs)
                                       .FirstOrDefault(dh => dh.MaDonHang == id);
            if (donHang == null)
            {
                return NotFound();
            }
            _db.CtDonhangs.RemoveRange(donHang.CtDonhangs);
            _db.Donhangs.Remove(donHang);
            _db.SaveChanges();
            TempData["SuccessMessage"] = "Đơn hàng đã được xóa thành công.";
            return RedirectToAction("DonHang");
        }
        #endregion

        #region ChiTietDonHang
        [Route("chi-tiet-don-hang/{id}")]
        public IActionResult ChiTietDonHang(int id)
        {
            var donHang = _db.Donhangs
                        .Include(dh => dh.CtDonhangs)
                        .ThenInclude(ct => ct.MaGiayNavigation) 
                        .Include(dh => dh.MaKhNavigation)
                        .FirstOrDefault(dh => dh.MaDonHang == id);
            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }
        #endregion

    }
}
