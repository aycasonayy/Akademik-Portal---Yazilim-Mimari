using AkademiPortal.Application.Interfaces;
using AkademiPortal.Domain.Entities;
using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Infrastructure.Services
{
    /// <summary>
    /// Fabrika Örüntüsü ürünü – SMS bildirim teslimatını simüle eder.
    /// Gerçek SMS ağ geçidi çağrısı yerine Tekil INotificationLogManager'a Türkçe log yazar.
    /// </summary>
    public sealed class SMSNotificationService : INotificationService
    {
        private readonly INotificationLogManager _logManager;

        public SMSNotificationService(INotificationLogManager logManager)
        {
            _logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
        }

        public NotificationType ServiceType => NotificationType.SMS;

        public void Send(User user, Announcement announcement)
        {
            string tipTurkish = announcement.AnnouncementType == AnnouncementType.Exam
                ? "Sınav" : "Etkinlik";

            string kisaIcerik = announcement.Content.Length > 60
                ? announcement.Content[..60] + "…"
                : announcement.Content;

            string log = $"[SMS]    📱 {user.UserTypeTurkish} {user.FullName} " +
                         $"({user.Phone}) numarasına gönderildi → " +
                         $"\"{announcement.Title}\" [{tipTurkish}]: {kisaIcerik}";

            _logManager.AddLog(log);
        }
    }
}
