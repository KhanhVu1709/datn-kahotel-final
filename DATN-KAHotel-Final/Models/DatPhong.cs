using System;
using System.Collections.Generic;

namespace DATN_KAHotel_Final.Models;

public partial class DatPhong
{
    public int Id { get; set; }

    public int? IdPhong { get; set; }

    public int? IdTaiKhoan { get; set; }

    public DateTime? BatDau { get; set; }

    public DateTime? KetThuc { get; set; }

    public double? TongTien { get; set; }

    public string? ThanhToan { get; set; }

    public bool? Status { get; set; }

    public int? IdTrangThai { get; set; }

    public DateTime? NgayDat { get; set; }

    public string? HoVaTen { get; set; }

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string? TinhTrangKh { get; set; }

    public string? MaBuuDien { get; set; }

    public string? GhiChu { get; set; }

    public virtual Phong? IdPhongNavigation { get; set; }

    public virtual TaiKhoan? IdTaiKhoanNavigation { get; set; }

    public virtual TrangThaiDon? IdTrangThaiNavigation { get; set; }
}
