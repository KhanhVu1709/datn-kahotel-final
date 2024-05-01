using System;
using System.Collections.Generic;

namespace DATN_KAHotel_Final.Models;

public partial class TaiKhoan
{
    public int Id { get; set; }

    public string? TenDangNhap { get; set; }

    public string? MatKhau { get; set; }

    public string? SoDienThoai { get; set; }

    public DateTime? NgaySinh { get; set; }

    public bool? TrangThai { get; set; }

    public string? Anh { get; set; }

    public string? HoTen { get; set; }

    public string? Email { get; set; }

    public bool? IsDelete { get; set; }

    public string? DiaChi { get; set; }

    public string? GhiChu { get; set; }

    public string? Facebook { get; set; }

    public string? Twitter { get; set; }

    public string? Instagram { get; set; }
    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();

    public virtual ICollection<GiaoDich> GiaoDiches { get; set; } = new List<GiaoDich>();

    public virtual ICollection<PhanQuyen> PhanQuyens { get; set; } = new List<PhanQuyen>();
}
