using web1.Models;

namespace web1.Repository
{
    public class ThuongHieuRepository : IThuongHieuRepository
    {
        private readonly ShopGiayContext _context;

        public ThuongHieuRepository(ShopGiayContext context)
        {
            _context = context;
        }

        public Thuonghieu Add(Thuonghieu thuonghieu)
        {
            throw new NotImplementedException();
        }

        public Thuonghieu Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Thuonghieu> GetAll()
        {
            return _context.Thuonghieus;
        }

        public Thuonghieu GetThuongHieu(int id)
        {
            throw new NotImplementedException();
        }

        public Thuonghieu Update(Thuonghieu thuonghieu)
        {
            throw new NotImplementedException();
        }
    }
}
