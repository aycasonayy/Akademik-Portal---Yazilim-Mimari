using AkademiPortal.Domain.Entities;
using AkademiPortal.Domain.Enums;

namespace AkademiPortal.Infrastructure.Data
{
    /// <summary>
    /// Dinamik bellek içi veri deposu.
    /// Önceden tanımlı 3 Öğrenci + 2 Akademisyen ile başlar.
    /// Çalışma zamanında kullanıcı eklenip çıkarılabilir (Gözlemci Yönetimi).
    /// Tüm üyeler thread-safe'dir.
    /// </summary>
    public static class DataSeeder
    {
        // ── Kullanıcı deposu ───────────────────────────────────────────────────

        private static readonly List<User> _users = new List<User>
        {
            new User(1, "Ayça Sonay Yıldırım", "ayca.yildirim@uni.edu.tr",  "+90-532-101-0001", UserType.Student),
            new User(2, "Elif Kaya",            "elif.kaya@uni.edu.tr",      "+90-532-101-0002", UserType.Student),
            new User(3, "Mert Demir",           "mert.demir@uni.edu.tr",     "+90-532-101-0003", UserType.Student),
            new User(4, "Prof. Dr. Ahmet Yılmaz","a.yilmaz@uni.edu.tr",     "+90-542-001-0001", UserType.Teacher),
            new User(5, "Dr. Zeynep Arslan",    "z.arslan@uni.edu.tr",       "+90-542-001-0002", UserType.Teacher)
        };

        private static readonly object _usersLock = new object();

        // Thread-safe auto-increment ID; önceden tanımlı kullanıcılar 1-5'i kullanır.
        private static int _nextId = 5;

        // ── Duyuru deposu ──────────────────────────────────────────────────────

        private static readonly List<Announcement> _announcements = new List<Announcement>();
        private static readonly object             _announcementsLock = new object();

        // ── Kullanıcı okuma ───────────────────────────────────────────────────

        public static IReadOnlyList<User> GetUsers()
        {
            lock (_usersLock) { return _users.ToList().AsReadOnly(); }
        }

        // ── Kullanıcı ekleme ──────────────────────────────────────────────────

        public static void AddUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            lock (_usersLock) { _users.Add(user); }
        }

        // ── Kullanıcı kaldırma ────────────────────────────────────────────────

        /// <summary>Belirtilen ID'li kullanıcıyı listeden kaldırır. Bulunamazsa false döner.</summary>
        public static bool RemoveUser(int userId)
        {
            lock (_usersLock)
            {
                var user = _users.FirstOrDefault(u => u.Id == userId);
                if (user == null) return false;
                _users.Remove(user);
                return true;
            }
        }

        /// <summary>Thread-safe yeni kullanıcı ID'si üretir (her çağrıda artar).</summary>
        public static int GetNextUserId()
        {
            return Interlocked.Increment(ref _nextId);
        }

        // ── Duyuru işlemleri ──────────────────────────────────────────────────

        public static IReadOnlyList<Announcement> GetAnnouncements()
        {
            lock (_announcementsLock) { return _announcements.ToList().AsReadOnly(); }
        }

        public static void AddAnnouncement(Announcement announcement)
        {
            if (announcement == null) throw new ArgumentNullException(nameof(announcement));
            lock (_announcementsLock) { _announcements.Add(announcement); }
        }

        // ── Toplamlar ─────────────────────────────────────────────────────────

        public static int TotalUsers
        {
            get { lock (_usersLock) { return _users.Count; } }
        }

        public static int TotalStudents
        {
            get { lock (_usersLock) { return _users.Count(u => u.UserType == UserType.Student); } }
        }

        public static int TotalTeachers
        {
            get { lock (_usersLock) { return _users.Count(u => u.UserType == UserType.Teacher); } }
        }

        public static int TotalAnnouncements
        {
            get { lock (_announcementsLock) { return _announcements.Count; } }
        }
    }
}
