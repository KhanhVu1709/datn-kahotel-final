using DATN_KAHotel_Final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DATN_KAHotel_Final.Areas.Admin.Controllers
{
    [Area("admin")]
    //[Authorize(Roles = "Quản lý phòng")]
    public class PhongController : Controller
    {
        QlksContext db = new QlksContext();
        // danh sach phong
        public IActionResult DanhMucPhong(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listPhong = db.Phongs.ToList().Where(x => (bool)!x.IsDelete).OrderBy(item => item.Id).ToList();
            PagedList<Phong> list = new PagedList<Phong>(listPhong, pageNumber, pageSize);
            return View(list);
        }

        // create phong
        public IActionResult create()
        {
            ViewBag.action = "/admin/phong/createpost";
            int? idKhachSanNew = HttpContext.Session.GetInt32("id_khachsanNew");
            if (idKhachSanNew != null)
            {
                ViewBag.IdKhachSan = new SelectList(db.KhachSans.ToList(), "Id", "TenKhachSan", idKhachSanNew);
            }
            else
            {
                ViewBag.IdKhachSan = new SelectList(db.KhachSans.ToList(), "Id", "TenKhachSan");
            }

            ViewBag.TienNghi = db.TienNghis.ToList();

            HttpContext.Session.Remove("id_khachsanNew");
            return View("FormCreateUpdate");
        }

        [HttpPost]
        public IActionResult createpost(IFormCollection fc)
        {
            Phong phong = new Phong();
            var ten_phong = fc["TenPhong"].ToString().Trim();
            double gia_phong = Convert.ToDouble(fc["GiaPhong"]);
            int id_khachsan = Convert.ToInt32(fc["IdKhachSan"]);
            int so_nguoi = Convert.ToInt32(fc["SoNguoi"]);
            int so_giuong = Convert.ToInt32(fc["SoGiuong"]);
            double kich_thuoc_phong = Convert.ToDouble(fc["KichThuocPhong"]);
            string mo_ta = fc["MoTa"].ToString();

            //
            List<string> fileNames = new List<string>();
            try
            {
                // Lặp qua tất cả các tệp tin được tải lên
                foreach (var file in Request.Form.Files)
                {
                    string fileName = file.FileName;
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var timestamp = DateTime.Now.ToFileTime();
                        fileName = timestamp + "_" + fileName;
                        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/phong", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // them tap tin vao danh sach
                        fileNames.Add(fileName);
                    }
                }

                if (fileNames.Count > 0)
                {
                    // them anh vao đối tượng sp
                    phong.AnhDaiDien = fileNames[0];
                }
            }
            catch {; }
            phong.TenPhong = ten_phong;
            phong.GiaPhong = gia_phong;
            phong.IdKhachSan = id_khachsan;
            phong.SoNguoi = so_nguoi;
            phong.SoGiuong = so_giuong;
            phong.KichThuocPhong = kich_thuoc_phong;
            phong.MoTa = mo_ta;
            phong.IsDelete = false;
            db.Phongs.Add(phong);
            db.SaveChanges();

            // lay id cua san pham vua them
            int id_p = phong.Id;

            // them chi tiet anh
            for (var i = 1; i < fileNames.Count; i++)
            {
                HinhAnh anh = new HinhAnh();
                if (fileNames.Count > 0)
                {
                    anh.UrlHinhAnh = fileNames[i];
                }

                anh.IsDelete = false;
                anh.IdPhong = id_p;
                db.HinhAnhs.Add(anh);
                db.SaveChanges();
            }

            // tiện nghi
            string[] tienNghiDaChon = fc["tienNghi"];

            if (tienNghiDaChon != null && tienNghiDaChon.Length > 0)
            {
                foreach (var tien_nghi in tienNghiDaChon)
                {
                    PhongTienNghi p_tn = new PhongTienNghi();
                    p_tn.IdPhong = id_p;
                    p_tn.IdTienNghi = Convert.ToInt32(tien_nghi);

                    db.PhongTienNghis.Add(p_tn);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("danhmucphong", "phong");
        }

        // update phong
        public IActionResult update(int idPhong)
        {
            ViewBag.tienNghiDaChon = (from p in db.Phongs
                                     from ptn in db.PhongTienNghis.Where(x => x.IdPhong == p.Id).DefaultIfEmpty()
                                     join tn in db.TienNghis on ptn.IdTienNghi equals tn.Id into tnGroup
                                     from tn in tnGroup.DefaultIfEmpty()
                                     where p.Id == idPhong
                                     select tn.TenTienNghi).ToList();

            Phong ph = db.Phongs.FirstOrDefault(k => k.Id == idPhong);
            ViewBag.IdKhachSan = new SelectList(db.KhachSans.ToList(), "Id", "TenKhachSan");
            ViewBag.TienNghi = db.TienNghis.ToList();
            ViewBag.action = "/admin/phong/updatepost";
            return View("FormCreateUpdate", ph);
        }

        [HttpPost]
        public IActionResult updatepost(int id, IFormCollection fc)
        {
            Phong phong = db.Phongs.FirstOrDefault(k => k.Id == id);

            string ten_phong = fc["TenPhong"].ToString().Trim();
            double gia = Convert.ToDouble(fc["GiaPhong"]);
            int id_ks = Convert.ToInt32(fc["IdKhachSan"]);
            int so_nguoi = Convert.ToInt32(fc["SoNguoi"]);
            int so_giuong = Convert.ToInt32(fc["SoGiuong"]);
            double kich_thuoc_phong = Convert.ToDouble(fc["KichThuocPhong"]);
            string mo_ta = fc["MoTa"].ToString();

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
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/phong", filename);
                // Lưu tệp tin vào đường dẫn vừa xác định
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Request.Form.Files[0].CopyTo(stream);
                }
                //update gia tri vao cot Anh trong csdl
                phong.AnhDaiDien = filename;
            }

            phong.TenPhong = ten_phong;
            phong.GiaPhong = gia;
            phong.IdKhachSan = id_ks;
            phong.SoNguoi = so_nguoi;
            phong.SoGiuong = so_giuong;
            phong.KichThuocPhong = kich_thuoc_phong;
            phong.MoTa = mo_ta;
            phong.IsDelete = false;
            db.Phongs.Update(phong);
            db.SaveChanges();

            // remove tiện nghi
            var kichThuocDaTonTai = db.PhongTienNghis.Where(ptn => ptn.IdPhong == id).ToList();
            foreach (var itemRemove in kichThuocDaTonTai)
            {
                db.PhongTienNghis.Remove(itemRemove);
                db.SaveChanges();
            }

            // tiện nghi
            string[] tienNghiDaChon = fc["tienNghi"];

            if (tienNghiDaChon != null && tienNghiDaChon.Length > 0)
            {
                foreach (var tien_nghi in tienNghiDaChon)
                {
                    PhongTienNghi p_tn = new PhongTienNghi();
                    p_tn.IdPhong = id;
                    p_tn.IdTienNghi = Convert.ToInt32(tien_nghi);

                    db.PhongTienNghis.Add(p_tn);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("danhmucphong", "phong");
        }

        public IActionResult delete(int idPhong)
        {
            Phong phong = db.Phongs.FirstOrDefault(k => k.Id == idPhong);
            phong.IsDelete = true;
            db.Phongs.Update(phong);
            db.SaveChanges();
            return RedirectToAction("danhmucphong", "phong");
        }
    }
}
