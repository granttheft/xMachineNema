# xMachineNema Modül Sınırları v1

## 1. Dokümanın amacı
Bu doküman xMachineNema içindeki ana domain ve modül sınırlarını tanımlar.

Amaç:
- hangi sorumluluğun hangi modülde olduğunu netleştirmek
- modüllerin birbirine nasıl bağımlı olacağını belirlemek
- çekirdek ürün ile add-on modülleri ayırmak
- Cursor ve geliştiricilerin iş kurallarını yanlış katmana koymasını önlemek
- müşteri özel istekler geldikçe çekirdeğin bozulmasını engellemek

Bu dosya mimari, veri modeli ve geliştirme sırasıyla birlikte okunmalıdır.

---

## 2. Modül yaklaşımı
xMachineNema modüler bir üründür.
Ancak burada “modül” kelimesi iki farklı anlamda kullanılır:

### 2.1 Domain modülü
Kod içindeki sınırdır.
Örnek:
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

### 2.2 Ticari modül
Lisansla açılıp kapanan ürün paketidir.
Örnek:
- Bakım
- Enerji
- Andon
- WIP / Depo
- Planlama
- AI

Her domain modülü ticari modül değildir.
Bazı domain modülleri ürünün çekirdeğidir ve her kurulumda vardır.

---

## 3. Modül tasarım ilkeleri

### 3.1 Tek sorumluluk ilkesi
Her modül kendi iş alanından sorumludur.
Bir modül başka modülün iş mantığını sahiplenmez.

### 3.2 Bağımlılık yönü kontrollü olmalı
Üst katman alt katmanı kullanabilir.
Ama modüller birbirine rastgele bağımlı hale gelmemelidir.

### 3.3 Çekirdek korunmalı
Müşteri özel istekler mümkün olduğunca:
- config
- mapping
- connector
- plugin
seviyesinde çözülmelidir.

### 3.4 Modül kapalıysa davranış da kapalı olmalı
Bir modül lisansla kapalıysa:
- ilgili menü görünmemeli
- ilgili API aksiyonu çalışmamalı
- ilgili background job devreye girmemeli

### 3.5 ERP/PLC bağımlılığı çekirdeğe sızmamalı
Integration modülü dış sistemle konuşur.
MES, Quality, Workflow gibi modüller doğrudan protokol bilgisi taşımaz.

### 3.6 UI modül sınırına saygı göstermeli
Bir ekran birden fazla modülden veri çekebilir.
Ama ekranın “sahibi” olan ana modül net olmalıdır.

---

## 4. Çekirdek domain modülleri

## 4.1 Platform Modülü
### Sorumluluklar
- tenant yönetimi
- enterprise/site/building/line/machine/station hiyerarşisi
- tenant ayarları
- branding / temel görünüm ayarları
- genel sistem konfigürasyonu

### Bu modül neyi yapmaz
- kullanıcı giriş işlemini yapmaz
- lisans doğrulamaz
- iş emri veya kalite akışı çalıştırmaz
- connector mantığı taşımaz

### Ana varlıklar
- Tenant
- Enterprise
- Site
- Building
- Line
- Machine
- Station
- TenantSetting
- BrandingProfile

### Kimler kullanır
- Auth
- Commercial
- Integration
- MES
- Eventing
- Reporting

Platform, sistemin organizasyon omurgasıdır.

---

## 4.2 Auth Modülü
### Sorumluluklar
- kullanıcı hesapları
- rol yönetimi
- yetki tanımları
- kullanıcı-rol atamaları
- scope bazlı yetki
- giriş/oturum temel mantığı

### Bu modül neyi yapmaz
- lisans doğrulamaz
- iş emri onayı vermez
- kalite kararını yönetmez
- connector health takip etmez

### Ana varlıklar
- UserAccount
- Role
- Permission
- RolePermission
- UserRoleAssignment
- AuditEntry

### Not
Audit teknik olarak tüm sistemde kullanılır ama ilk sürümde Auth/Security omurgası ile güçlü bağ taşıdığı için burada veya ayrı ortak katmanda yönetilebilir.

