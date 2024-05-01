namespace DATN_KAHotel_Final.Models
{
    public class KhachSanTienNghi
    {
        public int Id { get; set; }

        public int? IdKhachSan { get; set; }

        public int? IdTienNghi { get; set; }

        public virtual KhachSan? IdKhachSanNavigation { get; set; }

        public virtual TienNghi? IdTienNghiNavigation { get; set; }
    }
}
