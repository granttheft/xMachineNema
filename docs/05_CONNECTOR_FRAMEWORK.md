# xMachineNema Connector Framework v1

## 1. Dokümanın amacı
Bu doküman xMachineNema’daki connector framework yaklaşımını, ana bileşenlerini, sorumluluk sınırlarını, veri akışını ve geliştirme kurallarını tanımlar.

Bu doküman özellikle kritiktir.
Çünkü xMachineNema’nın ana stratejik farkı,
çekirdek ürünü bozmadan farklı PLC, ERP ve dış sistemleri bağlayabilmesidir.

---

## 2. Connector framework neden var?
Fabrikalarda aşağıdaki farklılıklar doğaldır:
- PLC markası farklıdır
- protokol farklıdır
- tag isimleri farklıdır
- veri tipi farklıdır
- ERP yapısı farklıdır
- veri alma/gönderme yöntemi farklıdır

Eğer ürün çekirdeği bu farklılıkları doğrudan üstlenirse:
- çekirdek bozulur
- her müşteri özel proje olur
- tekrar satılabilirlik düşer
- bakım maliyeti artar

Connector framework’ün amacı bu farklılıkları çekirdeğin dışına taşımaktır.

---

## 3. Ana prensip
xMachineNema içinde şu ilke sabittir:

> Dış sistemler farklı konuşabilir, ama xMachine içine giren veri ve komut modeli standart olmalıdır.

Bu yüzden connector framework:
- veri taşır
- veri dönüştürür
- veri eşler
- sağlık durumu raporlar
- kontrollü komut iletir

Ama:
- iş kuralı vermez
- üretim kararı vermez
- kalite kararı vermez
- lisans kararı vermez

---

## 4. Connector framework’ün ana bileşenleri

### 4.1 Connector Definition
Bir connector tipinin ürün içindeki tanımıdır.

Örnek:
- OPC UA Connector
- S7 Native Connector
- Modbus TCP Connector
- SAP Connector
- REST Connector

Connector definition aşağıdaki bilgileri taşıyabilir:
- kod
- isim
- kategori
- yön
- okuma/yazma yetenekleri
- desteklenen özellikler

### 4.2 Connector Instance
Bir müşteride çalışan somut bağlantıdır.

Örnek:
- Hat-1 OPC UA bağlantısı
- SAP iş emri import bağlantısı
- Logo stok export bağlantısı

Connector instance tenant ve gerekirse site bağlamına sahiptir.

### 4.3 Mapping Profile
Ham alanları standart alanlara çevirir.

Örnek:
- `DB100.RUN_STATE` → `machineState`
- `DB101.COUNT_OK` → `goodCount`
- `MATNR` → `materialCode`

### 4.4 Connector Runtime
Connector’ı gerçekten çalıştıran teknik katmandır.
Bağlanır, okur, dönüştürür, yayınlar, hata yönetir.

### 4.5 Sync / Command Pipeline
- veri merkeze gider
- gerekiyorsa komut geri döner
- yazma kontrollü yapılır

---

## 5. Connector kategorileri

## 5.1 PLC / saha connector’ları
- OPC UA
- S7 Native
- Modbus TCP
- Modbus RTU
- MQTT
- EtherNet/IP
- BACnet

## 5.2 ERP connector’ları
- SAP
- Logo
- REST tabanlı generic ERP
- SQL tabanlı generic ERP
- dosya tabanlı import/export

## 5.3 Yardımcı / dış sistem connector’ları
- REST servisleri
- SQL veri kaynakları
- CSV / Excel import
- kalite cihazı
- barkod / vision sistemi
- enerji sayaçları

---

## 6. Veri akışı
Connector veri akışı aşağıdaki şekilde olmalıdır:

Kaynak sistem  
→ Connector Runtime  
→ Mapping / Transformation  
→ Canonical Message  
→ xMachine Core

Bu akışın anlamı:
- çekirdek ürün ham protokol detayı görmez
- çekirdek ürün standardized message ile çalışır
- veri önce anlaşılır hale gelir, sonra iş akışına girer

