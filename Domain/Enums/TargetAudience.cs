namespace AkademiPortal.Domain.Enums
{
    /// <summary>
    /// Hedef kitle seçimi – bir duyurunun kimlere gönderileceğini belirler.
    /// </summary>
    public enum TargetAudience
    {
        All          = 0,   // Herkes
        StudentsOnly = 1,   // Sadece Öğrenciler
        TeachersOnly = 2    // Sadece Akademisyenler
    }
}