---

## 4.3 Commercial Modülü
### Sorumluluklar
- lisans kayıtları
- trial / subscription / offline lisans
- modül aktivasyonu
- hat lisansı
- paket tanımları
- entitlement mantığı

### Bu modül neyi yapmaz
- kullanıcı doğrulamaz
- connector veri akışı çalıştırmaz
- OEE hesaplamaz
- iş emri akışı yönetmez

### Ana varlıklar
- License
- Module
- TenantModuleActivation
- LicensedLine
- PackageCatalog
- PackageModule

### Kritik kurallar
- modül açık/kapalı kararı burada yönetilir
- write-back lisansı ayrı hak olarak düşünülebilir
- UI görünürlüğü ve API erişimi bu modülden etkilenir

---

## 4.4 Integration Modülü
### Sorumluluklar
- connector tanımları
- connector instance yönetimi
- mapping profilleri
- mapping kuralları
- tag map kayıtları
- sync job takibi
- connector health ve bağlantı durumu
- standart veri modeline dönüşüm altyapısı

### Bu modül neyi yapmaz
- iş emri yaşam döngüsü yönetmez
- kalite onayı vermez
- OEE hesaplamaz
- operatör ekran mantığı taşımaz

### Ana varlıklar
- ConnectorDefinition
- ConnectorInstance
- MappingProfile
- MappingRule
- AssetTagMap
- SyncJob

### Kritik ilke
Integration modülü veri taşır ve dönüştürür.
İş kararı vermez.

---

## 4.5 MES Modülü
### Sorumluluklar
- iş emri yaşam döngüsü
- üretim operasyonları
- reçete yönetimi
- reçete parametreleri
- iş emri-reçete ilişkisi
- batch / lot ana akışı
- üretim bildirimi
- hurda bildirimi
- malzeme tüketimi
- vardiya ve personel atamaları
- operatör ekranı davranışları

### Bu modül neyi yapmaz
- protokol bağlantısı kurmaz
- ham telemetry okumaz
- kullanıcı hesabı yönetmez
- lisans paketi çözmez

### Ana varlıklar
- ProductionOrder
- ProductionOperation
- Recipe
- RecipeParameter
- OrderRecipeAssignment
- LotBatch
- MaterialConsumption
- ProductionDeclaration
- ScrapDeclaration
- InventoryMovement
- Shift
- EmployeeAssignment

### MES modülünün sınırı
MES modülü üretim akışını yönetir.
Ancak enerji, bakım ve AI gibi alanlar çekirdeğe zorla gömülmez.

---

## 4.6 Quality Modülü
### Sorumluluklar
- kalite kontrol kayıtları
- kalite ölçümleri
- uygunsuzluk kayıtları
- disposition kararları
- kalite onayı
- kalite sonucu ile üretim ilişkisi

### Bu modül neyi yapmaz
- OEE hesaplamaz
- alarm/duruş yönetmez
- connector bağlantısı kurmaz

### Ana varlıklar
- QualityCheck
- QualityMeasurement
- Nonconformance
- QualityDisposition

### Not
Kalite modülü çekirdektedir.
Ama “Quality Plus” gibi gelişmiş ticari genişlemeler ileride ayrı add-on olabilir.

---

## 4.7 Eventing Modülü
### Sorumluluklar
- alarm ve event yönetimi
- duruş kayıtları
- OEE snapshot’ları
- KPI tanımları
- KPI sonuçları
- olay akışlarının görünür hale getirilmesi

### Bu modül neyi yapmaz
- ham protokol verisi okumaz
- kullanıcı yönetmez
- lisans kontrol etmez
- iş emri sahibi değildir

### Ana varlıklar
- AlarmEvent
- DowntimeRecord
- OeeSnapshot
- KpiDefinition
- KpiResult

### Kritik not
Eventing modülü, Integration’dan gelen veriyi işleyebilir ama Integration’ın yerini alamaz.

---

