using Microsoft.AspNetCore.Mvc;

namespace BlogProject.Controllers
{
    public class AdminPanelController : Controller
    {
        public IActionResult Index()
        {
            // Kullanıcının Rolünü Kontrol Et
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }

            return View();
        }
    }
}
