using AkademiPortal.Domain.Entities;
using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Application.Interfaces
{
    /// <summary>
    /// Fabrika Örüntüsü arabirimi – istenen AnnouncementType'a göre
    /// doğru Announcement alt sınıfını (ExamAnnouncement / EventAnnouncement) oluşturur.
    /// </summary>
    public interface IAnnouncementFactory
    {
        Announcement CreateAnnouncement(AnnouncementType type, string title, string content);
    }
}
