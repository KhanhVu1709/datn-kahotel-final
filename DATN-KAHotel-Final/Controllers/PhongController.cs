using Microsoft.AspNetCore.Mvc;
using DATN_KAHotel_Final.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNet.Identity;
using DATN_KAHotel_Final.Service;
using System.Linq;

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

        [HttpPost]
        public IActionResult LocKhachSan(int minPrice, int maxPrice, int city)
        {
            // lấy ra khách sạn có giá lớn hơn bằng minPrice và nhỏ hơn bằng maxPrice
            var result = from k in db.KhachSans
                         join p in db.Phongs on k.Id equals p.IdKhachSan
                         where k.IdTinhThanh == city
                         group p.GiaPhong by new { k.Id, k.IdTinhThanh } into g
                         where g.Average() >= minPrice && g.Average() <= maxPrice
                         select new
                         {
                             KhachSanId = g.Key.Id,
                             TinhThanhId = g.Key.IdTinhThanh,
                             AvgGiaPhong = g.Average()
                         };


            // Lấy danh sách các ID khách sạn từ kết quả truy vấn trước đó
            var idList = result.Select(r => r.KhachSanId).ToList();

            // Truy vấn để lấy thông tin chi tiết về các khách sạn có ID nằm trong danh sách idList
            var ks_result = db.KhachSans.Where(k => idList.Contains(k.Id));
            return PartialView("_HotelList", ks_result);
        }

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
            string ho = fc["ho"].ToString();
            string ten = fc["ten"].ToString();
            string ho_ten = ho + " " + ten;
            string phone = fc["phone"].ToString();
            string email = fc["email"].ToString();
            string thanh_pho = fc["thanhpho"].ToString();
            string duong_pho = fc["duongpho"].ToString();
            string tinh_trang = fc["tinhtrang"].ToString();
            string ma_buu_dien = fc["mabuudien"].ToString();
            string ghi_chu = fc["ghichu"].ToString();

            string payment = fc["payment"].ToString();

            HttpContext.Session.SetInt32("IdUser", id_user);

            string batDau = HttpContext.Session.GetString("bat_dau");
            string ketThuc = HttpContext.Session.GetString("ket_thuc");

            DateTime bat_dau = Convert.ToDateTime(batDau);
            DateTime ket_thuc = Convert.ToDateTime(ketThuc);
            TimeSpan khoang_cach = ket_thuc - bat_dau;
            int soNgay = (int)khoang_cach.TotalDays;

            // thánh toán VNPAY
            if (payment == "Bước thanh toán")
            {
                TaiKhoan user = db.TaiKhoans.FirstOrDefault(u => u.Id == id_user);
                var vnPayModel = new PaymentInfomationModel
                {
                    Amount = Cart.CartTotal(HttpContext.Session, bat_dau, ket_thuc),
                    CreatedDate = DateTime.Now,
                    OrderDescription = $"{ho_ten} {phone} {thanh_pho}",
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
