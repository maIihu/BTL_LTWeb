using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web1.Models;

public partial class Khachhang
{
    public int MaKh { get; set; }
    [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
    public string TaiKhoanKh { get; set; } = null!;
    [Required(ErrorMessage = "Mật khẩu không được để trống.")]
    public string MatKhau { get; set; } = null!;


    [NotMapped] // Thuộc tính này sẽ không được lưu vào cơ sở dữ liệu
    [Compare("MatKhau", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
    [Required(ErrorMessage = "Bạn cần nhập lại mật khẩu.")]
    public string MatKhauNhapLai { get; set; } = null!;

    [Required(ErrorMessage = "Họ tên là bắt buộc.")]
    public string HoTen { get; set; } = null!;
    [Required(ErrorMessage = "Bạn cần nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    public string EmailKh { get; set; } = null!;
    [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
    public string DiaChiKh { get; set; } = null!;
    [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
    public string DienThoaiKh { get; set; } = null!;
    [DataType(DataType.Date)]
    public DateTime NgaySinh { get; set; }

    public bool? TrangThai { get; set; }
    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();
}
