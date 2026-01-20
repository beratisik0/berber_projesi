using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BarberShop.Core.Interfaces;

namespace BarberShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IBarberService _barberService;
        private readonly IServiceService _serviceService;

        public DashboardController(
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
            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek + 1);
            var weekEnd = weekStart.AddDays(7);

            var allAppointments = await _appointmentService.GetAppointmentsByDateRangeAsync(today, today);
            var weekAppointments = await _appointmentService.GetAppointmentsByDateRangeAsync(weekStart, weekEnd);
            var barbers = await _barberService.GetAllBarbersAsync();
            var services = await _serviceService.GetAllServicesAsync();

            ViewBag.TodayAppointments = allAppointments.Count();
            ViewBag.WeekAppointments = weekAppointments.Count();
            ViewBag.TotalBarbers = barbers.Count();
            ViewBag.TotalServices = services.Count();
            ViewBag.RecentAppointments = allAppointments.Take(10);

            return View();
        }
    }
}
