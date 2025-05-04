using System;
using System.Collections.Generic; // ICollection için gerekli
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogProject.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        // Yorumun ait olduğu blog yazısı ID'si
        public int BlogId { get; set; }

        // Yorumun ait olduğu blog yazısı (Navigation property)
        // Tipini BlogPost olarak güncelledik
        [ForeignKey("BlogId")]
        public BlogPost Blog { get; set; }

        // Yorumu yapan kullanıcı ID'si
        public int UserId { get; set; }

        // Yorumu yapan kullanıcı (Navigation property)
        [ForeignKey("UserId")]
        public User User { get; set; } // Navigation property (Kullanıcı modeliniz User adında olduğunu varsayıyorum)

        // Yorum içeriği
        [Required(ErrorMessage = "Yorum içeriği boş olamaz.")]
        [StringLength(1000, ErrorMessage = "Yorum içeriği en fazla 1000 karakter olmalıdır.")]
        public string Content { get; set; }

        // Yorumun yapıldığı tarih ve saat
        public DateTime Timestamp { get; set; }

        // Eğer bu yorum bir cevapsa, cevap verilen yorumun ID'si
        public int? ParentCommentId { get; set; }

        // Cevap verilen yorum (Navigation property)
        [ForeignKey("ParentCommentId")]
        public Comment ParentComment { get; set; }

        // Bu yoruma verilen cevaplar (nested comments)
        public ICollection<Comment> Replies { get; set; }

        // Yapıcı metod (isteğe bağlı, koleksiyonları başlatmak için)
        public Comment()
        {
            Replies = new List<Comment>();
        }
    }
}
