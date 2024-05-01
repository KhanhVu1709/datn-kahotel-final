using System;
using System.Collections.Generic;

namespace DATN_KAHotel_Final.Models;

public partial class Quyen
{
    public int Id { get; set; }

    public string? TenQuyen { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<PhanQuyen> PhanQuyens { get; set; } = new List<PhanQuyen>();
}
