using System;
using System.Collections.Generic; // ICollection için gerekli
using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı boş olamaz.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3 ile 50 karakter arasında olmalıdır.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Şifre boş olamaz.")]
        // TODO: Şifreleri düz metin olarak saklamayın! Hashlenmiş şifre için bu property'nin adını değiştirebilirsiniz (örn: PasswordHash)
        // [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")] // Eğer hashlenmiş şifre saklıyorsanız bu StringLength gerekmeyebilir
        public string Password { get; set; } // Güvenlik açığı! Hashlenmiş şifre kullanın.

        // Kullanıcının rolü (örn: "Admin", "User")
        [Required(ErrorMessage = "Rol boş olamaz.")]
        public string Role { get; set; } = "User"; // Varsayılan rol "User" olabilir

        // Yeni Eklenen Alanlar:
        [Required(ErrorMessage = "E-posta adresi boş olamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; } = false; // Varsayılan olarak e-posta onaylanmamış

        // TODO: E-posta onay token'ı için bir alan ekleyebilirsiniz (isteğe bağlı)
        // public string EmailConfirmationToken { get; set; }


        // TODO: Kullanıcı kaydı sırasında şifreyi hashleyip Password (veya PasswordHash) alanına kaydetmeyi unutmayın.

        // Navigation properties:
        // Bu kullanıcıya ait yorumlar
        public ICollection<Comment> Comments { get; set; }

        // Bu kullanıcının beğendiği blog yazıları
        public ICollection<BlogPostLike> BlogPostLikes { get; set; }

        // Yapıcı metod (isteğe bağlı, koleksiyonları başlatmak için)
        public User()
        {
            Comments = new List<Comment>();
            BlogPostLikes = new List<BlogPostLike>();
        }
    }
}
