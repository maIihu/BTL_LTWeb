using System;
using System.Collections.Generic;

namespace web1.Models;

public partial class Sanpham
{
    public int MaGiay { get; set; }

    public string TenGiay { get; set; } = null!;

    public byte Size { get; set; }

    public string? AnhBia { get; set; }

    public decimal GiaBan { get; set; }

    public int? MaThuongHieu { get; set; }

    public bool? TrangThai { get; set; }

    public int? MaNcc { get; set; }

    public int? MaLoai { get; set; }

    public int? ThoiGianBaoHanh { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public int SoLuongTon { get; set; }

    public virtual ICollection<CtDonhang> CtDonhangs { get; set; } = new List<CtDonhang>();

    public virtual Loaigiay? MaLoaiNavigation { get; set; }

    public virtual Nhacungcap? MaNccNavigation { get; set; }

    public virtual Thuonghieu? MaThuongHieuNavigation { get; set; }
    
    public Boolean YeuThich {  get; set; }
}
