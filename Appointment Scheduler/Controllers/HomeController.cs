using Appointment_Scheduler.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Appointment_Scheduler.Controllers
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

        [HttpPost]
        public IActionResult Index(IFormCollection collection)
        {
            
            return RedirectToAction(actionName: "Index", controllerName: "Appointments", new { userId=1});
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(IFormCollection collection)
        {
            return RedirectToAction(actionName: "Index", controllerName: "Appointments", new { userId=1});

        }

        public IActionResult ResetPassword()
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