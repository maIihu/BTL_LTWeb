
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web1.Models
{
    public class NhanVien
    {
        public int CusID { get; set; }
        public string HoTen { get; set; }
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Please enter a valid Gmail address.")]
        public string Email { get; set; }
        public string GioiTinh { get; set; }
        public string? Imgpath { get; set; }
        public string? EmpFileName { get; set; }

        [NotMapped]

        public IFormFile Img { get; set; }
    }
    }

