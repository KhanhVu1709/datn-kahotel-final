using System;
using System.Collections.Generic;

namespace DATN_KAHotel_Final.Models;

public partial class KhachSan
{
    public int Id { get; set; }

    public int? IdTinhThanh { get; set; }

    public string? TenKhachSan { get; set; }

    public string? DiaChi { get; set; }

    public string? GioiThieu { get; set; }

    public string? TieuDe { get; set; }

    public string? GhiChu { get; set; }

    public string? AnhDaiDien { get; set; }

    public bool? IsDelete { get; set; }

    public int? SoSao { get; set; }

    public virtual TinhThanh? IdTinhThanhNavigation { get; set; }

    public virtual ICollection<KhachSanTienNghi> KhachSanTienNghis { get; set; } = new List<KhachSanTienNghi>();

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
