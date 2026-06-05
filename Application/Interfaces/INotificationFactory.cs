using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Application.Interfaces
{
    /// <summary>
    /// Fabrika Örüntüsü arabirimi – NotificationType'a göre
    /// doğru INotificationService uygulamasını çalışma zamanında çözümler.
    /// </summary>
    public interface INotificationFactory
    {
        INotificationService CreateNotificationService(NotificationType type);
    }
}
