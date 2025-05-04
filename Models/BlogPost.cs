using System;
using System.Collections.Generic; // List ve ICollection için gerekli
using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık boş olamaz.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İçerik boş olamaz.")]
        public string Content { get; set; }

        // Blog yazısının oluşturulma tarihi (genellikle otomatik ayarlanır)
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Beğenileri tutan BlogPostLike nesnelerinin koleksiyonunu ekliyoruz.
        // Bu koleksiyon, bir blog yazısının hangi beğenilere sahip olduğunu temsil eder.
        public ICollection<BlogPostLike> Likes { get; set; }

        // Yorumlar
        // Bir blog yazısının yorumlarını tutan koleksiyon.
        public ICollection<Comment> Comments { get; set; } // List yerine ICollection kullanmak daha geneldir

        // Yazar bilgisi
        // Blog yazısının yazarını tutan property.
        // [Required(ErrorMessage = "Yazar boş olamaz.")] // Yazarın zorunlu olmasını isterseniz ekleyin
        public string Author { get; set; }

        // Yayın tarihi (CreatedAt ile aynı veya farklı bir amaçla kullanılabilir)
        // [Required(ErrorMessage = "Yayın tarihi boş olamaz.")] // Yayın tarihinin zorunlu olmasını isterseniz ekleyin
        public DateTime PublishDate { get; set; }


        // Yapıcı metod (isteğe bağlı, koleksiyonları başlatmak için)
        // Bu metod, yeni bir BlogPost nesnesi oluşturulduğunda Likes ve Comments koleksiyonlarının null olmamasını sağlar.
        public BlogPost()
        {
            Likes = new List<BlogPostLike>();
            Comments = new List<Comment>();
        }
    }
}
