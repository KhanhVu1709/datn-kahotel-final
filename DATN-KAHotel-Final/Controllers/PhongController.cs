using Microsoft.AspNetCore.Mvc;
using DATN_KAHotel_Final.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNet.Identity;
using DATN_KAHotel_Final.Service;
using System.Linq;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;

namespace DATN_KAHotel_Final.Controllers
{
    public class PhongController : Controller
    {
        // khoi tao doi tuong db
        QlksContext db = new QlksContext();

        private readonly IVnPayService _vnPayService;

        public PhongController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;

        }

        [HttpGet]
        public IActionResult GoiYTimKiem(string query)
        {
            var kq = db.TinhThanhs.Where(x => x.TenTinh.Contains(query)).ToList();
            return Json(kq);
        }

        // ham tim kiem phong theo idTinh va ngayDen - ngayDi
        public IActionResult TimKiemPhong(Searching search, int? page)
        {
            // lấy thông tin đã nhập để tìm kiếm
            DateTime? bat_dau = search.ngayDen;
            string batDau = bat_dau.ToString();

            DateTime? ket_thuc = search.ngayDi;
            string ketThuc = ket_thuc.ToString();
            int idTinh = search.IdTinh;

            string tenTinh = db.TinhThanhs.Where(t => t.Id == idTinh).Select(a => a.TenTinh).FirstOrDefault();
            ViewBag.TenTinh = tenTinh;
            // lưu checkout checkin vào session
            HttpContext.Session.SetInt32("idTinh", idTinh);
            HttpContext.Session.SetString("bat_dau", batDau);
            HttpContext.Session.SetString("ket_thuc", ketThuc);

            IEnumerable<KhachSan> result;

            if (bat_dau != null && ket_thuc != null)
            {
                string sql = "EXEC sp_LayKhachSanCoPhongTrong @IdTinhThanh, @StartDate, @EndDate";
                var parameters = new[]
                {
                    new SqlParameter("@IdTinhThanh", idTinh),
                    new SqlParameter("@StartDate", bat_dau),
                    new SqlParameter("@EndDate", ket_thuc)
                };
                result = db.KhachSans.FromSqlRaw(sql, parameters).ToList();
            }
            else
            {
                result = db.KhachSans.Where(x => x.IdTinhThanh == idTinh).ToList();
            }

            return View(result);
        }

        // lọc khách sạn theo thanh filter
        [HttpPost]
        public IActionResult LocKhachSan(int minPrice, int maxPrice, int city, string[] ratings, string? start, string? end)
        {
            if(start != null && end != null)
            {
                HttpContext.Session.SetString("bat_dau", start);
                HttpContext.Session.SetString("ket_thuc", end);
            }
            if(city != null)
            {
                HttpContext.Session.SetInt32("idTinh", city);
            } 
            DateTime? bat_dau = Convert.ToDateTime(HttpContext.Session.GetString("bat_dau"));
            DateTime? ket_thuc = Convert.ToDateTime(HttpContext.Session.GetString("ket_thuc"));

            IEnumerable<KhachSan> result;

            if (bat_dau != null && ket_thuc != null)
            {
                string sql = "EXEC sp_LayKhachSanCoPhongTrong @IdTinhThanh, @StartDate, @EndDate";
                var parameters = new[]
                {
                    new SqlParameter("@IdTinhThanh", city),
                    new SqlParameter("@StartDate", bat_dau),
                    new SqlParameter("@EndDate", ket_thuc)
                };
                result = db.KhachSans.FromSqlRaw(sql, parameters).ToList();
            }
            else
            {
                result = db.KhachSans.Where(x => x.IdTinhThanh == city).ToList();
            }

            foreach(var item in result)
            {
                var phong = db.Phongs.Where(x => x.IdKhachSan == item.Id).ToList();
                item.Phongs = phong;
            }

            // Tính giá trung bình của các phòng cho mỗi khách sạn
            var avgPrices = result.Select(khachSan => new
            {
                KhachSan = khachSan,
                AvgGiaPhong = khachSan.Phongs.Average(phong => phong.GiaPhong)
            }).ToList();

            // Lọc danh sách khách sạn dựa trên giá trung bình đã tính
            var filteredResult = avgPrices.Where(x => x.AvgGiaPhong >= minPrice && x.AvgGiaPhong <= maxPrice)
                                          .Select(x => x.KhachSan)
                                          .ToList();

            if (ratings != null && ratings.Length > 0)
            {
                filteredResult = filteredResult.Where(khachSan => ratings.Contains(khachSan.SoSao.ToString())).ToList();
            }

            return PartialView("_HotelList", filteredResult);
        }

