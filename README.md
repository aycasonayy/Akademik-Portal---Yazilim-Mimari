# 📢 AkademiPortal_final - Akıllı Kampüs Duyuru & Bildirim Yönetimi

Bu proje, **ASP.NET Core MVC** ile **Katmanlı Mimari** (Clean Architecture) prensiplerine uygun olarak geliştirilmiş canlı bir web simülasyonudur. Projenin temel amacı, nesne yönelimli tasarımın 3 güçlü tasarım desenini çalışan bir arayüz üzerinde doğrulamaktır.

---

## 🧩 Uygulanan Tasarım Desenleri (Design Patterns)

* **Factory (Fabrika) Örüntüsü:** `Sınav` ve `Etkinlik` duyuru tipleri ile `E-Posta` ve `SMS` bildirim modelleri, gevşek bağlılık (loose coupling) ilkesine uygun olarak dinamik fabrikalar üzerinden üretilir.
* **Observer (Gözlemci) Örüntüsü:** Sistemdeki `Öğrenci` ve `Akademisyenler` birer gözlemcidir. Yeni bir duyuru yayınlandığında, seçilen hedef kitleye göre (Herkes / Sadece Öğrenci / Sadece Akademisyen) ilgili tüm gözlemcilerin `Update()` metodu tetiklenerek anlık bildirim gönderilir.
* **Singleton (Tekil) Örüntüsü:** Sistem genelindeki tüm simülasyon logları, web oturumu boyunca tek bir bellek alanında (`NotificationLogManager.Instance`) toplanır. Sayfa yenilense bile geçmiş silinmez ve canlı konsola yazdırılır.

---

## 🚀 Öne Çıkan Özellikler

* **Simülasyon Giriş Sistemi:** `Session` tabanlı giriş paneli ile Öğrenci İşleri veya Rektörlük olarak oturum açılabilir.
* **Dinamik Kullanıcı Yönetimi:** Arayüzdeki Bootstrap Modal form ile sisteme anlık yeni gözlemci eklenebilir veya listeden silinebilir.
* **Canlı Konsol Raporlama:** Gönderilen tüm e-posta ve SMS'lerin detayları ekranın altındaki terminal alanında anlık listelenir.

---

## 💻 Kurulum ve Çalıştırma

1. Proje ana dizininde konsolu açın ve çalıştırın:
```bash
   dotnet run
