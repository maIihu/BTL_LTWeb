using Microsoft.AspNetCore.Mvc;
using web1.Repository;

namespace web1.ViewComponents
{
    public class ThuongHieuMenuViewComponent : ViewComponent
    {
        private readonly IThuongHieuRepository _thuongHieu;
        public ThuongHieuMenuViewComponent(IThuongHieuRepository thuongHieu)
        {
            _thuongHieu = thuongHieu;
        }
        public IViewComponentResult Invoke()
        {
            var thuongHieu = _thuongHieu.GetAll().OrderBy(X => X.TenThuongHieu);
            return View(thuongHieu);
        }
    }
}
