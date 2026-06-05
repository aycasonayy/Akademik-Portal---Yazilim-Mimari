using Microsoft.AspNetCore.Mvc;
using AkademiPortal.Application.Interfaces;
using AkademiPortal.Domain.Entities;
using AkademiPortal.Domain.Enums;
using AkademiPortal.Infrastructure.Data;
using AkademiPortal.Models;

namespace AkademiPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAnnouncementFactory    _announcementFactory;
        private readonly INotificationFactory    _notificationFactory;
        private readonly INotificationLogManager _logManager;

        public HomeController(
            IAnnouncementFactory    announcementFactory,
            INotificationFactory    notificationFactory,
            INotificationLogManager logManager)
        {
            _announcementFactory = announcementFactory
                ?? throw new ArgumentNullException(nameof(announcementFactory));
            _notificationFactory = notificationFactory
                ?? throw new ArgumentNullException(nameof(notificationFactory));
            _logManager = logManager
                ?? throw new ArgumentNullException(nameof(logManager));
        }

        // GET /
        [HttpGet]
        public IActionResult Index()
        {
            var users         = DataSeeder.GetUsers();
            var announcements = DataSeeder.GetAnnouncements();
            var logs          = _logManager.GetLogs();

            var vm = new DashboardViewModel
            {
                Users              = users,
                Announcements      = announcements,
                NotificationLogs   = logs,
                TotalUsers         = DataSeeder.TotalUsers,
                TotalStudents      = DataSeeder.TotalStudents,
                TotalTeachers      = DataSeeder.TotalTeachers,
                TotalAnnouncements = DataSeeder.TotalAnnouncements,
                TotalNotifications = logs.Count,
                SystemStatus       = "Aktif",
                FlashMessage       = TempData["FlashMessage"] as string,
                FlashIsSuccess     = TempData["FlashIsSuccess"] is bool b ? b : true
            };

            return View(vm);
        }

        // POST /Home/PublishAnnouncement
        // targetAudience string values: "Herkes" | "Student" | "Teacher"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PublishAnnouncement(
            string           title,
            string           content,
            AnnouncementType type,
            string           targetAudience = "Herkes")
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                TempData["FlashMessage"]   = "⚠ Başlık ve İçerik alanları zorunludur.";
                TempData["FlashIsSuccess"] = false;
                return RedirectToAction(nameof(Index));
            }

            // Factory Pattern: instantiate the correct Announcement subclass
            var announcement = _announcementFactory.CreateAnnouncement(
                type, title.Trim(), content.Trim());

            // Observer Pattern: filter users by target audience
            var tumKullanicilar = DataSeeder.GetUsers();

            var hedefKullanicilar = targetAudience switch
            {
                "Student" => tumKullanicilar.Where(u => u.UserType == UserType.Student).ToList(),
                "Teacher" => tumKullanicilar.Where(u => u.UserType == UserType.Teacher).ToList(),
                _         => tumKullanicilar.ToList()   // "Herkes" and any unknown value
            };

            // Observer Pattern: register only filtered users, then publish
            foreach (var user in hedefKullanicilar)
                announcement.Register(user);

            // Publish() -> sets IsPublished, stamps PublishedAt, calls Notify() -> each User.Update()
            announcement.Publish();

            string tipAdi = announcement.AnnouncementType == AnnouncementType.Exam
                ? "Sınav" : "Etkinlik";

            string hedefAdi = targetAudience switch
            {
                "Student" => "Sadece Öğrenciler",
                "Teacher" => "Sadece Akademisyenler",
                _         => "Herkes"
            };

            // Singleton Pattern: write system log entry
            _logManager.AddLog(
                $"[SİSTEM] 📢 YENİ DUYURU YAYINLANDI → \"{announcement.Title}\" " +
                $"(Tip: {tipAdi}, ID: {announcement.Id}) | " +
                $"🎯 Hedef Kitle: {hedefAdi} | " +
                $"{hedefKullanicilar.Count}/{tumKullanicilar.Count} kullanıcı bildirildi.");

            // Factory Pattern: resolve Email and SMS services, send simulated notifications
            var emailSvc = _notificationFactory.CreateNotificationService(NotificationType.Email);
            var smsSvc   = _notificationFactory.CreateNotificationService(NotificationType.SMS);

            foreach (var user in hedefKullanicilar)
            {
                emailSvc.Send(user, announcement);
                smsSvc.Send(user, announcement);
            }

            int toplamGonderilen = hedefKullanicilar.Count * 2;

            _logManager.AddLog(
                $"[SİSTEM] ✅ Bildirim döngüsü TAMAMLANDI — {toplamGonderilen} bildirim " +
                $"({hedefKullanicilar.Count} E-Posta + {hedefKullanicilar.Count} SMS). " +
                $"Uygulanan filtre: {hedefAdi}.");

            DataSeeder.AddAnnouncement(announcement);

            TempData["FlashMessage"]   =
                $"✅ \"{announcement.Title}\" başarıyla yayınlandı! " +
                $"{toplamGonderilen} bildirim {hedefKullanicilar.Count} " +
                $"kullanıcıya ({hedefAdi}) gönderildi.";
            TempData["FlashIsSuccess"] = true;

            return RedirectToAction(nameof(Index));
        }

        // POST /Home/RemoveObserver
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveObserver(int userId)
        {
            var user = DataSeeder.GetUsers().FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                TempData["FlashMessage"]   = "⚠ Belirtilen kullanıcı bulunamadı.";
                TempData["FlashIsSuccess"] = false;
                return RedirectToAction(nameof(Index));
            }

            bool kaldirildi = DataSeeder.RemoveUser(userId);

            if (kaldirildi)
            {
                _logManager.AddLog(
                    $"[SİSTEM] 🗑 GÖZLEMCI KALDIRILDI → {user.UserTypeTurkish} {user.FullName} " +
                    $"({user.Email}) gözlemci listesinden çıkarıldı. " +
                    $"Kalan kullanıcı sayısı: {DataSeeder.TotalUsers}.");

                TempData["FlashMessage"]   =
                    $"🗑 {user.UserTypeTurkish} {user.FullName} " +
                    $"gözlemci listesinden başarıyla kaldırıldı.";
                TempData["FlashIsSuccess"] = true;
            }
            else
            {
                TempData["FlashMessage"]   = "⚠ Kullanıcı kaldırılırken bir hata oluştu.";
                TempData["FlashIsSuccess"] = false;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST /Home/AddObserver
        // Directly calls DataSeeder.AddUser(newUser) — exactly as shown in the screenshot.
        // Form field name attributes: name="name", name="email", name="phone", name="type"
        // must match these parameter names for ASP.NET Core model binding to work.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddObserver(string name, string email, string phone, string type)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email))
            {
                // Map string "type" value to UserType enum
                var userType = type == "Teacher" ? UserType.Teacher : UserType.Student;

                // Dynamic ID based on current list size + 1
                int newId = DataSeeder.GetUsers().Count + 1;

                string telefon = string.IsNullOrWhiteSpace(phone) ? "-" : phone.Trim();

                var newUser = new User(newId, name.Trim(), email.Trim(), telefon, userType);

                // Calling the exact static repository method from the screenshot
                DataSeeder.AddUser(newUser);

                string rolAdi = userType == UserType.Teacher ? "Akademisyen" : "Öğrenci";

                _logManager.AddLog(
                    $"[SİSTEM] 👤 YENİ GÖZLEMCI EKLENDİ → {rolAdi} {name.Trim()} " +
                    $"({email.Trim()}) gözlemci listesine katıldı. " +
                    $"Toplam kullanıcı: {DataSeeder.TotalUsers}.");

                TempData["FlashMessage"]   =
                    $"✅ {rolAdi} {name.Trim()} gözlemci listesine başarıyla eklendi.";
                TempData["FlashIsSuccess"] = true;
            }
            else
            {
                TempData["FlashMessage"]   = "⚠ Ad Soyad ve E-posta alanları zorunludur.";
                TempData["FlashIsSuccess"] = false;
            }

            return RedirectToAction("Index");
        }

        // GET /Home/GetLogs (JSON endpoint)
        [HttpGet]
        public IActionResult GetLogs()
        {
            var logs = _logManager.GetLogs();
            return Json(new
            {
                logs         = logs,
                adet         = logs.Count,
                ePostaAdet   = logs.Count(l => l.Contains("[E-POSTA]")),
                smsAdet      = logs.Count(l => l.Contains("[SMS]")),
                sistemAdet   = logs.Count(l => l.Contains("[SİSTEM]")),
                zamanDamgasi = DateTime.Now.ToString("HH:mm:ss")
            });
        }

        // POST /Home/ClearLogs
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearLogs()
        {
            _logManager.ClearLogs();
            TempData["FlashMessage"]   = "🗑 Bildirim logları başarıyla temizlendi.";
            TempData["FlashIsSuccess"] = true;
            return RedirectToAction(nameof(Index));
        }
    }
}
