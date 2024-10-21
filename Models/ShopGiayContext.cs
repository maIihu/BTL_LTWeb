using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace web1.Models;

public partial class ShopGiayContext : DbContext
{
    public ShopGiayContext()
    {
    }

    public ShopGiayContext(DbContextOptions<ShopGiayContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CtDonhang> CtDonhangs { get; set; }

    public virtual DbSet<Donhang> Donhangs { get; set; }

    public virtual DbSet<Khachhang> Khachhangs { get; set; }

    public virtual DbSet<Loaigiay> Loaigiays { get; set; }

    public virtual DbSet<Nhacungcap> Nhacungcaps { get; set; }

    public virtual DbSet<Phanquyen> Phanquyens { get; set; }

    public virtual DbSet<Quanly> Quanlies { get; set; }

    public virtual DbSet<Sanpham> Sanphams { get; set; }

    public virtual DbSet<Thuonghieu> Thuonghieus { get; set; }

    public virtual DbSet<Ykienkhachhang> Ykienkhachhangs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=ShopGiay;Integrated Security=True;Connect Timeout=30;Encrypt=True;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CtDonhang>(entity =>
        {
            entity.HasKey(e => new { e.MaDonHang, e.MaGiay });

            entity.ToTable("CT_DONHANG");

            entity.Property(e => e.GiaLucBan).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.CtDonhangs)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DONHANG");

            entity.HasOne(d => d.MaGiayNavigation).WithMany(p => p.CtDonhangs)
                .HasForeignKey(d => d.MaGiay)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SANPHAM_2");
        });

        modelBuilder.Entity<Donhang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang);

            entity.ToTable("DONHANG");

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.NgayDat).HasColumnType("datetime");
            entity.Property(e => e.NgayGiao).HasColumnType("datetime");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.Donhangs)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK_KHACHHANG_2");
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.MaKh);

            entity.ToTable("KHACHHANG");

            entity.HasIndex(e => e.EmailKh, "UQ__KHACHHAN__7ED9643FF0F6FC75").IsUnique();

            entity.HasIndex(e => e.TaiKhoanKh, "UQ__KHACHHAN__9A123AA24F4CEAA8").IsUnique();

            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.DiaChiKh)
                .HasMaxLength(100)
                .HasColumnName("DiaChiKH");
            entity.Property(e => e.DienThoaiKh)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DienThoaiKH");
            entity.Property(e => e.EmailKh)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("EmailKH");
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NgaySinh).HasColumnType("datetime");
            entity.Property(e => e.TaiKhoanKh)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TaiKhoanKH");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<Loaigiay>(entity =>
        {
            entity.HasKey(e => e.MaLoai);

            entity.ToTable("LOAIGIAY");

            entity.Property(e => e.TenLoai).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<Nhacungcap>(entity =>
        {
            entity.HasKey(e => e.MaNcc);

            entity.ToTable("NHACUNGCAP");

            entity.Property(e => e.MaNcc).HasColumnName("MaNCC");
            entity.Property(e => e.DiaChi).HasMaxLength(100);
            entity.Property(e => e.DienThoai)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TenNcc)
                .HasMaxLength(50)
                .HasColumnName("TenNCC");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<Phanquyen>(entity =>
        {
            entity.HasKey(e => e.MaPq);

            entity.ToTable("PHANQUYEN");

            entity.Property(e => e.MaPq).HasColumnName("MaPQ");
            entity.Property(e => e.MaQl).HasColumnName("MaQL");
            entity.Property(e => e.QlAdmin).HasColumnName("QL_Admin");
            entity.Property(e => e.QlDonHang).HasColumnName("QL_DonHang");
            entity.Property(e => e.QlKhachHang).HasColumnName("QL_KhachHang");
            entity.Property(e => e.QlLoaiGiay).HasColumnName("QL_LoaiGiay");
            entity.Property(e => e.QlNhaCungCap).HasColumnName("QL_NhaCungCap");
            entity.Property(e => e.QlSanPham).HasColumnName("QL_SanPham");
            entity.Property(e => e.QlThuongHieu).HasColumnName("QL_ThuongHieu");
            entity.Property(e => e.QlYkienKhachHang).HasColumnName("QL_YKienKhachHang");

            entity.HasOne(d => d.MaQlNavigation).WithMany(p => p.Phanquyens)
                .HasForeignKey(d => d.MaQl)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PHANQUYEN_QUANLY");
        });

        modelBuilder.Entity<Quanly>(entity =>
        {
            entity.HasKey(e => e.MaQl);

            entity.ToTable("QUANLY");

            entity.HasIndex(e => e.EmailQl, "UQ__QUANLY__7ED955FC28028B93").IsUnique();

            entity.HasIndex(e => e.TaiKhoanQl, "UQ__QUANLY__9A120A661952C614").IsUnique();

            entity.Property(e => e.MaQl).HasColumnName("MaQL");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.DienThoaiQl)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DienThoaiQL");
            entity.Property(e => e.EmailQl)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EmailQL");
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TaiKhoanQl)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TaiKhoanQL");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<Sanpham>(entity =>
        {
            entity.HasKey(e => e.MaGiay);

            entity.ToTable("SANPHAM");

            entity.Property(e => e.AnhBia)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.MaNcc).HasColumnName("MaNCC");
            entity.Property(e => e.NgayCapNhat).HasColumnType("datetime");
            entity.Property(e => e.TenGiay).HasMaxLength(50);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.Sanphams)
                .HasForeignKey(d => d.MaLoai)
                .HasConstraintName("FK_LOAIGIAY");

            entity.HasOne(d => d.MaNccNavigation).WithMany(p => p.Sanphams)
                .HasForeignKey(d => d.MaNcc)
                .HasConstraintName("FK_NHACUNGCAP");

            entity.HasOne(d => d.MaThuongHieuNavigation).WithMany(p => p.Sanphams)
                .HasForeignKey(d => d.MaThuongHieu)
                .HasConstraintName("FL_THUONGHIEU");
        });

        modelBuilder.Entity<Thuonghieu>(entity =>
        {
            entity.HasKey(e => e.MaThuongHieu);

            entity.ToTable("THUONGHIEU");

            entity.Property(e => e.TenThuongHieu).HasMaxLength(50);
        });

        modelBuilder.Entity<Ykienkhachhang>(entity =>
        {
            entity.HasKey(e => e.Maykien).HasName("PK_BAIDANHGIA");

            entity.ToTable("YKIENKHACHHANG");

            entity.Property(e => e.Maykien).HasColumnName("MAYKIEN");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.HoTen)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
