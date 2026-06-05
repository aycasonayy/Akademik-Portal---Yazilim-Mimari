using AkademiPortal.Domain.Entities;

namespace AkademiPortal.Domain.Abstractions
{
    /// <summary>
    /// Gözlemci Örüntüsü – Gözlemci arabirimi.
    /// Duyuru olaylarından haberdar olmak isteyen her varlık bu arabirimi uygulamalıdır.
    /// </summary>
    public interface IObserver
    {
        int    Id       { get; }
        string FullName { get; }

        /// <summary>
        /// Duyuru yayınlandığında konu (Subject) tarafından çağrılır.
        /// </summary>
        void Update(Announcement announcement);
    }
}
