# xMachineNema Lisans ve Paketleme Tasarımı v1

## 1. Dokümanın amacı
Bu doküman xMachineNema’nın ticari lisanslama mantığını, paketleme yapısını, modül açma yaklaşımını ve deployment bazlı ticari ayrımları tanımlar.

Bu dokümanın amacı:
- ürünün nasıl satılacağını netleştirmek
- teknik mimarinin ticari modelle uyumunu sağlamak
- lisans ve modül mantığını kod tasarımına doğru yansıtmak
- kullanıcı bazlı değil, operasyon bazlı bir fiyat mantığı kurmak
- küçük müşteri ile büyük müşteriyi aynı ürün ailesinde taşıyabilmek

Bu doküman fiyat rakamı vermez.
Fiyatlandırma matrisi ayrıca çıkarılacaktır.
Buradaki odak lisans mantığıdır.

---

## 2. Ana ticari ilke
xMachineNema tek eksenli lisans modeliyle satılmayacaktır.

Ana lisans modeli:
- aylık abonelik

Ana ölçüm eksenleri:
- platform
- aktif hat
- modül
- connector
- deployment tipi
- support seviyesi

Bu yaklaşım bilinçli olarak seçilmiştir.
Çünkü ürünün gerçek değeri kullanıcı sayısından değil:
- bağlanan hat sayısından
- çalışan modüllerden
- entegrasyon derinliğinden
- deployment karmaşıklığından
gelir.

---

## 3. Neden kullanıcı bazlı lisans ana model değil?
xMachineNema’da ana fiyat metriği kullanıcı olmamalıdır.

### 3.1 Sebepler
- fabrika değeri kullanıcı sayısından çok operasyon kapsamıyla ölçülür
- yönetici, kalite, bakım, operatör, entegratör gibi roller zamanla artar
- kullanıcı bazlı model müşteriyi gereksiz kısıtlar
- ürünün ana maliyeti veri toplama, entegrasyon ve operasyon akışıdır

### 3.2 Sonuç
Ana eksen:
- aktif hat
- modül
- connector
olmalıdır.

Kullanıcı bazlı sınırlar gerekiyorsa teknik sınırlama olarak değil, kurumsal paket özellikleri içinde düşünülmelidir.

---

## 4. Lisans katmanları
xMachineNema lisans modeli 4 temel katmandan oluşur.

### 4.1 Platform lisansı
Bu lisans ürünün temel açılış katmanıdır.

Kapsam:
- tenant oluşturma
- temel portal erişimi
- kullanıcı / rol / yetki omurgası
- site / line / machine modeli
- audit
- config
- branding temeli
- sistem yönetim ekranları

Platform lisansı olmadan ürün kullanılmaz.

### 4.2 Hat lisansı
Operasyonel kullanım lisansıdır.

Kapsam:
- aktif veri toplama
- hat bazlı dashboard
- alarm ve event görünürlüğü
- OEE / duruş
- operatör ekranı
- iş emri bağlamı
- hat bazlı MES akışı

Bu lisans müşterinin gerçek kullanım alanına bağlanır.

### 4.3 Modül lisansı
Çekirdeğe eklenen iş değerini açar.

Örnek modüller:
- bakım
- enerji
- andon
- WIP / depo
- planlama
- doküman genişletmeleri
- AI

### 4.4 Connector lisansı
Bazı connector’lar çekirdek üründe gelebilir, bazıları premium olabilir.

Örnek premium connector alanları:
- SAP
- Logo
- gelişmiş SQL connector
- gelişmiş REST connector
- özel müşteri connector’ları
- write-back yetenekli connector’lar

---

## 5. Lisans tipleri

### 5.1 Trial lisansı
Ürünü sınırlı şekilde denemek içindir.

Özellikleri:
- süreli
- sınırlı tenant / site / hat
- çekirdek özelliklerin gösterimi
- üretim dışı veya demo amaçlı kullanım

### 5.2 Subscription lisansı
Ana ticari lisanstır.

Özellikleri:
- aylık abonelik
- aktif hat ve modül kombinasyonu
- düzenli yenileme
- support planına bağlanabilir

### 5.3 Offline lisans
Özellikle dedicated veya on-prem müşteriler için uygundur.

Özellikleri:
- internet bağımsız aktivasyon imkanı
- süreli lisans dosyası mantığı
- renewal dosyası veya dönemsel aktivasyon
- lokalde lisans doğrulama

---

## 6. Paketleme mantığı
xMachineNema paketleri, lisans katmanlarının kullanıcıya anlaşılır hale getirilmiş sunumudur.

Önerilen ana paket ailesi:

### 6.1 Foundation
Amaç:
- ürünün giriş seviyesi başlangıcı
- küçük müşteri / PoC / ilk referans

İçerik:
- platform lisansı
- temel kullanıcı/rol yapısı
- temel dashboard
- temel alarm görünürlüğü
- sınırlı hat lisansı
- temel yönetim ekranları

### 6.2 Core MES
Amaç:
- ilk gerçek ticari çekirdek
- üretim operasyon değerini göstermek

İçerik:
- dashboard
- alarm / event
- OEE
- duruş
- iş emri
- operatör ekranı
- reçete
- batch / lot
- kalite
- raporlama
- KPI
- audit
- connector yönetimi
- lisans yönetimi

### 6.3 Operations Plus
Amaç:
- çekirdeği daha güçlü operasyon katmanına taşımak

İçerik:
- gelişmiş workflow
- daha derin KPI
- andon
- genişletilmiş raporlama
- doküman / prosedür bağlantıları
- multi-site görünürlük iyileştirmeleri

