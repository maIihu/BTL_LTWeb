using System.Text.Json;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web1.Helpers;
using web1.Models;

namespace web1.Controllers
{
    public class GioHangController : Controller
    {
        private readonly ILogger<GioHangController> _logger;
        private readonly ShopGiayContext _db;  
        public GioHangController(ShopGiayContext db, ILogger<GioHangController> logger)
        {
            this._db = db;
            _logger = logger;
        }
        #region GioHang
        public IActionResult GioHang()
        {
            // Lấy tên đăng nhập từ Claims
            var currentUserName = User.Identity.Name;

            // Nếu không có người dùng đăng nhập, chuyển hướng đến trang đăng nhập
            if (string.IsNullOrEmpty(currentUserName))
            {
                return RedirectToAction("DangNhap", "User");
            }

            // Tìm người dùng dựa trên tên đăng nhập
            var currentUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == currentUserName);

            if (currentUser == null)
            {
                return RedirectToAction("DangNhap", "User"); // Nếu không tìm thấy người dùng, chuyển hướng đến đăng nhập
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
        public IActionResult AddToCart(int MaGiay, int Size, int SoLuong)
        {
            var product = _db.Sanphams.FirstOrDefault(p => p.MaGiay == MaGiay);

            // Lấy tên đăng nhập từ Claims
            var currentUserName = User.Identity.Name; // Đây là cách lấy tên đăng nhập từ Claims

            if (product != null)
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                if (string.IsNullOrEmpty(currentUserName))
                {
                    return Json(new { success = false, redirectUrl = Url.Action("DangNhap", "User") }); // Chuyển hướng đến trang đăng nhập
                }

                // Tìm người dùng dựa trên tên đăng nhập
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
        public IActionResult RemoveFromCart(int MaGiay)
        {
            // Lấy tên đăng nhập từ Claims
            var currentUserName = User.Identity.Name;

            // Nếu không có người dùng đăng nhập, chuyển hướng đến trang đăng nhập
            if (string.IsNullOrEmpty(currentUserName))
            {
                return RedirectToAction("DangNhap", "User");
            }

            // Tìm người dùng dựa trên tên đăng nhập
            var currentUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == currentUserName);

            if (currentUser == null)
            {
                return RedirectToAction("DangNhap", "User"); // Nếu không tìm thấy người dùng, chuyển hướng đến đăng nhập
            }

            // Tìm hóa đơn hiện tại của người dùng (chưa giao hàng)
            var currentOrder = _db.Donhangs.FirstOrDefault(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);

            if (currentOrder == null)
            {
                return RedirectToAction("GioHang"); // Nếu không có hóa đơn, quay lại trang giỏ hàng
            }

            // Tìm sản phẩm cần xóa trong chi tiết hóa đơn
            var itemToRemove = _db.CtDonhangs.FirstOrDefault(item => item.MaGiay == MaGiay && item.MaDonHang == currentOrder.MaDonHang);

            if (itemToRemove != null)
            {
                // Cập nhật tổng tiền cho hóa đơn
                currentOrder.TongTien -= itemToRemove.ThanhTien ?? 0;

                // Xóa sản phẩm khỏi chi tiết hóa đơn
                _db.CtDonhangs.Remove(itemToRemove);
                _db.SaveChanges();

                Console.WriteLine("Sản phẩm đã được xóa khỏi giỏ hàng.");
            }
            else
            {
                Console.WriteLine("Không tìm thấy sản phẩm để xóa.");
            }

            return RedirectToAction("GioHang");
        }
        #endregion

        #region ThemSlTrongGio
        [HttpPost]
        public JsonResult UpdateQuantity(int MaGiay, int SoLuong)
        {
            try
            {
                // Lấy tên đăng nhập từ Claims
                var currentUserName = User.Identity.Name;

                if (string.IsNullOrEmpty(currentUserName))
                {
                    return Json(new { success = false, message = "Người dùng chưa đăng nhập." });
                }

                // Tìm người dùng dựa trên tên đăng nhập
                var currentUser = _db.Khachhangs.FirstOrDefault(u => u.TaiKhoanKh == currentUserName);

                if (currentUser == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng." });
                }

                // Tìm hóa đơn hiện tại (chưa giao hàng) của người dùng
                var currentOrder = _db.Donhangs.FirstOrDefault(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);

                if (currentOrder == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hóa đơn hiện tại." });
                }

                // Tìm sản phẩm trong chi tiết hóa đơn
                var cartItem = _db.CtDonhangs.FirstOrDefault(item => item.MaGiay == MaGiay && item.MaDonHang == currentOrder.MaDonHang);

                if (cartItem != null)
                {
                    // Cập nhật số lượng và tính lại tổng tiền cho sản phẩm
                    cartItem.SoLuong = SoLuong;
                    cartItem.ThanhTien = cartItem.GiaLucBan * SoLuong;

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    _db.SaveChanges();

                    // Cập nhật tổng tiền cho hóa đơn
                    currentOrder.TongTien = _db.CtDonhangs
                                            .Where(c => c.MaDonHang == currentOrder.MaDonHang)
                                            .Sum(c => c.ThanhTien);
                    _db.SaveChanges();

                    return Json(new { success = true, message = "Cập nhật số lượng thành công." });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        [HttpPost]
        public IActionResult Checkout([FromBody] List<CtDonhang> cartItems)
        {
            if (cartItems != null && cartItems.Any())
            {
                // Thực hiện logic thanh toán
                // ...

                // Sau khi thanh toán thành công, xóa hết sản phẩm trong giỏ hàng
                var maDonHang = cartItems.FirstOrDefault()?.MaDonHang;
                if (maDonHang.HasValue)
                {
                    // Xóa hết các sản phẩm của đơn hàng trong CSDL
                    var chiTietDonHangs = _db.CtDonhangs.Where(x => x.MaDonHang == maDonHang.Value).ToList();
                    if (chiTietDonHangs.Any())
                    {
                        _db.CtDonhangs.RemoveRange(chiTietDonHangs);
                        _db.SaveChanges();
                    }
                }

                // Xóa giỏ hàng trong session
                HttpContext.Session.Remove("Cart");

                // Trả về kết quả thành công
                return Json(new { success = true, message = "Thanh toán thành công!" });
            }

            return Json(new { success = false, message = "Có lỗi xảy ra!" });
        }


    }
}
