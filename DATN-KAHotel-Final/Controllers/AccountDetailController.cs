using DATN_KAHotel_Final.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace DATN_KAHotel_Final.Controllers
{
    public class AccountDetailController : Controller
    {
        QlksContext db = new QlksContext();
        public IActionResult ThongTinTaiKhoan(int? id)
        {
            TaiKhoan tai_khoan = db.TaiKhoans.FirstOrDefault(t => t.Id == id);
            return View(tai_khoan);
        }

        [HttpPost]
        public IActionResult CapNhatTaiKhoan(int? id, IFormCollection fc)
        {
            TaiKhoan tai_khoan = db.TaiKhoans.FirstOrDefault(t => t.Id == id);

            var ho_ten = fc["hoTen"].ToString();
            var email = fc["email"].ToString();
            var dia_chi = fc["diaChi"].ToString();
            var sdt = fc["sdt"].ToString();
            var ghi_chu = fc["ghiChu"].ToString();
            DateTime ngay_sinh = Convert.ToDateTime(fc["ngaySinh"]);

            var facebook = fc["facebook"].ToString();
            var twitter = fc["twitter"].ToString();
            var instagram = fc["instagram"].ToString();

            string filename = "";
            // kiểm tra và lấy tệp tin
            try
            {
                filename = Request.Form.Files[0].FileName;
            }
            catch {; }
            if (!string.IsNullOrEmpty(filename))
            {
                // Tạo một timestamp để thêm vào tên tệp tin
                var timestamp = DateTime.Now.ToFileTime();
                filename = timestamp + "_" + filename;
                // Xác định đường dẫn lưu tệp tin
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/taikhoan", filename);
                // Lưu tệp tin vào đường dẫn vừa xác định
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Anh trong csdl
                tai_khoan.Anh = filename;
            }

            tai_khoan.HoTen = ho_ten;
            tai_khoan.Email = email;
            tai_khoan.SoDienThoai = sdt;
            tai_khoan.NgaySinh = ngay_sinh;
            tai_khoan.DiaChi = dia_chi;
            tai_khoan.GhiChu = ghi_chu;
            tai_khoan.Facebook = facebook;
            tai_khoan.Twitter = twitter;
            tai_khoan.Instagram = instagram;

            db.TaiKhoans.Update(tai_khoan);
            db.SaveChanges();

            return Redirect("/accountdetail/thongtintaikhoan?id="+id);
        }
    }
}
