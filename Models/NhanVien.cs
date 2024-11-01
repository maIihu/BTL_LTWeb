
using System.ComponentModel.DataAnnotations.Schema;

namespace web1.Models
{
    public class NhanVien
    {
        public int CusID { get; set; }
        public string HoTen { get; set; }
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string Email { get; set; }
        public string GioiTinh { get; set; }
        public string? Imgpath { get; set; }
        public string? EmpFileName { get; set; }

        [NotMapped]

        public IFormFile Img { get; set; }
    }
    }

