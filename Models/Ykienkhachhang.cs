using System;
using System.Collections.Generic;

namespace web1.Models;

public partial class Ykienkhachhang
{
    public int Maykien { get; set; }

    public string? Email { get; set; }

    public string HoTen { get; set; } = null!;

    public DateOnly? NgayGui { get; set; }

    public string NoiDung { get; set; } = null!;
}
