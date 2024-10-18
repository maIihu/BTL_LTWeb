using System;
using System.Collections.Generic;

namespace web1.Models;

public partial class Loaigiay
{
    public int MaLoai { get; set; }

    public string TenLoai { get; set; } = null!;

    public bool? TrangThai { get; set; }

    public bool? GioiTinh { get; set; }

    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
