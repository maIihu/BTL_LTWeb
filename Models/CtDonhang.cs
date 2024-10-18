using System;
using System.Collections.Generic;

namespace web1.Models;

public partial class CtDonhang
{
    public int MaDonHang { get; set; }

    public int MaGiay { get; set; }

    public decimal GiaLucBan { get; set; }

    public int SoLuong { get; set; }

    public decimal? ThanhTien { get; set; }

    public virtual Donhang MaDonHangNavigation { get; set; } = null!;

    public virtual Sanpham MaGiayNavigation { get; set; } = null!;
}
