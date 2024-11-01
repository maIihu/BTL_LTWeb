using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using web1.Models;
using System.Linq;

namespace web1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/QuanLyYKienKhachHang")]
    public class QuanLyYKienKhachHangController : Controller
    {
        private readonly ShopGiayContext _db;
        //Khai báo biến toàn cụ pageSize
        private int pageSize = 10; //số bản ghi mỗi trang

        public QuanLyYKienKhachHangController(ShopGiayContext db)
        {
            _db = db;
        }

        // Danh sách ý kiến khách hàng
        [HttpGet("DanhSach")]
        public async Task<IActionResult> DanhSachYKien(int page = 1)
        {
            // Tổng số bản ghi
            var totalItems = await _db.Ykienkhachhangs.CountAsync();

            // Lấy danh sách ý kiến khách hàng với phân trang
            var yKienList = await _db.Ykienkhachhangs
                .Skip((page - 1) * pageSize)  // Bỏ qua các bản ghi trước đó
                .Take(pageSize)               // Lấy các bản ghi trong trang hiện tại
                .ToListAsync();

            // Đưa thông tin phân trang vào ViewBag để sử dụng trong View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(yKienList);
        }


        // Chi tiết ý kiến khách hàng
        [HttpGet("ChiTiet/{id}")]
        public async Task<IActionResult> ChiTietYKien(int id)
        {
            var ykien = await _db.Ykienkhachhangs.FindAsync(id);
            if (ykien == null) return NotFound();

            return View(ykien);
        }

        // Xác nhận xóa ý kiến khách hàng
        [HttpPost("XacNhanXoa/{id}")]
        public async Task<IActionResult> XacNhanXoa(int id)
        {
            var ykien = await _db.Ykienkhachhangs.FindAsync(id);
            if (ykien == null) return Json(new { success = false, message = "Không tìm thấy ý kiến!" });

            _db.Ykienkhachhangs.Remove(ykien);
            await _db.SaveChangesAsync();
            return Json(new { success = true, message = "Xóa thành công!" });
        }

    }
}
