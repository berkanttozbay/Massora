# HafriyatProjem Mobil Uygulama

Bu proje, HafriyatProjem şantiye yönetimi sisteminin mobil uygulamasıdır. Operatörler ve şoförler için tasarlanmış olup, çalışma süresi takibi ve yakıt alım kayıtları için kullanılır.

## Özellikler

- 🔐 **Güvenli Kimlik Doğrulama**: OAuth2 + PKCE flow ile IdentityServer entegrasyonu
- ⏱️ **Çalışma Süresi Takibi**: İş başlatma ve durdurma işlemleri
- ⛽ **Yakıt Alım Kayıtları**: Yakıt miktarı ve maliyet takibi
- 🚗 **Araç Yönetimi**: Araç listesi ve seçimi
- 🔄 **Otomatik Token Yenileme**: Access token'ların otomatik yenilenmesi
- 💾 **Güvenli Veri Saklama**: SecureStore ile token'ların güvenli saklanması

## Teknolojiler

- **React Native** - Mobil uygulama framework'ü
- **Expo** - Geliştirme platformu
- **Expo Router** - Dosya tabanlı routing
- **React Native Paper** - UI component library
- **Expo Auth Session** - OAuth2 kimlik doğrulama
- **Expo Secure Store** - Güvenli veri saklama

## Kurulum

1. Bağımlılıkları yükleyin:
   ```bash
   npm install
   ```

2. Uygulamayı başlatın:
   ```bash
   npx expo start
   ```

## Konfigürasyon

### Auth Sunucusu
- **URL**: `http://192.168.1.42:5139`
- **Client ID**: `mobil-hafriyat-client`
- **Redirect URI**: `com.massora.hafriyatapp:/oauth2redirect`

### API Sunucusu
- **URL**: `http://192.168.1.42:5260`
- **Endpoints**:
  - `GET /api/vehicles` - Araç listesi
  - `POST /api/workhistories` - Çalışma başlatma
  - `PUT /api/workhistories/{id}` - Çalışma durdurma
  - `POST /api/fuelhistories` - Yakıt kaydı ekleme

## Proje Yapısı

```
HafriyatMobilApp/
├── app/                    # Expo Router sayfaları
│   ├── (auth)/            # Kimlik doğrulama sayfaları
│   │   ├── login.tsx      # Giriş sayfası
│   │   └── _layout.tsx    # Auth layout
│   ├── (app)/             # Ana uygulama sayfaları
│   │   ├── home.tsx       # Ana sayfa
│   │   ├── work-tracking.tsx  # Çalışma takibi
│   │   ├── fuel-add.tsx   # Yakıt ekleme
│   │   └── _layout.tsx    # App layout
│   ├── _layout.tsx        # Ana layout
│   └── index.tsx          # Giriş noktası
├── components/            # Yeniden kullanılabilir bileşenler
├── contexts/             # React Context'ler
│   └── AuthContext.tsx   # Kimlik doğrulama context'i
├── services/             # API servisleri
│   └── api.ts           # API client
├── constants/            # Sabitler
│   └── config.ts        # Konfigürasyon
├── types/               # TypeScript tip tanımları
│   └── auth.ts         # Auth tipleri
└── assets/             # Statik dosyalar
```

## Kullanım

### Giriş Yapma
1. Uygulama açıldığında giriş sayfası görünür
2. "Şirket Hesabıyla Giriş Yap" butonuna tıklayın
3. IdentityServer giriş sayfası açılır
4. Kullanıcı adı ve şifrenizi girin
5. Başarılı giriş sonrası ana sayfaya yönlendirilirsiniz

### Çalışma Takibi
1. Ana sayfada "Çalışma Başlat" butonuna tıklayın
2. Araç seçin
3. "İŞİ BAŞLAT" butonuna tıklayın
4. Çalışma süresi sayacı başlar
5. İş bitince "İŞİ DURDUR" butonuna tıklayın

### Yakıt Ekleme
1. Ana sayfada "Yakıt Ekle" butonuna tıklayın
2. Araç seçin
3. Yakıt miktarı ve tutarını girin
4. "KAYDET" butonuna tıklayın

## Geliştirme

### Yeni Özellik Ekleme
1. İlgili sayfayı `app/(app)/` dizininde oluşturun
2. API endpoint'ini `services/api.ts`'e ekleyin
3. Gerekirse yeni tip tanımlarını `types/` dizininde oluşturun

### Auth Yapısı
- `contexts/AuthContext.tsx`: Kimlik doğrulama state yönetimi
- `constants/config.ts`: Auth ve API konfigürasyonu
- `types/auth.ts`: Auth ile ilgili tip tanımları

## Lisans

Bu proje özel kullanım için geliştirilmiştir.
