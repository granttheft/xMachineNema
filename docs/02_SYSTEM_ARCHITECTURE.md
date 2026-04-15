# xMachineNema Sistem Mimarisi v1

## 1. Dokümanın amacı
Bu doküman xMachineNema’nın üst seviye sistem mimarisini, ana katmanlarını, bileşenlerini ve mimari kurallarını tanımlar.

Bu dosya:
- proje yapısını yönlendirmek
- servis sınırlarını belirlemek
- Cursor’un çözüm yapısını doğru kurmasını sağlamak
- sonraki teknik dokümanlar için temel oluşturmak
amacıyla hazırlanır.

---

## 2. Ana mimari yaklaşım
xMachineNema ilk ticari çekirdekte **modüler monolith** olarak geliştirilecektir.

### 2.1 Bu ne demek?
- tek solution içinde çok proje olabilir
- net domain sınırları olabilir
- modüller ayrı tutulabilir
- ama sistem ilk aşamada erken mikroservis yüküne sokulmaz

### 2.2 Neden mikroservis ile başlamıyoruz?
Çünkü bu aşamada öncelik:
- ürün çekirdeğini hızla ayağa kaldırmak
- domainleri doğru ayırmak
- geliştirme karmaşıklığını düşürmek
- debug ve bakım maliyetini kontrol etmektir

İleride bazı alanlar servisleştirilebilir.
Ancak V1 için resmi yaklaşım modüler monolith’tir.

---

## 3. Mimari karakter
xMachineNema aşağıdaki mimari ilkelerle tasarlanır:

- edge-first
- connector-driven
- config-first
- tenant-aware
- audit-friendly
- extensible
- product-first
- domain-separated

---

## 4. Ana katmanlar
Sistem 5 ana katmanda düşünülür:

1. Presentation Layer
2. Core Application Layer
3. Integration Layer
4. Edge Layer
5. Data Layer

---

## 5. Presentation Layer
Presentation Layer kullanıcıların gördüğü yüzeydir.

### 5.1 Ana bileşenler
- Blazor Web App tabanlı portal
- yönetici arayüzü
- operatör ekranları
- kalite ekranları
- alarm ve olay ekranları
- connector yönetim ekranları
- tenant / lisans ekranları

### 5.2 Kurallar
- web tabanlı çalışır
- rol bazlı görünürlük uygular
- tenant bağlamı ile çalışır
- modül aktivasyonuna göre menü ve ekranları şekillenir
- doğrudan PLC ya da ERP iş mantığı taşımaz

### 5.3 Presentation Layer’in görevi
- veri göstermek
- kullanıcı aksiyonu almak
- iş kurallarını API ve application katmanına iletmek

UI katmanı iş kuralı merkezi olmayacaktır.

---

## 6. Core Application Layer
Bu katman sistemin ana iş akışlarını ve uygulama kurallarını yürütür.

### 6.1 Temel domain alanları
- Platform
- Auth
- Commercial
- Integration metadata
- MES
- Quality
- Eventing
- Workflow
- Docs
- Plugin altyapısı

### 6.2 Görevleri
- tenant ve organizasyon yönetimi
- rol ve yetki yönetimi
- lisans / modül aktivasyonu
- iş emri yaşam döngüsü
- reçete yönetimi
- batch / lot yönetimi
- kalite akışları
- alarm / event işleme
- OEE ve KPI hesaplamaları
- workflow / onay yönetimi
- audit üretimi

### 6.3 Kurallar
- iş kuralları burada yaşar
- connector katmanı iş kuralı taşımaz
- UI bu katmanı bypass etmez

---

## 7. Integration Layer
Bu katman xMachine ile dış sistemler arasındaki standart köprüdür.

### 7.1 Kapsam
- PLC connector host
- ERP connector host
- REST / SQL / File connector’lar
- mapping engine
- transformation engine
- sync mekanizması
- health monitoring

### 7.2 Temel görevler
- dış sistemle güvenli bağlantı kurmak
- ham veriyi almak
- gerekli dönüşümleri yapmak
- ortak modele çevirmek
- xMachine çekirdeğine standart formatta iletmek
- gerekiyorsa kontrollü write-back komutu taşımak

### 7.3 Ana kural
Connector’lar çekirdeğe özel veri modeli ile değil,
**ortak connector sözleşmesi** ile konuşur.

### 7.4 Kritik ilke
Bağımlılık şu sırada kalır:

Kaynak sistem  
→ connector runtime  
→ mapping / transformation  
→ canonical model  
→ application layer

Ham veri doğrudan MES tablolarına yazılmaz.

---

## 8. Edge Layer
Edge Layer sahaya yakın çalışan çalışma katmanıdır.

### 8.1 Bileşenler
- edge agent
- local buffer / local store
- sync service
- command handler
- local connector runtime

### 8.2 Görevler
- PLC ve cihazlardan veri toplamak
- event-based ve polling toplama yapmak
- veri tamponlamak
- bağlantı yoksa veriyi kaybetmemek
- bağlantı geri geldiğinde senkron yapmak
- kontrollü write-back komutlarını uygulamak

