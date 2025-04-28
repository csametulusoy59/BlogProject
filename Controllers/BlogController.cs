using Microsoft.AspNetCore.Mvc;
using BlogProject.Data;
using BlogProject.Models;
using System.Linq;

namespace BlogProject.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Blog yazılarını listele
        public IActionResult Index()
        {
            var blogPosts = _context.BlogPosts.OrderByDescending(x => x.CreatedAt).ToList();
            return View(blogPosts);
        }

        // Yeni yazı eklemek için form
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage); // VS Output penceresinde hata göreceksin
                    }
                }
                _context.BlogPosts.Add(blogPost);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Yazı başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "ModelState hatalı!";
            return View(blogPost);
        }

        // Yorum ekleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int blogPostId, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var comment = new Comment
                {
                    Content = content,
                    BlogPostId = blogPostId
                };

                _context.Comments.Add(comment);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = blogPostId });
        }

        // Blog yazısının detayları (yorumları dahil)
        public IActionResult Details(int id)
        {
            var blogPost = _context.BlogPosts
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (blogPost == null)
            {
                return NotFound();
            }

            // Yorumları çek
            blogPost.Comments = _context.Comments
                .Where(c => c.BlogPostId == id)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return View(blogPost);
        }

    }
}
