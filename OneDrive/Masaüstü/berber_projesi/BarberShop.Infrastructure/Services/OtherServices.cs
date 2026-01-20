using Microsoft.EntityFrameworkCore;
using BarberShop.Core.Entities;
using BarberShop.Core.Interfaces;
using BarberShop.Infrastructure.Data;

namespace BarberShop.Infrastructure.Services
{
    public class BarberService : IBarberService
    {
        private readonly ApplicationDbContext _context;

        public BarberService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Barber>> GetAllBarbersAsync()
        {
            return await _context.Barbers
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Barber>> GetActiveBarbersAsync()
        {
            return await _context.Barbers
                .Where(b => b.IsActive)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<Barber?> GetBarberByIdAsync(int id)
        {
            return await _context.Barbers
                .Include(b => b.WorkingHours)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Barber> CreateBarberAsync(Barber barber)
        {
            barber.CreatedAt = DateTime.UtcNow;
            _context.Barbers.Add(barber);
            await _context.SaveChangesAsync();
            return barber;
        }

        public async Task UpdateBarberAsync(Barber barber)
        {
            _context.Barbers.Update(barber);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBarberAsync(int id)
        {
            var barber = await _context.Barbers.FindAsync(id);
            if (barber != null)
            {
                barber.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }

    public class ServiceService : IServiceService
    {
        private readonly ApplicationDbContext _context;

        public ServiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _context.Services
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Service>> GetActiveServicesAsync()
        {
            return await _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task<Service> CreateServiceAsync(Service service)
        {
            service.CreatedAt = DateTime.UtcNow;
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return service;
        }

        public async Task UpdateServiceAsync(Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteServiceAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                service.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }

    public class AnnouncementService : IAnnouncementService
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Announcement>> GetAllAnnouncementsAsync()
        {
            return await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Announcement>> GetActiveAnnouncementsAsync()
        {
            return await _context.Announcements
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Announcement?> GetAnnouncementByIdAsync(int id)
        {
            return await _context.Announcements.FindAsync(id);
        }

        public async Task<Announcement> CreateAnnouncementAsync(Announcement announcement)
        {
            announcement.CreatedAt = DateTime.UtcNow;
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
            return announcement;
        }

        public async Task UpdateAnnouncementAsync(Announcement announcement)
        {
            announcement.UpdatedAt = DateTime.UtcNow;
            _context.Announcements.Update(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnnouncementAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);
            if (announcement != null)
            {
                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();
            }
        }
    }

    public class WorkingHoursService : IWorkingHoursService
    {
        private readonly ApplicationDbContext _context;

        public WorkingHoursService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkingHours>> GetWorkingHoursByBarberAsync(int barberId)
        {
            return await _context.WorkingHours
                .Where(w => w.BarberId == barberId)
                .OrderBy(w => w.DayOfWeek)
                .ToListAsync();
        }

        public async Task<WorkingHours?> GetWorkingHoursAsync(int barberId, DayOfWeek dayOfWeek)
        {
            return await _context.WorkingHours
                .FirstOrDefaultAsync(w => w.BarberId == barberId && w.DayOfWeek == dayOfWeek);
        }

        public async Task UpdateWorkingHoursAsync(int barberId, IEnumerable<WorkingHours> workingHours)
        {
            var existingHours = await _context.WorkingHours
                .Where(w => w.BarberId == barberId)
                .ToListAsync();

            _context.WorkingHours.RemoveRange(existingHours);
            
            foreach (var wh in workingHours)
            {
                wh.BarberId = barberId;
                _context.WorkingHours.Add(wh);
            }

            await _context.SaveChangesAsync();
        }
    }
}
