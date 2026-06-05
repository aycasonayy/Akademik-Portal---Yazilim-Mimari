using AkademiPortal.Domain.Abstractions;
using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Domain.Entities
{
    /// <summary>
    /// Tüm kampüs duyuruları için soyut temel sınıf.
    /// ISubject'i uygular (Gözlemci Örüntüsü) – Publish() çağrıldığında kayıtlı tüm
    /// IObserver örneklerine yayın yapar.
    /// </summary>
    public abstract class Announcement : ISubject
    {
        private readonly List<IObserver> _observers = new List<IObserver>();

        public int              Id               { get; protected set; }
        public string           Title            { get; protected set; }
        public string           Content          { get; protected set; }
        public AnnouncementType AnnouncementType { get; protected set; }
        public DateTime         PublishedAt      { get; private set; }
        public bool             IsPublished      { get; private set; }
        public int              ObserverCount    => _observers.Count;

        protected Announcement(int id, string title, string content, AnnouncementType type)
        {
            Id               = id;
            Title            = title;
            Content          = content;
            AnnouncementType = type;
            IsPublished      = false;
        }

        // ── ISubject ──────────────────────────────────────────────────────────

        public void Register(IObserver observer)
        {
            if (!_observers.Any(o => o.Id == observer.Id))
                _observers.Add(observer);
        }

        public void Remove(IObserver observer)
        {
            var existing = _observers.FirstOrDefault(o => o.Id == observer.Id);
            if (existing != null) _observers.Remove(existing);
        }

        public void Notify()
        {
            foreach (var observer in _observers.ToList())
                observer.Update(this);
        }

        // ── Yayın ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Duyuruyu yayınlar, zaman damgasını ayarlar ve gözlemci döngüsünü başlatır.
        /// </summary>
        public void Publish()
        {
            IsPublished = true;
            PublishedAt = DateTime.Now;
            Notify();
        }

        public IReadOnlyList<IObserver> GetObservers() => _observers.AsReadOnly();
    }
}
