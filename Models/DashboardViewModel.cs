using AkademiPortal.Domain.Entities;
using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Models
{
    /// <summary>
    /// Ana gösterge paneli görünümü için ViewModel.
    /// Kullanıcı/gözlemci listesi, duyurular, bildirim logları, özet sayaçlar
    /// ve yeni form durumu alanlarını bir arada taşır.
    /// </summary>
    public class DashboardViewModel
    {
        // ── Gözlemci listesi ─────────────────────────────────────────────────

        public IReadOnlyList<User>         Users         { get; set; } = new List<User>();

        // ── Duyuru geçmişi ───────────────────────────────────────────────────

        public IReadOnlyList<Announcement> Announcements { get; set; } = new List<Announcement>();

        // ── Tekil log ────────────────────────────────────────────────────────

        public IReadOnlyList<string> NotificationLogs { get; set; } = new List<string>();

        // ── Gösterge sayaçları ───────────────────────────────────────────────

        public int    TotalUsers          { get; set; }
        public int    TotalStudents       { get; set; }
        public int    TotalTeachers       { get; set; }
        public int    TotalAnnouncements  { get; set; }
        public int    TotalNotifications  { get; set; }
        public string SystemStatus        { get; set; } = "Aktif";

        // ── Flash mesajı ─────────────────────────────────────────────────────

        public string? FlashMessage   { get; set; }
        public bool    FlashIsSuccess { get; set; } = true;

        // ── Hesaplanan sayaçlar ───────────────────────────────────────────────

        public int EmailNotificationCount =>
            NotificationLogs.Count(l => l.Contains("[E-POSTA]"));

        public int SmsNotificationCount =>
            NotificationLogs.Count(l => l.Contains("[SMS]"));

        public int SystemEventCount =>
            NotificationLogs.Count(l => l.Contains("[SİSTEM]") || l.Contains("[SISTEM]"));

        /// <summary>En yeni önce sıralanmış duyurular.</summary>
        public IEnumerable<Announcement> AnnouncementsDescending =>
            Announcements.OrderByDescending(a => a.PublishedAt);

        // ── Statik Türkçe dönüştürücüler ─────────────────────────────────────

        public static string AnnouncementTypeTurkish(AnnouncementType type) =>
            type == AnnouncementType.Exam ? "Sınav" : "Etkinlik";

        public static string UserTypeTurkish(UserType type) =>
            type == UserType.Student ? "Öğrenci" : "Akademisyen";

        public static string TargetAudienceTurkish(TargetAudience ta) => ta switch
        {
            TargetAudience.StudentsOnly => "Sadece Öğrenciler",
            TargetAudience.TeachersOnly => "Sadece Akademisyenler",
            _                           => "Herkes"
        };

        public static string TargetAudienceIcon(TargetAudience ta) => ta switch
        {
            TargetAudience.StudentsOnly => "🎓",
            TargetAudience.TeachersOnly => "👨‍🏫",
            _                           => "👥"
        };
    }
}
