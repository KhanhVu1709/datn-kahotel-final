namespace DATN_KAHotel_Final.Models
{
    public class DanhGium
    {
        public int Id { get; set; }

        public int? DiemSachSe { get; set; }

        public int? DiemThoaiMai { get; set; }

        public int? DiemNhanVien { get; set; }

        public int? DiemCoSo { get; set; }

        public string? HoTen { get; set; }

        public string? Email { get; set; }

        public string? NoiDung { get; set; }

        public int? IdTaiKhoan { get; set; }

        public int? IdKhachSan { get; set; }

        public virtual TaiKhoan? IdTaiKhoanNavigation { get; set; }
    }
}
