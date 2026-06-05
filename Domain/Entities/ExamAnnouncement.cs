using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Domain.Entities
{
    /// <summary>
    /// Fabrika Örüntüsü ürünü – Sınav türünde somut duyuru.
    /// Sınav'a özgü meta veri taşır: ders adı, tarih ve salon.
    /// </summary>
    public sealed class ExamAnnouncement : Announcement
    {
        public string   SubjectName { get; private set; }
        public DateTime ExamDate    { get; private set; }
        public string   ExamRoom    { get; private set; }

        public ExamAnnouncement(int id, string title, string content,
                                string subjectName, DateTime examDate, string examRoom = "Belirtilmedi")
            : base(id, title, content, AnnouncementType.Exam)
        {
            SubjectName = subjectName;
            ExamDate    = examDate;
            ExamRoom    = examRoom;
        }

        public string GetExamSummary()
            => $"{SubjectName} — {ExamDate:dd MMMM yyyy} — Salon: {ExamRoom}";
    }
}