        //
        //[HttpGet]
        //public IActionResult LoadMoreData(int currentPage, int pageSize)
        //{
        //    // lấy thông tin đã nhập để tìm kiếm
        //    DateTime? bat_dau = Convert.ToDateTime(HttpContext.Session.GetString("bat_dau"));
        //    string? batDau = HttpContext.Session.GetString("bat_dau");

        //    DateTime? ket_thuc = Convert.ToDateTime(HttpContext.Session.GetString("ket_thuc"));
        //    string? ketThuc = HttpContext.Session.GetString("ket_thuc");
        //    int? idTinh = HttpContext.Session.GetInt32("idTinh");

        //    string? tenTinh = db.TinhThanhs.Where(t => t.Id == idTinh).Select(a => a.TenTinh).FirstOrDefault();
        //    ViewBag.TenTinh = tenTinh;
        //    // lưu checkout checkin vào session

        //    IEnumerable<KhachSan> result;

        //    if (bat_dau != null && ket_thuc != null)
        //    {
        //        string sql = "EXEC sp_LayKhachSanCoPhongTrong @IdTinhThanh, @StartDate, @EndDate";
        //        var parameters = new[]
        //        {
        //            new SqlParameter("@IdTinhThanh", idTinh),
        //            new SqlParameter("@StartDate", bat_dau),
        //            new SqlParameter("@EndDate", ket_thuc)
        //        };
        //        result = db.KhachSans.FromSqlRaw(sql, parameters).ToList();
        //    }
        //    else
        //    {
        //        result = db.KhachSans.Where(x => x.IdTinhThanh == idTinh).ToList();
        //    }

        //    return PartialView("_HotelList", result);
        //}

        public IActionResult ChiTietKhachSan(int id)
        {
            KhachSan khach_sans = db.KhachSans.FirstOrDefault(x => x.Id == id);

            HttpContext.Session.SetInt32("idTinh", id);
            string batDau = HttpContext.Session.GetString("bat_dau");
            string ketThuc = HttpContext.Session.GetString("ket_thuc");

            if (batDau != null && ketThuc != null)
            {
                string sql = "EXEC sp_LayDanhSachPhongTrong @IdKhachSan, @StartDate, @EndDate";
                var parameters = new[]
                {
                    new SqlParameter("@IdKhachSan", id),
                    new SqlParameter("@StartDate", batDau),
                    new SqlParameter("@EndDate", ketThuc)
                };
                ViewBag.phongs = db.Phongs.FromSqlRaw(sql, parameters).ToList();
            }
            else
            {
                ViewBag.phongs = db.Phongs.Where(x => x.IdKhachSan == id).ToList();
            }

            return View(khach_sans);
        }

        // xem chi tiet phong 
        public IActionResult ChiTietPhong(int id) 
        {
            var phong = db.Phongs.FirstOrDefault(p => p.Id == id);

            return View(phong);
        }

