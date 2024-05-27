using Microsoft.AspNetCore.Mvc;
using DATN_KAHotel_Final.Models;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DATN_KAHotel_Final.Areas.Admin.Controllers
{
    [Area("admin")]
    public class TienNghiController : Controller
    {
        QlksContext db = new QlksContext();
        public IActionResult DanhMucTienNghi(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listTienNghi = db.TienNghis.ToList();
            PagedList<TienNghi> list = new PagedList<TienNghi>(listTienNghi, pageNumber, pageSize);
            return View(list);
        }

        // create phong
        public IActionResult create()
        {
            ViewBag.action = "/admin/tiennghi/createpost";
            return View("FormCreateUpdate");
        }

        [HttpPost]
        public IActionResult createpost(IFormCollection fc)
        {
            var ten_tien_nghi = fc["tenTienNghi"].ToString();
            var icon_tien_nghi = fc["iconTienNghi"].ToString();

            TienNghi tien_nghi = new TienNghi();
            tien_nghi.TenTienNghi = ten_tien_nghi;
            tien_nghi.IconTienNghi = icon_tien_nghi;
            db.TienNghis.Add(tien_nghi);
            db.SaveChanges();

            return RedirectToAction("danhmuctiennghi", "tiennghi");
        }

        // update phong
        public IActionResult update(int idTienNghi)
        {

            TienNghi tn = db.TienNghis.FirstOrDefault(k => k.Id == idTienNghi);
            ViewBag.action = "/admin/phong/updatepost";
            return View("FormCreateUpdate", tn);
        }

        [HttpPost]
        public IActionResult updatepost(int id, IFormCollection fc)
        {
            TienNghi tien_nghi = db.TienNghis.FirstOrDefault(k => k.Id == id);

            var ten_tien_nghi = fc["tenTienNghi"].ToString();
            var icon_tien_nghi = fc["iconTienNghi"].ToString();

            tien_nghi.TenTienNghi = ten_tien_nghi;
            tien_nghi.IconTienNghi = icon_tien_nghi;
            db.TienNghis.Update(tien_nghi);
            db.SaveChanges();

            return RedirectToAction("danhmuctiennghi", "tiennghi");
        }

        public IActionResult delete(int idTienNghi)
        {
            //TienNghi tien_nghi = db.TienNghis.FirstOrDefault(k => k.Id == idTienNghi);
            //db.TienNghis.Remove(tien_nghi);
            //db.SaveChanges();
            //return RedirectToAction("danhmuctiennghi", "tiennghi");

            TempData["Message"] = "";
            PhongTienNghi phongTN = db.PhongTienNghis.FirstOrDefault(k => k.IdTienNghi == idTienNghi);
            KhachSanTienNghi ksTN = db.KhachSanTienNghis.FirstOrDefault(k => k.IdTienNghi == idTienNghi);
            if (phongTN != null || ksTN != null)
            {
                TempData["Message"] = "Không xoá được tiện nghi này";
                return RedirectToAction("danhmuctiennghi", "tiennghi");
            }
            db.Remove(db.TienNghis.Find(idTienNghi));
            db.SaveChanges();
            TempData["Message"] = "Tiện nghi đã được xoá";
            return RedirectToAction("danhmuctiennghi", "tiennghi");
        }
    }
}
