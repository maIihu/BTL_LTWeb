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
                var cartItem = new CtDonhang
                {
                    MaGiay = product.MaGiay,
                    SoLuong = SoLuong,
                    GiaLucBan = product.GiaBan,
                    ThanhTien = product.GiaBan * SoLuong
                };

                List<CtDonhang> cart = HttpContext.Session.Get<List<CtDonhang>>("Cart") ?? new List<CtDonhang>();

                cart.Add(cartItem);

                HttpContext.Session.Set("Cart", cart);
                return RedirectToAction("GioHang");
            }

            return RedirectToAction("ChiTietGiay", new { maSp = MaGiay });
        }

    }
}
