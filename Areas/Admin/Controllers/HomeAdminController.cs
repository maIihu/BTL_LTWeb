using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using web1.Models;

namespace web1.Areas.Admin.Controllers
{
    //[Authorize] // đảm bảo đăng nhập mới vào được
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        private readonly ShopGiayContext _db;

        public HomeAdminController(ShopGiayContext db)
        {
            _db = db;
        }
        [Route("")]
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
