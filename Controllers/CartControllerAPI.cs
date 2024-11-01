using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartControllerAPI : ControllerBase
    {
        private readonly ShopGiayContext _db;

        public CartControllerAPI(ShopGiayContext db)
        {
            _db = db;
        }

        #region XemGiỏHàng
        [HttpGet("ShowCart")]
        public async Task<IActionResult> ShowCart()
        {
            var currentUserName = User.Identity.Name;

            if (string.IsNullOrEmpty(currentUserName))
            {
                return Unauthorized(new { message = "Bạn cần đăng nhập." });
            }

            var currentUser = await _db.Khachhangs.FirstOrDefaultAsync(u => u.TaiKhoanKh == currentUserName);
            if (currentUser == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            var currentOrder = await _db.Donhangs.FirstOrDefaultAsync(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);
            if (currentOrder == null)
            {
                return Ok(new { cartItems = new List<CtDonhang>(), totalAmount = 0 });
            }

            var cart = await _db.CtDonhangs.Where(c => c.MaDonHang == currentOrder.MaDonHang).ToListAsync();
            decimal totalAmount = cart.Sum(item => item.ThanhTien ?? 0);

            return Ok(new { cartItems = cart, totalAmount });
        }
        #endregion

        #region ThêmSảnPhẩmVàoGiỏ
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var product = await _db.Sanphams.FirstOrDefaultAsync(p => p.MaGiay == request.MaGiay);
            var currentUserName = User.Identity.Name;

            if (product == null)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại." });
            }

            if (string.IsNullOrEmpty(currentUserName))
            {
                return Unauthorized(new { message = "Bạn cần đăng nhập." });
            }

            var currentUser = await _db.Khachhangs.FirstOrDefaultAsync(u => u.TaiKhoanKh == currentUserName);
            if (currentUser == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            var currentOrder = await _db.Donhangs.FirstOrDefaultAsync(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);
            if (currentOrder == null)
            {
                currentOrder = new Donhang
                {
                    MaKh = currentUser.MaKh,
                    NgayDat = DateTime.Now,
                    TongTien = 0,
                    TinhTrangGiaoHang = null
                };
                await _db.Donhangs.AddAsync(currentOrder);
                await _db.SaveChangesAsync();
            }

            var existingCtDonhang = await _db.CtDonhangs.FirstOrDefaultAsync(c => c.MaDonHang == currentOrder.MaDonHang && c.MaGiay == request.MaGiay);
            if (existingCtDonhang != null)
            {
                existingCtDonhang.SoLuong += request.SoLuong;
                existingCtDonhang.ThanhTien = existingCtDonhang.GiaLucBan * existingCtDonhang.SoLuong;
            }
            else
            {
                var newCtDonhang = new CtDonhang
                {
                    MaDonHang = currentOrder.MaDonHang,
                    MaGiay = product.MaGiay,
                    SoLuong = request.SoLuong,
                    GiaLucBan = product.GiaBan,
                    ThanhTien = product.GiaBan * request.SoLuong
                };
                await _db.CtDonhangs.AddAsync(newCtDonhang);
            }

            currentOrder.TongTien += product.GiaBan * request.SoLuong;
            await _db.SaveChangesAsync();

            return Ok(new { success = true });
        }
        #endregion

        #region XóaSảnPhẩmKhỏiGiỏ
        [HttpDelete("RemoveFromCart/{maGiay}")]
        public async Task<IActionResult> RemoveFromCart(int maGiay)
        {
            var currentUserName = User.Identity.Name;

            if (string.IsNullOrEmpty(currentUserName))
            {
                return Unauthorized(new { message = "Bạn cần đăng nhập." });
            }

            var currentUser = await _db.Khachhangs.FirstOrDefaultAsync(u => u.TaiKhoanKh == currentUserName);
            if (currentUser == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            var currentOrder = await _db.Donhangs.FirstOrDefaultAsync(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);
            if (currentOrder == null)
            {
                return NotFound(new { message = "Không có hóa đơn nào." });
            }

            var itemToRemove = await _db.CtDonhangs.FirstOrDefaultAsync(item => item.MaGiay == maGiay && item.MaDonHang == currentOrder.MaDonHang);
            if (itemToRemove != null)
            {
                currentOrder.TongTien -= itemToRemove.ThanhTien ?? 0;
                _db.CtDonhangs.Remove(itemToRemove);
                await _db.SaveChangesAsync();
                return Ok(new { success = true, message = "Sản phẩm đã được xóa khỏi giỏ hàng.", totalAmount = currentOrder.TongTien });
            }
            else
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm để xóa." });
            }
        }
        #endregion

        #region CậpNhậtSốLượng
        [HttpPut("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var currentUserName = User.Identity.Name;

            if (string.IsNullOrEmpty(currentUserName))
            {
                return Unauthorized(new { message = "Người dùng chưa đăng nhập." });
            }

            var currentUser = await _db.Khachhangs.FirstOrDefaultAsync(u => u.TaiKhoanKh == currentUserName);
            if (currentUser == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            var currentOrder = await _db.Donhangs.FirstOrDefaultAsync(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);
            if (currentOrder == null)
            {
                return NotFound(new { message = "Không tìm thấy hóa đơn hiện tại." });
            }

            var cartItem = await _db.CtDonhangs.FirstOrDefaultAsync(item => item.MaGiay == request.MaGiay && item.MaDonHang == currentOrder.MaDonHang);
            if (cartItem != null)
            {
                cartItem.SoLuong = request.SoLuong;
                cartItem.ThanhTien = cartItem.GiaLucBan * cartItem.SoLuong;

                // Cập nhật tổng tiền cho hóa đơn
                currentOrder.TongTien = await _db.CtDonhangs
                    .Where(c => c.MaDonHang == currentOrder.MaDonHang)
                    .SumAsync(c => c.ThanhTien);

                await _db.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật số lượng thành công." });
            }
            else
            {
                return NotFound(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }
        }
        #endregion

        #region ThanhToan
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] List<CtDonhang> cartItems)
        {
            var currentUserName = User.Identity.Name;

            var currentUser = await _db.Khachhangs.FirstOrDefaultAsync(u => u.TaiKhoanKh == currentUserName);
            if (currentUser == null)
            {
                return NotFound(new { message = "Không tìm thấy người dùng." });
            }

            var currentOrder = await _db.Donhangs.FirstOrDefaultAsync(o => o.MaKh == currentUser.MaKh && o.TinhTrangGiaoHang == null);

            if (currentOrder != null)
            {
                currentOrder.TinhTrangGiaoHang = false;
                currentOrder.NgayGiao = DateTime.Now.AddDays(3);
                await _db.SaveChangesAsync();
            }

            currentOrder = new Donhang
            {
                MaKh = currentUser.MaKh,
                NgayDat = DateTime.Now,
                TongTien = 0,
                TinhTrangGiaoHang = null
            };
            await _db.Donhangs.AddAsync(currentOrder);
            await _db.SaveChangesAsync();

            if (cartItems != null && cartItems.Any())
            {
                // Xử lý cartItems để thêm vào chi tiết hóa đơn nếu cần thiết
                return Ok(new { success = true, message = "Đặt hàng thành công." });
            }

            return BadRequest(new { message = "Không có sản phẩm trong giỏ hàng." });
        }
        #endregion
    }

    public class AddToCartRequest
    {
        public int MaGiay { get; set; }
        public int SoLuong { get; set; }
    }

    public class UpdateQuantityRequest
    {
        public int MaGiay { get; set; }
        public int SoLuong { get; set; }
    }
}
