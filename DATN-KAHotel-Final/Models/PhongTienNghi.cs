namespace DATN_KAHotel_Final.Models
{
    public class PhongTienNghi
    {
        public int Id { get; set; }

        public int? IdPhong { get; set; }

        public int? IdTienNghi { get; set; }

        public virtual Phong? IdPhongNavigation { get; set; }

        public virtual TienNghi? IdTienNghiNavigation { get; set; }
    }
}
