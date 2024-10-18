using System;
using System.Collections.Generic;

namespace web1.Models;

public partial class Khachhang
{
    public int MaKh { get; set; }

    public string? TaiKhoanKh { get; set; }

    public string MatKhau { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string? EmailKh { get; set; }

    public string? DiaChiKh { get; set; }

    public string? DienThoaiKh { get; set; }

    public DateTime? NgaySinh { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();
}
