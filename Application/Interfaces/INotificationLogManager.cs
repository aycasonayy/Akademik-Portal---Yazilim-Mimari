namespace AkademiPortal.Application.Interfaces
{
    /// <summary>
    /// Tekil Örüntüsü arabirimi – merkezi, bellek içi bildirim log deposu.
    /// Somut uygulama thread-safe olmalı ve işlem başına tek örnek bulunmalıdır.
    /// </summary>
    public interface INotificationLogManager
    {
        void AddLog(string logEntry);
        IReadOnlyList<string> GetLogs();
        void ClearLogs();
        int TotalLogCount { get; }
    }
}
