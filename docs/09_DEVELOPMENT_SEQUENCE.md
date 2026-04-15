# xMachineNema Geliştirme Sırası v1

## 1. Dokümanın amacı
Bu doküman xMachineNema’nın teknik geliştirme sırasını, faz mantığını ve ilk canlıya çıkış yolunu tanımlar.

Bu dokümanın amacı:
- hangi işin hangi sırayla yapılacağını netleştirmek
- Cursor ve geliştiricilerin rastgele alanlara dağılmasını önlemek
- önce omurgayı, sonra iş değerini, sonra genişlemeyi inşa etmek
- “her şeyi aynı anda yapma” tuzağını engellemek

---

## 2. Resmi geliştirme sırası
Sabit geliştirme sırası:

1. Platform
2. Integration
3. MES
4. Modüller

Bu sıra öneri değil, resmi yön kabul edilir.

---

## 3. Bu sıranın mantığı

### 3.1 Neden önce Platform?
Çünkü:
- tenant olmadan ürünleşme olmaz
- kullanıcı/rol olmadan ekran güvenliği olmaz
- site/line/machine modeli olmadan hiçbir veri bağlamı kurulamaz
- lisans/modül açma olmadan ticari çekirdek kurulamaz

### 3.2 Neden sonra Integration?
Çünkü:
- veri gelmeden dashboard anlamlı değildir
- connector omurgası kurulmadan MES akışları yapay kalır
- mapping ve standardization ürünün ana farkıdır

### 3.3 Neden sonra MES?
Çünkü:
- iş emri, reçete, kalite, lot ve operatör akışı gerçek veri ve bağlamla değer üretir

### 3.4 Neden en son Modüller?
Çünkü:
- bakım, enerji, planlama gibi alanlar çekirdeğin üstüne oturmalıdır
- çekirdek stabil olmadan modüller sadece yük getirir

---

## 4. Faz 0 - Dokümantasyon ve repo hazırlığı
### Amaç
- ürün kararlarını repo içine taşımak
- docs klasörünü oluşturmak
- ortak gerçeklik seti oluşturmak

### Çıktılar
- master architecture snapshot
- product positioning
- system architecture
- module boundaries
- database design
- connector framework
- licensing and packaging
- v1 scope
- supported connectors matrix
- development sequence

### Başarı kriteri
Repo artık “boş kod deposu” değil, belgelenmiş ürün temeli haline gelmiş olmalıdır.

---

## 5. Faz 1 - Solution ve proje iskeleti
### Amaç
- ana solution yapısını kurmak
- proje sınırlarını oluşturmak
- modüler monolith omurgasını ayağa kaldırmak

### Beklenen projeler
- Web
- Api
- AppHost
- ServiceDefaults
- SharedKernel
- Domain
- Application
- Infrastructure
- Persistence
- Module.Platform
- Module.Auth
- Module.Commercial
- Module.Integration
- Module.MES
- Module.Quality
- Module.Eventing
- Module.Workflow
- Module.Docs
- Module.Plugin
- Connectors.Abstractions
- Connectors.Contracts
- Connectors.Runtime
- Connectors.Mapping
- Connectors.Health
- Connectors.Sync
- Connectors.Commands
- Connector.OPCUA
- Connector.S7
- Connector.ModbusTcp
- Connector.SAP
- Connector.REST
- Edge.Agent
- Edge.LocalStore

### Başarı kriteri
- solution derlenir
- proje referansları mantıklıdır
- mimari tek projeye çökertilmemiştir

---

## 6. Faz 2 - Platform çekirdeği
### Amaç
Ürünün ürünleşme omurgasını kurmak.

### Kapsam
- tenant
- enterprise/site/building/line/machine/station
- kullanıcı / rol / yetki
- scope mantığı
- lisans / modül aktivasyonu
- tenant ayarları
- branding temeli

### Başarı kriteri
- temel veri modeli migration ile oluşur
- tenant bazlı giriş ve yetki omurgası oluşur
- lisans ve modül görünürlüğü altyapısı oluşur

---

## 7. Faz 3 - Integration çekirdeği
### Amaç
xMachine’in ana fark yaratan connector omurgasını kurmak.

### Kapsam
- connector abstractions
- connector contracts
- connector runtime
- mapping engine
- transformation mantığı
- health izleme
- sync yapısı
- connector registry / instance mantığı

### İlk somut connector hedefi
- OPC UA
- S7 Native
- Modbus TCP
- SAP
- REST

