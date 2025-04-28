using System;
using System.ComponentModel.DataAnnotations;

namespace BlogProject.Models
{
    public class Comment
    {
        public int Id { get; set; }

        
        [StringLength(200)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Yorumun bağlı olduğu BlogPost
        public int BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }
    }
}
