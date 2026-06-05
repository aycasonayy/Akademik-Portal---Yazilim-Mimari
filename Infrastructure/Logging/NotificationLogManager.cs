using AkademiPortal.Application.Interfaces;

namespace AkademiPortal.Infrastructure.Logging
{
    /// <summary>
    /// Tekil Örüntüsü – İşlem genelinde tek, bellek içi bildirim log deposu.
    /// Thread güvenliği, her değiştirme işleminde ayrılmış bir kilit nesnesiyle sağlanır.
    ///
    /// DI kaydı (Program.cs):
    ///   builder.Services.AddSingleton&lt;INotificationLogManager&gt;(_ => NotificationLogManager.Instance);
    /// </summary>
    public sealed class NotificationLogManager : INotificationLogManager
    {
        // ── Tekil altyapısı ───────────────────────────────────────────────────

        private static NotificationLogManager? _instance;
        private static readonly object         _singletonLock = new object();

        private NotificationLogManager() { }

        /// <summary>
        /// Thread-safe, geç başlatılan tekil örnek.
        /// Çift kontrollü kilitleme yalnızca bir örneğin oluşturulmasını garanti eder.
        /// </summary>
        public static NotificationLogManager Instance
        {
            get
            {
                if (_instance is null)
                {
                    lock (_singletonLock)
                    {
                        _instance ??= new NotificationLogManager();
                    }
                }
                return _instance;
            }
        }

        // ── Bellek içi log deposu ─────────────────────────────────────────────

        private readonly List<string> _logs     = new List<string>();
        private readonly object       _logsLock = new object();

        // ── INotificationLogManager uygulaması ───────────────────────────────

        /// <summary>
        /// logEntry'ye SS:dd:SS zaman damgası ekleyerek depolar. Thread-safe.
        /// </summary>
        public void AddLog(string logEntry)
        {
            if (string.IsNullOrWhiteSpace(logEntry)) return;
            string timestamped = $"[{DateTime.Now:HH:mm:ss}] {logEntry}";
            lock (_logsLock) { _logs.Add(timestamped); }
        }

        /// <summary>
        /// Tüm logların sıralı, salt okunur anlık görüntüsünü döner. Thread-safe.
        /// </summary>
        public IReadOnlyList<string> GetLogs()
        {
            lock (_logsLock) { return _logs.ToList().AsReadOnly(); }
        }

        /// <summary>Tüm log girişlerini siler. Thread-safe.</summary>
        public void ClearLogs()
        {
            lock (_logsLock) { _logs.Clear(); }
        }

        /// <summary>Depodaki mevcut log girişi sayısı. Thread-safe.</summary>
        public int TotalLogCount
        {
            get { lock (_logsLock) { return _logs.Count; } }
        }
    }
}
