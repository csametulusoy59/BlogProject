using BlogProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies; // Çerez kimlik doðrulamasý için gerekli namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kimlik doðrulama servislerini ekle
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Varsayýlan kimlik doðrulama þemasýný belirt
    .AddCookie(options => // Çerez kimlik doðrulamasýný yapýlandýr
    {
        // Kimliði doðrulanmamýþ kullanýcýlarýn yönlendirileceði giriþ sayfasýnýn yolunu belirtin
        options.LoginPath = "/Account/Login"; // Örnek: Giriþ sayfanýzýn yolu
        options.AccessDeniedPath = "/Account/AccessDenied"; // Örnek: Eriþim reddedildi sayfasýnýn yolu
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Çerezin geçerlilik süresi
        options.SlidingExpiration = true; // Çerezin süresinin her istekte yenilenmesini saðlar
    });

builder.Services.AddSession(); // Session servisi zaten vardý

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); // Yönlendirme önce gelmeli

// Kimlik doðrulama ve yetkilendirme, yönlendirmeden sonra ve endpoint'lerden önce gelmeli
app.UseAuthentication(); // Kimlik doðrulama önce gelmeli
app.UseAuthorization();  // Yetkilendirme sonra gelmeli

// Session middleware'i genellikle kimlik doðrulama ve yetkilendirmeden sonra gelir
// Oturum verilerine eriþim yetkilendirme gerektirebilir
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blog}/{action=Index}/{id?}");

app.Run();
