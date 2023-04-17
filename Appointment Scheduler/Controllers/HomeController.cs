using Appointment_Scheduler.Database;
using Appointment_Scheduler.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Appointment_Scheduler.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseUtils database;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration)
        {
            _logger = logger;
            database = new(configuration);
        }

        public IActionResult Index()
        {
            ViewBag.emailExists = true;
            ViewBag.passwordMatches = true; 
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public ActionResult Index(IFormCollection collection)
        {
            User user = database.GetUserByEmail(collection["email"]);
            string password = database._getHashedPassowrd( collection["password"]);
            Console.WriteLine(user.Id);


            if (user.Id == 0)
            {
                ViewBag.emailExists = false;
               return View();
            }

            if (!user.Password.Equals(password))
            {
                ViewBag.passwordMatches = false;
                return View();

            }


            HttpContext.Session.SetInt32("userId", user.Id);
          
           return RedirectToAction(actionName: "Index", controllerName: "Appointments", new { UserId=user.Id});


        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(IFormCollection collection)
        {
            string password = collection["password"];
            string confirmPassword = collection["confirm-password"];
            if(!password.Equals(confirmPassword))
            {
                ViewBag.passwordMatches = false; 
                return View();
            }

            int id = database.CreateUser(collection["username"], collection["email"], collection["password"]);
            //UQ__Users__72E12F1B793B4E92' for username

            Console.WriteLine("Id in signup " + id);
            if (id > 0)
            HttpContext.Session.SetInt32("userId", id);
            {
                return RedirectToAction(actionName: "Index", controllerName: "Appointments");
            }

            if (id == -1)
                ViewBag.emailExists = true;
            if (id == -2)
                ViewBag.userNameExists = true;
            return View();

        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        public void ForgotPassword()
        {

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}