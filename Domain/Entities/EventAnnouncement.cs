using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Domain.Entities
{
    /// <summary>
    /// Fabrika Örüntüsü ürünü – Etkinlik türünde somut duyuru.
    /// Etkinliğe özgü meta veri taşır: mekan, tarih ve düzenleyen.
    /// </summary>
    public sealed class EventAnnouncement : Announcement
    {
        public string   Venue     { get; private set; }
        public DateTime EventDate { get; private set; }
        public string   Organizer { get; private set; }

        public EventAnnouncement(int id, string title, string content,
                                 string venue, DateTime eventDate,
                                 string organizer = "Öğrenci İşleri Dairesi")
            : base(id, title, content, AnnouncementType.Event)
        {
            Venue     = venue;
            EventDate = eventDate;
            Organizer = organizer;
        }

        public string GetEventSummary()
            => $"{Venue} — {EventDate:dd MMMM yyyy} — Düzenleyen: {Organizer}";
    }
}
