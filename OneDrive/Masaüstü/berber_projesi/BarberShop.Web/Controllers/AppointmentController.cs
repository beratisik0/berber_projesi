using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BarberShop.Core.Interfaces;
using BarberShop.Core.Entities;
using BarberShop.Core.DTOs;
using BarberShop.Core.Enums;
using System.Security.Claims;

namespace BarberShop.Web.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IBarberService _barberService;
        private readonly IServiceService _serviceService;

        public AppointmentController(
            IAppointmentService appointmentService,
            IBarberService barberService,
            IServiceService serviceService)
        {
            _appointmentService = appointmentService;
            _barberService = barberService;
            _serviceService = serviceService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");
            
            var appointments = await _appointmentService.GetAppointmentsByUserAsync(userId);
            return View(appointments);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Barbers = await _barberService.GetActiveBarbersAsync();
            ViewBag.Services = await _serviceService.GetActiveServicesAsync();
            return View(new AppointmentCreateViewModel
            {
                AppointmentDate = DateTime.Today.AddDays(1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Barbers = await _barberService.GetActiveBarbersAsync();
                ViewBag.Services = await _serviceService.GetActiveServicesAsync();
                return View(model);
            }

            var service = await _serviceService.GetServiceByIdAsync(model.ServiceId);
            if (service == null)
            {
                ModelState.AddModelError("", "Hizmet bulunamadı.");
                ViewBag.Barbers = await _barberService.GetActiveBarbersAsync();
                ViewBag.Services = await _serviceService.GetActiveServicesAsync();
                return View(model);
            }

            var startTime = TimeSpan.Parse(model.StartTime);
            var endTime = startTime.Add(TimeSpan.FromMinutes(service.DurationMinutes));

            // Check availability
            var isAvailable = await _appointmentService.IsTimeSlotAvailableAsync(
                model.BarberId, model.AppointmentDate, startTime, endTime);

            if (!isAvailable)
            {
                ModelState.AddModelError("", "Seçilen saat dolu. Lütfen başka bir saat seçiniz.");
                ViewBag.Barbers = await _barberService.GetActiveBarbersAsync();
                ViewBag.Services = await _serviceService.GetActiveServicesAsync();
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");

            var appointment = new Appointment
            {
                UserId = userId,
                BarberId = model.BarberId,
                ServiceId = model.ServiceId,
                AppointmentDate = model.AppointmentDate.Date,
                StartTime = startTime,
                EndTime = endTime,
                TotalPrice = service.Price,
                Notes = model.Notes,
                Status = AppointmentStatus.Confirmed
            };

            await _appointmentService.CreateAppointmentAsync(appointment);
            TempData["Success"] = "Randevunuz başarıyla oluşturuldu!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableSlots(int barberId, string date, int serviceId)
        {
            if (!DateTime.TryParse(date, out var appointmentDate))
                return Json(new List<string>());

            var service = await _serviceService.GetServiceByIdAsync(serviceId);
            if (service == null)
                return Json(new List<string>());

            var slots = await _appointmentService.GetAvailableTimeSlotsAsync(
                barberId, appointmentDate, service.DurationMinutes);

            return Json(slots.Select(s => s.ToString(@"hh\:mm")));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            
            if (appointment == null || appointment.UserId != userId)
            {
                return NotFound();
            }

            await _appointmentService.CancelAppointmentAsync(id);
            TempData["Success"] = "Randevunuz iptal edildi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
