using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DATN_KAHotel_Final.Models;

public partial class QlksContext : DbContext
{
    public QlksContext()
    {
    }

    public QlksContext(DbContextOptions<QlksContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DanhGium> DanhGia { get; set; }

    public virtual DbSet<DatPhong> DatPhongs { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<GiaoDich> GiaoDiches { get; set; }

    public virtual DbSet<HinhAnh> HinhAnhs { get; set; }

    public virtual DbSet<KhachSan> KhachSans { get; set; }

    public virtual DbSet<KhachSanTienNghi> KhachSanTienNghis { get; set; }

    public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }

    public virtual DbSet<PhanQuyen> PhanQuyens { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<PhongTienNghi> PhongTienNghis { get; set; }

    public virtual DbSet<Quyen> Quyens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<TienNghi> TienNghis { get; set; }

    public virtual DbSet<TinhThanh> TinhThanhs { get; set; }

    public virtual DbSet<TrangThaiDon> TrangThaiDons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-MP2EPV8\\SQLEXPRESS;Initial Catalog=QLKS;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DanhGium>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.HoTen).HasMaxLength(255);

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("FK_DanhGia_TaiKhoan");
        });

        modelBuilder.Entity<DatPhong>(entity =>
        {
            entity.ToTable("DatPhong");

            entity.Property(e => e.BatDau).HasColumnType("datetime");
            entity.Property(e => e.KetThuc).HasColumnType("datetime");
            entity.Property(e => e.NgayDat).HasColumnType("datetime");
            entity.Property(e => e.ThanhToan).HasMaxLength(100);

            entity.HasOne(d => d.IdPhongNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.IdPhong)
                .HasConstraintName("FK_DatPhong_Phong");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("FK_DatPhong_TaiKhoan");

            entity.HasOne(d => d.IdTrangThaiNavigation).WithMany(p => p.DatPhongs)
                .HasForeignKey(d => d.IdTrangThai)
                .HasConstraintName("FK_DatPhong_TrangThaiDon");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.ToTable("DichVu");

            entity.Property(e => e.GhiChu)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.TenDichVu).HasMaxLength(255);
        });

        modelBuilder.Entity<GiaoDich>(entity =>
        {
            entity.ToTable("GiaoDich");

            entity.Property(e => e.ThoiGianGiaoDich).HasColumnType("datetime");

            entity.HasOne(d => d.IdDichVuNavigation).WithMany(p => p.GiaoDiches)
                .HasForeignKey(d => d.IdDichVu)
                .HasConstraintName("FK_GiaoDich_DichVu");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.GiaoDiches)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("FK_GiaoDich_TaiKhoan");
        });

        modelBuilder.Entity<HinhAnh>(entity =>
        {
            entity.ToTable("HinhAnh");

            entity.Property(e => e.UrlHinhAnh).HasMaxLength(255);

            entity.HasOne(d => d.IdPhongNavigation).WithMany(p => p.HinhAnhs)
                .HasForeignKey(d => d.IdPhong)
                .HasConstraintName("FK_HinhAnh_Phong");
        });

        modelBuilder.Entity<KhachSan>(entity =>
        {
            entity.ToTable("KhachSan");

            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.TenKhachSan).HasMaxLength(255);
            entity.Property(e => e.TieuDe).HasMaxLength(255);

            entity.HasOne(d => d.IdTinhThanhNavigation).WithMany(p => p.KhachSans)
                .HasForeignKey(d => d.IdTinhThanh)
                .HasConstraintName("FK_KhachSan_TinhThanh");
        });

        modelBuilder.Entity<KhachSanTienNghi>(entity =>
        {
            entity.ToTable("KhachSan_TienNghi");

            entity.HasOne(d => d.IdKhachSanNavigation).WithMany(p => p.KhachSanTienNghis)
                .HasForeignKey(d => d.IdKhachSan)
                .HasConstraintName("FK_KhachSan_TienNghi_KhachSan");

            entity.HasOne(d => d.IdTienNghiNavigation).WithMany(p => p.KhachSanTienNghis)
                .HasForeignKey(d => d.IdTienNghi)
                .HasConstraintName("FK_KhachSan_TienNghi_TienNghi");
        });

        modelBuilder.Entity<LoaiPhong>(entity =>
        {
            entity.ToTable("LoaiPhong");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.TenLoai).HasMaxLength(255);
        });

        modelBuilder.Entity<PhanQuyen>(entity =>
        {
            entity.ToTable("PhanQuyen");

            entity.HasOne(d => d.IdQuyenNavigation).WithMany(p => p.PhanQuyens)
                .HasForeignKey(d => d.IdQuyen)
                .HasConstraintName("FK_PhanQuyen_Quyen");

            entity.HasOne(d => d.IdTaiKhoanNavigation).WithMany(p => p.PhanQuyens)
                .HasForeignKey(d => d.IdTaiKhoan)
                .HasConstraintName("FK_PhanQuyen_TaiKhoan");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.ToTable("Phong");

            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.IdViTri).HasColumnName("Id_ViTri");
            entity.Property(e => e.TenPhong).HasMaxLength(255);

            entity.HasOne(d => d.IdKhachSanNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.IdKhachSan)
                .HasConstraintName("FK_Phong_KhachSan");

            entity.HasOne(d => d.IdLoaiPhongNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.IdLoaiPhong)
                .HasConstraintName("FK_Phong_LoaiPhong");
        });

        modelBuilder.Entity<PhongTienNghi>(entity =>
        {
            entity.ToTable("Phong_TienNghi");

            entity.HasOne(d => d.IdPhongNavigation).WithMany(p => p.PhongTienNghis)
                .HasForeignKey(d => d.IdPhong)
                .HasConstraintName("FK_Phong_TienNghi_Phong");

            entity.HasOne(d => d.IdTienNghiNavigation).WithMany(p => p.PhongTienNghis)
                .HasForeignKey(d => d.IdTienNghi)
                .HasConstraintName("FK_Phong_TienNghi_TienNghi");
        });

        modelBuilder.Entity<Quyen>(entity =>
        {
            entity.ToTable("Quyen");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.TenQuyen).HasMaxLength(255);
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.ToTable("TaiKhoan");

            entity.Property(e => e.Anh).HasMaxLength(255);
            entity.Property(e => e.DiaChi)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Facebook).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(50);
            entity.Property(e => e.Instagram).HasMaxLength(255);
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NgaySinh).HasColumnType("date");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Twitter).HasMaxLength(255);
        });

        modelBuilder.Entity<TienNghi>(entity =>
        {
            entity.ToTable("TienNghi");

            entity.Property(e => e.IconTienNghi).HasMaxLength(255);
            entity.Property(e => e.TenTienNghi).HasMaxLength(255);
        });

        modelBuilder.Entity<TinhThanh>(entity =>
        {
            entity.ToTable("TinhThanh");

            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.TenTinh).HasMaxLength(255);
        });

        modelBuilder.Entity<TrangThaiDon>(entity =>
        {
            entity.ToTable("TrangThaiDon");

            entity.Property(e => e.TenTrangThai).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
