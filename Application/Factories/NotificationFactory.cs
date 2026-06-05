using AkademiPortal.Application.Interfaces;
using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Application.Factories
{
    /// <summary>
    /// Fabrika Örüntüsü – INotificationService örnekleri için somut üretici.
    /// DI kabı, yapıcıya IEnumerable&lt;INotificationService&gt; olarak
    /// tüm kayıtlı uygulamaları enjekte eder; fabrika bunları ServiceType'a göre indeksler.
    /// </summary>
    public sealed class NotificationFactory : INotificationFactory
    {
        private readonly IReadOnlyDictionary<NotificationType, INotificationService> _serviceMap;

        public NotificationFactory(IEnumerable<INotificationService> services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            _serviceMap = services.ToDictionary(s => s.ServiceType, s => s);
        }

        public INotificationService CreateNotificationService(NotificationType type)
        {
            if (_serviceMap.TryGetValue(type, out var service))
                return service;

            throw new ArgumentException(
                $"'{type}' türü için kayıtlı INotificationService uygulaması bulunamadı. " +
                $"Mevcut türler: [{string.Join(", ", _serviceMap.Keys)}]",
                nameof(type));
        }
    }
}
