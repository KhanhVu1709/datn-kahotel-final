using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DATN_KAHotel_Final.Models;
using Microsoft.AspNet.Identity;
using X.PagedList;

namespace DATN_KAHotel_Final.Areas.Admin.Controllers
{
    [Area("admin")]
    //[Authorize(Roles = "quản lý khách hàng")]
    public class TaiKhoanController : Controller
    {
        QlksContext db = new QlksContext();
        public IActionResult DanhMucTaiKhoan(int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listUser = db.TaiKhoans.OrderBy(item => item.Id).ToList();
            PagedList<TaiKhoan> list = new PagedList<TaiKhoan>(listUser, pageNumber, pageSize);
            return View(list);
        }

        public IActionResult create()
        {
            ViewBag.action = "/admin/taikhoan/createpost";
            return View("FormCreateUpdate");
        }

        [HttpPost]
        public IActionResult createpost(IFormCollection fc)
        {
            // tim doi tuong theo id
            TaiKhoan user = new TaiKhoan();

            // 
            string tai_khoan = fc["TenDangNhap"].ToString().Trim();
            string mat_Khau = fc["MatKhau"].ToString();
            string ho_ten = fc["HoTen"].ToString().Trim();
            string email = fc["Email"].ToString().Trim();
            DateTime ngay_sinh = Convert.ToDateTime(fc["NgaySinh"]);
            string sdt = fc["SoDienThoai"].ToString().Trim();
            bool trang_thai = Convert.ToBoolean(Convert.ToInt32(fc["TrangThai"]));

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
                user.Anh = filename;
            }

            // 
            // kiểm tra tài khoản đã tồn tại chưa
            var existingUser = db.TaiKhoans.FirstOrDefault(u => u.TenDangNhap == tai_khoan);
            if (existingUser != null)
            {
                TempData["Error"] = "Đã tồn tại tài khoản này rồi.";
                return RedirectToAction("Create", "User");
            }
            else
            {
                string plainPassword = mat_Khau;
                // Tạo một instance của PasswordHasher
                var passwordHasher = new PasswordHasher();
                // Mã hoá mật khẩu
                string hashedPassword = passwordHasher.HashPassword(plainPassword);
                user.TenDangNhap = tai_khoan;
                user.MatKhau = hashedPassword;
                user.HoTen = ho_ten;
                user.NgaySinh = ngay_sinh;
                user.Email = email;
                user.SoDienThoai = sdt;
                user.TrangThai = trang_thai;
                db.Add(user);
                db.SaveChanges();
                return RedirectToAction("danhmuctaikhoan", "taikhoan");
            }
        }

        public IActionResult update(int id)
        {
            // tim doi tuong theo id
            TaiKhoan user = db.TaiKhoans.FirstOrDefault(u => u.Id == id);
            ViewBag.action = "/admin/taikhoan/updatepost";
            return View("FormCreateUpdate", user);
        }

        [HttpPost]
        public IActionResult updatepost(int id, IFormCollection fc)
        {
            // tim doi tuong theo id
            TaiKhoan user = db.TaiKhoans.FirstOrDefault(u => u.Id == id);

            string tai_khoan = fc["TenDangNhap"].ToString();
            string mat_Khau = fc["MatKhau"].ToString();
            string ho_ten = fc["HoTen"].ToString().Trim();
            string email = fc["Email"].ToString().Trim();
            DateTime ngay_sinh = Convert.ToDateTime(fc["NgaySinh"]);
            string sdt = fc["SoDienThoai"].ToString().Trim();
            bool trang_thai = Convert.ToBoolean(Convert.ToInt32(fc["TrangThai"]));

            if (!String.IsNullOrEmpty(mat_Khau))
            {
                //
                string plainPassword = mat_Khau;
                // Tạo một instance của PasswordHasher
                var passwordHasher = new PasswordHasher();
                // Mã hoá mật khẩu
                string hashedPassword = passwordHasher.HashPassword(plainPassword);
                user.MatKhau = hashedPassword;
            }

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
                user.Anh = filename;
            }

            user.HoTen = ho_ten;
            user.Email = email;
            user.SoDienThoai = sdt;
            user.NgaySinh = ngay_sinh;
            user.TrangThai = trang_thai;

            db.TaiKhoans.Update(user);
            db.SaveChanges();

            return RedirectToAction("danhmuctaikhoan", "taikhoan");
        }

        public IActionResult delete(int id)
        {
            TaiKhoan user = db.TaiKhoans.FirstOrDefault(u => u.Id == id);
            user.IsDelete = true;
            db.Update(user);
            db.SaveChanges();
            return RedirectToAction("danhmuctaikhoan", "taikhoan");
        }

        // phân quyền tài khoản
        public IActionResult PhanQuyenTaiKhoan(int id)
        {
            TaiKhoan user = db.TaiKhoans.FirstOrDefault(u => u.Id == id);

            ViewBag.QuyenDaChon = (from u in db.TaiKhoans
                                   join ulu in db.PhanQuyens on u.Id equals ulu.IdTaiKhoan into uluGroup
                                   from ulu in uluGroup.DefaultIfEmpty()
                                   join lu in db.Quyens on ulu.IdQuyen equals lu.Id into luGroup
                                   from lu in luGroup.DefaultIfEmpty()
                                   where u.Id == id
                                   select lu.TenQuyen).ToList();

            ViewBag.action = "/admin/taikhoan/phanquyenpost";

            return View(user);
        }

        public IActionResult phanquyenpost(int id, IFormCollection fc)
        {
            TaiKhoan user = db.TaiKhoans.FirstOrDefault(u => u.Id == id);
            //remove quyen
            var quyenDaTonTai = db.PhanQuyens.Where(q => q.IdTaiKhoan == id).ToList();
            db.PhanQuyens.RemoveRange(quyenDaTonTai);
            //foreach(var quyenRemove in quyenDaTonTai)
            //{
            //    db.UserLoaiUsers.Remove(quyenRemove);
            //    db.SaveChanges();
            //}

            // lay ra nhung input name=loaiuser da duoc checked
            // add quyen
            string[] quyen = fc["loaiuser"];
            if (quyen != null && quyen.Length > 0)
            {
                foreach (var item in quyen)
                {
                    PhanQuyen u = new PhanQuyen();

                    u.IdTaiKhoan = id;
                    u.IdQuyen = Convert.ToInt32(item);

                    db.PhanQuyens.Add(u);
                }
            }

            db.SaveChanges();

            return RedirectToAction("danhmuctaikhoan", "taikhoan");
        }
    }
}
