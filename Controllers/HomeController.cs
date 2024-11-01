using System.Diagnostics;
using Azure;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web1.Models;
using X.PagedList;
namespace web1.Controllers
{
    public class HomeController : Controller
    {
        ShopGiayContext db = new ShopGiayContext();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        #region Index 
        public IActionResult Index(string tenGiay, int? page, string? thuongHieu, decimal? from, decimal? to)
        {
            int pageSize = 8;
            int pageNum = page == null || page < 0 ? 1 : page.Value;

            ViewData["SearchTerm"] = tenGiay;
            ViewData["SelectedBrand"] = thuongHieu;
            ViewData["FromPrice"] = from;
            ViewData["ToPrice"] = to;

            var sanPhams = from s in db.Sanphams
                           join th in db.Thuonghieus on s.MaThuongHieu equals th.MaThuongHieu
                           where (string.IsNullOrEmpty(tenGiay) || s.TenGiay.Contains(tenGiay)) // Lọc theo tên giày
                           && (string.IsNullOrEmpty(thuongHieu) || th.TenThuongHieu.Contains(thuongHieu)) // Lọc theo tên thương hiệu
                           select new
                           {
                               s,
                               TenThuongHieu = th.TenThuongHieu
                           };

            // Lọc theo khoảng giá
            if (from.HasValue)
            {
                sanPhams = sanPhams.Where(p => p.s.GiaBan >= from.Value);
            }

            if (to.HasValue)
            {
                sanPhams = sanPhams.Where(p => p.s.GiaBan <= to.Value);
            }


            // Phân trang và chuyển danh sách vào dạng PagedList
            var lstSanpham = sanPhams.Select(p => p.s).AsNoTracking().OrderBy(x => x.TenGiay);
            PagedList<Sanpham> lst = new PagedList<Sanpham>(lstSanpham, pageNum, pageSize);

            // Kiểm tra xem có phải là yêu cầu Ajax không và trả về PartialView nếu có
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductListPartial", lst);
            }

            return View(lst);
        }


        #endregion

        #region GiayTheoThuongHieu
        public IActionResult GiayTheoThuongHieu(int maThuongHieu, int? page)
        {
            int pageSize = 1;
            int pageNum = page == null || page < 0 ? 1 : page.Value;
            List<Sanpham> lstsanpham = db.Sanphams.Where(x => x.MaThuongHieu == maThuongHieu).OrderBy(X => X.TenGiay).ToList();
            PagedList<Sanpham> lst = new PagedList<Sanpham>(lstsanpham, pageNum, pageSize);
            ViewBag.MaThuongHieu = maThuongHieu;
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_GiayTheoThuongHieuPartial", lst);
            }
            return View(lst);
        }
        #endregion

        #region ChiTietGiay
        public IActionResult ChiTietGiay(int maSp) // Dùng ViewBag
        {
            var sanPham = db.Sanphams.SingleOrDefault(x => x.MaGiay == maSp);
            if (sanPham != null)
            {
                ViewBag.AnhBia = sanPham.AnhBia; // Lấy ảnh và lưu vào ViewBag
            }
            return View(sanPham);
        }
        #endregion

        #region GiayTheoLoai
        public IActionResult GiayTheoLoai(int maLoai, int? page)
        {
            int pageSize = 8;
            int pageNum = page == null || page < 0 ? 1 : page.Value;
            List<Sanpham> lstsanpham = db.Sanphams.Where(x => x.MaLoai == maLoai).OrderBy(X => X.TenGiay).ToList();
            PagedList<Sanpham> lst = new PagedList<Sanpham>(lstsanpham, pageNum, pageSize);
            ViewBag.MaLoai = maLoai;
            ViewBag.Title = maLoai == 1 ? "Giày Nam" : "Giày Nữ";
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_GiayTheoLoaiPartial", lst);
            }
            return View(lst);
        }
        #endregion

        #region DongGopYKien
        [Authorize]
        [HttpGet]
        public IActionResult GopY()
        {

            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GopY(Ykienkhachhang ykienkhachhang)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            await db.Ykienkhachhangs.AddAsync(ykienkhachhang);
            await db.SaveChangesAsync();

            return Json(new { success = true, message = "Gửi ý kiến thành công! Shop cám ơn bạn vì đã quan tâm ^^" });
        }
        #endregion

        #region HeThongCuaHang
        public IActionResult HeThongCuaHang()
        {
            return View();
        }
        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult UpdateLike(int maGiay)
        {
            var sanPham = db.Sanphams.SingleOrDefault(x => x.MaGiay == maGiay);
            if (sanPham != null)
            {
                sanPham.YeuThich = !sanPham.YeuThich; // Đổi trạng thái yêu thích
                db.SaveChanges();


                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Sản phẩm không tồn tại." });
        }


    }
}
