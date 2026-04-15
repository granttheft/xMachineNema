# xMachineNema Veritabanı Tasarımı v1

## 1. Dokümanın amacı
Bu doküman xMachineNema’nın ilişkisel veri tasarım yaklaşımını, ana şema ayrımını, temel varlık ailelerini, tablo seviyesinde tasarım kurallarını ve veri saklama prensiplerini tanımlar.

Bu doküman tam SQL dosyası değildir.
Ama SQL ve migration üretilecek seviyede yeterli mimari doğruluğu sağlar.

---

## 2. Ana veri ilkesi
xMachineNema tek tip veri sistemi değildir.

Sistem aynı anda:
- ürün ve tenant verisi
- kullanıcı ve yetki verisi
- lisans ve modül verisi
- üretim işlem verisi
- kalite verisi
- alarm ve olay verisi
- audit verisi
- connector ve mapping verisi
- doküman metadata
taşır.

Bunlara ek olarak telemetry / zaman serisi veri de vardır.
Ancak time-series tasarımı bu dokümanda detaylandırılmaz.
O katman ileride ayrı dokümanla ele alınacaktır.

---

## 3. Ana veri katmanları

### 3.1 Operational Relational DB
Aşağıdaki alanlar burada tutulur:
- tenant yapısı
- organizasyon hiyerarşisi
- kullanıcı / rol / yetki
- lisans / modül aktivasyonu
- connector ve mapping metadata
- iş emri
- reçete
- batch / lot
- kalite
- workflow
- audit
- doküman metadata
- edge metadata

### 3.2 Time-series katmanı
Şimdilik detaylandırılmayacak.
İleride:
- telemetry point
- machine state history
- process values
- counters
- energy readings
gibi alanlar için ayrı tasarlanacak.

### 3.3 Object storage
Fiziksel dosya içeriği için kullanılacaktır:
- dokümanlar
- görseller
- kalite medya kayıtları
- dış sistem import/export dosyaları

### 3.4 Edge local store
Sahadaki IPC veya mini PC üzerinde:
- buffer
- sync queue
- local state
- retry mantığı
için kullanılacaktır.

---

## 4. İlişkisel veri tasarımı için ana kurallar

### 4.1 Her iş tablosunda tenant bağlamı
`tenant_id` kritik iş tablolarında zorunludur.

### 4.2 UUID tabanlı anahtar
Ana varlıklarda uuid kullanılacaktır.

### 4.3 Kod ve insan okunur alan ayrımı
`id` teknik anahtardır.
`code`, `order_no`, `lot_no` gibi alanlar iş kullanıcılarının gördüğü anlamlı alanlardır.

### 4.4 Timestamptz kullanımı
Tüm tarih/saat alanları timezone farklarını taşıyabilecek şekilde tasarlanmalıdır.

### 4.5 Soft delete yerine status tercihi
Çoğu tabloda `is_deleted` yerine `status` yaklaşımı kullanılacaktır.

### 4.6 Dış sistem izi
Gereken tablolarda aşağıdaki alanlar tutulabilir:
- `source_system`
- `source_reference`

Böylece ERP ya da dış sistem bağlantısı izlenebilir kalır.

### 4.7 Audit ayrı tutulur
Audit log, iş tablolarının içine gömülmeyecek; ayrı yapıda saklanacaktır.

---

## 5. Ana şema ayrımı
Operational PostgreSQL veritabanında aşağıdaki şemalar kullanılacaktır:

- `platform`
- `auth`
- `commercial`
- `integration`
- `mes`
- `quality`
- `eventing`
- `workflow`
- `docs`
- `plugin`
- `edge`

Bu ayrım:
- domain sınırlarını görünür kılar
- migration yönetimini kolaylaştırır
- tablo ailelerini daha okunur yapar

---

## 6. Platform şeması
Platform şeması organizasyon ve tenant omurgasını taşır.

### Ana varlıklar
- Tenant
- Enterprise
- Site
- Building
- Line
- Machine
- Station
- BrandingProfile
- TenantSetting

### Kurallar
- tenant her şeyin üst bağlamıdır
- site, line, machine hiyerarşisi kararlı tutulur
- makine ve hat kodları tenant içinde benzersiz olmalıdır
- branding ve genel config tenant seviyesinde yönetilir

### Önemli not
Makine ve istasyon modeli sadece ekran için değil;
aynı zamanda:
- connector bağlama
- alarm bağlamı
- OEE bağlamı
- lot/batch bağlamı
için temel veri omurgasıdır.

---

## 7. Auth şeması
Auth şeması kullanıcı ve yetki yapısını taşır.

### Ana varlıklar
- UserAccount
- Role
- Permission
- RolePermission
- UserRoleAssignment

