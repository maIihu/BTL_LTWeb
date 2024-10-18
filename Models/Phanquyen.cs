using System;
using System.Collections.Generic;

namespace web1.Models;

public partial class Phanquyen
{
    public int MaPq { get; set; }

    public int MaQl { get; set; }

    public bool QlAdmin { get; set; }

    public bool QlNhaCungCap { get; set; }

    public bool QlSanPham { get; set; }

    public bool QlThuongHieu { get; set; }

    public bool QlLoaiGiay { get; set; }

    public bool QlDonHang { get; set; }

    public bool QlKhachHang { get; set; }

    public bool QlYkienKhachHang { get; set; }

    public virtual Quanly MaQlNavigation { get; set; } = null!;
}
