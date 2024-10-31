using Microsoft.AspNetCore.Mvc;
using web1.Models;
using web1.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using web1.Enums;

namespace web1.Controllers
{
	public class UserController : Controller
	{
		private readonly ShopGiayContext _db;
		private readonly ILogger<UserController> _logger;
		private readonly IEmailSender _emailSender;
		public UserController(ShopGiayContext db, ILogger<UserController> logger, IEmailSender emailSender)
		{
			_db = db;
			_logger = logger;
			_emailSender = emailSender;
		}
		#region DangKy
		[HttpGet]
		public IActionResult DangKy()
		{
			return View();
		}
		[HttpPost]
		public IActionResult DangKy(Khachhang khachhang)
		{
			if (!ModelState.IsValid)
			{
				return View(khachhang);
			}

			var existingUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == khachhang.TaiKhoanKh);
			if (existingUser != null)
			{
				ModelState.AddModelError("TaiKhoanKh", "Tên đăng nhập đã tồn tại.");
				return View(khachhang);
			}

			_db.Khachhangs.Add(khachhang);
			_db.SaveChanges();

			return RedirectToAction("DangNhap", "User");
		}
		#endregion

		#region DangNhap
		public IActionResult DangNhap()
		{
			if (HttpContext.Session.GetString("Username") == null)
			{
				return View();
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		[HttpPost]
		public async Task<IActionResult> DangNhap(string taikhoan, string matkhau, bool rememberMe)
		{
			var adminAccount = _db.Quanlies.SingleOrDefault(a => a.TaiKhoanQl == taikhoan && a.MatKhau == matkhau);
			var userAccount = _db.Khachhangs.SingleOrDefault(a => a.TaiKhoanKh == taikhoan && a.MatKhau == matkhau);

			if (adminAccount != null)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, adminAccount.TaiKhoanQl),
					new Claim("UserType", UserType.Admin.ToString())
				};
				var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				var principal = new ClaimsPrincipal(identity);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
				return Json(new { success = true, redirectUrl = Url.Action("Index", "HomeAdmin", new { area = "Admin" }) });
			}
			else if (userAccount != null)
			{
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, userAccount.TaiKhoanKh),
					new Claim("UserType", UserType.Customer.ToString())
				};
				var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				var principal = new ClaimsPrincipal(identity);

				// Cấu hình thuộc tính authentication
				var authProperties = new AuthenticationProperties
				{
					IsPersistent = rememberMe,
					ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(7) : (DateTimeOffset?)null
				};

				// Đăng nhập với cấu hình authProperties
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
				return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
			}
			else
			{
				return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không đúng." });
			}
		}




		#endregion

		#region DangXuat
		public async Task<IActionResult> DangXuat()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("DangNhap", "User");
		}
		#endregion

		#region TaiKhoanCuaToi
		public IActionResult TaiKhoanCuaToi()
		{
			return View();
		}
		
		[HttpGet]
		public IActionResult LoadDoiMatKhau()
		{
			return PartialView("_DoiMatKhau");
		}

		[HttpGet]
		public IActionResult LoadHoSo()
		{
			if (User.Identity.IsAuthenticated)
			{
				string taiKhoan = User.Identity.Name;
				var khachhang = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == taiKhoan);

				if (khachhang != null)
				{
					if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
					{
						return PartialView("_HoSo", khachhang);
					}
					return View(khachhang);
				}
			}
			return View(null);
		}
		[HttpPost]
		public IActionResult HoSo(Khachhang updatedKhachhang)
		{
			if (User.Identity.IsAuthenticated)
			{
				string taiKhoan = User.Identity.Name;
				var khachhang = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == taiKhoan);

				if (khachhang != null)
				{
					khachhang.HoTen = updatedKhachhang.HoTen;
					khachhang.DiaChiKh = updatedKhachhang.DiaChiKh;
					khachhang.DienThoaiKh = updatedKhachhang.DienThoaiKh;
					khachhang.NgaySinh = updatedKhachhang.NgaySinh;

					_db.SaveChanges();

					return Json(new { success = true });
				}
			}

			return Json(new { success = false });
		}


		[HttpPost]
		public JsonResult DoiMatKhau(string matKhauCu, string matKhauMoi)
		{
			try
			{
				if (User.Identity.IsAuthenticated)
				{
					string taiKhoan = User.Identity.Name;
					var khachhang = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == taiKhoan);

					if (khachhang != null && khachhang.MatKhau == matKhauCu) // So sánh mật khẩu cũ
					{
						khachhang.MatKhau = matKhauMoi; // Cập nhật mật khẩu mới
						_db.SaveChanges();
						return Json(new { success = true, message = "Đổi mật khẩu thành công!" });
					}
					else
					{
						return Json(new { success = false, message = "Mật khẩu cũ không chính xác." });
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return Json(new { success = false, message = "Đã xảy ra lỗi khi đổi mật khẩu." });
		}

		#endregion

		#region QuenMatKhau
		[HttpGet]
		public IActionResult QuenMatKhau()
		{
			// Lấy email từ session nếu có
			var email = HttpContext.Session.GetString("Email");
			ViewBag.Email = email; // Truyền email vào ViewBag

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> QuenMatKhau(string email, string verificationCode, bool sendCode)
		{
			// Kiểm tra nếu gửi mã
			if (sendCode)
			{
				if (string.IsNullOrEmpty(email))
				{
					ModelState.AddModelError("", "Email không được để trống.");
					return View();
				}

				// Kiểm tra nếu mã đã được gửi trong vòng 30 phút
				var savedCode = HttpContext.Session.GetString("VerificationCode");
				var codeCreationTime = HttpContext.Session.GetString("CodeCreationTime");

				if (savedCode != null && codeCreationTime != null)
				{
					var creationTime = DateTime.Parse(codeCreationTime);
					if (DateTime.UtcNow <= creationTime.AddMinutes(30))
					{
						ViewBag.Message = "Mã xác nhận đã được gửi. Vui lòng kiểm tra email của bạn.";
						ViewBag.Email = email; // Giữ lại email đã nhập
						return View();
					}
				}

				// Tạo mã xác nhận mới
				var generatedCode = new Random().Next(100000, 999999).ToString();
				HttpContext.Session.SetString("VerificationCode", generatedCode);
				HttpContext.Session.SetString("Email", email);
				HttpContext.Session.SetString("CodeCreationTime", DateTime.UtcNow.ToString());

				var subject = "Mã xác nhận của bạn";
				var message = $"Mã xác nhận của bạn là {generatedCode}";

				// Gửi email bất đồng bộ
				await _emailSender.SendEmailAsync(email, subject, message);
				ViewBag.Message = "Mã xác nhận đã được gửi đến email.";
			}
			else // Nếu người dùng nhấn nút "Xác nhận mã"
			{
				var savedCode = HttpContext.Session.GetString("VerificationCode");
				var codeCreationTime = HttpContext.Session.GetString("CodeCreationTime");

				if (savedCode == null || codeCreationTime == null)
				{
					ViewBag.Message = "Mã xác nhận đã hết hiệu lực.";
					return View();
				}

				var creationTime = DateTime.Parse(codeCreationTime);
				if (DateTime.UtcNow > creationTime.AddMinutes(30))
				{
					ViewBag.Message = "Mã xác nhận đã hết hiệu lực.";
					return View();
				}

				if (savedCode == verificationCode)
				{
					return RedirectToAction("XacNhanMatKhau"); // Điều hướng tới trang nhập mật khẩu mới
				}
				else
				{
					ViewBag.Message = "Mã xác nhận không đúng.";
				}
			}

			ViewBag.Email = email; // Đảm bảo truyền lại email vào ViewBag khi post dữ liệu
			return View();
		}

		[HttpGet]
		public IActionResult XacNhanMatKhau()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> XacNhanMatKhau(string newPassword, string confirmPassword)
		{
			if (newPassword != confirmPassword)
			{
				ModelState.AddModelError("", "Mật khẩu mới và xác nhận mật khẩu không khớp.");
				return View();
			}

			var email = HttpContext.Session.GetString("Email");
			if (email == null)
			{
				return RedirectToAction("QuenMatKhau"); // Điều hướng lại nếu không có email trong session
			}

			var user = await _db.Khachhangs.FirstOrDefaultAsync(u => u.EmailKh == email);
			if (user != null)
			{
				user.MatKhau = newPassword;
				await _db.SaveChangesAsync();
				return RedirectToAction("DangNhap", "User");
			}

			ModelState.AddModelError("", "Không thể đổi mật khẩu.");
			return View();
		}
		#endregion

		#region DonHang
		public IActionResult DanhSachDonHang()
		{
			if (User.Identity.IsAuthenticated)
			{
				string tk = User.Identity.Name;
				var kh = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == tk);
				if (kh != null)
				{
					var dsDonHang = _db.Donhangs.Where(dh => dh.MaKh == kh.MaKh).Include(dh => dh.CtDonhangs).ToList();
					if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
					{
						return PartialView("_DonHangPartial", dsDonHang);
					}
					return View(dsDonHang);
				}

			}
			return RedirectToAction("DangNhap");
		}
		public IActionResult ChiTietDonHang(int id)
		{
			var donHang = _db.Donhangs.Include(dh => dh.CtDonhangs).ThenInclude(ct => ct.MaGiayNavigation)
							 .FirstOrDefault(dh => dh.MaDonHang == id);
			if (donHang == null)
			{
				return NotFound();
			}

			if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
			{
				return PartialView("_ChiTietDonHangPartial", donHang);
			}

			return View(donHang);
		}

		#endregion

		#region GetUserInfo
		[HttpGet]
		public IActionResult GetUserInfo()
		{
			if (User.Identity.IsAuthenticated)
			{
				string taiKhoan = User.Identity.Name;
				var khachhang = _db.Khachhangs.FirstOrDefault(k => k.TaiKhoanKh == taiKhoan);

				if (khachhang != null)
				{
					return Json(new
					{
						hoTen = khachhang.HoTen,
						email = khachhang.EmailKh, // Giả sử EmailKh là thuộc tính lưu email
						soDienThoai = khachhang.DienThoaiKh, // Giả sử DienThoaiKh là thuộc tính lưu số điện thoại
						diaChi = khachhang.DiaChiKh, // Giả sử DiaChiKh là thuộc tính lưu địa chỉ
					});
				}
			}

			return NotFound(); // Trả về 404 nếu không tìm thấy thông tin người dùng
		}
		#endregion
	}
}
