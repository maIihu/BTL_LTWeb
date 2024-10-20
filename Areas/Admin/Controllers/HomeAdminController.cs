using Microsoft.AspNetCore.Mvc;

namespace web1.Areas.Admin.Controllers
{
    public class HomeAdminController : Controller
    {
        [Area("admin")]
        [Route("admin")]
        [Route("admin/homeadmin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
