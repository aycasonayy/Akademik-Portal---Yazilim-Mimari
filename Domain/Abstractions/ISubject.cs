using AkademiPortal.Domain.Abstractions;

namespace AkademiPortal.Domain.Abstractions
{
    /// <summary>
    /// Gözlemci Örüntüsü – Konu (Subject) arabirimi.
    /// Bir duyuru konu olarak hareket eder; gözlemcileri kayıt altına alır ve yayınlandığında bildirir.
    /// </summary>
    public interface ISubject
    {
        void Register(IObserver observer);
        void Remove(IObserver observer);
        void Notify();
    }
}
