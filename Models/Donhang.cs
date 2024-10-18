using System;
using System.Collections.Generic;

namespace web1.Models;

public partial class Donhang
{
    public int MaDonHang { get; set; }

    public bool? TinhTrangGiaoHang { get; set; }

    public DateTime? NgayDat { get; set; }

    public DateTime? NgayGiao { get; set; }

    public decimal? TongTien { get; set; }

    public int? MaKh { get; set; }

    public virtual ICollection<CtDonhang> CtDonhangs { get; set; } = new List<CtDonhang>();

    public virtual Khachhang? MaKhNavigation { get; set; }
}
