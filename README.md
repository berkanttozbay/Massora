# Massora Hafriyat Yönetim Sistemi

![.NET](https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-17-DD0031?logo=angular)
![React Native](https://img.shields.io/badge/React%20Native-Expo-61DAFB?logo=react)
![IdentityServer](https://img.shields.io/badge/Duende%20IdentityServer-blue?logo=auth0)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2571A9?logo=microsoft-sql-server)

**Massora**, şantiye ve hafriyat operasyonlarını dijitalleştirmek, araç takibini, operatör puantajını ve yakıt yönetimini tek bir platformda birleştirmek için tasarlanmış modern bir yönetim sistemidir.

---

## İçindekiler
1.  [Proje Hakkında](#1-proje-hakkında)
2.  [Teknolojiler](#2-teknolojiler)
3.  [Mimarî](#3-mimarî)
4.  [Ana Özellikler](#4-ana-özellikler)
5.  [Kurulum ve Başlatma](#5-kurulum-ve-başlatma)
6.  [API Endpoint'leri](#6-api-endpointleri)
7.  [Lisans](#7-lisans)

---

## 1. Proje Hakkında

Bu proje, hafriyat sahalarındaki operasyonel verimliliği artırmayı hedefler. Kağıt üzerinde tutulan ve takibi zor olan çalışma ve yakıt kayıtlarını, operatörlerin sahada kullanabileceği basit bir mobil uygulama ve yöneticilerin ofisten takip edebileceği bir web paneli aracılığıyla dijitalleştirir.

Sistem, merkezi bir kimlik doğrulama sunucusu, ana iş mantığını yürüten bir backend API, bir web yönetim paneli ve bir mobil operatör uygulamasından oluşur.

## 2. Teknolojiler

Bu projede kullanılan ana teknolojiler ve kütüphaneler aşağıda listelenmiştir.

### Backend & Auth
* **.NET 8 / ASP.NET Core 8:** Ana API ve kimlik doğrulama sunucusu altyapısı.
* **Duende IdentityServer:** OpenID Connect ve OAuth 2.0 standartlarında merkezi kimlik doğrulama.
* **Entity Framework Core 8:** Veritabanı işlemleri için ORM.
* **SQL Server:** Veritabanı sunucusu.
* **AutoMapper:** DTO (Data Transfer Object) ve Entity dönüşümleri.
* **FluentValidation:** Gelen isteklerin ve DTO'ların sunucu tarafında doğrulanması.

### Frontend (Web - Yönetim Paneli)
* **Angular 17:** Yönetim paneli için SPA (Single Page Application) framework'ü.
* **Angular Material:** Modern ve tutarlı UI bileşenleri.
* **TypeScript:** Tip güvenliği ve ölçeklenebilir kod yapısı.
* **angular-auth-oidc-client:** OpenID Connect akışını yönetmek için.

### Frontend (Mobil - Operatör Uygulaması)
* **React Native (Expo):** iOS ve Android için cross-platform mobil uygulama geliştirme.
* **Expo Router:** Dosya tabanlı navigasyon yapısı.
* **TypeScript:** Tip güvenliği.
* **expo-auth-session:** OpenID Connect (PKCE) akışını yönetmek için.
* **React Native Paper:** Modern ve platform uyumlu UI bileşenleri.
* **Axios:** API isteklerini yönetmek için.

## 3. Mimarî

Proje, birbirinden bağımsız çalışabilen servis odaklı bir mimari kullanır:

* **Auth (IdentityServer):** Kullanıcıların kimliğini doğrular ve `access_token` üretir. Tüm kimlik yönetimi buradan yapılır.
* **BackendApi:** Ana iş mantığını içerir. Araçlar, şoförler, çalışma geçmişi ve yakıt kayıtları gibi tüm veritabanı işlemlerini yöneten API endpoint'lerini barındırır.
* **AngularWeb:** Yöneticilerin kullandığı web tabanlı arayüz. BackendApi'den veri çeker ve gösterir.
* **MassoraMobil (React Native):** Sahadaki operatörlerin kullandığı mobil arayüz. Veri girişi (çalışma başlatma/durdurma, yakıt ekleme) için kullanılır.

## 4. Ana Özellikler

-   **Merkezi Kimlik Doğrulama:** Web ve mobil uygulamalar için tek bir kullanıcı adı ve şifre ile güvenli giriş (SSO).
-   **Çalışma Süresi Takibi:** Mobil uygulama üzerinden operatörlerin çalışma saatlerini başlatıp durdurması ve bu verilerin anlık olarak sisteme kaydedilmesi.
-   **Yakıt Kaydı Yönetimi:** Operatörlerin yakıt alımlarını (litre, tutar) mobil uygulama üzerinden kolayca girmesi.
-   **Yönetim Paneli:** Web arayüzü üzerinden tüm araç, şoför, çalışma ve yakıt kayıtlarının görüntülendiği, raporlandığı ve yönetildiği merkezi panel.

## 5. Kurulum ve Başlatma

Projenin tüm parçalarını yerel geliştirme ortamınızda çalıştırmak için aşağıdaki adımları izleyin.

### Ön Gereksinimler
-   .NET 8 SDK
-   Node.js (LTS sürümü)
-   Angular CLI (`npm install -g @angular/cli`)
-   Expo CLI (`npm install -g expo-cli`)
-   SQL Server (veya SQL Server Express)
-   Android Studio (Android Emülatör için) veya Xcode (iOS Simülatörü için)

### A. Backend Kurulumu (`Auth` ve `BackendApi`)

1.  **Veritabanı Ayarları:**
    * `Auth` ve `BackendApi` projelerindeki `appsettings.Development.json` dosyalarını açın.
    * `ConnectionStrings` bölümündeki veritabanı bağlantı cümlenizi kendi SQL Server'ınıza göre güncelleyin.

2.  **Veritabanını Oluşturma:**
    * Her iki proje için de ayrı ayrı terminalde ilgili proje klasörüne gidin ve migration'ları uygulayın:
        ```bash
        cd Auth
        dotnet ef database update
        cd ../BackendApi
        dotnet ef database update
        ```

3.  **Sunucuları Çalıştırma:**
    * Her iki projeyi de `debug` modunda olmadan çalıştırın:
        ```bash
        # Auth projesi için
        cd Auth
        dotnet run

        # BackendApi projesi için (ayrı bir terminalde)
        cd BackendApi
        dotnet run
        ```
    * Varsayılan olarak `Auth` sunucusu `5139`, `BackendApi` sunucusu ise `5260` portunda çalışacaktır.

### B. Mobil (React Native) Kurulumu

1.  **Bağımlılıkları Yükleme:**
    * Mobil projenizin (`MassoraMobil`) ana dizinine gidin ve Expo için optimize edilmiş kurulum komutunu çalıştırın:
        ```bash
        cd MassoraMobil
        npx expo install
        ```

2.  **IP Adresini Yapılandırma:**
    * `src/config/index.ts` dosyasını açın.
    * `YOUR_PC_IP` değişkenini, `ipconfig` veya `ifconfig` komutuyla öğrendiğiniz kendi yerel IP adresinizle güncelleyin.

3.  **Uygulamayı Çalıştırma:**
    * Android Emülatörünüzü veya iOS Simülatörünüzü başlatın.
    * Terminalde aşağıdaki komutu çalıştırın:
        ```bash
        npx expo run:android
        # veya
        npx expo run:ios
        ```

## 6. API Endpoint'leri

Backend API, Swagger/OpenAPI arayüzü ile kendi kendini belgelemektedir. API'yi çalıştırdıktan sonra, mevcut tüm endpoint'leri ve modelleri görmek için aşağıdaki adresi ziyaret edebilirsiniz:

**`http://localhost:5260/swagger`**

## 7. Lisans

Bu proje MIT Lisansı altında lisanslanmıştır. Detaylar için `LICENSE` dosyasına bakınız.
