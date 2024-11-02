using System.Text.Json;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web1.Helpers;
using web1.Models;

namespace web1.Controllers
{
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ShopGiayContext _db;  
        public CartController(ShopGiayContext db, ILogger<CartController> logger)
        {
            this._db = db;
            _logger = logger;
        }
        #region XemGioHang
        public IActionResult ShowCart()
        {
            // Lấy tên đăng nhập từ Claims
            var currentUserName = User.Identity.Name;

            if (string.IsNullOrEmpty(currentUserName))
            {
                return RedirectToAction("DangNhap", "User");
            }

            var currentUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == currentUserName);

            if (currentUser == null)
            {
                return RedirectToAction("DangNhap", "User"); 
            }

            // Tìm hóa đơn chưa giao hàng (giỏ hàng) của người dùng
            var currentOrder = _db.Donhangs.FirstOrDefault(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);

            // Nếu không có hóa đơn nào, trả về một giỏ hàng rỗng
            if (currentOrder == null)
            {
                ViewBag.TotalAmount = 0;
                return View(new List<CtDonhang>());
            }

            // Lấy danh sách chi tiết hóa đơn (giỏ hàng) của hóa đơn hiện tại
            var cart = _db.CtDonhangs.Where(c => c.MaDonHang == currentOrder.MaDonHang).ToList();

            // Tính tổng tiền trong giỏ hàng
            decimal totalAmount = cart.Sum(item => item.ThanhTien ?? 0);