### Başarı kriteri
- en az bir kaynaktan veri alınabilir
- mapping ile canonical modele çevrilebilir
- connector health görülebilir
- sync kayıtları izlenebilir

---

## 8. Faz 4 - MES çekirdeği
### Amaç
gerçek iş değerini üreten operasyon çekirdeğini kurmak

### Kapsam
- iş emri
- reçete
- reçete parametresi
- lot / batch
- üretim bildirimi
- kalite
- alarm / event
- duruş
- OEE
- KPI
- audit
- operatör akışı

### Başarı kriteri
- ERP’den gelen iş emri sistem içinde görülebilir
- reçete bağlanabilir
- lot/batch oluşabilir
- kalite kaydı girilebilir
- OEE ve duruş görülebilir

---

## 9. Faz 5 - İlk çalışan UI
### Amaç
teknik omurgayı görünür ve test edilebilir kullanıcı deneyimine çevirmek

### Kapsam
- login
- yönetici ana dashboard
- hat detay
- makine detay
- iş emri ekranı
- reçete ekranı
- kalite ekranı
- alarm ekranı
- connector yönetimi
- tenant / lisans yönetimi

### Başarı kriteri
V1 çekirdeğin tüm ana kullanım alanları UI’den dolaşılabilir hale gelmelidir.

---

## 10. Faz 6 - İlk canlı çekirdek
### Amaç
kontrollü bir ilk saha kapsamı tanımlamak

### Önerilen kapsam
- 1 tenant
- 1 site
- 1–2 hat
- OPC UA + S7 Native
- SAP veya generic REST ERP
- Dashboard
- Alarm / Event
- OEE
- Duruş
- İş emri
- Operatör
- Reçete
- Batch / lot
- Kalite
- Audit
- Connector yönetimi
- Lisans yönetimi

### Başarı kriteri
Ürün bir pilot veya ilk canlı kurulum için sahaya taşınabilecek seviyeye gelmiş olur.

---

## 11. Faz 7 - Add-on modüller
### Amaç
çekirdeği bozmadan ticari genişlemeyi başlatmak

### Kapsam
- Bakım
- Enerji
- Andon
- Doküman genişletmeleri
- WIP / Depo
- Planlama

### Kural
Add-on modüller çekirdeğe gömülmeyecek; lisansla açılacaktır.

---

## 12. Faz 8 - İleri fazlar
### Kapsam
- AI
- Mobil saha
- gelişmiş kurumsal güvenlik
- genişletilmiş connector ekosistemi
- public plugin/SDK olgunlaştırması
- gelişmiş time-series tasarımı

---

## 13. Seed / demo veri kullanımı
Geliştirme ve demo için minimal ama genişletilebilir seed veri kabul edilir.

Başlangıç seti örneği:
- 1 tenant
- 1 enterprise
- 1 site
- 2 line
- birkaç machine
- temel kullanıcı rolleri
- çekirdek modül aktivasyonları
- örnek connector instance’ları
- örnek production order
- örnek recipe
- örnek lot
- örnek quality record
- örnek alarm/downtime

Bu veri:
- zorunlu minimum olarak düşünülebilir
- ama gerektiğinde Cursor veya geliştirici tarafından tutarlı biçimde genişletilebilir

---

## 14. Cursor ve geliştirme aracı davranış kuralı
Geliştirme araçları ve yardımcı AI’ler:
- docs klasörünü kaynak gerçeklik kabul etmelidir
- mimariyi sessizce değiştirmemelidir
- gerekli seed veri ve yardımcı yapıları genişletebilir
- ama çekirdek kararları değiştiremez
- eksik alan görürse yeni varsayım üretmeden önce mevcut docs ile uyumu korumalıdır

---

## 15. Özellikle kaçınılacak hatalar
- UI ile başlamak
- önce bakım ya da enerji modülüne dalmak
- connector omurgasını atlamak
- lisans ve tenant yapısını sona bırakmak
- telemetry detayına erken gömülmek
- bütün resmi destek matrisini ilk sprintte yapmaya çalışmak

---

## 16. Resmi ilk odak
İlk büyük hedef:
**çalışan ticari çekirdek**

Yani:
- ürün omurgası
- connector omurgası
- çekirdek MES omurgası
- temel ekranlar
- temel lisans ve tenant modeli

Bu dört alan kurulmadan proje “başlamış” sayılmaz.

---

## 17. Son karar
xMachineNema geliştirme sırası:
- omurga önce
- veri standardizasyonu sonra
- iş değeri sonra
- genişleme en son
mantığıyla korunacaktır.