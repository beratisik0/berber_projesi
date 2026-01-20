using System.ComponentModel.DataAnnotations;

namespace BarberShop.Core.DTOs
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad Soyad gereklidir")]
        [MaxLength(100)]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir")]
        [StringLength(100, ErrorMessage = "Şifre en az {2} karakter olmalıdır", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrar")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = "Berber seçiniz")]
        [Display(Name = "Berber")]
        public int BarberId { get; set; }

        [Required(ErrorMessage = "Hizmet seçiniz")]
        [Display(Name = "Hizmet")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "Tarih seçiniz")]
        [DataType(DataType.Date)]
        [Display(Name = "Tarih")]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Saat seçiniz")]
        [Display(Name = "Saat")]
        public string StartTime { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Not")]
        public string? Notes { get; set; }
    }

    public class ServiceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hizmet adı gereklidir")]
        [MaxLength(100)]
        [Display(Name = "Hizmet Adı")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Fiyat gereklidir")]
        [Range(0.01, 10000, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        [Display(Name = "Fiyat (₺)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Süre gereklidir")]
        [Range(5, 480, ErrorMessage = "Süre 5 ile 480 dakika arasında olmalıdır")]
        [Display(Name = "Süre (dk)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;
    }

    public class AnnouncementViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık gereklidir")]
        [MaxLength(200)]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik gereklidir")]
        [Display(Name = "İçerik")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;
    }
}
