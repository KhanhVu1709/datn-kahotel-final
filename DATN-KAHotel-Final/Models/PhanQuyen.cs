using System;
using System.Collections.Generic;

namespace DATN_KAHotel_Final.Models;

public partial class PhanQuyen
{
    public int Id { get; set; }

    public int? IdQuyen { get; set; }

    public int? IdTaiKhoan { get; set; }

    public virtual Quyen? IdQuyenNavigation { get; set; }

    public virtual TaiKhoan? IdTaiKhoanNavigation { get; set; }
}
