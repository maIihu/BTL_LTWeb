using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web1.Models;

public partial class Khachhang
{
    public int MaKh { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    public string TaiKhoanKh { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    public string MatKhau { get; set; } = null!;

    [Required(ErrorMessage = "Nhập lại mật khẩu là bắt buộc")]
    [DataType(DataType.Password)]
    [Compare("MatKhau", ErrorMessage = "Mật khẩu và nhập lại mật khẩu không khớp.")]
    [NotMapped]
    public string NhapLaiMatKhau { get; set; } = null!;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    public string HoTen { get; set; } = null!;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string EmailKh { get; set; } = null!;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    public string DiaChiKh { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string DienThoaiKh { get; set; } = null!;

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    [DataType(DataType.Date)]
    public DateTime NgaySinh { get; set; }

    public bool? TrangThai { get; set; }
    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();
}
