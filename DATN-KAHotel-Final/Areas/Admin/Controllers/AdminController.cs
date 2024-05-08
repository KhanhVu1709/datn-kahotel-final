using DATN_KAHotel_Final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DATN_KAHotel_Final.Areas.Admin.Controllers
{
    [Area("admin")]
    //[Authorize(Roles = "Quản lý khách hàng, Quản lý đơn đặt, Quản lý phòng, Quản lý khách sạn", Policy = "AnyRole")]
    public class AdminController : Controller
    {
        QlksContext db = new QlksContext();
        public IActionResult Index()
        {
            return View();
        }

        // biểu đồ cột: tính doanh thu
        public IActionResult DoanhThu(int? yearSelected)
        {
            // Lấy danh sách các đơn đặt phòng từ cơ sở dữ liệu
            var datPhongs = db.DatPhongs.Where(d => d.NgayDat.Value.Year == yearSelected).ToList();

            // Tạo một dictionary để lưu trữ tổng doanh thu của mỗi tháng
            Dictionary<int, decimal> monthlyRevenue = new Dictionary<int, decimal>();

            // Lặp qua từng đơn đặt phòng để tính tổng doanh thu của mỗi tháng
            foreach (var datPhong in datPhongs)
            {
                // chỉ tính doanh thu với những đơn có trạng thái là "Hoàn tất"
                if(datPhong.IdTrangThai == 3)
                {
                    // Kiểm tra xem ngày đặt có hợp lệ không
                    if (datPhong.NgayDat.HasValue)
                    {
                        // Lấy thông tin tháng và năm từ ngày đặt
                        int month = datPhong.NgayDat.Value.Month;
                        int year = datPhong.NgayDat.Value.Year;

                        // Tạo một khóa để nhận dạng mỗi tháng
                        int monthKey = year * 100 + month;

                        // Kiểm tra xem tổng doanh thu của tháng đã được tính trước đó chưa
                        if (!monthlyRevenue.ContainsKey(monthKey))
                        {
                            // Nếu chưa, thêm một bản ghi mới với giá trị ban đầu là 0
                            monthlyRevenue[monthKey] = 0;
                        }

                        // Cộng tổng doanh thu của tháng
                        monthlyRevenue[monthKey] += (decimal)datPhong.TongTien;
                    }
                }
            }

            // Tạo danh sách dữ liệu để trả về dưới dạng JSON
            List<object> dataPoints = new List<object>();

            // Lặp qua từng tháng trong năm và tạo dữ liệu tương ứng
            for (int month = 1; month <= 12; month++)
            {
                // Tạo một khóa để nhận dạng mỗi tháng
                int monthKey = DateTime.Now.Year * 100 + month;

                // Kiểm tra xem có dữ liệu doanh thu cho tháng này không
                if (monthlyRevenue.ContainsKey(monthKey))
                {
                    // Nếu có, thêm dữ liệu doanh thu của tháng vào danh sách
                    dataPoints.Add(new { x = "Tháng " + month, y = monthlyRevenue[monthKey] });
                }
                else
                {
                    // Nếu không, thêm dữ liệu doanh thu 0 cho tháng vào danh sách
                    dataPoints.Add(new { x = "Tháng " + month, y = 0 });
                }
            }

            // Trả về danh sách dữ liệu dưới dạng JSON
            return Json(dataPoints);
        }

        // biểu đồ tròn: trạng thái đơn
        public IActionResult GetOrderStatusData(int? yearSelected)
        {
            // Truy vấn dữ liệu từ bảng DatPhong và TrangThaiDon để lấy số lượng đơn theo từng trạng thái
            var orderStatusData = from tt in db.TrangThaiDons
                                  join dp in db.DatPhongs.Where(dp => dp.NgayDat.HasValue && dp.NgayDat.Value.Year == yearSelected) on tt.Id equals dp.IdTrangThai into gj
                                  from subDp in gj.DefaultIfEmpty()
                                  group subDp by tt.Id into g
                                  select new
                                  {
                                      TrangThaiId = g.Key,
                                      Count = g.Count(subDp => subDp != null)
                                  };

            // Tạo một danh sách để lưu trữ số lượng đơn theo từng trạng thái
            int[] orderCounts = new int[4]; // Giả sử có 4 trạng thái (cần điều chỉnh nếu có nhiều hoặc ít hơn)

            // Ánh xạ trạng thái Id với tên tương ứng của trạng thái
            Dictionary<int, string> statusMap = new Dictionary<int, string>
                                                {
                                                    { 1, "Chưa xác nhận" },
                                                    { 2, "Đã xác nhận" },
                                                    { 3, "Hoàn tất" },
                                                    { 4, "Huỷ bỏ" }
                                                };

            List<object> orderStatusList = new List<object>();

            // Lặp qua dữ liệu đã nhóm để cập nhật số lượng đơn theo từng trạng thái vào danh sách
            foreach (var data in orderStatusData)
            {
                string statusName = statusMap.ContainsKey(data.TrangThaiId) ? statusMap[data.TrangThaiId] : "Unknown";
                orderStatusList.Add(new { x = statusName, y = data.Count });
            }

            // Trả về dữ liệu dưới dạng JSON để sử dụng trong JavaScript
            return Json(orderStatusList);
        }


    }
}
