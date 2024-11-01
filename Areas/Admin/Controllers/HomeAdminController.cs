
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using web1.Models;
using X.PagedList;
using Microsoft.AspNetCore.Authorization;


namespace web1.Areas.Admin.Controllers
{
	//[Authorize] // đảm bảo đăng nhập mới vào được
	[Area("admin")]
	[Route("admin")]
	[Route("admin/homeadmin")]
	public class HomeAdminController : Controller
	{
		ShopGiayContext db = new ShopGiayContext();
		private readonly ShopGiayContext _db;

		public HomeAdminController(ShopGiayContext db)
		{
			_db = db;
		}
		[Route("trangchu")]
		public IActionResult TrangChu()
		{
			return View();
		}
		[Route("danhmucsanpham")]
		public IActionResult DanhMucSanPham(int? page, string tenGiay, decimal? from, decimal? to) // phan trang
		{
			int pageSize = 8;
			int pageNum = page == null || page < 0 ? 1 : page.Value;
			ViewData["SearchTerm"] = tenGiay;
			ViewData["FromPrice"] = from;
			ViewData["ToPrice"] = to;

			// Truy vấn sản phẩm theo tên và giá
			var sanPhams = db.Sanphams
							 .AsNoTracking()
							 .Where(s => (string.IsNullOrEmpty(tenGiay) || s.TenGiay.Contains(tenGiay)) &&
										  (!from.HasValue || s.GiaBan >= from.Value) &&
							 (!to.HasValue || s.GiaBan <= to.Value))
							 .OrderBy(s => s.TenGiay);
			PagedList<Sanpham> lst = new PagedList<Sanpham>(sanPhams, pageNum, pageSize);
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
		public async Task<IActionResult> ThemSanPhamMoiAsync(Sanpham sanPham, IFormFile AnhBia)
		{
			if (ModelState.IsValid)
			{
				if (AnhBia != null && AnhBia.Length > 0)
				{
					var filePath = Path.Combine("wwwroot/img/AnhGiay", AnhBia.FileName);

					Console.WriteLine(filePath.ToString());
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await AnhBia.CopyToAsync(stream);
					}
					sanPham.AnhBia = AnhBia.FileName;
				}
				else
				{
					ModelState.AddModelError("AnhBia", "Bạn cần chọn một ảnh.");
					return View(sanPham);
				}
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

		[Route("XoaSanPham")]
		[HttpGet]
		public IActionResult XoaSanPham(int maSanPham)
		{
			TempData["Message"] = "";
			var ctDonhangs = db.CtDonhangs.Where(x => x.MaGiay == maSanPham).ToList();
			if (ctDonhangs.Count > 0)
			{
				TempData["Message"] = "Không xóa được sản phẩm này";
				return RedirectToAction("DanhMucSanPham", "HomeAdmin");
			}
			db.Remove(db.Sanphams.Find(maSanPham));
			db.SaveChanges();
			TempData["Message"] = "Sản phẩm đã được xóa";
			return RedirectToAction("DanhMucSanPham", "HomeAdmin");
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
			return RedirectToAction("DanhSachDonHang");
		}

	}
}
