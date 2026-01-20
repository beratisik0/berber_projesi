using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BarberShop.Core.Entities
{
    public class WorkingHours
    {
        public int Id { get; set; }

        [Required]
        public int BarberId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan OpenTime { get; set; }

        [Required]
        public TimeSpan CloseTime { get; set; }

        public bool IsOpen { get; set; } = true;

        // Navigation property
        [ForeignKey("BarberId")]
        public virtual Barber? Barber { get; set; }
    }
}
