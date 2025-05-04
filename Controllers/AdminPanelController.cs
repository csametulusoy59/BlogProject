using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // Authorize özniteliği için bu namespace gereklidir

namespace BlogProject.Controllers
{
    // Bu öznitelik, bu controller içindeki tüm action'ların yetkilendirme gerektirdiğini belirtir.
    // Sadece "Admin" rolüne sahip kullanıcılar bu controller'a erişebilir.
    [Authorize(Roles = "Admin")]
    public class AdminPanelController : Controller
    {
        // Index action'ı artık doğrudan rol kontrolü yapmaz, çünkü yetkilendirme özniteliği bunu halleder.
        public IActionResult Index()
        {
            // Eğer kullanıcı "Admin" rolüne sahip değilse, Authorize özniteliği isteği yakalar
            // ve yapılandırmanıza göre bir yetkisiz (401) veya yasaklanmış (403) yanıtı döndürür
            // veya AccessDenied sayfasına yönlendirir.
            // Buraya kadar gelindiyse, kullanıcının "Admin" rolüne sahip olduğu varsayılır.

            return View();
        }

        // Eğer AdminPanelController içinde sadece belirli action'ların Admin rolü gerektirmesini isterseniz,
        // [Authorize(Roles = "Admin")] özniteliğini sadece o action'lara uygulayabilirsiniz.
        // Örneğin:
        // [Authorize(Roles = "Admin")]
        // public IActionResult ManageUsers() { ... }

        // Eğer bir action'ın yetkilendirme gerektirmemesini isterseniz (Controller seviyesinde Authorize varsa),
        // [AllowAnonymous] özniteliğini kullanabilirsiniz.
        // [AllowAnonymous]
        // public IActionResult PublicInfo() { ... }
    }
}