## 4.8 Workflow Modülü
### Sorumluluklar
- onay akışı tanımları
- onay adımları
- çalışan workflow instance’ları
- onay / red / iptal kayıtları

### İlk sürümdeki ana kullanım alanları
- reçete onayı
- kalite onayı
- iş emri kapatma onayı
- bakım kapatma onayı

### Bu modül neyi yapmaz
- kullanıcı doğrulamaz
- lisans kontrol etmez
- protokole komut yazmaz

### Ana varlıklar
- WorkflowDefinition
- WorkflowStep
- WorkflowInstance
- WorkflowAction

---

## 4.9 Docs Modülü
### Sorumluluklar
- doküman metadata
- doküman varlık ilişkileri
- görsel/dosya bağları
- entity bazlı dosya ekleri

### Bu modül neyi yapmaz
- object storage fiziksel mantığını tek başına sahiplenmez
- kalite kararını vermez
- bakım ya da planlama iş akışını yönetmez

### Ana varlıklar
- Document
- DocumentLink
- MediaFile

### Not
V1’de temel düzeyde olabilir.
Daha gelişmiş doküman işlevleri add-on mantığına kayabilir.

---

## 4.10 Plugin Modülü
### Sorumluluklar
- plugin tanımları
- plugin aktivasyonları
- extension config alanları
- ileride SDK ile gelen genişlemeler için zemin hazırlamak

### Bu modül neyi yapmaz
- çekirdeğin iş kurallarını yeniden yazmaz
- lisans modülünün yerine geçmez
- connector framework’ü by-pass etmez

### Ana varlıklar
- PluginDefinition
- TenantPluginActivation
- ExtensionConfig

---

## 5. Add-on ticari modüller

Aşağıdaki alanlar ürün ailesinde vardır ancak V1 çekirdeğin ana parçası değildir:

### 5.1 Bakım
- bakım emri
- bakım kapanış akışı
- arıza kaydı
- bakım geçmişi
- planlı bakım

### 5.2 Enerji
- enerji sayaçları
- enerji trendleri
- enerji KPI
- hat/makine bazlı tüketim

### 5.3 Andon
- problem çağrıları
- eskalasyon
- operatör çağrı mantığı

### 5.4 WIP / Depo
- WIP item takibi
- hareket geçmişi
- yarı mamul akışı
- lokasyon görünürlüğü

### 5.5 Planlama
- gelişmiş planlama
- kapasite dengesi
- sıralama önerileri

Bu modüller çekirdeğe zorla gömülmeyecek;
gerektiğinde lisansla açılacaktır.

---

## 6. V1 dışı modüller
Aşağıdaki alanlar ürün yol haritasında vardır ancak V1 dışında tutulur:

### 6.1 AI
- anomali tespiti
- kestirimci bakım
- kalite tahmini
- öneri motorları

### 6.2 Mobil saha
- telefon ağırlıklı saha iş akışı
- mobil servis ekranları
- genişletilmiş saha veri girişi

---

## 7. Modül bağımlılık kuralları

### 7.1 Platform bağımlılığı
Aşağıdaki modüller Platform’a doğal olarak bağımlıdır:
- Auth
- Commercial
- Integration
- MES
- Quality
- Eventing
- Workflow
- Docs
- Plugin

### 7.2 Auth bağımlılığı
Aşağıdaki modüller Auth’a ihtiyaç duyar:
- Commercial
- MES
- Quality
- Eventing
- Workflow
- Docs

### 7.3 Commercial bağımlılığı
Commercial modülü ürün erişim ve modül görünürlüğünü etkiler ama başka modüllerin iş mantığını sahiplenmez.

### 7.4 Integration bağımlılığı
MES, Eventing, Quality gibi modüller entegrasyondan beslenebilir ama Integration modülüne iş kuralı yükleyemez.

### 7.5 Workflow bağımlılığı
Workflow modülü:
- MES
- Quality
- add-on bakım
gibi alanlara hizmet eder.

---

## 8. Modüller arası veri akışı örnekleri

