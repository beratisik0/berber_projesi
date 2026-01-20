using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BarberShop.Core.Entities;

namespace BarberShop.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create roles
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            // Create admin user
            if (await userManager.FindByEmailAsync("admin@barbershop.com") == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin@barbershop.com",
                    Email = "admin@barbershop.com",
                    FullName = "Admin User",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Create sample customer
            if (await userManager.FindByEmailAsync("musteri@example.com") == null)
            {
                var customer = new ApplicationUser
                {
                    UserName = "musteri@example.com",
                    Email = "musteri@example.com",
                    FullName = "Örnek Müşteri",
                    PhoneNumber = "05551234567",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(customer, "Musteri123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer, "Customer");
                }
            }

            // Seed Barbers
            if (!await context.Barbers.AnyAsync())
            {
                var barbers = new List<Barber>
                {
                    new Barber { Name = "Ahmet Usta", Phone = "05551112233", Description = "20 yıllık tecrübeli berber" },
                    new Barber { Name = "Mehmet Usta", Phone = "05552223344", Description = "Modern saç kesimi uzmanı" }
                };
                context.Barbers.AddRange(barbers);
                await context.SaveChangesAsync();
            }

            // Seed Services
            if (!await context.Services.AnyAsync())
            {
                var services = new List<Service>
                {
                    new Service { Name = "Saç Kesimi", Price = 150, DurationMinutes = 30, Description = "Klasik saç kesimi" },
                    new Service { Name = "Sakal Tıraşı", Price = 100, DurationMinutes = 20, Description = "Sakal düzeltme ve şekillendirme" },
                    new Service { Name = "Saç + Sakal", Price = 200, DurationMinutes = 45, Description = "Saç kesimi ve sakal tıraşı paketi" },
                    new Service { Name = "Çocuk Kesimi", Price = 100, DurationMinutes = 20, Description = "12 yaş altı çocuklar için" },
                    new Service { Name = "Cilt Bakımı", Price = 250, DurationMinutes = 40, Description = "Profesyonel cilt bakımı" }
                };
                context.Services.AddRange(services);
                await context.SaveChangesAsync();
            }

            // Seed Working Hours
            if (!await context.WorkingHours.AnyAsync())
            {
                var barbers = await context.Barbers.ToListAsync();
                var workingHours = new List<WorkingHours>();

                foreach (var barber in barbers)
                {
                    // Monday to Saturday
                    for (int i = 1; i <= 6; i++)
                    {
                        workingHours.Add(new WorkingHours
                        {
                            BarberId = barber.Id,
                            DayOfWeek = (DayOfWeek)i,
                            OpenTime = new TimeSpan(9, 0, 0),
                            CloseTime = new TimeSpan(19, 0, 0),
                            IsOpen = true
                        });
                    }
                    // Sunday - closed
                    workingHours.Add(new WorkingHours
                    {
                        BarberId = barber.Id,
                        DayOfWeek = DayOfWeek.Sunday,
                        OpenTime = new TimeSpan(0, 0, 0),
                        CloseTime = new TimeSpan(0, 0, 0),
                        IsOpen = false
                    });
                }
                context.WorkingHours.AddRange(workingHours);
                await context.SaveChangesAsync();
            }

            // Seed Announcements
            if (!await context.Announcements.AnyAsync())
            {
                var announcements = new List<Announcement>
                {
                    new Announcement 
                    { 
                        Title = "Hoş Geldiniz!", 
                        Content = "Online randevu sistemimize hoş geldiniz. Artık randevunuzu kolayca alabilirsiniz.",
                        IsActive = true
                    },
                    new Announcement 
                    { 
                        Title = "Bayram Tatili", 
                        Content = "Kurban Bayramı süresince dükkanımız kapalı olacaktır. İyi bayramlar dileriz.",
                        IsActive = false
                    }
                };
                context.Announcements.AddRange(announcements);
                await context.SaveChangesAsync();
            }
        }
    }
}
