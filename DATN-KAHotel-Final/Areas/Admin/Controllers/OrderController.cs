using DATN_KAHotel_Final.Areas.Admin.Models;
using DATN_KAHotel_Final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;
using X.PagedList;

namespace DATN_KAHotel_Final.Areas.Admin.Controllers
{
    [Area("admin")]
    //[Authorize]
    public class OrderController : Controller
    {
        QlksContext db = new QlksContext();
        public IActionResult DanhMucGiaoDich(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            ViewBag.trangThai = db.TrangThaiDons.ToList();

            var linq = (from t in db.TaiKhoans
                        join dp in db.DatPhongs on t.Id equals dp.IdTaiKhoan
                        join tt in db.TrangThaiDons on dp.IdTrangThai equals tt.Id
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
                        }).ToList();
            PagedList<ChiTietGiaoDich> list = new PagedList<ChiTietGiaoDich>(linq, pageNumber, pageSize);
            return View(list);
        }

        [HttpPost]
        public IActionResult UpdateTrangThai(int trangThai, int hoaDonId)
        {
            try
            {
                DatPhong dat_phong = db.DatPhongs.FirstOrDefault(hd => hd.Id == hoaDonId);
                if (dat_phong != null)
                {
                    dat_phong.IdTrangThai = trangThai;
                    db.DatPhongs.Update(dat_phong);
                    db.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        // xuất hoá đơn pdf
        public IActionResult Invoice(int idHoaDon)
        {
            // 
            ViewBag.chiTietPhong = (from  p in db.Phongs
                                   join dp in db.DatPhongs on p.Id equals dp.IdPhong
                                   where dp.Id == idHoaDon
                                   select new
                                   {
                                       TenPhong = p.TenPhong,
                                       GiaPhong = p.GiaPhong,
                                       Anh = p.AnhDaiDien,
                                   }).ToList();

            // tim hoa don theo id
            var linq = (from t in db.TaiKhoans
                        join dp in db.DatPhongs on t.Id equals dp.IdTaiKhoan
                        join tt in db.TrangThaiDons on dp.IdTrangThai equals tt.Id
                        where dp.Id == idHoaDon
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
                        }).FirstOrDefault();

            return View(linq);
        }
    }
}