---

## 7. Canonical model yaklaşımı
Connector framework’ün merkezinde standart veri modeli vardır.

### 7.1 Ham alan
Kaynağın kendi alanı
Örnek:
- `DB100.DBX0.0`
- `ns=3;s=Line1.Run`
- `HoldingRegister40001`

### 7.2 Logical source field
Daha okunabilir mantıksal isim
Örnek:
- RunState
- GoodCounter
- RecipeCode

### 7.3 Canonical field
xMachine’in standart alanı
Örnek:
- machineState
- goodCount
- recipeId

Amaç:
aynı anlam farklı fabrikalarda farklı isimle gelse bile çekirdekte tek alan olarak görünmesidir.

---

## 8. Standart message envelope
Bütün connector’lar veriyi mümkün olduğunca ortak zarf içinde üretmelidir.

Örnek alanlar:
- tenantId
- siteId
- lineId
- machineId
- stationId
- connectorCode
- messageType
- timestamp
- payload
- quality
- sourceReference

### messageType örnekleri
- telemetry
- alarm
- state_change
- production_event
- quality_event
- erp_order_in
- erp_result_out
- command_result

Bu yaklaşım sayesinde çekirdek ürün connector markasını değil, mesaj tipini bilir.

---

## 9. Mapping engine yaklaşımı
Mapping engine, connector framework’ün en kritik parçasıdır.

### 9.1 Görevleri
- source field → canonical field eşleme
- veri tipi dönüşümü
- unit dönüşümü
- enum eşleme
- default değer verme
- validation

### 9.2 Örnek dönüşümler
- `"1"` → `true`
- `"RUN"` → `machineState = Running`
- `0.1 scale` ile analog değer dönüştürme
- sıcaklık birimi dönüştürme

### 9.3 Kural
Mapping engine business rule vermez.
Yalnızca veri standardizasyonu yapar.

---

## 10. Transformation yaklaşımı
Bazı alanlarda sadece eşleme yetmez, dönüşüm gerekir.

### Örnekler
- string to int
- bit to bool
- raw register to engineering unit
- enum mapping
- tarih/saat format dönüştürme

Bu mantık mapping’den ayrıştırılabilir veya mapping’in alt parçası olabilir.
Ama çekirdek iş kurallarına dönüşmemelidir.

---

## 11. Health ve gözlemlenebilirlik
Her connector için sağlık görünürlüğü olmalıdır.

### Asgari health başlıkları
- connection state
- last successful read
- last successful write
- last heartbeat
- error count
- last error
- retry state

### Neden önemli?
Çünkü ürün sadece veriyi almakla kalmamalı;
entegrasyonun sağlıklı çalışıp çalışmadığını da göstermelidir.

---

## 12. Retry ve hata yönetimi
Connector framework geçici hataları tolere etmelidir.

### Gerekli davranışlar
- retry
- backoff
- dead-letter veya failed message yaklaşımı
- loglama
- health durumuna yansıtma

### Kural
Hatalı veri veya geçici bağlantı sorunu yüzünden çekirdek ürün kilitlenmemelidir.

---

## 13. Write-back yaklaşımı
Write-back ürün içinde ayrı ve kontrollü alandır.

### 13.1 Write-back neden hassas?
Çünkü okuma ile yazma aynı risk seviyesinde değildir.
Yazma:
- operasyonu etkiler
- kaliteyi etkiler
- güvenlik riski doğurabilir

### 13.2 Desteklenebilecek alanlar
- reçete yazma
- setpoint yazma
- iş emri parametresi gönderme

### 13.3 Kurallar
- ayrı lisansla açılır
- yetki kontrolünden geçer
- kritik aksiyonlar workflow onayı ister
- edge command handler üzerinden gider
- connector direct UI write yapmaz

### 13.4 Yasak kapsam
Emergency stop yazılım komutu olarak connector framework içinde yer almaz.

