using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } = "User";  // Varsayılan olarak normal kullanıcı
    }
}
