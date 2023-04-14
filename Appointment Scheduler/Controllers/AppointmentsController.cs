using Appointment_Scheduler.Database;
using Appointment_Scheduler.Models;
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

        public ActionResult Index(int userId)
        {
            List<Models.Appointment>appointments= database.GetAppointmentsOfUser(userId);
            ViewBag.Appointments = appointments;
           
            return View();
        }

        // GET: AppointmentsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AppointmentsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AppointmentsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AppointmentsController/Edit/5
        public ActionResult Edit(int userId,int appointmentId)
        {
            ViewBag.EditMode = true;
            ViewBag.appointmentId = appointmentId;
            List<Models.Appointment> appointments = database.GetAppointmentsOfUser(userId);
            ViewBag.Appointments = appointments;

            return View("Index");
        }

        // POST: AppointmentsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,int userId ,IFormCollection collection)
        {
            database.UpdateAppointment(id, collection["title"], collection["description"], collection["date"]);

            return RedirectToAction(actionName: "Index", new { userId });


        }

        // GET: AppointmentsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AppointmentsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
