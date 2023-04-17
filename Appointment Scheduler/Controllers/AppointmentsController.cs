using Appointment_Scheduler.Database;
using Appointment_Scheduler.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_Scheduler.Controllers
{
    public class AppointmentsController : Controller
    {
        // GET: AppointmentsController
        private DatabaseUtils database;
        
        public AppointmentsController(IConfiguration configuration)
        {
            database = new(configuration);
        }

        public ActionResult Index()
        {
          
             
            try {
                int id = (int) HttpContext.Session.GetInt32("userId");

                List<Models.Appointment> appointments = database.GetAppointmentsOfUser(id);
                ViewBag.Appointments = appointments;
                TempData["searchText"] = "";
                TempData["UserId"] = id;
                return View();

            }
            catch (InvalidOperationException nullException){
                return RedirectToAction(actionName: "Index", controllerName: "Home");
            }

        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(actionName: "Index", controllerName: "Home");
        }

    

        // GET: AppointmentsController/Create
        public ActionResult Create()
        {
            try
            {
                int id = (int)HttpContext.Session.GetInt32("userId");
                TempData["UserId"] = id;
                
                return View();

            }
            catch (InvalidOperationException e)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");

            }
        }

        // POST: AppointmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                int id = (int)HttpContext.Session.GetInt32("userId");

                database.CreateAppointment(collection["title"], collection["description"], collection["date"], id);

                return RedirectToAction(actionName: "Index");
            }

            catch(InvalidOperationException ex)
            {
                Console.WriteLine(ex.Message);
                return RedirectToAction(actionName: "Index", controllerName: "Home");

            }
        }

        // GET: AppointmentsController/Edit/5
        public ActionResult Edit(int appointmentId, string searchText)
        {
            try
            {
                int id = (int)HttpContext.Session.GetInt32("userId");

                TempData["EditMode"] = true;
                TempData["AppointmentId"] = appointmentId;
                List<Models.Appointment> appointments = database.GetAppointmentsOfUser(id);
                TempData["SearchText"]= searchText;
                ViewBag.Appointments = appointments;
                TempData["UserId"] = id;
                return View("Index");

            }
            catch (InvalidOperationException e)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");

            }
        }
           

        // POST: AppointmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int appointmentId ,IFormCollection collection)
        {
            try
            {
                int id = (int)HttpContext.Session.GetInt32("userId");

                database.UpdateAppointment(appointmentId, collection["title"], collection["description"], collection["date"]);

                return RedirectToAction(actionName: "Index");

            }
            catch (InvalidOperationException e)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");

            }
           


        }

        // GET: AppointmentsController/Delete/5
        public ActionResult Delete()
        {
            return View();
        }

        // POST: AppointmentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int appointmentId)
        {
            try
            {
                int id = (int)HttpContext.Session.GetInt32("userId");
               
                database.DeleteAppointment(id, appointmentId);

                return RedirectToAction(actionName: "Index");

            }
            catch (InvalidOperationException e)
            {
                return RedirectToAction(actionName: "I  ndex", controllerName: "Home");

            }


        }
        [HttpPost]
        public ActionResult Search(IFormCollection collection)
        {
            try
            {
                int id = (int)HttpContext.Session.GetInt32("userId");

                TempData["UserId"] = id;
                TempData["SearchText"] = collection["searchText"];
                Console.WriteLine("inside srch :"+ collection["searchText"]);
                List<Models.Appointment> appointments = database.GetAppointmentsOfUser(id);
                ViewBag.Appointments = appointments;
                return View("Index");
             

            }
            catch (InvalidOperationException e)
            {
                return RedirectToAction(actionName: "Index", controllerName: "Home");

            }


        }
    }
}
