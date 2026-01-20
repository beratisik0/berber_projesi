using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BarberShop.Core.Interfaces;
using BarberShop.Core.Entities;
using BarberShop.Core.DTOs;

namespace BarberShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ServicesController : Controller
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _serviceService.GetAllServicesAsync();
            return View(services);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ServiceViewModel { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var service = new Service
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                DurationMinutes = model.DurationMinutes,
                IsActive = model.IsActive
            };

            await _serviceService.CreateServiceAsync(service);
            TempData["Success"] = "Hizmet başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            if (service == null) return NotFound();

            var model = new ServiceViewModel
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price,
                DurationMinutes = service.DurationMinutes,
                IsActive = service.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var service = await _serviceService.GetServiceByIdAsync(model.Id);
            if (service == null) return NotFound();

            service.Name = model.Name;
            service.Description = model.Description;
            service.Price = model.Price;
            service.DurationMinutes = model.DurationMinutes;
            service.IsActive = model.IsActive;

            await _serviceService.UpdateServiceAsync(service);
            TempData["Success"] = "Hizmet başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _serviceService.DeleteServiceAsync(id);
            TempData["Success"] = "Hizmet başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
