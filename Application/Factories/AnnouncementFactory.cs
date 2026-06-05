using AkademiPortal.Application.Interfaces;
using AkademiPortal.Domain.Entities;
using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Application.Factories
{
    /// <summary>
    /// Fabrika Örüntüsü – Duyuru nesneleri için somut üretici.
    /// Çağıran taraf yalnızca Announcement soyutlamasına bağlı kalır;
    /// ExamAnnouncement veya EventAnnouncement'ı doğrudan bilmek zorunda değildir.
    /// </summary>
    public sealed class AnnouncementFactory : IAnnouncementFactory
    {
        private static int           _idCounter   = 0;
        private static readonly object _counterLock = new object();

        public Announcement CreateAnnouncement(AnnouncementType type, string title, string content)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Duyuru başlığı boş olamaz.", nameof(title));
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Duyuru içeriği boş olamaz.", nameof(content));

            int newId;
            lock (_counterLock) { newId = ++_idCounter; }

            return type switch
            {
                AnnouncementType.Exam => new ExamAnnouncement(
                    id:          newId,
                    title:       title,
                    content:     content,
                    subjectName: ExtractSubjectHint(title),
                    examDate:    DateTime.Now.AddDays(7),
                    examRoom:    $"D-{(char)('A' + (newId % 5))}{100 + newId}"),

                AnnouncementType.Event => new EventAnnouncement(
                    id:        newId,
                    title:     title,
                    content:   content,
                    venue:     "Ana Konferans Salonu",
                    eventDate: DateTime.Now.AddDays(3),
                    organizer: "Öğrenci İşleri Dairesi"),

                _ => throw new ArgumentOutOfRangeException(nameof(type),
                         $"Desteklenmeyen AnnouncementType: {type}")
            };
        }

        /// <summary>
        /// Sınav duyuruları için başlıktan ders adı ipucu türetir.
        /// Uygun kelime bulunamazsa "Genel Ders" döner.
        /// </summary>
        private static string ExtractSubjectHint(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return "Genel Ders";

            var keywords = new[]
            {
                "Matematik", "Fizik", "Kimya", "Biyoloji", "Tarih",
                "Bilgisayar", "Edebiyat", "Mühendislik", "İktisat", "Ekonomi",
                "Hukuk", "Tıp", "İngilizce", "Türkçe", "Felsefe", "Psikoloji"
            };

            var words = title.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
                foreach (var keyword in keywords)
                    if (word.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        return keyword;

            return words.Length >= 2 ? $"{words[0]} {words[1]}" : words[0];
        }
    }
}