        #region Thanhtoan
        private void SendConfirmationEmail(DatPhong bookingModel, string customerEmail, string paymentMethod)
        {
            string subject;
            string paymentInfo;
            var start = HttpContext.Session.GetString("bat_dau");
            var end = HttpContext.Session.GetString("ket_thuc");
            // Tạo nội dung email
            var tenks = (from ks in db.KhachSans
                         join p in db.Phongs on ks.Id equals p.IdKhachSan
                         where p.Id == bookingModel.IdPhong
                         select ks.TenKhachSan).FirstOrDefault();
            var diachi = (from ks in db.KhachSans
                          join p in db.Phongs on ks.Id equals p.IdKhachSan
                          where p.Id == bookingModel.IdPhong
                          select ks.DiaChi).FirstOrDefault();
            var phong = db.Phongs.FirstOrDefault(x => x.Id == bookingModel.IdPhong);
            var data = Cart.GetCart(HttpContext.Session); ;
            var strSanPham = "<table border='1'>";
            strSanPham += "<thead><tr><th>Tên khách sạn</th><th>Tên phòng</th><th>Ngày đến</th><th>Ngày đi</th><th>Địa chỉ</th><th>";
            if (paymentMethod == "Thanh toán cọc")
            {
                strSanPham += "Tiền cọc</th><th>Cần thanh toán thêm</th>";
            }
            else
            {
                strSanPham += "Tổng tiền</th>";
            }
            strSanPham += "</tr></thead>";
            strSanPham += "<tbody>";
            strSanPham += "<tr>";
            strSanPham += $"<td>{tenks}</td>";
            strSanPham += $"<td>{phong.TenPhong}</td>";
            strSanPham += $"<td>{start}</td>";
            strSanPham += $"<td>{end}</td>";
            strSanPham += $"<td>{diachi}</td>";
            if (paymentMethod == "Thanh toán cọc")
            {
                //strSanPham += $"<td>{data.Sum(x => x.TongTien()) * 40 / 100}</td>";
                //strSanPham += $"<td>{data.Sum(x => x.TongTien()) * 60 / 100}</td>";
            }
            else
            {
                strSanPham += $"<td>{data.Sum(x => x.phong.GiaPhong)}</td>";
            }
            strSanPham += "</tr>";
            strSanPham += "</tbody></table>";
            var strThongTinKhachHang = $@"
    <p>Họ tên khách hàng: {bookingModel.HoVaTen}</p>
    <p>Email: {bookingModel.Email}</p>
    <p>Số điện thoại: {bookingModel.SoDienThoai}</p>
    ";
            var fullContent = $@"
    <h2>Thông tin đặt phòng</h2>
        {strSanPham}
    <h2>Thông tin khách hàng</h2>
        {strThongTinKhachHang}
    ";

            // Xác định tiêu đề và thông tin thanh toán dựa trên phương thức thanh toán
            if (paymentMethod == "Thanh toán cọc")
            {
                subject = "Xác nhận đặt cọc phòng thành công";
                paymentInfo = "Bạn đã đặt cọc phòng thành công.";
            }
            else
            {
                subject = "Phòng bạn được đặt thành công";
                paymentInfo = "Cảm ơn bạn đã đặt phòng.";
            }

            // Gửi email
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("doducviet3012@gmail.com", "ebfwregutahnwhrj"),
                EnableSsl = true,
            };
            var fromAddress = new MailAddress("doducviet3012@gmail.com", "Hotel");
            var toAddress = new MailAddress(customerEmail);
            var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = $"{fullContent}<p>{paymentInfo}</p>",
                IsBodyHtml = true
            };
            smtpClient.Send(mailMessage);
        }

        public IActionResult Checkout(int id, DateTime ngayDen, DateTime ngayDi)
        {
            Cart.AddItem(HttpContext.Session, id, ngayDen, ngayDi);

            List<Item> cart = Cart.GetCart(HttpContext.Session);
            if (cart != null)
            {
                ViewBag.Cart = cart;
            }

            return View();
        }

        [HttpPost]
        public IActionResult Checkout(IFormCollection fc)
        {
            int id_user = Convert.ToInt32(fc["id"]);
            string hoVaTen = fc["ho"].ToString() + " " + fc["ten"].ToString();
            //string ten = fc["ten"].ToString();
            //string ho_ten = ho + " " + ten;
            string phone = fc["phone"].ToString();
            string email = fc["email"].ToString();
            string diaChi = fc["duongpho"].ToString() + " " + fc["thanhpho"].ToString();
            string tinh_trang = fc["tinhtrang"].ToString();
            string ma_buu_dien = fc["mabuudien"].ToString();
            string ghi_chu = fc["ghichu"].ToString();

            string payment = fc["payment"].ToString();

            // Lưu trữ các biến vào session
            HttpContext.Session.SetString("hoVaTen", hoVaTen);
            HttpContext.Session.SetString("phone", phone);
            HttpContext.Session.SetString("email", email);
            HttpContext.Session.SetString("diaChi", diaChi);
            HttpContext.Session.SetString("tinhtrang", tinh_trang);
            HttpContext.Session.SetString("mabuudien", ma_buu_dien);
            HttpContext.Session.SetString("ghichu", ghi_chu);
            HttpContext.Session.SetString("payment", payment);

            HttpContext.Session.SetInt32("IdUser", id_user);

            string batDau = HttpContext.Session.GetString("bat_dau");
            string ketThuc = HttpContext.Session.GetString("ket_thuc");

            DateTime bat_dau = Convert.ToDateTime(batDau);
            DateTime ket_thuc = Convert.ToDateTime(ketThuc);
            TimeSpan khoang_cach = ket_thuc - bat_dau;
            int soNgay = (int)khoang_cach.TotalDays;

            // thánh toán VNPAY
            if (payment == "Thanh toán VnPay")
            {
                TaiKhoan user = db.TaiKhoans.FirstOrDefault(u => u.Id == id_user);
                var vnPayModel = new PaymentInfomationModel
                {
                    Amount = Cart.CartTotal(HttpContext.Session, bat_dau, ket_thuc),
                    CreatedDate = DateTime.Now,
                    OrderDescription = $"{hoVaTen} {phone} {diaChi}",
                    Name = user.HoTen,
                    OrderId = new Random().Next(1000, 10000)
                };
                return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
            }

            string vnpay_payment = "Thanh toán VnPay";
            DatPhong dat_phong = new DatPhong();
            List<Item> cart = Cart.GetCart(HttpContext.Session);

            if (cart != null)
            {
                foreach (var line in cart)
                {
                    var id_phong = line.phong.Id;
                    var don_gia = line.phong.GiaPhong * soNgay;
                    var id_trang_thai = 1;

                    dat_phong.IdPhong = id_phong;
                    dat_phong.IdTaiKhoan = id_user;
                    dat_phong.TongTien = don_gia;
                    dat_phong.BatDau = bat_dau;
                    dat_phong.KetThuc = ket_thuc;
                    dat_phong.ThanhToan = vnpay_payment;
                    dat_phong.Status = false;
                    dat_phong.IdTrangThai = id_trang_thai;
                    dat_phong.NgayDat = DateTime.Now;

                    db.DatPhongs.Add(dat_phong);
                }
                db.SaveChanges();
                Cart.CartDestroy(HttpContext.Session);
                return RedirectToAction("Index", "Home");
            }
            
            return View();
        }

        //
        public IActionResult PaymentCallBack()
        {
            // Nhận lại các giá trị từ session
            string? hoVaTen = HttpContext.Session.GetString("hoVaTen");
            string? phone = HttpContext.Session.GetString("phone");
            string? email = HttpContext.Session.GetString("email");
            string? diaChi = HttpContext.Session.GetString("diaChi");
            string? tinh_trang = HttpContext.Session.GetString("tinhtrang");
            string? ma_buu_dien = HttpContext.Session.GetString("mabuudien");
            string? ghi_chu = HttpContext.Session.GetString("ghichu");
            string? payment = HttpContext.Session.GetString("payment");

            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VNPay {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            string vnpay_payment = "Thanh toán VnPay";
            string batDau = HttpContext.Session.GetString("bat_dau");
            string ketThuc = HttpContext.Session.GetString("ket_thuc");

            DateTime bat_dau = Convert.ToDateTime(batDau);
            DateTime ket_thuc = Convert.ToDateTime(ketThuc);
            TimeSpan khoang_cach = ket_thuc - bat_dau;
            int soNgay = (int)khoang_cach.TotalDays;

            int id_user = (int)HttpContext.Session.GetInt32("IdUser");

            // lưu đơn hàng thành công
            DatPhong dat_phong = new DatPhong();
            List<Item> cart = Cart.GetCart(HttpContext.Session);

            if (cart != null)
            {
                foreach (var line in cart)
                {
                    var id_phong = line.phong.Id;
                    var don_gia = line.phong.GiaPhong * soNgay;
                    var id_trang_thai = 1;

                    dat_phong.IdPhong = id_phong;
                    dat_phong.IdTaiKhoan = id_user;
                    dat_phong.TongTien = don_gia;
                    dat_phong.BatDau = bat_dau;
                    dat_phong.KetThuc = ket_thuc;
                    dat_phong.ThanhToan = payment;
                    dat_phong.Status = false;
                    dat_phong.IdTrangThai = id_trang_thai;
                    dat_phong.NgayDat = DateTime.Now;
                    dat_phong.HoVaTen = hoVaTen;
                    dat_phong.Email = email;
                    dat_phong.DiaChi = diaChi;
                    dat_phong.GhiChu = ghi_chu;
                    dat_phong.TinhTrangKh = tinh_trang;
                    dat_phong.MaBuuDien = ma_buu_dien;
                    dat_phong.SoDienThoai = phone;

                    db.DatPhongs.Add(dat_phong);

                    // gửi mail khi đặt phòng thành công
                    SendConfirmationEmail(new DatPhong { HoVaTen = hoVaTen, Email = email, SoDienThoai = phone, DiaChi = diaChi, GhiChu = ghi_chu, MaBuuDien = ma_buu_dien, TinhTrangKh = tinh_trang, IdPhong = id_phong }, email, payment);
                }
                db.SaveChanges();

                // Xóa các session sau khi đã hoàn thành
                HttpContext.Session.Remove("hoVaTen");
                HttpContext.Session.Remove("phone");
                HttpContext.Session.Remove("email");
                HttpContext.Session.Remove("diaChi");
                HttpContext.Session.Remove("tinhtrang");
                HttpContext.Session.Remove("mabuudien");
                HttpContext.Session.Remove("ghichu");
                HttpContext.Session.Remove("payment");
                HttpContext.Session.Remove("bat_dau");
                HttpContext.Session.Remove("ket_thuc");

                Cart.CartDestroy(HttpContext.Session);
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public IActionResult PaymentFail()
        {
            return View();
        }

        public IActionResult PaymentSuccess()
        {
            // Lấy giá trị id_datphong từ TempData
            int? id_datphong = TempData["IdDatPhong"] as int?;

            if (id_datphong.HasValue)
            {
                // Đã lấy được giá trị, có thể sử dụng nó trong trang "paymentsuccess"
                ViewBag.IdDatPhong = id_datphong;

                List<Item> cart = Cart.GetCart(HttpContext.Session);
                if (cart != null)
                {
                    ViewBag.Cart = cart;
                }

                return View();
            }

            // Xử lý khi không có thông tin đơn đặt phòng
            return RedirectToAction("Index");
        }
        #endregion

        #region DanhGia
        [HttpPost]
        public IActionResult DanhGia(IFormCollection fc)
        {
            var idKs = Convert.ToInt32(fc["idKhachSan"]);
            // Tạo một đối tượng Review từ dữ liệu được gửi từ form
            var danhGia = new DanhGium
            {
                HoTen = fc["hoTen"],
                Email = fc["email"],
                NoiDung = fc["noiDung"],
                DiemSachSe = Convert.ToInt32(fc["rgcl"][0]),
                DiemThoaiMai = Convert.ToInt32(fc["rgcl"][1]),
                DiemNhanVien = Convert.ToInt32(fc["rgcl"][2]),
                DiemCoSo = Convert.ToInt32(fc["rgcl"][3]),
                IdTaiKhoan = HttpContext.Session.GetInt32("idUser"),
                IdKhachSan = idKs
            };
            db.DanhGia.Add(danhGia);
            db.SaveChanges();

            return Redirect("/Phong/ChiTietKhachSan?id=" + idKs);
        }
        #endregion
    }
}
