using BlogProject.Data;
using BlogProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // [Authorize] için
using System.Security.Claims; // ClaimTypes için
using System; // DateTime için
using System.Linq; // Count için

namespace BlogProject.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Blog
        public async Task<IActionResult> Index()
        {
            // Blog yazılarını beğenileri ile birlikte yükle
            var blogs = await _context.BlogPosts
                                     .Include(b => b.Likes) // Beğenileri yükle
                                     .ToListAsync();
            return View(blogs);
        }

        // GET: /Blog/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Blog yazısını, yorumlarını ve beğenilerini Include ile birlikte yükle
            // Yorumları ve beğenileri yapan kullanıcıları da ThenInclude ile yüklemek çok önemli
            var blog = await _context.BlogPosts
                                     .Include(b => b.Comments) // Yorumları yükle
                                        .ThenInclude(c => c.User) // Yorumu yapan kullanıcıları yükle
                                     .Include(b => b.Likes) // Beğenileri yükle
                                        .ThenInclude(l => l.User) // Beğeniyi yapan kullanıcıları yükle
                                     .FirstOrDefaultAsync(m => m.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: /Blog/Create (Admin Paneli'nden yönlendirilecek)
        // Bu action'a sadece Admin rolüne sahip kullanıcılar erişebilir
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Blog/Create
        // Bu action'a sadece Admin rolüne sahip kullanıcılar erişebilir
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,Author,PublishDate")] BlogPost blog)
        {
            if (ModelState.IsValid)
            {
                // TODO: Yazar ve Yayın Tarihi (veya Oluşturulma Tarihi) alanlarını burada ayarlayın
                // blog.Author = User.Identity.Name; // Giriş yapmış kullanıcı adını yazar olarak atayabilirsiniz
                // blog.CreatedAt = DateTime.UtcNow; // Oluşturulma tarihini ayarlayın

                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // TODO: Edit, Delete gibi action'ları da Admin rolü ile koruyun


        // POST: /Blog/ToggleLike
        // AJAX isteği ile blog yazısını beğenir veya beğeniyi geri alır (Sadece kimliği doğrulanmış kullanıcılar)
        // Index sayfasındaki JavaScript bu tek Action'ı çağırır
        [HttpPost]
        [Authorize] // Sadece kimliği doğrulanmış kullanıcılar beğenebilir
        [ValidateAntiForgeryToken] // Güvenlik için
        public async Task<IActionResult> ToggleLike(int blogId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                // Kullanıcı kimliği bulunamazsa veya dönüştürülemezse 401 Unauthorized döndür
                // Bu, JavaScript'teki error callback'ini tetikler ve "Beğenmek için lütfen giriş yapın." mesajını gösterir.
                return Unauthorized();
                // Alternatif olarak: return Json(new { success = false, message = "Kullanıcı kimliği doğrulanamadı." });
            }

            // Kullanıcının bu blog yazısını daha önce beğenip beğenmediğini kontrol et
            var existingLike = await _context.BlogPostLikes
                                             .FirstOrDefaultAsync(l => l.BlogId == blogId && l.UserId == userId);

            bool likedStatus; // İşlem sonrası beğeni durumu (true: beğenildi, false: beğeni geri alındı)

            if (existingLike != null)
            {
                // Zaten beğenmişse, beğeniyi kaldır
                _context.BlogPostLikes.Remove(existingLike);
                likedStatus = false; // Artık beğenilmemiş olacak
            }
            else
            {
                // Beğenmemişse, yeni beğeni ekle
                var like = new BlogPostLike
                {
                    BlogId = blogId,
                    UserId = userId,
                    Timestamp = DateTime.UtcNow
                };
                _context.BlogPostLikes.Add(like);
                likedStatus = true; // Artık beğenilmiş olacak
            }

            await _context.SaveChangesAsync();

            // Güncel beğeni sayısını al
            var likeCount = await _context.BlogPostLikes.CountAsync(l => l.BlogId == blogId);

            // Başarılı yanıt döndür, güncel sayıyı ve yeni beğeni durumunu (likedStatus) ekle
            return Json(new { success = true, newLikeCount = likeCount, liked = likedStatus });
        }

        // LikePost ve UnlikePost Action'larını kaldırdık, ToggleLike kullanıyoruz.
        // Eğer hala ihtiyacınız varsa bırakabilirsiniz, ama Index sayfasındaki JS tek Action'ı çağırıyor.
        /*
        // POST: /Blog/LikePost (Artık kullanılmıyor, ToggleLike kullanılıyor)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LikePost(int blogId)
        {
            // ... (mevcut LikePost mantığı) ...
             // Başarılı yanıt döndürürken liked: true ekleyin
             var likeCount = await _context.BlogPostLikes.CountAsync(l => l.BlogId == blogId);
             return Json(new { success = true, newLikeCount = likeCount, liked = true });
        }

         // POST: /Blog/UnlikePost (Artık kullanılmıyor, ToggleLike kullanılıyor)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlikePost(int blogId)
        {
            // ... (mevcut UnlikePost mantığı) ...
            // Başarılı yanıt döndürürken liked: false ekleyin
             var likeCount = await _context.BlogPostLikes.CountAsync(l => l.BlogId == blogId);
             return Json(new { success = true, newLikeCount = likeCount, liked = false });
        }
        */

    }
}
