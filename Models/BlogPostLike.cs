using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace BlogProject.Models
{
    public class BlogPostLike
    {
        [Key]
        public int Id { get; set; }

        // Beğeninin ait olduğu blog yazısı
        public int BlogId { get; set; }
        [ForeignKey("BlogId")]
        public BlogPost Blog { get; set; } // Navigation property

        // Beğeniyi yapan kullanıcı
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; } // Navigation property (Kullanıcı modeliniz User adında olduğunu varsayıyorum)

        // Beğeninin yapıldığı tarih ve saat (isteğe bağlı, takip için faydalı olabilir)
        public DateTime Timestamp { get; set; }
    }
}
