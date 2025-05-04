using BlogProject.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies; // �erez kimlik do�rulamas� i�in gerekli namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Kimlik do�rulama servislerini ekle
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Varsay�lan kimlik do�rulama �emas�n� belirt
    .AddCookie(options => // �erez kimlik do�rulamas�n� yap�land�r
    {
        // Kimli�i do�rulanmam�� kullan�c�lar�n y�nlendirilece�i giri� sayfas�n�n yolunu belirtin
        options.LoginPath = "/Account/Login"; // �rnek: Giri� sayfan�z�n yolu
        options.AccessDeniedPath = "/Account/AccessDenied"; // �rnek: Eri�im reddedildi sayfas�n�n yolu
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // �erezin ge�erlilik s�resi
        options.SlidingExpiration = true; // �erezin s�resinin her istekte yenilenmesini sa�lar
    });

builder.Services.AddSession(); // Session servisi zaten vard�

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

app.UseRouting(); // Y�nlendirme �nce gelmeli

// Kimlik do�rulama ve yetkilendirme, y�nlendirmeden sonra ve endpoint'lerden �nce gelmeli
app.UseAuthentication(); // Kimlik do�rulama �nce gelmeli
app.UseAuthorization();  // Yetkilendirme sonra gelmeli

// Session middleware'i genellikle kimlik do�rulama ve yetkilendirmeden sonra gelir
// Oturum verilerine eri�im yetkilendirme gerektirebilir
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blog}/{action=Index}/{id?}");

app.Run();
