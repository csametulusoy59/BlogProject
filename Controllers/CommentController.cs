using BlogProject.Data;
using BlogProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;


namespace BlogProject.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompositeViewEngine _viewEngine;

        public CommentController(ApplicationDbContext context, ICompositeViewEngine viewEngine)
        {
            _context = context;
            _viewEngine = viewEngine;
        }

        // GET: /Comment/Manage (Admin Paneli için)
        // Sadece Admin rolüne sahip kullanıcılar erişebilir
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            // Tüm yorumları, ilişkili blog ve kullanıcı bilgileriyle birlikte çek
            var comments = await _context.Comments
                                         .Include(c => c.Blog) // Yorumun ait olduğu blog yazısını yükle
                                         .Include(c => c.User) // Yorumu yapan kullanıcıyı yükle
                                         .OrderByDescending(c => c.Timestamp) // En son yorumlar üstte
                                         .ToListAsync();

            return View(comments); // Yorum listesini View'a gönder
        }


        // POST: /Comment/AddComment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int blogId, string content, int? parentCommentId = null)
        {
            // ... (Mevcut AddComment kodunuz) ...

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Json(new { success = false, message = "Kullanıcı kimliği doğrulanamadı." });
            }

            var blogExists = await _context.BlogPosts.AnyAsync(b => b.Id == blogId);
            if (!blogExists)
            {
                return Json(new { success = false, message = "Blog yazısı bulunamadı." });
            }

            if (parentCommentId.HasValue)
            {
                var parentComment = await _context.Comments
                                                .Include(c => c.Blog)
                                                .FirstOrDefaultAsync(c => c.Id == parentCommentId.Value);

                if (parentComment == null)
                {
                    return Json(new { success = false, message = "Cevap verilmek istenen yorum bulunamadı." });
                }
                if (parentComment.BlogId != blogId)
                {
                    return Json(new { success = false, message = "Cevap verilmek istenen yorum farklı bir bloga ait." });
                }
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Yorum içeriği boş olamaz." });
            }

            var comment = new Comment
            {
                BlogId = blogId,
                UserId = userId,
                Content = content.Trim(),
                Timestamp = DateTime.UtcNow,
                ParentCommentId = parentCommentId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var commentCount = await _context.Comments.CountAsync(c => c.BlogId == blogId);

            var newComment = await _context.Comments
                                        .Include(c => c.User)
                                        .Include(c => c.Blog)
                                        .FirstOrDefaultAsync(c => c.Id == comment.Id);

            string commentHtml = await RenderPartialViewToStringAsync("_CommentPartial", newComment);


            return Json(new { success = true, commentHtml = commentHtml, commentCount = commentCount });
        }


        // TODO: Yorum silme, düzenleme gibi action'ları ekleyebilirsiniz.
        // Bu action'lar için kullanıcının yorumun sahibi olup olmadığını veya Admin rolüne sahip olup olmadığını kontrol etmeniz gerekir.


        // Helper metod: Partial View'ı string olarak render etmek için
        private async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.ActionDescriptor.ActionName;
            }

            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                ViewEngineResult viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

                if (!viewResult.Success)
                {
                    throw new ArgumentNullException($"Partial view '{viewName}' was not found.");
                }

                ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
