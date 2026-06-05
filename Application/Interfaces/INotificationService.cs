using AkademiPortal.Domain.Entities;
using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Application.Interfaces
{
    /// <summary>
    /// Tek kanallı bildirim teslimat servisinin soyutlaması (E-Posta veya SMS).
    /// Somut uygulamalar, log girişini Tekil INotificationLogManager'a yazar.
    /// </summary>
    public interface INotificationService
    {
        NotificationType ServiceType { get; }
        void Send(User user, Announcement announcement);
    }
}