### 8.3 Kurallar
- edge katmanı merkezi backend’in aynısı değildir
- edge minimal, kararlı ve görev odaklı çalışır
- edge veri kaynağına yakın olur
- edge ürün çekirdeğinin saha uzantısıdır

---

## 9. Data Layer
Veri katmanı tek parça değildir.

### 9.1 Bileşenler
- Operational relational DB
- Time-series katmanı
- Object storage
- Edge local store

### 9.2 Operational DB
Şunları tutar:
- tenant
- kullanıcı
- rol
- lisans
- modül aktivasyonu
- connector metadata
- iş emri
- reçete
- lot / batch
- kalite
- workflow
- audit
- doküman metadata

### 9.3 Time-series katmanı
İleride ayrı detaylandırılacaktır.
Telemetry, state history, counters, energy ve process values burada tutulacaktır.

### 9.4 Object storage
Dosya, görsel ve doküman verileri için kullanılır.

### 9.5 Edge local store
Offline tampon, sync queue ve lokal durum yönetimi için kullanılır.

---

## 10. Domain sınırları
Sistem şu ana domain bloklarına ayrılır:

- Platform
- Auth
- Commercial
- Integration
- MES
- Quality
- Eventing
- Workflow
- Docs
- Plugin

Bu domainler modül sınırlarını ve proje yapısını belirler.

---

## 11. Presentation ile domain ilişkisi
UI ekranları doğrudan tablo mantığıyla kurulmaz.
Her ekran ilgili domain servislerinden beslenir.

Örnek:
- dashboard → eventing + mes + telemetry aggregation
- iş emri ekranı → MES
- kalite ekranı → Quality
- lisans ekranı → Commercial
- connector ekranı → Integration

Bu sayede ekran tasarımı veri yapısına aşırı bağımlı olmaz.

---

## 12. Write-back mimarisi
Write-back sistem içinde özel ve kontrollü akıştır.

### 12.1 Akış
Kullanıcı aksiyonu  
→ yetki kontrolü  
→ workflow / onay  
→ command service  
→ edge command handler  
→ concrete connector  
→ sonuç / ack

### 12.2 Kurallar
- UI doğrudan PLC’ye yazmaz
- connector business rule taşımaz
- write-back ayrı lisansla açılır
- kritik komutlar onay ister
- emergency stop yazılım komutu ürün kapsamına alınmaz

---

## 13. Güvenlik mimarisi
İlk sürüm güvenlik yaklaşımı sade ama kontrollü tutulur.

### 13.1 Zorunlu güvenlik alanları
- kullanıcı adı / şifre
- rol bazlı yetki
- scope bazlı yetki
- audit trail
- onay akışları
- şifre politikası
- oturum zaman aşımı

### 13.2 Dış servis kullanıcısı
- sadece izleme
- alarm görme
- connector health görme
- log görme

---

## 14. Deployment yaklaşımı
xMachineNema aynı mimariyi farklı deployment modelleriyle çalıştırabilir.

### 14.1 Shared cloud
- düşük giriş maliyeti
- küçük / orta ölçek için uygun

### 14.2 Dedicated cloud
- daha güçlü izolasyon
- kurumsal müşteriye uygun

### 14.3 On-prem
- tesis içi kurulum
- ERP yakın entegrasyonu
- güvenlik hassas müşterilere uygun

### 14.4 Hybrid
- edge sahada
- yönetim / portal merkezde
- ana referans model budur

---

## 15. Genişletilebilirlik kuralları
Sistem genişlerken şu ilkelere uyulmalıdır:

- çekirdek domainler bozulmamalı
- müşteri özel ihtiyaçlar mümkünse config ile çözülmeli
- yeni entegrasyon connector olarak gelmeli
- yeni iş alanı modül olarak eklenmeli
- plugin/SDK alanı çekirdekten ayrı düşünülmeli

---

## 16. Mimari olarak özellikle kaçınılacak şeyler
- her müşteri için yeni mimari üretmek
- erken mikroservisleşme
- connector içine iş kuralı gömmek
- UI içine kritik iş kuralı taşımak
- doğrudan tablo bağımlı ekranlar yapmak
- telemetry mantığını transactional veriyle karıştırmak
- Node-RED’i ürün çekirdeği yapmak

---

## 17. V1 sistem hedefi
V1’de amaç bütün vizyonu tamamlamak değil,
ticari çekirdeği ayağa kaldırmaktır.

Bu yüzden sistem mimarisi V1’de şu hedefi karşılamalıdır:
- 1 tenant
- 1 site
- 1–2 hat
- temel connector yönetimi
- iş emri / reçete / kalite / OEE / alarm / batch akışı
- denetlenebilir audit ve lisans yapısı

---

## 18. Son karar
xMachineNema sistem mimarisi:
- web tabanlı
- modüler monolith
- edge-first
- connector-driven
- tenant-aware
- audit-friendly
- ileride servisleşmeye uygun
bir omurga üstüne kurulacaktır.