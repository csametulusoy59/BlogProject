using BlogProject.Data;
using BlogProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BlogProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcıyı veritabanına ekle
                _context.Users.Add(user);
                _context.SaveChanges();

                // Başarılı kayıt sonrası login sayfasına yönlendir
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User loginUser)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                    .FirstOrDefault(u => u.Username == loginUser.Username && u.Password == loginUser.Password);

                if (user != null)
                {
                    // Session'a kullanıcı bilgilerini kaydediyoruz
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Role", user.Role);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                }
            }

            return View(loginUser);
        }


    }
}
