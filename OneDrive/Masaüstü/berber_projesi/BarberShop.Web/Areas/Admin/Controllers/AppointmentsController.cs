using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BarberShop.Core.Interfaces;
using BarberShop.Core.Enums;

namespace BarberShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IBarberService _barberService;

        public AppointmentsController(IAppointmentService appointmentService, IBarberService barberService)
        {
            _appointmentService = appointmentService;
            _barberService = barberService;
        }

        public async Task<IActionResult> Index(DateTime? date, int? barberId)
        {
            ViewBag.Barbers = await _barberService.GetAllBarbersAsync();
            ViewBag.SelectedDate = date ?? DateTime.Today;
            ViewBag.SelectedBarberId = barberId;

            var startDate = date ?? DateTime.Today;
            var endDate = startDate;
            
            var appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(startDate, endDate);
            
            if (barberId.HasValue)
            {
                appointments = appointments.Where(a => a.BarberId == barberId.Value);
            }

            return View(appointments);
        }

        public async Task<IActionResult> All()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return View(appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, AppointmentStatus status)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = status;
            await _appointmentService.UpdateAppointmentAsync(appointment);

            TempData["Success"] = "Randevu durumu güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            await _appointmentService.CancelAppointmentAsync(id);
            TempData["Success"] = "Randevu iptal edildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
