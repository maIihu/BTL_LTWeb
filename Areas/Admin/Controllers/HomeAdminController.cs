using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using web1.Models;
using X.PagedList;

namespace web1.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        ShopGiayContext db = new ShopGiayContext();
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("danhmucsanpham")]
        public IActionResult DanhMucSanPham(int? page) // phan trang
        {
            int pageSize = 8;
            int pageNum = page == null || page < 0 ? 1 : page.Value;
            var lstSanpham = db.Sanphams.AsNoTracking().OrderBy(X => X.TenGiay);
            PagedList<Sanpham> lst = new PagedList<Sanpham>(lstSanpham, pageNum, pageSize);
            return View(lst);
        
       }
        [Route("Themsanphammoi")]
        [HttpGet]
        public IActionResult ThemSanPhamMoi()
        {
            ViewBag.MaThuongHieu = new SelectList(db.Thuonghieus.ToList(), "MaThuongHieu", "TenThuongHieu");
            ViewBag.MaNcc = new SelectList(db.Nhacungcaps.ToList(), "MaNcc", "TenNcc");
            ViewBag.MaLoai = new SelectList(db.Loaigiays.ToList(), "MaLoai", "TenLoai");

            return View();
        }

        [Route("Themsanphammoi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPhamMoi(Sanpham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Sanphams.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham");
            }
            return View(sanPham);
        }
        [Route("SuaSanPham")]
        [HttpGet]
        public IActionResult SuaSanPham(int maSanPham)
        {
            ViewBag.MaThuongHieu = new SelectList(db.Thuonghieus.ToList(), "MaThuongHieu", "TenThuongHieu");
            ViewBag.MaNcc = new SelectList(db.Nhacungcaps.ToList(), "MaNcc", "TenNcc");
            ViewBag.MaLoai = new SelectList(db.Loaigiays.ToList(), "MaLoai", "TenLoai");
            var sanPham = db.Sanphams.Find(maSanPham);
            return View(sanPham);
        }

        [Route("SuaSanPham")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaSanPham(Sanpham sanPham)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("DanhMucSanPham", "HomeAdmin");
            }
            return View(sanPham);
        }
    }
}
