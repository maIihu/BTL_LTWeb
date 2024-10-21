using Microsoft.AspNetCore.Mvc;

namespace web1.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {

        public IActionResult DangNhap()
        {
            return View();
        }
    }
}
