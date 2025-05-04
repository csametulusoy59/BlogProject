using BlogProject.Models; // Modelleriniz için
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Mevcut DbSet'leriniz
        // Blog yazılarına ait DbSet adını BlogPost modeline uygun olarak "BlogPosts" olarak değiştirdik.
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<User> Users { get; set; } // Kullanıcı modeliniz User adında olduğunu varsayıyorum

        // Yeni DbSet'ler
        public DbSet<Comment> Comments { get; set; }
        public DbSet<BlogPostLike> BlogPostLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Comment modeli için ilişki yapılandırması
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.ParentComment) // Bir yorumun bir üst yorumu olabilir
                .WithMany(c => c.Replies) // Bir üst yorumun birden çok cevabı olabilir
                .HasForeignKey(c => c.ParentCommentId) // ParentCommentId foreign key olarak kullanılır
                .IsRequired(false); // ParentCommentId null olabilir (ana yorumlar için)

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Blog) // Bir yorumun bir blog yazısı vardır (Blog modeliniz BlogPost olarak değiştiyse burayı da düzeltmeniz gerekebilir)
                .WithMany(b => b.Comments) // Bir blog yazısının birden çok yorumu olabilir
                .HasForeignKey(c => c.BlogId)
                .OnDelete(DeleteBehavior.Cascade); // Blog silindiğinde yorumları da sil

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User) // Bir yorumun bir kullanıcısı vardır
                .WithMany(u => u.Comments) // Bir kullanıcının birden çok yorumu olabilir
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silindiğinde yorumları silme (veya Cascade olarak değiştirebilirsiniz)

            // BlogPostLike modeli için ilişki yapılandırması
            modelBuilder.Entity<BlogPostLike>()
               .HasOne(l => l.Blog) // Bir beğeninin bir blog yazısı vardır (Blog modeliniz BlogPost olarak değiştiyse burayı da düzeltmeniz gerekebilir)
               .WithMany(b => b.Likes) // Bir blog yazısının birden çok beğenisi olabilir
               .HasForeignKey(l => l.BlogId)
               .OnDelete(DeleteBehavior.Cascade); // Blog silindiğinde beğenileri de sil

            modelBuilder.Entity<BlogPostLike>()
                .HasOne(l => l.User) // Bir beğeninin bir kullanıcısı vardır
                .WithMany(u => u.BlogPostLikes) // Bir kullanıcının birden çok blog beğenisi olabilir (Kullanıcı modelinizde BlogPostLikes koleksiyonu olmalı)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Kullanıcı silindiğinde beğenileri silme (veya Cascade olarak değiştirebilirsiniz)

            // Kullanıcı modelinize Comments ve BlogPostLikes koleksiyonlarını eklemeniz gerekebilir
            // public ICollection<Comment> Comments { get; set; }
            // public ICollection<BlogPostLike> BlogPostLikes { get; set; }

            // BlogPost modelinize Comments ve Likes koleksiyonlarını eklemeniz gerekebilir (Model adınız BlogPost olduğu için)
            // public ICollection<Comment> Comments { get; set; }
            // public ICollection<BlogPostLike> Likes { get; set; }
        }
    }
}