### 8.1 İş emri senaryosu
ERP Connector  
→ Integration  
→ MES.ProductionOrder  
→ Operator ekranı  
→ Eventing / Reporting

### 8.2 Reçete onayı senaryosu
MES.Recipe  
→ Workflow  
→ Auth / yetki kontrolü  
→ onay sonucu MES’e geri dönüş

### 8.3 Alarm akışı
PLC Connector  
→ Integration  
→ Eventing.AlarmEvent  
→ Dashboard / Notification / Operator UI

### 8.4 Kalite akışı
MES iş emri ve lot  
→ QualityCheck  
→ QualityMeasurement  
→ QualityDisposition  
→ gerekirse ERP geri bildirimi

---

## 9. UI sahiplik ilkesi
Her ekranın bir ana sahibi olmalıdır.

Örnek:
- Tenant / lisans ekranı → Platform + Commercial
- İş emri ekranı → MES
- Reçete ekranı → MES + Workflow
- Kalite ekranı → Quality
- Alarm ekranı → Eventing
- Connector ekranı → Integration

Ekran birden fazla modülden veri kullanabilir ama ana sahiplik bulanık olmamalıdır.

---

## 10. Background job sahipliği
Her zaman çalışan işler doğru modülde yaşamalıdır.

Örnek:
- lisans süre kontrolü → Commercial
- connector retry/sync → Integration
- OEE aggregation → Eventing
- workflow timeout → Workflow
- kalite özetleme → Quality
- iş emri senkronu → MES + Integration koordinasyonu

---

## 11. Config ile çözülecek alanlar
Aşağıdaki ihtiyaçlar mümkün olduğunca config ile çözülsün:
- ekran kolon görünürlüğü
- bazı KPI formülleri
- bazı mapping alanları
- tenant bazlı metinler
- branding
- modül açık/kapalı durumu
- bazı workflow adım varyasyonları

---

## 12. Connector ile çözülecek alanlar
Aşağıdakiler çekirdeğe gömülmemeli:
- PLC marka/protokol farklılıkları
- ERP veri taşıma farklılıkları
- dış sistem alan adı eşlemeleri
- veri alma/gönderme kanalı farklılıkları

---

## 13. Plugin ile çözülecek alanlar
Aşağıdakiler plugin adayı olabilir:
- müşteri özel rapor genişletmesi
- özel hesaplama motoru
- özel export formatı
- özel yardımcı UI bileşenleri

Ama plugin çekirdeğin temel iş kuralını bozmamalıdır.

---

## 14. Özellikle yasaklanan anti-patternler

### 14.1 Integration içinde iş emri kararı vermek
Yanlış.
Integration veri taşır, MES karar verir.

### 14.2 UI içinde kritik iş mantığı çalıştırmak
Yanlış.
UI sadece kullanıcı etkileşimi yönetir.

### 14.3 Bakım modülünü V1 çekirdeğe zorla gömmek
Yanlış.
Bakım ayrı add-on mantığında kalmalıdır.

### 14.4 Her müşteri isteğini yeni modül yapmak
Yanlış.
Önce config, sonra connector, sonra plugin, en son yeni modül düşünülmelidir.

### 14.5 Commercial modülünü sadece faturalama sanmak
Yanlış.
Commercial lisans ve hak setinin merkezidir.

---

## 15. V1 için resmi modül kararı
### V1 çekirdek
- Platform
- Auth
- Commercial
- Integration
- MES
- Quality
- Eventing
- Workflow
- Docs (temel düzey)
- Plugin altyapısı (hafif başlangıç)

### V1 add-on
- Bakım
- Enerji
- Andon
- WIP / Depo
- Planlama

### V1 dışı
- AI
- Mobil saha

---

## 16. Son karar
xMachineNema modül mimarisi:
- net domain sınırları olan
- çekirdeği koruyan
- add-on mantığını destekleyen
- connector ve config ile genişleyen
- müşteri özel sapmayı sınırlayan
bir ürün omurgası olarak korunacaktır.