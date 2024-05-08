namespace DATN_KAHotel_Final.Areas.Admin.Models
{
    public class AdminAccessMiddleware
    {
        private readonly RequestDelegate _next;

        public AdminAccessMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Kiểm tra nếu URL trỏ đến trang admin
            if (context.Request.Path.StartsWithSegments("/admin/taikhoan/danhmuctaikhoan"))
            {
                // Kiểm tra xem người dùng có quyền truy cập không
                if (!context.User.IsInRole("Quản lý khách hàng"))
                {
                    // Nếu không có quyền, chuyển hướng đến trang chủ
                    context.Response.Redirect("/home/index");
                    return;
                } 
            }
            if (context.Request.Path.StartsWithSegments("/admin/khachsan/danhmuckhachsan"))
            {
                // Kiểm tra xem người dùng có quyền truy cập không
                if (!context.User.IsInRole("Quản lý khách sạn"))
                {
                    // Nếu không có quyền, chuyển hướng đến trang chủ
                    context.Response.Redirect("/home/index");
                    return;
                }
            }
            if (context.Request.Path.StartsWithSegments("/admin/order/danhmucgiaodich"))
            {
                // Kiểm tra xem người dùng có quyền truy cập không
                if (!context.User.IsInRole("Quản lý đơn đặt"))
                {
                    // Nếu không có quyền, chuyển hướng đến trang chủ
                    context.Response.Redirect("/home/index");
                    return;
                }
            }
            if (context.Request.Path.StartsWithSegments("/admin/phong/danhmucphong"))
            {
                // Kiểm tra xem người dùng có quyền truy cập không
                if (!context.User.IsInRole("Quản lý phòng"))
                {
                    // Nếu không có quyền, chuyển hướng đến trang chủ
                    context.Response.Redirect("/home/index");
                    return;
                }
            }
            if (context.Request.Path.StartsWithSegments("/admin/admin/index"))
            {
                // Kiểm tra xem người dùng có quyền truy cập không
                if (!context.User.IsInRole("Quản lý khách hàng") && !context.User.IsInRole("Quản lý phòng") && !context.User.IsInRole("Quản lý đơn đặt") && !context.User.IsInRole("Quản lý khách sạn") && !context.User.IsInRole("Quản lý khách hàng"))
                {
                    // Nếu không có quyền, chuyển hướng đến trang chủ
                    context.Response.Redirect("/home/index");
                    return;
                }
            }

            // Nếu không phải trang admin hoặc có quyền, chuyển tiếp yêu cầu đến middleware tiếp theo
            await _next(context);
        }
    }
}
