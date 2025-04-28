using System;
using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık boş olamaz.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "İçerik boş olamaz.")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int Likes { get; set; } = 0;

        // Yorumlar zorunlu değilse bu satırda bir değişiklik yapmamıza gerek yok
        public List<Comment> Comments { get; set; }
    }

}
