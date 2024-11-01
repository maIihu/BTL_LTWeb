using Microsoft.AspNetCore.Mvc;
using web1.Models;

namespace web1.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/adminaccount")]
    public class AdminAccountController : Controller
    {
        ShopGiayContext db = new ShopGiayContext();
        private readonly ShopGiayContext _db;

        public AdminAccountController(ShopGiayContext db)
        {
            _db = db;
        }
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("hoso")]
        public IActionResult HoSo()
        {
            if (User.Identity.IsAuthenticated && User.HasClaim("IsAdmin", "true"))
            {
                string adminAccount = User.Identity.Name;
                var admin = _db.Quanlies.FirstOrDefault(a => a.TaiKhoanQl == adminAccount);

                if (admin != null)
                {
                    return View(admin);
                }
            }

            ViewBag.ErrorMessage = "Không tìm thấy thông tin admin.";
            return View(null);
        }

    }
}
