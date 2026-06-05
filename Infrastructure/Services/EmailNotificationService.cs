using AkademiPortal.Application.Interfaces;
using AkademiPortal.Domain.Entities;
using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Infrastructure.Services
{
    /// <summary>
    /// Fabrika Örüntüsü ürünü – E-Posta bildirim teslimatını simüle eder.
    /// Gerçek SMTP göndermek yerine Tekil INotificationLogManager'a Türkçe log yazar.
    /// </summary>
    public sealed class EmailNotificationService : INotificationService
    {
        private readonly INotificationLogManager _logManager;

        public EmailNotificationService(INotificationLogManager logManager)
        {
            _logManager = logManager ?? throw new ArgumentNullException(nameof(logManager));
        }

        public NotificationType ServiceType => NotificationType.Email;

        public void Send(User user, Announcement announcement)
        {
            string tipTurkish = announcement.AnnouncementType == AnnouncementType.Exam
                ? "Sınav" : "Etkinlik";

            string detay = announcement switch
            {
                ExamAnnouncement exam =>
                    $"Ders: {exam.SubjectName}, Tarih: {exam.ExamDate:dd MMMM yyyy}, Salon: {exam.ExamRoom}",
                EventAnnouncement evt =>
                    $"Mekan: {evt.Venue}, Tarih: {evt.EventDate:dd MMMM yyyy}, Düzenleyen: {evt.Organizer}",
                _ => announcement.Content.Length > 60
                         ? announcement.Content[..60] + "…"
                         : announcement.Content
            };

            string log = $"[E-POSTA] ✉  {user.UserTypeTurkish} {user.FullName} " +
                         $"<{user.Email}> adresine gönderildi → " +
                         $"\"{announcement.Title}\" [{tipTurkish}] | {detay}";

            _logManager.AddLog(log);
        }
    }
}
