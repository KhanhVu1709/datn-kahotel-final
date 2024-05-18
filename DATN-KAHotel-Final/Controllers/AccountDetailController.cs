using DATN_KAHotel_Final.Areas.Admin.Models;
using DATN_KAHotel_Final.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;
using System.Net.WebSockets;
using System.Runtime.Intrinsics.Arm;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DATN_KAHotel_Final.Controllers
{
    public class AccountDetailController : Controller
    {
        QlksContext db = new QlksContext();

        // thông tin tài khoản
        public IActionResult ThongTinTaiKhoan(int? id)
        {
            TaiKhoan tai_khoan = db.TaiKhoans.FirstOrDefault(t => t.Id == id);
            return View(tai_khoan);
        }

        // cập nhật chỉnh sửa tài khoản
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

        // thay đổi mật khẩu
        public IActionResult DoiMatKhau(int? id)
        {
            TaiKhoan tai_khoan = db.TaiKhoans.FirstOrDefault(t => t.Id == id);
            return View(tai_khoan);
        }

        [HttpPost]
        public IActionResult DoiMatKhau(int? id, IFormCollection fc)
        {
            TaiKhoan tai_khoan = db.TaiKhoans.FirstOrDefault(t => t.Id == id);

            var matKhauCur = fc["matKhauCur"].ToString();
            var matKhauNew = fc["matKhauNew"].ToString();
            var matKhauConf = fc["matKhauConf"].ToString();

            var passwordHasher = new PasswordHasher();

            // Kiểm tra mật khẩu nhập vào có khớp với mật khẩu đã mã hoá hay không
            var passwordVerification = passwordHasher.VerifyHashedPassword(tai_khoan.MatKhau, matKhauCur);

            // Mật khẩu đúng => chuyển hướng đến trang khác
            if (passwordVerification == Microsoft.AspNet.Identity.PasswordVerificationResult.Success)
            {
                if(matKhauNew.Trim() == matKhauCur.Trim())
                {
                    TempData["TrungMK"] = "Mật khẩu mới giống mật khẩu cũ";
                    return Redirect("/accountdetail/doimatkhau?id=" + id);
                } else
                {
                    if (matKhauConf.Trim() != matKhauNew.Trim())
                    {
                        TempData["KhongKhopMK"] = "Mật khẩu mới và xác nhận mật khẩu không khớp nhau";
                        return Redirect("/accountdetail/doimatkhau?id=" + id);
                    }
                    else
                    {
                        string hashedPassword = passwordHasher.HashPassword(matKhauNew);

                        if (ModelState.IsValid)  // thông tin vượt qua được validation
                        {
                            tai_khoan.MatKhau = hashedPassword;
                            db.TaiKhoans.Update(tai_khoan);
                            db.SaveChanges();

                            return Redirect("/accountdetail/doimatkhau?id=" + id);
                        }
                    }
                }
            }
            else
            {
                // Mật khẩu không khớp hoặc không tìm thấy người dùng
                TempData["SaiMK"] = "Sai mật khẩu";
                return Redirect("/accountdetail/doimatkhau?id=" + id);
            }

            return View();
        }

        // Lịch sử đặt phòng của khách hàng
        public IActionResult LichSuDatPhong(int? id, int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            TaiKhoan tai_khoan = db.TaiKhoans.FirstOrDefault(t => t.Id == id);

            ViewBag.tai_khoan = tai_khoan;

            var linq = (from t in db.TaiKhoans
                        join dp in db.DatPhongs on t.Id equals dp.IdTaiKhoan
                        join tt in db.TrangThaiDons on dp.IdTrangThai equals tt.Id
                        join p in db.Phongs on dp.IdPhong equals p.Id
                        join ks in db.KhachSans on p.IdKhachSan equals ks.Id
                        where t.Id == id
                        orderby dp.Id descending
                        select new ChiTietGiaoDich
                        {
                            IdHoaDon = dp.Id,
                            IdTaiKhoan = t.Id,
                            HoTen = t.HoTen,
                            Email = t.Email,
                            Sdt = t.SoDienThoai,
                            BatDau = dp.BatDau,
                            KetThuc = dp.KetThuc,
                            TrangThai = tt.TenTrangThai,
                            TongTien = dp.TongTien,
                            AnhDaiDienPhong = p.AnhDaiDien,
                            SoNguoi = p.SoNguoi,
                            IdKhachSan = ks.Id,
                            TenKhachSan = ks.TenKhachSan,
                            TenPhong = p.TenPhong,
                        }).ToList();
            //ViewBag.lichSuDatPhong = linq;
            PagedList<ChiTietGiaoDich> list = new PagedList<ChiTietGiaoDich>(linq, pageNumber, pageSize);

            return View(list);
        }

        public IActionResult YeuCauHuyPhong(int? id, int? idHoaDon)
        {
            DatPhong dat_phong = db.DatPhongs.FirstOrDefault(d => d.Id == idHoaDon);
            dat_phong.IdTrangThai = 4;
            db.DatPhongs.Update(dat_phong);
            db.SaveChanges();
            return Redirect("/accountdetail/lichsudatphong?id=" + id);
        }

    }
}
