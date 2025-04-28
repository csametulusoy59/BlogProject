using BlogProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogProject.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Kullanıcıları Listele
        public async Task<IActionResult> Manage()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // Kullanıcı Sil
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Manage));
        }
    }
}