            ViewBag.TotalAmount = totalAmount; // Gửi tổng tiền đến view
            return View(cart);
        }
        #endregion

        #region ThemGioHang
        [HttpPost]
        public IActionResult AddToCart(int MaGiay, int Size, int SoLuong) // AJAX
        {
            var product = _db.Sanphams.FirstOrDefault(p => p.MaGiay == MaGiay);

            // Lấy tên đăng nhập từ Claims
            var currentUserName = User.Identity.Name; // Đây là cách lấy tên đăng nhập từ Claims

            if (product != null)
            {
                if (string.IsNullOrEmpty(currentUserName))
                {
                    return Json(new { success = false, redirectUrl = Url.Action("DangNhap", "User") }); // Chuyển hướng đến trang đăng nhập
                }

                var currentUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == currentUserName);

                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng." });
                }

                // Tìm hóa đơn hiện tại của người dùng (chưa giao hàng)
                var currentOrder = _db.Donhangs.FirstOrDefault(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);

                // Nếu không có hóa đơn nào đang xử lý, tạo hóa đơn mới
                if (currentOrder == null)
                {
                    currentOrder = new Donhang
                    {
                        MaKh = currentUser.MaKh,
                        NgayDat = DateTime.Now,
                        TongTien = 0, // Sẽ cập nhật sau khi thêm sản phẩm
                        TinhTrangGiaoHang = null
                    };
                    _db.Donhangs.Add(currentOrder);
                    _db.SaveChanges(); // Lưu hóa đơn mới vào cơ sở dữ liệu
                }

                // Kiểm tra chi tiết hóa đơn (CTDonhang) hiện tại
                var existingCtDonhang = _db.CtDonhangs.FirstOrDefault(c => c.MaDonHang == currentOrder.MaDonHang && c.MaGiay == MaGiay);

                if (existingCtDonhang != null)
                {
                    // Nếu sản phẩm đã có trong chi tiết hóa đơn, cập nhật số lượng và thành tiền
                    existingCtDonhang.SoLuong += SoLuong;
                    existingCtDonhang.ThanhTien = existingCtDonhang.GiaLucBan * existingCtDonhang.SoLuong; // Cập nhật thành tiền
                }
                else
                {
                    // Nếu sản phẩm chưa có, thêm mới vào chi tiết hóa đơn
                    var newCtDonhang = new CtDonhang
                    {
                        MaDonHang = currentOrder.MaDonHang, // Liên kết với hóa đơn hiện tại
                        MaGiay = product.MaGiay,
                        SoLuong = SoLuong,
                        GiaLucBan = product.GiaBan,
                        ThanhTien = product.GiaBan * SoLuong
                    };
                    _db.CtDonhangs.Add(newCtDonhang); // Thêm chi tiết hóa đơn mới
                }

                // Cập nhật tổng tiền cho hóa đơn
                currentOrder.TongTien += product.GiaBan * SoLuong;

                _db.SaveChanges(); // Lưu cập nhật chi tiết hóa đơn và hóa đơn vào cơ sở dữ liệu

                return Json(new { success = true }); // Trả về thành công
            }

            return Json(new { success = false, message = "Sản phẩm không tồn tại." }); // Trả về thất bại nếu không tìm thấy sản phẩm
        }
        #endregion

        #region XoaSpKhoiGio
        [HttpPost]
        public JsonResult RemoveFromCart(int maGiay) // AJAX
        {
            // Lấy tên đăng nhập từ Claims
            var currentUserName = User.Identity.Name;

            if (string.IsNullOrEmpty(currentUserName))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập." });
            }

            var currentUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == currentUserName);

            if (currentUser == null)
            {
                return Json(new { success = false, message = "Không tìm thấy người dùng." });
            }

            // Tìm hóa đơn hiện tại của người dùng (chưa giao hàng)
            var currentOrder = _db.Donhangs.FirstOrDefault(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);

            if (currentOrder == null)
            {
                return Json(new { success = false, message = "Không có hóa đơn nào." });
            }

            // Tìm sản phẩm cần xóa trong chi tiết hóa đơn
            var itemToRemove = _db.CtDonhangs.FirstOrDefault(item => item.MaGiay == maGiay && item.MaDonHang == currentOrder.MaDonHang);

            if (itemToRemove != null)
            {
                currentOrder.TongTien -= itemToRemove.ThanhTien ?? 0;
                _db.CtDonhangs.Remove(itemToRemove);
                _db.SaveChanges();
                return Json(new { success = true, message = "Sản phẩm đã được xóa khỏi giỏ hàng.", totalAmount = currentOrder.TongTien });
            }
            else
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm để xóa." });
            }
        }
		#endregion

		#region ThemSlTrongGio
		[HttpPost]
		public IActionResult UpdateQuantity(int maGiay, int soLuong)
		{
			try
			{
				// Tìm sản phẩm trong giỏ hàng và cập nhật số lượng
				var item = _db.CtDonhangs.FirstOrDefault(i => i.MaGiay == maGiay);
				if (item == null) return Json(new { success = false });

				item.SoLuong = soLuong;
				_db.SaveChanges();

				// Tính tổng tiền mới
				var tongTien = _db.CtDonhangs
					.Where(i => i.MaDonHang == item.MaDonHang)
					.Sum(i => i.GiaLucBan * i.SoLuong);

				return Json(new { success = true, tongTien, unitPrice = item.GiaLucBan });
			}
			catch
			{
				return Json(new { success = false });
			}
		}

		#endregion

		#region Checkout
		[HttpPost]
        public IActionResult Checkout([FromBody] List<CtDonhang> cartItems)
        {
            var currentUserName = User.Identity.Name;
            var currentUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == currentUserName);

            // Tạo hoặc cập nhật hóa đơn hiện tại của người dùng
            var currentOrder = _db.Donhangs.FirstOrDefault(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);
            if (currentOrder != null)
            {
                currentOrder.TinhTrangGiaoHang = false;
                currentOrder.NgayGiao = DateTime.Now.AddDays(3);
                _db.SaveChanges();
            }

            currentOrder = new Donhang
            {
                MaKh = currentUser.MaKh,
                NgayDat = DateTime.Now,
                TongTien = 0, // Sẽ cập nhật sau khi thêm sản phẩm
                TinhTrangGiaoHang = null
            };
            _db.Donhangs.Add(currentOrder);
            _db.SaveChanges();

            if (cartItems != null && cartItems.Any())
            {
                foreach (var item in cartItems)
                {
                    // Lấy thông tin sản phẩm từ bảng Sanphams
                    var product = _db.Sanphams.FirstOrDefault(p => p.MaGiay == item.MaGiay);
                    if (product != null)
                    {
                        // Giảm số lượng sản phẩm trong kho
                        product.SoLuongTon -= item.SoLuong;
                    }
                }

                // Lưu các thay đổi vào database
                _db.SaveChanges();

                // Trả về kết quả thành công
                return Json(new { success = true, message = "Thanh toán thành công!" });
            }


            return Json(new { success = false, message = "Có lỗi xảy ra!" });
        }
        #endregion


    }
}
