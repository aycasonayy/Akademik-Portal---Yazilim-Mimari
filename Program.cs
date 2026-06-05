using System.Globalization;
using AkademiPortal.Application.Factories;
using AkademiPortal.Application.Interfaces;
using AkademiPortal.Infrastructure.Logging;
using AkademiPortal.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────────────────────────────────────
// Kültür – Türkçe tarih/saat biçimlendirmesi için tr-TR kültürü ayarlanıyor.
// ─────────────────────────────────────────────────────────────────────────────
var trKultur = new CultureInfo("tr-TR");
CultureInfo.DefaultThreadCurrentCulture   = trKultur;
CultureInfo.DefaultThreadCurrentUICulture = trKultur;

// ─────────────────────────────────────────────────────────────────────────────
// Bağımlılık Enjeksiyonu Kayıtları
// ─────────────────────────────────────────────────────────────────────────────

builder.Services.AddControllersWithViews();

// ── Tekil Örüntüsü ──────────────────────────────────────────────────────────
// NotificationLogManager statik .Instance özelliğiyle tam olarak bir örnek sağlar.
// Aynı nesneyi DI kabına vererek INotificationLogManager'a bağlı her bileşen
// uygulama ömrü boyunca aynı thread-safe tekil örneği alır.
builder.Services.AddSingleton<INotificationLogManager>(
    _ => NotificationLogManager.Instance);

// ── Bildirim Servisleri (Fabrika Örüntüsü girdileri) ────────────────────────
// Her ikisi de INotificationService arabirimi altında kayıtlıdır.
// ASP.NET Core DI, NotificationFactory yapıcısına IEnumerable<INotificationService>
// olarak otomatik enjekte eder; fabrika bunları ServiceType'a göre indeksler.
builder.Services.AddTransient<INotificationService, EmailNotificationService>();
builder.Services.AddTransient<INotificationService, SMSNotificationService>();

// ── Fabrika Örüntüsü ─────────────────────────────────────────────────────────
// AnnouncementFactory → SınavDuyurusu / EtkinlikDuyurusu alt sınıflarını oluşturur.
// NotificationFactory → Çalışma zamanında NotificationType'a göre doğru servisi çözümler.
builder.Services.AddTransient<IAnnouncementFactory, AnnouncementFactory>();
builder.Services.AddTransient<INotificationFactory, NotificationFactory>();

// ─────────────────────────────────────────────────────────────────────────────
// Ara Yazılım Boru Hattı
// ─────────────────────────────────────────────────────────────────────────────

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
