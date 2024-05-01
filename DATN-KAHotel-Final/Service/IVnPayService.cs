using DATN_KAHotel_Final.Models;

namespace DATN_KAHotel_Final.Service
{
    public interface IVnPayService
    {
        // CreatePaymentUrl sẽ có trách nhiệm tạo ra URL thanh toán tại VnPay
        string CreatePaymentUrl(HttpContext context, PaymentInfomationModel model);

        // PaymentExecute sẽ thực kiểm tra thông tin giao dịch và sẽ lưu lại thông tin đó sau khi thanh toán thành công.
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