### Kurallar
- kullanıcı tenant bağlamına sahiptir
- roller tenant bazlı olabilir
- yetkiler modül + aksiyon mantığıyla tanımlanabilir
- scope bazlı atama desteklenmelidir

### Scope örnekleri
- tenant
- site
- line
- machine

Bu yapı sayesinde bir kullanıcı aynı tenant içinde sadece belirli hat veya tesis üzerinde yetkili olabilir.

---

## 8. Commercial şeması
Commercial şeması lisans ve hak setini taşır.

### Ana varlıklar
- License
- Module
- TenantModuleActivation
- LicensedLine
- PackageCatalog
- PackageModule

### Lisans tipleri
- trial
- subscription
- offline

### Tasarım ilkeleri
- lisans sadece “var/yok” değildir
- hangi modüllerin açık olduğu net tutulur
- hangi hatların lisanslı olduğu tutulur
- tenant bazlı hak seti yönetilir

### Not
Gelecekte entitlement daha detaylı gerekirse ayrı tablo ailesi eklenebilir.

---

## 9. Integration şeması
Integration şeması connector metadata ve mapping omurgasını taşır.

### Ana varlıklar
- ConnectorDefinition
- ConnectorInstance
- MappingProfile
- MappingRule
- AssetTagMap
- SyncJob

### Tasarım ilkeleri
- connector tipi ile connector instance ayrıdır
- mapping profilleri versiyonlanabilir olmalıdır
- source field ile canonical field eşlemesi kalıcı tutulmalıdır
- machine/tag ilişkilendirmesi açık olmalıdır
- sync çalışmaları izlenebilir olmalıdır

### Önemli not
Bu şema telemetry’nin kendisini tutmaz.
Telemetry’nin çekirdeğe nasıl bağlanacağını tarif eden metadata’yı tutar.

---

## 10. MES şeması
MES şeması çekirdek operasyon verisini taşır.

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

### Tasarım ilkeleri
- iş emri ERP’den gelebilir ama içeride de yönetilebilir olmalıdır
- reçete versiyonlu tutulmalıdır
- lot / batch ana üretim izi için zorunludur
- üretim bildirimi, hurda ve tüketim ayrık kaydedilmelidir
- iş emri, reçete, lot ve kalite bağlantısı açık olmalıdır

### Kritik ilişki zinciri
ProductionOrder  
→ Recipe  
→ LotBatch  
→ ProductionDeclaration / MaterialConsumption / QualityCheck

---

## 11. Quality şeması
Quality şeması kalite kayıtlarının sahibi olur.

### Ana varlıklar
- QualityCheck
- QualityMeasurement
- Nonconformance
- QualityDisposition

### Tasarım ilkeleri
- kalite ölçümü lot veya iş emri ile ilişkilenebilmelidir
- kalite sonucu ayrı saklanmalıdır
- uygunsuzluk ve disposition ayrı kavramlardır
- kalite onayı workflow ile ilişkilendirilebilir

---

## 12. Eventing şeması
Eventing şeması alarm, duruş ve hesaplanmış sonuçları taşır.

### Ana varlıklar
- AlarmEvent
- DowntimeRecord
- OeeSnapshot
- KpiDefinition
- KpiResult

### Tasarım ilkeleri
- alarm ve event kayıtları transaction kaydı gibi ele alınır
- duruş ayrı kimliğe sahip olmalıdır
- OEE hesap sonucu snapshot mantığında saklanmalıdır
- KPI tanımı ile KPI sonucu ayrılmalıdır

### Not
Ham event stream ile rapor dostu eventing tabloları aynı şey değildir.
Bu şema, işlenmiş ve sistem tarafından kullanılabilir olay kaydını temsil eder.

---

## 13. Workflow şeması
Workflow onay ve süreç akışlarını taşır.

### Ana varlıklar
- WorkflowDefinition
- WorkflowStep
- WorkflowInstance
- WorkflowAction

### Kullanım alanları
- reçete onayı
- kalite onayı
- iş emri kapatma onayı
- bakım kapatma onayı

### Tasarım ilkeleri
- workflow tanımı ile çalışan instance ayrıdır
- referans tipi ve referans id üzerinden generic ilişki kurulabilir
- action geçmişi silinmemelidir

---

## 14. Docs şeması
Docs şeması dosya metadata katmanıdır.

### Ana varlıklar
- Document
- DocumentLink
- MediaFile

### Tasarım ilkeleri
- dosya içeriği doğrudan DB içinde tutulmak zorunda değildir
- storage_url / object reference mantığı tercih edilir
- bir doküman birden fazla varlıkla ilişkilenebilir
- kalite görselleri ve operasyon dokümanları ayrıştırılabilir

---

## 15. Plugin şeması
Plugin şeması genişleme zeminidir.

### Ana varlıklar
- PluginDefinition
- TenantPluginActivation
- ExtensionConfig

