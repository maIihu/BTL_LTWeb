using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using web1.Models;
using System.Linq;

namespace web1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/QuanLyDoanhThu")]
    public class QuanLyDoanhThuController : Controller
    {
        private readonly ShopGiayContext _db;

        public QuanLyDoanhThuController(ShopGiayContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var doanhThu = await _db.Donhangs
                .Where(dh => dh.TinhTrangGiaoHang == true)
                .SumAsync(dh => dh.TongTien ?? 0);

            ViewBag.TongDoanhThu = doanhThu;
            return View();
        }

        // Lấy doanh thu theo ngày
        [HttpGet("DoanhThuTheoNgay/{ngay}/{thang}/{nam}")]
        public async Task<IActionResult> DoanhThuTheoNgay(int ngay, int thang, int nam)
        {
            var doanhThu = await _db.Donhangs
                .Where(dh => dh.NgayDat.Value.Day == ngay &&
                             dh.NgayDat.Value.Month == thang &&
                             dh.NgayDat.Value.Year == nam &&
                             dh.TinhTrangGiaoHang == true)
                .SumAsync(dh => dh.TongTien ?? 0);

            return Json(doanhThu);
        }

        // Lấy doanh thu theo tháng (biểu đồ cho từng ngày trong tháng)
        [HttpGet("DoanhThuTheoThang/{thang}/{nam}")]
        public async Task<IActionResult> DoanhThuTheoThang(int thang, int nam)
        {
            var daysInMonth = DateTime.DaysInMonth(nam, thang);
            var doanhThuTheoNgay = new List<decimal>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var doanhThu = await _db.Donhangs
                    .Where(dh => dh.NgayDat.Value.Day == day &&
                                 dh.NgayDat.Value.Month == thang &&
                                 dh.NgayDat.Value.Year == nam &&
                                 dh.TinhTrangGiaoHang == true)
                    .SumAsync(dh => dh.TongTien ?? 0);
                doanhThuTheoNgay.Add(doanhThu);
            }

            return Json(doanhThuTheoNgay);
        }

        // Lấy doanh thu theo năm (biểu đồ cho từng tháng trong năm)
        [HttpGet("DoanhThuTheoNam/{nam}")]
        public async Task<IActionResult> DoanhThuTheoNam(int nam)
        {
            var doanhThuTheoThang = new List<decimal>();

            for (int month = 1; month <= 12; month++)
            {
                var doanhThu = await _db.Donhangs
                    .Where(dh => dh.NgayDat.Value.Month == month &&
                                 dh.NgayDat.Value.Year == nam &&
                                 dh.TinhTrangGiaoHang == true)
                    .SumAsync(dh => dh.TongTien ?? 0);
                doanhThuTheoThang.Add(doanhThu);
            }

            return Json(doanhThuTheoThang);
        }

        // Lấy chi tiết đơn hàng theo ngày
        [HttpGet("ChiTietDonHangNgay/{ngay}/{thang}/{nam}")]
        public async Task<IActionResult> ChiTietDonHangNgay(int ngay, int thang, int nam)
        {
            var donHangs = await _db.Donhangs
                .Include(dh => dh.CtDonhangs)
                .Where(dh => dh.NgayDat.Value.Day == ngay &&
                             dh.NgayDat.Value.Month == thang &&
                             dh.NgayDat.Value.Year == nam &&
                             dh.TinhTrangGiaoHang == true)
                .ToListAsync();

            return PartialView("_ChiTietDonHang", donHangs);
        }

        // Lấy chi tiết đơn hàng theo tháng
        [HttpGet("ChiTietDonHangThang/{thang}/{nam}")]
        public async Task<IActionResult> ChiTietDonHangThang(int thang, int nam)
        {
            var donHangs = await _db.Donhangs
                .Include(dh => dh.CtDonhangs)
                .Where(dh => dh.NgayDat.Value.Month == thang &&
                             dh.NgayDat.Value.Year == nam &&
                             dh.TinhTrangGiaoHang == true)
                .ToListAsync();

            return PartialView("_ChiTietDonHang", donHangs);
        }

        // Lấy chi tiết đơn hàng theo năm
        [HttpGet("ChiTietDonHangNam/{nam}")]
        public async Task<IActionResult> ChiTietDonHangNam(int nam)
        {
            var donHangs = await _db.Donhangs
                .Include(dh => dh.CtDonhangs)
                .Where(dh => dh.NgayDat.Value.Year == nam &&
                             dh.TinhTrangGiaoHang == true)
                .ToListAsync();

            return PartialView("_ChiTietDonHang", donHangs);
        }
    }
}