### 6.4 Enterprise
Amaç:
- kurumsal müşteriler
- güçlü izolasyon
- özel deployment
- daha gelişmiş güvenlik ve support

İçerik:
- dedicated deployment opsiyonları
- gelişmiş support
- güçlü tenant izolasyonu
- white-label genişletmeleri
- kurumsal deployment özellikleri

---

## 7. Add-on modüller
Add-on modüller çekirdeğe gömülmeyecek, lisansla açılacaktır.

### 7.1 Bakım modülü
- bakım emri
- planlı bakım
- arıza kaydı
- bakım kapanış akışı
- bakım geçmişi

### 7.2 Enerji modülü
- enerji sayaçları
- enerji trendleri
- tüketim dashboardları
- enerji KPI

### 7.3 Andon modülü
- çağrı
- problem görünürlüğü
- eskalasyon

### 7.4 WIP / Depo modülü
- yarı mamul akışı
- WIP görünürlüğü
- hareket geçmişi
- ara stok mantığı

### 7.5 Planlama modülü
- gelişmiş planlama
- sıralama
- kapasite dengesi

### 7.6 Doküman genişletme modülü
- ileri doküman yönetimi
- prosedür / talimat akışları
- kalite görseli / versiyon bağları

### 7.7 AI modülü
- anomali
- kestirim
- kalite tahmini
- öneri motorları

---

## 8. Write-back lisansı
Write-back ayrı lisans katmanı olarak değerlendirilir.

### 8.1 Neden ayrı?
Çünkü write-back:
- operasyon riskini artırır
- daha güçlü güvenlik ister
- farklı connector kabiliyetlerine bağlıdır
- ürün değerini doğrudan artırır
- fiyatlandırmada ayrıca anlam taşır

### 8.2 Write-back lisansı neyi açar?
- reçete yazma
- setpoint yazma
- iş emri parametresi gönderme
- write yeteneği olan connector’ların aktif kullanımı

### 8.3 Write-back lisansı neyi açmaz?
- güvenlik zinciri dışı komutlar
- emergency stop
- sınırsız ve denetimsiz PLC kontrolü

---

## 9. Deployment bazlı ticari ayrım
Aynı ürün farklı deployment tipleriyle sunulabilir.
Bu, ticari olarak da fark yaratmalıdır.

### 9.1 Shared cloud
- en düşük giriş maliyeti
- küçük / orta ölçek müşteriler
- hızlı başlangıç

### 9.2 Dedicated cloud
- daha yüksek izolasyon
- daha güçlü kurumsal yapı
- daha yüksek fiyat seviyesi

### 9.3 On-prem
- müşteri iç altyapısında kurulum
- güvenlik hassas tesisler
- ERP yakın entegrasyonu

### 9.4 Hybrid
- edge sahada
- portal ve merkezi yönetim merkezde
- saha ile merkez arasında dengeli model

Deployment tipi fiyatı etkileyen bir parametre olacaktır.

---

## 10. Support seviyesi
Support lisansın içine gömülü tek bir model olmak zorunda değildir.

Önerilen seviyeler:

### 10.1 Standard
- temel destek
- ticket / e-posta
- standart güncelleme

### 10.2 Enterprise
- öncelikli destek
- kurumsal SLA
- uzaktan koordinasyon
- daha kontrollü update planı

İlk kararda support tarafı enterprise mantığında ayrıştırılmak istendiği için bu model korunur.

---

## 11. Trial ve PoC farkı
Trial ile PoC aynı şey değildir.

### 11.1 Trial
- kısa süreli
- ürünün genel kabiliyetini göstermek için
- sınırlı kapsam
- düşük onboarding maliyeti

### 11.2 PoC
- belirli müşteri için
- belirli hat/tesis odaklı
- daha kontrollü ve daha ciddi kurulum
- bazı premium modüller geçici açılabilir

Bu ayrım satış sürecinde önemlidir.

---

## 12. Lisans hakları (entitlement) yaklaşımı
Lisans sadece “aktif/pasif” olarak düşünülmemelidir.

Her tenant için tutulabilecek haklar:
- aktif hat sayısı
- açık modüller
- açık connector’lar
- write-back açık mı
- offline lisans açık mı
- white-label açık mı
- deployment tipi
- support seviyesi

Bu mantık daha sonra teknik entitlement tablosuna dönüştürülebilir.

---

## 13. Lisans tasarımının teknik etkileri
Lisans tasarımı sadece satış konusu değildir.
Aşağıdaki alanlara doğrudan etki eder:
- menü görünürlüğü
- API erişimi
- background job çalışması
- connector runtime davranışı
- write-back izni
- ekran bileşeni görünürlüğü
- tenant onboarding akışı

Bu yüzden Commercial modülü çekirdekte yer alır.

---

## 14. Özellikle kaçınılacak ticari hatalar
- ana fiyatı kullanıcı sayısına bağlamak
- bütün modülleri çekirdeğe gömmek
- write-back’i ücretsiz çekirdek hak gibi görmek
- deployment farkını fiyatlamamak
- premium connector’ları ücretsiz çekirdeğe dağıtmak
- trial ile production lisansını aynı mantıkta ele almak

---

## 15. V1 için sabit ticari kararlar
- ana model aylık abonelik
- ana eksen aktif hat
- modüller add-on olabilir
- write-back ayrı lisans olabilir
- offline lisans vardır
- demo/trial vardır
- backend modül aktivasyonu vardır

---

## 16. Son karar
xMachineNema lisans modeli:
- ürünleşebilir
- küçükten büyüğe ölçeklenebilir
- modül satışını destekleyen
- connector değerini fiyatlayabilen
- deployment ve support farkını yansıtabilen
bir ticari omurga olarak korunacaktır.