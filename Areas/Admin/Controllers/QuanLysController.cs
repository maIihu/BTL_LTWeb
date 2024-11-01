using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using web1.Models;
using System.Collections.Generic;
using System.Linq;

namespace web1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/QuanLys")]
    public class QuanLysController : Controller
    {
        private readonly ShopGiayContext _db;
        private readonly ILogger<QuanLysController> _logger;

        public QuanLysController(ShopGiayContext db, ILogger<QuanLysController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            List<Quanly> quanlyList = _db.Quanlies.ToList();
            return View(quanlyList);
        }

        // GET: QuanLy/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.PhanQuyenList = _db.Phanquyens.ToList(); // Lấy tất cả quyền
            return PartialView("_CreateQuanLy");
        }

        // POST: QuanLy/Create
        [HttpPost]
        [Route("Create")]
        public IActionResult Create(Quanly quanLy, List<string> PhanQuyen)
        {
            _logger.LogInformation("Tạo mới quản lý: {TaiKhoanQl}", quanLy.TaiKhoanQl);

            if (ModelState.IsValid)
            {
                _db.Quanlies.Add(quanLy);
                _db.SaveChanges();

                // Tạo một đối tượng phân quyền mới
                var phanQuyen = new Phanquyen
                {
                    MaQl = quanLy.MaQl,
                    QlAdmin = PhanQuyen.Contains("Admin"),
                    QlNhaCungCap = PhanQuyen.Contains("NhaCungCap"),
                    QlSanPham = PhanQuyen.Contains("SanPham"),
                    QlThuongHieu = PhanQuyen.Contains("ThuongHieu"),
                    QlLoaiGiay = PhanQuyen.Contains("LoaiGiay"),
                    QlDonHang = PhanQuyen.Contains("DonHang"),
                    QlKhachHang = PhanQuyen.Contains("KhachHang"),
                    QlYkienKhachHang = PhanQuyen.Contains("YKienKhachHang"),
                };

                // Lưu vào cơ sở dữ liệu
                _db.Phanquyens.Add(phanQuyen);
                _db.SaveChanges();

                _logger.LogInformation("Quản lý đã được tạo thành công với ID: {MaQl}", quanLy.MaQl);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("Model không hợp lệ khi tạo quản lý: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            ViewBag.PhanQuyenList = _db.Phanquyens.ToList();
            return PartialView("_CreateQuanLy", quanLy);
        }

    }
}
