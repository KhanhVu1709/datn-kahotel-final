using DATN_KAHotel_Final.Service;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add services Session
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(120);     // thời gian hết phiên
});

builder.Services.AddSingleton<IVnPayService, VnPayService>();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    // Thiết lập thời gian sống của cookie (ExpireTimeSpan)
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

    // Đường dẫn để chuyển hướng người dùng đến trang đăng nhập khi họ chưa xác thực
    options.LoginPath = "/Account/Login";

    // Đường dẫn để chuyển hướng người dùng khi họ không có quyền truy cập
    options.AccessDeniedPath = "/AccessDenied";
});

// Authorization
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("AnyRole", policy =>
    {
        policy.RequireRole("Quản lý khách hàng", "Quản lý đơn đặt", "Quản lý phòng", "Quản lý khách sạn", "Tài khoản thường");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
            name: "area",
            pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");
app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
