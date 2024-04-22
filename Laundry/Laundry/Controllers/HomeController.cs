using Laundry.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Laundry.Controllers
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
        public IActionResult About() 
        {
            return View("~/Views/Home/about.cshtml"); 
        }
        public IActionResult Blog()
        {
            return View("~/Views/Home/blog.cshtml");
        }
        public IActionResult Services()
        {
            return View("~/Views/Home/services.cshtml");
        }
    }
}
