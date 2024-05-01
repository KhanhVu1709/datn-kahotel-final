using System;
using System.Collections.Generic;

namespace DATN_KAHotel_Final.Models;

public partial class GiaoDich
{
    public int Id { get; set; }

    public int? IdDatPhong { get; set; }

    public int? IdTaiKhoan { get; set; }

    public int? IdDichVu { get; set; }

    public int? SoLuong { get; set; }

    public DateTime? ThoiGianGiaoDich { get; set; }

    public virtual DichVu? IdDichVuNavigation { get; set; }

    public virtual TaiKhoan? IdTaiKhoanNavigation { get; set; }
}