### Tasarım ilkeleri
- plugin çekirdek veri modelini rastgele bozmamalıdır
- tenant bazlı aktivasyon gerekir
- config alanı json/jsonb ile esnek tutulabilir

---

## 16. Edge şeması
Edge şeması merkezde edge metadata’yı taşır.

### Ana varlıklar
- EdgeNode
- EdgeConnectorState
- EdgeSyncCheckpoint

### Tasarım ilkeleri
- fiziksel edge cihazı tanımlanabilmeli
- hangi connector’ların o node üzerinde çalıştığı izlenebilmeli
- sync checkpoint merkezi düzeyde izlenebilmelidir

### Not
Edge’in kendi lokal veritabanı bu şemanın dışında tutulur.

---

## 17. Ortak alan standardı
Aşağıdaki alanlar gerektiği yerde ortaklaştırılmalıdır:

- id
- tenant_id
- created_at
- updated_at
- created_by
- updated_by
- status
- source_system
- source_reference

Tüm tablolarda bunların hepsi zorunlu değildir.
Ama ortak modelleme anlayışı bu eksende olmalıdır.

---

## 18. Referans bütünlüğü kuralları

### Sert foreign key kullanılacak alanlar
- tenant → site → line → machine
- user → role assignment
- production order → operation
- recipe → recipe parameter
- quality check → measurement
- workflow instance → workflow action

### Esnek ilişki / generic referans kullanılabilecek alanlar
- document_link
- workflow_instance reference
- audit entry entity reference
- bazı dış sistem referans alanları

Sebep:
Her ilişkide sert FK kullanmak esnekliği düşürür.
Özellikle generic referans alanlarında uygulama düzeyinde doğrulama daha uygundur.

---

## 19. Audit tasarımı
Audit kaydı ayrı tutulur.

### Audit’in amacı
- kim
- neyi
- ne zaman
- hangi modülde
- hangi önceki değerden hangi yeni değere
değiştirdiği izlenebilsin.

### Audit alanları örnek
- user_id
- module_code
- entity_name
- entity_id
- action_type
- old_value_json
- new_value_json
- action_time
- ip_address

Audit silinmez.
Arşivlenebilir ama iş anlamında korunur.

---

## 20. İndeksleme ilkeleri
Detay SQL daha sonra çıkarılacaktır.
Ama aşağıdaki kurallar sabittir:

- her ana tabloda tenant odaklı index düşünülmeli
- tenant + status yaygın sorgu deseni olarak kabul edilmeli
- zaman bazlı tablolarda zaman alanı indekslenmeli
- dış referanslı tablolarda source_reference alanı değerlendirilmelidir
- ağır event / audit tabloları için partition stratejisi düşünülmelidir

---

## 21. Partition yaklaşımı
Aşağıdaki tablo aileleri partition adayıdır:
- alarm/event
- downtime
- audit
- KPI sonuçları
- ileride time-series tabloları

Şimdilik bu dokümanda detay partition SQL’i verilmez.
Ama tasarım buna hazır olmalıdır.

---

## 22. Time-series konusunda bilinçli sınır
Bu doküman time-series katmanını detaylandırmaz.

Sebep:
- konu ayrı tasarım derinliği gerektirir
- telemetry retention / aggregation / raw vs summarized veri ayrımı ayrıca ele alınmalıdır
- V1 çekirdeği kurarken operational DB tasarımını netleştirmek daha önceliklidir

Cursor veya geliştiriciler time-series tarafında sessiz varsayım üretmemelidir.
Gerekirse ilgili doküman beklenecektir.

---

## 23. Demo / seed veri yaklaşımı
Development ortamı için minimal ama anlamlı seed veri düşünülebilir.

Örnek:
- 1 tenant
- 1 enterprise
- 1 site
- 2 line
- birkaç machine
- temel roller
- çekirdek modül aktivasyonları
- birkaç production order
- birkaç recipe
- birkaç lot
- birkaç quality record

Bu veri zorunlu değildir ama local geliştirme için yararlıdır.
Gerektiğinde kontrollü genişletilebilir.

---

## 24. Özellikle kaçınılacak veri hataları
- tenant_id olmayan kritik iş tabloları açmak
- platform hiyerarşisini zayıf modellemek
- job ve telemetry mantığını aynı tabloya zorlamak
- lot/batch ilişkisini yüzeysel bırakmak
- kalite verisini üretimden kopuk modellemek
- connector mapping verisini geçici / uçucu bırakmak

---

## 25. Son karar
xMachineNema veri tasarımı:
- domain bazlı şema ayrımı olan
- tenant-aware
- iş verisi ile telemetry’yi ayıran
- lisans ve connector mantığını birinci sınıf alan olarak ele alan
- ürünleşmeye uygun
bir ilişkisel çekirdek üstüne kurulacaktır.