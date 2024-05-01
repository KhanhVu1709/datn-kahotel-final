using System;
using System.Collections.Generic;

namespace DATN_KAHotel_Final.Models;

public partial class TrangThaiDon
{
    public int Id { get; set; }

    public string? TenTrangThai { get; set; }

    public virtual ICollection<DatPhong> DatPhongs { get; set; } = new List<DatPhong>();
}
