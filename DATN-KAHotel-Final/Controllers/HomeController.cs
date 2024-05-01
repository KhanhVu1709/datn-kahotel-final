using DATN_KAHotel_Final.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DATN_KAHotel_Final.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            HttpContext.Session.Remove("cart");
            HttpContext.Session.Remove("bat_dau");
            HttpContext.Session.Remove("ket_thuc");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}