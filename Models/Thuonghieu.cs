using System;
using System.Collections.Generic;

namespace web1.Models;

public partial class Thuonghieu
{
    public int MaThuongHieu { get; set; }

    public string TenThuongHieu { get; set; } = null!;

    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
