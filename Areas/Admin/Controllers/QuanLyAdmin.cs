using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web1.Models;

namespace web1.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/quanlyadmin")]
    public class QuanLyAdmin : Controller
    {
        private readonly ShopGiayContext _db;
        public QuanLyAdmin(ShopGiayContext db)
        {
            _db = db;
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

        #region XoaDonHang
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
            _db.CtDonhangs.RemoveRange(donHang.CtDonhangs);
            _db.Donhangs.Remove(donHang);
            _db.SaveChanges();
            TempData["SuccessMessage"] = "Đơn hàng đã được xóa thành công.";
            return RedirectToAction("DonHang");
        }
        #endregion
    }
}
