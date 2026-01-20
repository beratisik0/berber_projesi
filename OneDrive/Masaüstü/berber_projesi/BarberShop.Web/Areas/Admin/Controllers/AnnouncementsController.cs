using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BarberShop.Core.Interfaces;
using BarberShop.Core.Entities;
using BarberShop.Core.DTOs;

namespace BarberShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AnnouncementsController : Controller
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementsController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<IActionResult> Index()
        {
            var announcements = await _announcementService.GetAllAnnouncementsAsync();
            return View(announcements);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AnnouncementViewModel { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnouncementViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var announcement = new Announcement
            {
                Title = model.Title,
                Content = model.Content,
                IsActive = model.IsActive
            };

            await _announcementService.CreateAnnouncementAsync(announcement);
            TempData["Success"] = "Duyuru başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var announcement = await _announcementService.GetAnnouncementByIdAsync(id);
            if (announcement == null) return NotFound();

            var model = new AnnouncementViewModel
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Content = announcement.Content,
                IsActive = announcement.IsActive
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AnnouncementViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var announcement = await _announcementService.GetAnnouncementByIdAsync(model.Id);
            if (announcement == null) return NotFound();

            announcement.Title = model.Title;
            announcement.Content = model.Content;
            announcement.IsActive = model.IsActive;

            await _announcementService.UpdateAnnouncementAsync(announcement);
            TempData["Success"] = "Duyuru başarıyla güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _announcementService.DeleteAnnouncementAsync(id);
            TempData["Success"] = "Duyuru başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
