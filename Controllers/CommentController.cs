using BlogProject.Data;
using BlogProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Yorumları Listele
        public async Task<IActionResult> Manage()
        {
            var comments = await _context.Comments.Include(c => c.BlogPost).ToListAsync();
            return View(comments);
        }

        // Yorum Sil
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage));
        }
    }
}
