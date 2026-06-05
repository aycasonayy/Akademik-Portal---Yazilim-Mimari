using AkademiPortal.Domain.Abstractions;
using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Domain.Entities
{
    /// <summary>
    /// Kampüs kullanıcısını (Öğrenci veya Akademisyen) temsil eder.
    /// IObserver'ı uygular (Gözlemci Örüntüsü) – kayıtlıyken Duyuru güncellemelerini alır.
    /// Update() metodu, kullanıcının yerel bildirim geçmişine gelen duyuruyu kaydeder.
    /// Asıl E-Posta/SMS gönderimi denetleyici katmanındaki bildirim servisleri tarafından yapılır.
    /// </summary>
    public class User : IObserver
    {
        // ── IObserver üyeleri ─────────────────────────────────────────────────
        public int    Id       { get; private set; }
        public string FullName { get; private set; }

        // ── Ek özellikler ─────────────────────────────────────────────────────
        public string   Email    { get; private set; }
        public string   Phone    { get; private set; }
        public UserType UserType { get; private set; }

        /// <summary>
        /// Bu kullanıcının oturum boyunca aldığı bildirimlerin geçmişi.
        /// </summary>
        public List<string> ReceivedNotifications { get; private set; } = new List<string>();

        public User(int id, string fullName, string email, string phone, UserType userType)
        {
            Id       = id;
            FullName = fullName;
            Email    = email;
            Phone    = phone;
            UserType = userType;
        }

        // ── IObserver.Update ──────────────────────────────────────────────────

        public void Update(Announcement announcement)
        {
            string entry = $"[{DateTime.Now:HH:mm:ss}] \"{announcement.Title}\" ({UserTypeTurkish}) bildirildi — " +
                           $"Yayın: {announcement.PublishedAt:HH:mm:ss}";
            ReceivedNotifications.Add(entry);
        }

        // ── Türkçe görüntüleme yardımcıları ───────────────────────────────────

        public string UserTypeTurkish => UserType switch
        {
            UserType.Student => "Öğrenci",
            UserType.Teacher => "Akademisyen",
            _                => "Bilinmiyor"
        };

        public string UserTypeBadgeClass => UserType switch
        {
            UserType.Student => "bg-primary",
            UserType.Teacher => "bg-warning text-dark",
            _                => "bg-secondary"
        };

        public string UserTypeIcon => UserType switch
        {
            UserType.Student => "bi-mortarboard-fill",
            UserType.Teacher => "bi-person-badge-fill",
            _                => "bi-person-fill"
        };

        public override string ToString() => $"{UserTypeTurkish} {FullName} <{Email}>";
    }
}
