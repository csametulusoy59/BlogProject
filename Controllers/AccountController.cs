using BlogProject.Data;
using BlogProject.Models; // User modeliniz için
using BlogProject.Models.ViewModels; // LoginViewModel ve RegisterViewModel için
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authentication; // SignInAsync için
using Microsoft.AspNetCore.Authentication.Cookies; // CookieAuthenticationDefaults için
using System.Security.Claims; // Claims için
using System.Threading.Tasks; // Async işlemler için
using Microsoft.EntityFrameworkCore; // FirstOrDefaultAsync, AnyAsync için
using System; // DateTime için
using System.Collections.Generic; // List için
using System.ComponentModel.DataAnnotations; // ValidationContext için (isteğe bağlı)


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
        // Kayıt formunu gösterir
        public IActionResult Register()
        {
            // Eğer kullanıcı zaten kimliği doğrulanmışsa, ana sayfaya yönlendir
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); // veya Blog Index
            }
            return View();
        }

        // POST: /Account/Register
        // Kayıt formundan gelen verileri işler
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF saldırılarına karşı koruma
        public async Task<IActionResult> Register(RegisterViewModel model) // RegisterViewModel alacak şekilde değiştirildi
        {
            // Model doğrulamasını kontrol et (DataAnnotations kuralları)
            if (!ModelState.IsValid)
            {
                // Doğrulama hatası varsa, hataları ModelState'e ekler ve View'ı model ile tekrar gösterir.
                return View(model);
            }

            // Kullanıcı adının daha önce alınıp alınmadığını kontrol et (büyük/küçük harf duyarsız)
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == model.Username.ToLower()))
            {
                ModelState.AddModelError("Username", "Bu kullanıcı adı zaten alınmış.");
                return View(model);
            }

            // E-posta adresinin daha önce kullanılıp kullanılmadığını kontrol et (büyük/küçük harf duyarsız)
            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == model.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                return View(model);
            }


            // Yeni kullanıcı nesnesini oluştur
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                // TODO: Şifreyi veritabanına kaydetmeden önce GÜVENLİ BİR ŞEKİLDE HASHLEMEK KRİTİKTİR!
                // BCrypt.Net.Core gibi bir kütüphane kullanabilirsiniz.
                // user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password); // Örnek: BCrypt kullanımı
                Password = model.Password, // GEÇİCİ VE GÜVENSİZ: Düz metin şifre kaydı (Gerçek uygulamada KULLANMAYIN!)
                Role = "User", // Varsayılan rolü "User" olarak ayarla
                IsEmailConfirmed = false // E-posta başlangıçta onaylanmamış
                // TODO: E-posta onay token'ı oluşturup kaydedin (isteğe bağlı)
                // EmailConfirmationToken = Guid.NewGuid().ToString();
            };

            _context.Users.Add(user); // Kullanıcıyı veritabanına ekle
            await _context.SaveChangesAsync(); // Async SaveChanges kullanın

            // TODO: Kullanıcının e-posta adresine aktivasyon kodu/linki gönderin.
            // Bu kısım bir e-posta gönderme servisi veya kütüphanesi gerektirir.
            // Örnek: EmailService.SendConfirmationEmail(user.Email, user.EmailConfirmationToken);


            // Başarılı kayıt sonrası login sayfasına yönlendir
            // İsteğe bağlı olarak bir başarı mesajı gösterebilirsiniz (TempData kullanarak)
            TempData["SuccessMessage"] = "Kaydınız başarıyla tamamlandı. E-posta adresinize gönderilen linke tıklayarak hesabınızı aktif edebilirsiniz.";
            return RedirectToAction("Login");
        }

        // TODO: E-posta Onay Action'ı ekleyin (örneğin /Account/ConfirmEmail)
        // Bu action, kullanıcının e-postadaki linke tıkladığında çalışacak.
        // Token'ı doğrulayacak ve kullanıcının IsEmailConfirmed alanını true yapacak.
        /*
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            // userId ve token ile kullanıcıyı veritabanında bul
            // Token'ı doğrula
            // Kullanıcıyı bulup IsEmailConfirmed = true yap
            // Veritabanını kaydet
            // Başarılı veya başarısız mesajı gösteren bir View'a yönlendir
        }
        */


        // GET: /Account/Login
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home"); // veya Blog Index
            }
            // TempData'da başarı mesajı varsa View'a taşı
            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            }
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF saldırılarına karşı koruma
        public async Task<IActionResult> Login(LoginViewModel model) // LoginViewModel alacak şekilde değiştirildi
        {
            // Model doğrulamasını kontrol et (DataAnnotations kuralları)
            if (!ModelState.IsValid)
            {
                return View(model); // Doğrulama hatası varsa formu tekrar göster
            }

            // Kullanıcıyı veritabanından kullanıcı adına göre bul
            // Şifre kontrolünü veritabanı sorgusunda YAPMAYIN! Güvenli değildir.
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.Username == model.Username);

            // Kullanıcı bulundu ve şifre doğruysa (Şifre doğrulama mantığınızı buraya ekleyin)
            // NOT: Şifreleri düz metin olarak saklamayın! Güvenli bir hashing algoritması kullanın (örn: BCrypt, Argon2).
            // Aşağıdaki kontrol sadece bir yer tutucudur. Gerçek uygulamada güvenli şifre doğrulama yapılmalıdır.
            // bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user?.PasswordHash); // Örnek: BCrypt kullanımı
            bool isPasswordValid = VerifyPassword(model.Password, user?.Password); // Şifre doğrulama metodunuzu çağırın


            if (user != null && isPasswordValid)
            {
                // Kullanıcı için claim'leri oluştur
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Kullanıcı ID'si
                    new Claim(ClaimTypes.Name, user.Username), // Kullanıcı adı
                    new Claim(ClaimTypes.Role, user.Role) // Kullanıcının rolü (veritabanından gelen)
                    // İhtiyacınız olursa başka claim'ler de ekleyebilirsiniz (örn: email, tam ad vb.)
                };

                // ClaimsIdentity ve ClaimsPrincipal oluştur
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    // Beni Hatırla özelliği için
                    IsPersistent = model.RememberMe,
                    // Çerez süresi (Program.cs'deki ExpireTimeSpan ile uyumlu olmalı veya burada ayarlanabilir)
                    // Eğer RememberMe seçiliyse belirli bir süre, yoksa tarayıcı kapatılana kadar geçerli
                    ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddMinutes(30) : (DateTimeOffset?)null // Örnek süre: 30 dakika
                };

                // Kullanıcıyı sisteme kaydet (cookie oluşturur)
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Rolü kontrol et ve yönlendirme yap
                if (user.Role == "Admin") // Kullanıcı modelinizdeki role property'sine göre ayarlayın
                {
                    return RedirectToAction("Index", "AdminPanel"); // Admin paneline yönlendir
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Normal kullanıcıları ana sayfaya yönlendir
                }
            }
            else
            {
                // Kimlik doğrulama başarısız olursa
                ModelState.AddModelError(string.Empty, "Geçersiz kullanıcı adı veya şifre.");
                return View(model); // Hata mesajıyla formu tekrar göster
            }
        }

        // GET: /Account/AccessDenied
        // Erişim reddedildi sayfasını gösterir (Authorize özniteliği yönlendirdiğinde)
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Account/Logout
        // Kullanıcının oturumunu kapatır
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home"); // Oturum kapatıldıktan sonra ana sayfaya yönlendir
        }

        // TODO: Kullanıcı listesi (Admin Paneli için) action'ı ekleyin (isteğe bağlı)
        // [Authorize(Roles = "Admin")]
        // public async Task<IActionResult> Users() { ... }


        // Şifre doğrulama mantığınızı buraya ekleyin
        // Bu metod, saklanan hashlenmiş şifre ile kullanıcının girdiği şifreyi karşılaştırmalıdır.
        // Şifreleri düz metin olarak saklamayın!
        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // TODO: Güvenli şifre doğrulama mantığını uygulayın.
            // Bu kısım, kullanıcıları kaydederken kullandığınız hashing algoritmasıyla eşleşmelidir.
            // Örneğin, BCrypt.Net.Core gibi bir kütüphane kullanabilirsiniz.
            // return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPasswordHash); // Örnek BCrypt kullanımı

            // GEÇİCİ VE GÜVENSİZ: Sadece test amaçlı düz metin karşılaştırması (Gerçek uygulamada KULLANMAYIN!)
            if (string.IsNullOrEmpty(storedPasswordHash)) return false;
            return enteredPassword == storedPasswordHash;

            // throw new NotImplementedException("Güvenli şifre doğrulama mantığı uygulanmadı.");
        }

        // TODO: Kullanıcı kaydı (Register) sırasında şifreleri HASHLEMEYİ UNUTMAYIN!
        // Register POST action'ında kullanıcıyı veritabanına eklemeden önce şifreyi hashleyin.
        // user.Password = SecurePasswordHasher.Hash(user.Password); // Örnek
    }
}
