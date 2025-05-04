using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models.ViewModels // Projenizin ViewModels klasörüne göre ayarlayın
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı gerekli.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı en az {2}, en fazla {1} karakter uzunluğunda olmalıdır.")] // Minimum 3 karakter
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre gerekli.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az {2} karakter uzunluğunda olmalıdır.")] // Minimum 6 karakter
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrarı gerekli.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre Tekrarı")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")] // Şifre tekrarı ile şifreyi karşılaştırır
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "E-posta adresi gerekli.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")] // E-posta formatı doğrulaması
        [Display(Name = "E-posta")]
        public string Email { get; set; }

        // Kullanıcı rolünü burada almayacağız, varsayılan olarak "User" atayacağız (isteğe bağlı olarak eklenebilir)
        // public string Role { get; set; }
    }
}
