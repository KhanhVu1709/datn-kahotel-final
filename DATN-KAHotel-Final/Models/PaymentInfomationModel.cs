namespace DATN_KAHotel_Final.Models
{
    public class PaymentInfomationModel
    {
        public string Name { get; set; }
        public int OrderId { get; set; }
        public double Amount { get; set; }
        public string OrderDescription { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
