using Microsoft.EntityFrameworkCore;
using BarberShop.Core.Entities;
using BarberShop.Core.Enums;
using BarberShop.Core.Interfaces;
using BarberShop.Infrastructure.Data;

namespace BarberShop.Infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWorkingHoursService _workingHoursService;

        public AppointmentService(ApplicationDbContext context, IWorkingHoursService workingHoursService)
        {
            _context = context;
            _workingHoursService = workingHoursService;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByUserAsync(string userId)
        {
            return await _context.Appointments
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByBarberAsync(int barberId, DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .Where(a => a.BarberId == barberId && a.AppointmentDate.Date == date.Date && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int barberId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeAppointmentId = null)
        {
            var query = _context.Appointments
                .Where(a => a.BarberId == barberId
                         && a.AppointmentDate.Date == date.Date
                         && a.Status != AppointmentStatus.Cancelled);

            if (excludeAppointmentId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAppointmentId.Value);
            }

            // Check for overlapping appointments
            var hasOverlap = await query.AnyAsync(a =>
                (startTime >= a.StartTime && startTime < a.EndTime) ||
                (endTime > a.StartTime && endTime <= a.EndTime) ||
                (startTime <= a.StartTime && endTime >= a.EndTime));

            return !hasOverlap;
        }

        public async Task<IEnumerable<TimeSpan>> GetAvailableTimeSlotsAsync(int barberId, DateTime date, int serviceDurationMinutes)
        {
            var availableSlots = new List<TimeSpan>();
            
            // Get working hours for this day
            var workingHours = await _workingHoursService.GetWorkingHoursAsync(barberId, date.DayOfWeek);
            if (workingHours == null || !workingHours.IsOpen)
                return availableSlots;

            // Get existing appointments for this day
            var existingAppointments = await GetAppointmentsByBarberAsync(barberId, date);

            // Generate time slots (every 30 minutes)
            var slotInterval = TimeSpan.FromMinutes(30);
            var currentTime = workingHours.OpenTime;
            var serviceDuration = TimeSpan.FromMinutes(serviceDurationMinutes);

            while (currentTime + serviceDuration <= workingHours.CloseTime)
            {
                var endTime = currentTime + serviceDuration;
                
                // Check if this slot is available
                var isAvailable = !existingAppointments.Any(a =>
                    (currentTime >= a.StartTime && currentTime < a.EndTime) ||
                    (endTime > a.StartTime && endTime <= a.EndTime) ||
                    (currentTime <= a.StartTime && endTime >= a.EndTime));

                // Don't show past time slots for today
                if (date.Date == DateTime.Today && currentTime <= DateTime.Now.TimeOfDay)
                {
                    isAvailable = false;
                }

                if (isAvailable)
                {
                    availableSlots.Add(currentTime);
                }

                currentTime = currentTime.Add(slotInterval);
            }

            return availableSlots;
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            appointment.CreatedAt = DateTime.UtcNow;
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            appointment.UpdatedAt = DateTime.UtcNow;
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
