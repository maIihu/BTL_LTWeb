using System;
using System.Collections.Generic;

namespace web1.Models;

public partial class Quanly
{
    public int MaQl { get; set; }

    public string TaiKhoanQl { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string? HoTen { get; set; }

    public string? EmailQl { get; set; }

    public string? DienThoaiQl { get; set; }

    public bool? TrangThai { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<Phanquyen> Phanquyens { get; set; } = new List<Phanquyen>();
}
