using Microsoft.AspNetCore.Mvc;
using BarberShop.Core.Interfaces;
using System.Diagnostics;
using BarberShop.Web.Models;

namespace BarberShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAnnouncementService _announcementService;
        private readonly IBarberService _barberService;
        private readonly IServiceService _serviceService;

        public HomeController(
            IAnnouncementService announcementService,
            IBarberService barberService,
            IServiceService serviceService)
        {
            _announcementService = announcementService;
            _barberService = barberService;
            _serviceService = serviceService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Announcements = await _announcementService.GetActiveAnnouncementsAsync();
            ViewBag.Services = await _serviceService.GetActiveServicesAsync();
            ViewBag.Barbers = await _barberService.GetActiveBarbersAsync();
            return View();
        }

        public async Task<IActionResult> Services()
        {
            var services = await _serviceService.GetActiveServicesAsync();
            return View(services);
        }

        public async Task<IActionResult> Barbers()
        {
            var barbers = await _barberService.GetActiveBarbersAsync();
            return View(barbers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
