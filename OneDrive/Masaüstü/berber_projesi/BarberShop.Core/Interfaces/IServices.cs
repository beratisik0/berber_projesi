using BarberShop.Core.Entities;
using BarberShop.Core.Enums;

namespace BarberShop.Core.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetAppointmentsByUserAsync(string userId);
        Task<IEnumerable<Appointment>> GetAppointmentsByBarberAsync(int barberId, DateTime date);
        Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<bool> IsTimeSlotAvailableAsync(int barberId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeAppointmentId = null);
        Task<IEnumerable<TimeSpan>> GetAvailableTimeSlotsAsync(int barberId, DateTime date, int serviceDurationMinutes);
        Task<Appointment> CreateAppointmentAsync(Appointment appointment);
        Task UpdateAppointmentAsync(Appointment appointment);
        Task<bool> CancelAppointmentAsync(int id);
    }

    public interface IBarberService
    {
        Task<IEnumerable<Barber>> GetAllBarbersAsync();
        Task<IEnumerable<Barber>> GetActiveBarbersAsync();
        Task<Barber?> GetBarberByIdAsync(int id);
        Task<Barber> CreateBarberAsync(Barber barber);
        Task UpdateBarberAsync(Barber barber);
        Task DeleteBarberAsync(int id);
    }

    public interface IServiceService
    {
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<IEnumerable<Service>> GetActiveServicesAsync();
        Task<Service?> GetServiceByIdAsync(int id);
        Task<Service> CreateServiceAsync(Service service);
        Task UpdateServiceAsync(Service service);
        Task DeleteServiceAsync(int id);
    }

    public interface IAnnouncementService
    {
        Task<IEnumerable<Announcement>> GetAllAnnouncementsAsync();
        Task<IEnumerable<Announcement>> GetActiveAnnouncementsAsync();
        Task<Announcement?> GetAnnouncementByIdAsync(int id);
        Task<Announcement> CreateAnnouncementAsync(Announcement announcement);
        Task UpdateAnnouncementAsync(Announcement announcement);
        Task DeleteAnnouncementAsync(int id);
    }

    public interface IWorkingHoursService
    {
        Task<IEnumerable<WorkingHours>> GetWorkingHoursByBarberAsync(int barberId);
        Task<WorkingHours?> GetWorkingHoursAsync(int barberId, DayOfWeek dayOfWeek);
        Task UpdateWorkingHoursAsync(int barberId, IEnumerable<WorkingHours> workingHours);
    }
}
