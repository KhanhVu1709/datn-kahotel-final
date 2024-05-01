namespace DATN_KAHotel_Final.Areas.Admin.Models
{
    public class ChiTietGiaoDich
    {
        public int IdHoaDon { get; set; }
        public string? HoTen { get; set; }
        public string? Email { get; set; }
        public string? DiaChi { get; set; }
        public string? Sdt { get; set; }
        //public DateTime? NgayBan { get; set; }
        public string? TrangThai { get; set; }
        public double? TongTien { get; set; }
        public int IdTaiKhoan { get; set; }
        public DateTime? BatDau { get; set; }
        public DateTime? KetThuc { get; set; }

    }
}