---

## 14. PLC connector contract mantığı
Her PLC connector ortak sözleşmeye uymalıdır.

### Temel yetenekler
- connection validation
- telemetry read
- event/alarm read
- browse tag/model
- polling
- subscription
- health report
- command write (opsiyonel)

### Temel operasyonlar
- ValidateConfig
- Connect
- Disconnect
- ReadSnapshot
- StartStreaming
- StopStreaming
- WriteCommand
- BrowseModel
- GetHealthStatus

Somut isimler değişebilir ama contract mantığı korunmalıdır.

---

## 15. ERP connector contract mantığı
ERP connector’lar PLC connector’larından farklı karakter taşır.

### ERP tarafında beklenen ana operasyonlar
- iş emri alma
- reçete alma
- personel / vardiya alma
- üretim sonucu gönderme
- tüketim gönderme
- hurda gönderme
- kalite sonucu gönderme
- bakım olayı gönderme
- bağlantı testi

### Kural
ERP connector veri taşır.
MES’in iş kararlarını vermez.

---

## 16. Node-RED’in ürün içindeki yeri
Node-RED ürün çekirdeği değildir.

### Kabul edilen rolü
- opsiyonel saha adaptasyon aracı
- prototipleme yardımcı katmanı
- özel glue logic ortamı
- bazı müşteri özel edge akışlarında ara katman

### Kabul edilmeyen rolü
- ürünün ana connector motoru
- lisans ve modül omurgası
- ana mapping otoritesi
- çekirdek entegrasyon runtime’ı

---

## 17. Proje yapısına etkisi
Connector framework kod içinde ayrı alan olarak ele alınmalıdır.

Asgari mantıksal ayrım:
- abstractions
- contracts
- runtime
- mapping
- transformations
- health
- sync
- commands
- somut connector projeleri
- edge agent / local store

Bu yapı, çekirdeği koruyarak yeni connector eklenmesini kolaylaştırır.

---

## 18. İlk resmi destek matrisi
İlk resmi destek kapsamı:
- OPC UA
- S7 Native
- Modbus TCP
- Modbus RTU
- MQTT
- EtherNet/IP
- BACnet
- REST
- SQL
- dosya aktarımı

ERP tarafında:
- SAP
- Logo
- REST tabanlı generic ERP
- SQL tabanlı generic ERP
- dosya tabanlı import/export

Not:
Mimari bunlardan fazlasına açık olabilir.
Ama resmi destek matrisi ayrı dokümanla belirlenir.

---

## 19. İlk geliştirme önceliği
Pratik başlangıç için önerilen connector geliştirme sırası:

### Öncelik 1
- OPC UA
- S7 Native
- Modbus TCP
- SAP
- REST
- Mapping
- Sync
- Health

### Öncelik 2
- Logo
- SQL
- File
- MQTT

### Öncelik 3
- EtherNet/IP
- BACnet
- Modbus RTU
- gelişmiş connector SDK

---

## 20. Demo / development seed yaklaşımı
Local development ve demo için aşağıdaki tipte örnek connector metadata üretilebilir:
- opcua-demo
- s7-demo
- modbus-demo
- sap-demo
- rest-demo

Bu metadata başlangıç içindir.
Gerektiğinde tutarlı biçimde genişletilebilir.

---

## 21. Özellikle kaçınılacak hatalar
- connector içine iş kuralı koymak
- ERP connector’dan doğrudan iş emri state yönetmek
- mapping’i hardcoded yapmak
- her müşteri için yeni connector çatısı yazmak
- UI’dan connector bypass ile PLC’ye yazmak
- health ve retry mekanizmasını ihmal etmek

---

## 22. Son karar
xMachineNema connector framework’ü:
- çekirdekten ayrılmış
- standart message ve canonical model kullanan
- mapping ve transformation içeren
- health ve retry destekleyen
- controlled write-back mantığına sahip
- ürünleşebilir entegrasyon omurgası
olarak tasarlanacaktır.