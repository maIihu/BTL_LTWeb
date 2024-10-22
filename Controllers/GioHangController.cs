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

        public IActionResult GioHang()
        {
            List<CtDonhang> cart = HttpContext.Session.Get<List<CtDonhang>>("Cart") ?? new List<CtDonhang>();
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int MaGiay, int Size, int SoLuong)
        {
            var product = _db.Sanphams.FirstOrDefault(p => p.MaGiay == MaGiay);

            if (product != null)
            {
                // Lấy giỏ hàng từ session
                List<CtDonhang> cart = HttpContext.Session.Get<List<CtDonhang>>("Cart") ?? new List<CtDonhang>();

                // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
                var existingItem = cart.FirstOrDefault(item => item.MaGiay == MaGiay);

                if (existingItem != null)
                {
                    // Nếu sản phẩm đã có, cộng thêm số lượng
                    existingItem.SoLuong += SoLuong;
                    existingItem.ThanhTien = existingItem.GiaLucBan * existingItem.SoLuong; // Cập nhật tổng tiền
                }
                else
                {
                    // Nếu sản phẩm chưa có, thêm mới vào giỏ hàng
                    var cartItem = new CtDonhang
                    {
                        MaGiay = product.MaGiay,
                        SoLuong = SoLuong,
                        GiaLucBan = product.GiaBan,
                        ThanhTien = product.GiaBan * SoLuong
                    };
                    cart.Add(cartItem); // Thêm mới vào giỏ hàng
                }

                // Lưu lại giỏ hàng vào session
                HttpContext.Session.Set("Cart", cart);

                return Json(new { success = true }); // Trả về thành công
            }

            return Json(new { success = false }); // Trả về thất bại nếu không tìm thấy sản phẩm
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int MaGiay, int MaDonHang)
        {
            List<CtDonhang> cart = HttpContext.Session.Get<List<CtDonhang>>("Cart") ?? new List<CtDonhang>();

            Console.WriteLine($"Xóa sản phẩm với MaGiay: {MaGiay}, MaDonHang: {MaDonHang}");

            var itemToRemove = cart.FirstOrDefault(item => item.MaGiay == MaGiay && item.MaDonHang == MaDonHang);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                Console.WriteLine("Sản phẩm đã được xóa khỏi giỏ hàng.");
            }
            else
            {
                Console.WriteLine("Không tìm thấy sản phẩm để xóa.");
            }

            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction("GioHang");
        }




    }
}
