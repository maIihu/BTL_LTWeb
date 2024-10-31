using System.Diagnostics;
using Azure;
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
        private readonly GiayDAO _giayDAO;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        #region Index 
        public IActionResult Index(int? page) // Partial View
        {
            int pageSize = 8;
            int pageNum = page == null || page < 0 ? 1 : page.Value;
            var lstSanpham = db.Sanphams.AsNoTracking().OrderBy(x => x.TenGiay);
            PagedList<Sanpham> lst = new PagedList<Sanpham>(lstSanpham, pageNum, pageSize);

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
            List<Sanpham> lstsanpham = db.Sanphams.Where(x => x.MaThuongHieu == maThuongHieu).OrderBy(X=>X.TenGiay).ToList();
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
        public IActionResult DongGopYKien() { 
            return View();
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
                sanPham.YeuThich = !sanPham.YeuThich;
                db.SaveChanges();
            }

            return Json(new { success = true });
        }

    }
}
