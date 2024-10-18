using web1.Models;

namespace web1.Repository
{
    public interface IThuongHieuRepository
    {
        Thuonghieu Add(Thuonghieu thuonghieu);
        Thuonghieu Update(Thuonghieu thuonghieu);
        Thuonghieu Delete(int id);
        Thuonghieu GetThuongHieu(int id);
        IEnumerable<Thuonghieu> GetAll();
    }
}
