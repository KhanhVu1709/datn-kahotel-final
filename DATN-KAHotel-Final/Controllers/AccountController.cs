using DATN_KAHotel_Final.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DATN_KAHotel_Final.ViewModels;
using Microsoft.AspNet.Identity;

namespace DATN_KAHotel_Final.Controllers
{
    public class AccountController : Controller
    {
        QlksContext db = new QlksContext();
        public IActionResult Login()
        {
            ViewBag.action = "/account/login";
            return View();
        }

        [HttpPost]
        public IActionResult Login(TaiKhoan tk)
        {
            if (tk.TenDangNhap == null)
            {
                TempData["TK"] = "Tài khoản không được để trống";
            }
            else if (tk.MatKhau == null)
            {
                TempData["MK"] = "Mật khẩu không được để trống";
            }
            else
            {
                var item = db.TaiKhoans.FirstOrDefault(u => u.TenDangNhap == tk.TenDangNhap);

                var usersWithLoaiList = (from u in db.TaiKhoans
                                         join ulu in db.PhanQuyens on u.Id equals ulu.IdTaiKhoan into userLoaiGroups
                                         from ulu in userLoaiGroups.DefaultIfEmpty()
                                         join lu in db.Quyens on ulu.IdQuyen equals lu.Id into loaiGroups
                                         from lu in loaiGroups.DefaultIfEmpty()
                                         where u.TenDangNhap == tk.TenDangNhap
                                         select new
                                         {
                                             Id = u.Id,
                                             TaiKhoan = u.TenDangNhap,
                                             MatKhau = u.MatKhau,
                                             TrangThai = u.TrangThai,
                                             Loai = lu.TenQuyen,
                                         }).ToList();

                if (item != null)
                {
                    int id_user = item.Id;
                    var passwordHasher = new PasswordHasher();

                    // Kiểm tra mật khẩu nhập vào có khớp với mật khẩu đã mã hoá hay không
                    var passwordVerification = passwordHasher.VerifyHashedPassword(item.MatKhau, tk.MatKhau);

                    // Mật khẩu đúng => chuyển hướng đến trang khác
                    if (passwordVerification == Microsoft.AspNet.Identity.PasswordVerificationResult.Success)
                    {
                        if (item.TrangThai == false)
                        {
                            TempData["baned"] = "Tài khoản đã bị cấm";
                            return View();
                        }
                        else
                        {

                            var claim = new List<Claim>();
                            {
                                new Claim(ClaimTypes.Name, item.TenDangNhap);
                            };

                            // Thêm các vai trò vào danh sách claim
                            foreach (var userWithLoai in usersWithLoaiList)
                            {
                                if (userWithLoai.Loai != null)
                                {
                                    claim.Add(new Claim(ClaimTypes.Role, userWithLoai.Loai.ToString()));
                                }
                            }

                            var claimIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
                            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity));

                            HttpContext.Session.SetString("login", "true");
                            HttpContext.Session.SetString("taikhoan", item.TenDangNhap);
                            HttpContext.Session.SetInt32("idUser", id_user);

                            return RedirectToAction("Index", "Home");
                        }
                    }
                    // Mật khẩu không khớp hoặc không tìm thấy người dùng
                    TempData["SaiMK"] = "Sai tài khoản hoặc mật khẩu";
                    return RedirectToAction("Login", "Account");
                }
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult Register()
        {
            ViewBag.action = "/account/register";
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM rvm)
        {
            // kiểm tra tài khoản đã tồn tại chưa
            var existingUser = db.TaiKhoans.FirstOrDefault(u => u.TenDangNhap == rvm.TenDangNhap);
            if (existingUser != null)
            {
                TempData["Error"] = "Đã tồn tại tài khoản này rồi.";
                return View();
            }
            else
            {
                string plainPassword = rvm.MatKhau;
                // Tạo một instance của PasswordHasher
                var passwordHasher = new PasswordHasher();
                // Mã hoá mật khẩu
                string hashedPassword = passwordHasher.HashPassword(plainPassword);
                if (ModelState.IsValid)  // thông tin vượt qua được validation
                {
                    TaiKhoan user = new TaiKhoan();
                    user.TenDangNhap = rvm.TenDangNhap;
                    user.MatKhau = hashedPassword;
                    user.TrangThai = true;
                    user.Anh = "avatar5.png";
                    user.IsDelete = false;
                    db.TaiKhoans.Add(user);
                    db.SaveChanges();

                    int id_newUser = user.Id;
                    PhanQuyen pq = new PhanQuyen();
                    pq.IdTaiKhoan = id_newUser;
                    pq.IdQuyen = 5;
                    db.PhanQuyens.Add(pq);

                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("New error", "Invalid data");
                    return View();
                }
            }
        }
    }
}
