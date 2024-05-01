using DATN_KAHotel_Final.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace DATN_KAHotel_Final.Areas.Admin.Controllers
{
    [Area("admin")]
    public class KhachSanController : Controller
    {
        QlksContext db = new QlksContext();
        public IActionResult DanhMucKhachSan(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listKhachSan = db.KhachSans.OrderBy(item => item.Id).ToList();
            PagedList<KhachSan> list = new PagedList<KhachSan>(listKhachSan, pageNumber, pageSize);
            return View(list);
        }
        public IActionResult create()
        {
            ViewBag.action = "/admin/khachsan/createpost";
            ViewBag.IdTinhThanh = new SelectList(db.TinhThanhs.ToList(), "Id", "TenTinh");
            ViewBag.TienNghi = db.TienNghis.ToList();
            return View("FormCreateUpdate");
        }

        [HttpPost]
        public IActionResult createpost(IFormCollection fc)
        {
            KhachSan khach_san = new KhachSan();
            string ten_khach_san = fc["TenKhachSan"].ToString().Trim();
            string dia_chi = fc["DiaChi"].ToString().Trim();
            string gioi_thieu = fc["GioiThieu"].ToString().Trim();
            int id_tinh = Convert.ToInt32(fc["IdTinhThanh"]);
            int so_sao = Convert.ToInt32(fc["SoSao"]);

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
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/khachsan", filename);
                // Lưu tệp tin vào đường dẫn vừa xác định
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Anh trong csdl
                khach_san.AnhDaiDien = filename;
            }

            khach_san.TenKhachSan = ten_khach_san;
            khach_san.DiaChi = dia_chi;
            khach_san.GioiThieu = gioi_thieu;
            khach_san.IdTinhThanh = id_tinh;
            khach_san.IsDelete = false;
            khach_san.SoSao = so_sao;
            db.KhachSans.Add(khach_san);
            db.SaveChanges();

            // lấy id khách sạn mới được thêm
            int id_ks = khach_san.Id;

            // tiện nghi
            string[] tienNghiDaChon = fc["tienNghi"];

            if (tienNghiDaChon != null && tienNghiDaChon.Length > 0)
            {
                foreach (var tien_nghi in tienNghiDaChon)
                {
                    KhachSanTienNghi k_tn = new KhachSanTienNghi();
                    k_tn.IdKhachSan = id_ks;
                    k_tn.IdTienNghi = Convert.ToInt32(tien_nghi);

                    db.KhachSanTienNghis.Add(k_tn);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("danhmuckhachsan", "khachsan");
        }

        [HttpPost]
        public IActionResult CreatePostAndAddRoom(IFormCollection fc)
        {
            KhachSan khach_san = new KhachSan();
            string ten_khach_san = fc["TenKhachSan"].ToString().Trim();
            string dia_chi = fc["DiaChi"].ToString().Trim();
            string gioi_thieu = fc["GioiThieu"].ToString().Trim();
            int id_tinh = Convert.ToInt32(fc["IdTinhThanh"]);
            int so_sao = Convert.ToInt32(fc["SoSao"]);

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
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/khachsan", filename);
                // Lưu tệp tin vào đường dẫn vừa xác định
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Anh trong csdl
                khach_san.AnhDaiDien = filename;
            }

            khach_san.TenKhachSan = ten_khach_san;
            khach_san.DiaChi = dia_chi;
            khach_san.GioiThieu = gioi_thieu;
            khach_san.IdTinhThanh = id_tinh;
            khach_san.IsDelete = false;
            khach_san.SoSao = so_sao;
            db.KhachSans.Add(khach_san);
            db.SaveChanges();

            // lấy ra id khách sạn mới được thêm
            int id_ks = khach_san.Id;

            // tiện nghi
            string[] tienNghiDaChon = fc["tienNghi"];

            if (tienNghiDaChon != null && tienNghiDaChon.Length > 0)
            {
                foreach (var tien_nghi in tienNghiDaChon)
                {
                    KhachSanTienNghi k_tn = new KhachSanTienNghi();
                    k_tn.IdKhachSan = id_ks;
                    k_tn.IdTienNghi = Convert.ToInt32(tien_nghi);

                    db.KhachSanTienNghis.Add(k_tn);
                    db.SaveChanges();
                }
            }

            HttpContext.Session.SetInt32("id_khachsanNew", id_ks);
            return Redirect("/admin/phong/create");
        }

        public IActionResult update(int idKhachSan)
        {
            ViewBag.tienNghiDaChon = (from k in db.KhachSans
                                      from ktn in db.KhachSanTienNghis.Where(x => x.IdKhachSan == k.Id).DefaultIfEmpty()
                                      join tn in db.TienNghis on ktn.IdTienNghi equals tn.Id into tnGroup
                                      from tn in tnGroup.DefaultIfEmpty()
                                      where k.Id == idKhachSan
                                      select tn.TenTienNghi).ToList();

            KhachSan ks = db.KhachSans.FirstOrDefault(k => k.Id == idKhachSan);
            ViewBag.IdTinhThanh = new SelectList(db.TinhThanhs.ToList(), "Id", "TenTinh");
            ViewBag.action = "/admin/khachsan/updatepost";
            ViewBag.TienNghi = db.TienNghis.ToList();
            return View("FormCreateUpdate", ks);
        }

        [HttpPost]
        public IActionResult updatepost(int id, IFormCollection fc)
        {
            KhachSan khach_san = db.KhachSans.FirstOrDefault(k => k.Id == id);

            string ten_khach_san = fc["TenKhachSan"].ToString().Trim();
            string dia_chi = fc["DiaChi"].ToString().Trim();
            string gioi_thieu = fc["GioiThieu"].ToString().Trim();
            int id_tinh = Convert.ToInt32(fc["IdTinhThanh"]);
            int so_sao = Convert.ToInt32(fc["SoSao"]);

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
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/khachsan", filename);
                // Lưu tệp tin vào đường dẫn vừa xác định
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Anh trong csdl
                khach_san.AnhDaiDien = filename;
            }

            khach_san.TenKhachSan = ten_khach_san;
            khach_san.DiaChi = dia_chi;
            khach_san.GioiThieu = gioi_thieu;
            khach_san.IdTinhThanh = id_tinh;
            khach_san.IsDelete = false;
            khach_san.SoSao = so_sao;
            db.KhachSans.Update(khach_san);
            db.SaveChanges();

            // remove tiện nghi
            var kichThuocDaTonTai = db.KhachSanTienNghis.Where(ktn => ktn.IdKhachSan == id).ToList();
            foreach (var itemRemove in kichThuocDaTonTai)
            {
                db.KhachSanTienNghis.Remove(itemRemove);
                db.SaveChanges();
            }

            // tiện nghi
            string[] tienNghiDaChon = fc["tienNghi"];

            if (tienNghiDaChon != null && tienNghiDaChon.Length > 0)
            {
                foreach (var tien_nghi in tienNghiDaChon)
                {
                    KhachSanTienNghi k_tn = new KhachSanTienNghi();
                    k_tn.IdKhachSan = id;
                    k_tn.IdTienNghi = Convert.ToInt32(tien_nghi);

                    db.KhachSanTienNghis.Add(k_tn);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("danhmuckhachsan", "khachsan");
        }

        [HttpPost]
        public IActionResult UpdatePostAndAddRoom(int id, IFormCollection fc)
        {
            KhachSan khach_san = db.KhachSans.FirstOrDefault(k => k.Id == id);

            string ten_khach_san = fc["TenKhachSan"].ToString().Trim();
            string dia_chi = fc["DiaChi"].ToString().Trim();
            string gioi_thieu = fc["GioiThieu"].ToString().Trim();
            int id_tinh = Convert.ToInt32(fc["IdTinhThanh"]);
            int so_sao = Convert.ToInt32(fc["SoSao"]);

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
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/khachsan", filename);
                // Lưu tệp tin vào đường dẫn vừa xác định
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Anh trong csdl
                khach_san.AnhDaiDien = filename;
            }

            khach_san.TenKhachSan = ten_khach_san;
            khach_san.DiaChi = dia_chi;
            khach_san.GioiThieu = gioi_thieu;
            khach_san.IdTinhThanh = id_tinh;
            khach_san.IsDelete = false;
            khach_san.SoSao = so_sao;
            db.KhachSans.Update(khach_san);
            db.SaveChanges();

            // remove tiện nghi
            var kichThuocDaTonTai = db.KhachSanTienNghis.Where(ktn => ktn.IdKhachSan == id).ToList();
            foreach (var itemRemove in kichThuocDaTonTai)
            {
                db.KhachSanTienNghis.Remove(itemRemove);
                db.SaveChanges();
            }

            // tiện nghi
            string[] tienNghiDaChon = fc["tienNghi"];

            if (tienNghiDaChon != null && tienNghiDaChon.Length > 0)
            {
                foreach (var tien_nghi in tienNghiDaChon)
                {
                    KhachSanTienNghi k_tn = new KhachSanTienNghi();
                    k_tn.IdKhachSan = id;
                    k_tn.IdTienNghi = Convert.ToInt32(tien_nghi);

                    db.KhachSanTienNghis.Add(k_tn);
                    db.SaveChanges();
                }
            }

            int id_khachsanNew = khach_san.Id;
            HttpContext.Session.SetInt32("id_khachsanNew", id_khachsanNew);
            return Redirect("/admin/phong/create");
        }

        public IActionResult delete(int idKhachSan)
        {
            KhachSan khach_san = db.KhachSans.FirstOrDefault(k => k.Id == idKhachSan);
            khach_san.IsDelete = true;
            db.KhachSans.Update(khach_san);
            db.SaveChanges();
            return RedirectToAction("danhmuckhachsan", "khachsan");
        }
    }
}
