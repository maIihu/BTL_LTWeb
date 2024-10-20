using System.Diagnostics;
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

        public IActionResult Index(int? page) // phan trang
        {
            int pageSize = 8;
            int pageNum = page == null || page < 0 ? 1 : page.Value;
            var lstSanpham = db.Sanphams.AsNoTracking().OrderBy(X => X.TenGiay);
            PagedList<Sanpham> lst = new PagedList<Sanpham>(lstSanpham, pageNum, pageSize);
            return View(lst);
        }
        public IActionResult GiayTheoThuongHieu(int maThuongHieu, int? page)
        {
            int pageSize = 1;
            int pageNum = page == null || page < 0 ? 1 : page.Value;
            List<Sanpham> lstsanpham = db.Sanphams.Where(x => x.MaThuongHieu == maThuongHieu).OrderBy(X=>X.TenGiay).ToList();
            PagedList<Sanpham> lst = new PagedList<Sanpham>(lstsanpham, pageNum, pageSize);
            ViewBag.MaThuongHieu = maThuongHieu;
            return View(lst);
        }
        // Dùng ViewBag
        public IActionResult ChiTietGiay(int maSp)
        {
            var sanPham = db.Sanphams.SingleOrDefault(x => x.MaGiay == maSp);
            if (sanPham != null)
            {
                ViewBag.AnhBia = sanPham.AnhBia; // Lấy ảnh và lưu vào ViewBag
            }
            return View(sanPham);
        }


        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Shop()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
