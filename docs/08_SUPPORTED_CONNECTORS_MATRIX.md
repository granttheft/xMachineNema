# xMachineNema Desteklenen Connector Matrisi v1

## 1. Dokümanın amacı
Bu doküman xMachineNema’nın resmi destek kapsamını tanımlar.

Bu çok önemli bir ayrımdır:
**mimari olarak açık olmak** ile **resmi olarak desteklenmek** aynı şey değildir.

Bir teknolojiye mimari olarak alan bırakılabilir.
Ama bu onun V1’de resmi olarak test edilmiş, desteklenen ve satışta taahhüt edilecek kapsam olduğu anlamına gelmez.

Bu dosyanın amacı:
- satış tarafının yanlış söz vermesini önlemek
- geliştirme ekibinin önceliğini belirlemek
- destek ekibinin kapsamını netleştirmek
- Cursor’un “her şeye alan var, hepsini birden yapayım” yaklaşımını engellemek

---

## 2. Connector desteği nasıl sınıflanır?
Destek seviyesi tek tip değildir.

### 2.1 Mimari destek
Teorik olarak sisteme eklenebilir.

### 2.2 Geliştirme hedefi
Projede planlanmıştır ama henüz resmi çekirdekte bitmemiş olabilir.

### 2.3 Resmi destek
V1 kapsamında test edilmiş, dokümante edilmiş, demo ve canlı kullanım için hedeflenen kapsamdır.

Bu doküman resmi destek seviyesini esas alır.

---

## 3. V1 resmi desteklenen PLC / protokoller
Form kararına göre V1 resmi destek listesi geniş tutulmuştur. :contentReference[oaicite:1]{index=1}

### 3.1 Resmi V1 listesi
- OPC UA
- S7 Native
- Modbus TCP
- Modbus RTU
- MQTT
- EtherNet/IP
- BACnet
- REST
- SQL
- Dosya aktarımı

### 3.2 Yorum
Bu liste resmi kapsam olarak yazılmış olsa da, teknik geliştirme önceliği farklı olabilir.
İlk implementasyon sırası ile resmi destek listesi aynı olmak zorunda değildir.

---

## 4. PLC/protokol öncelik grupları
Resmi liste geniş olsa da uygulama önceliği aşağıdaki gibi ele alınmalıdır.

### 4.1 Öncelik A
- OPC UA
- S7 Native
- Modbus TCP

Sebep:
- üretimde yüksek pratik değer
- ilk canlı kurulum için en anlamlı kombinasyon

### 4.2 Öncelik B
- REST
- SQL
- Dosya aktarımı

Sebep:
- PLC dışı veri kaynakları ve genel entegrasyon ihtiyaçları için kritik

### 4.3 Öncelik C
- MQTT
- Modbus RTU
- EtherNet/IP
- BACnet

Sebep:
- resmi destek kapsamındadır
- ama ilk canlı çekirdekte zorunlu öncelik olmayabilir

---

## 5. ERP tarafı resmi destek
İlk resmi desteklenen ERP / dış sistem yönleri:

- SAP
- Logo
- REST tabanlı generic ERP
- SQL tabanlı generic ERP
- Dosya tabanlı import/export

### 5.1 Öncelik yorumu
İlk iş değeri açısından en güçlü kombinasyonlar:
- SAP
- generic REST ERP

Logo ve diğer generic kanallar mimaride korunur ama canlı önceliği müşteri ihtiyacına göre değişebilir.

---

## 6. ERP connector yetenekleri
Resmi ERP desteği aşağıdaki akış alanlarını kapsayabilir:

- iş emri alma
- üretim sonucu gönderme
- tüketim gönderme
- hurda gönderme
- kalite sonucu gönderme
- reçete / parametre alma
- personel / vardiya bilgisi taşıma

Her ERP connector bütün yetenekleri aynı anda sunmak zorunda değildir.
Capability matrisi connector bazında ayrıca dokümante edilmelidir.

---

## 7. Kurulum tipi resmi destek
İlk resmi desteklenen kurulum tipleri:

- Shared cloud
- Dedicated cloud
- On-prem
- Hybrid

### 7.1 Ana referans model
Hybrid yaklaşım mimari referans model olarak korunur.

### 7.2 Yorum
Resmi desteklenen deployment listesi geniştir.
Ancak V1 saha başlangıcı için tek bir deployment yolu seçmek mümkündür.
Bu, dokümanla çelişmez.

---

## 8. İstemci cihaz resmi destek
Resmi desteklenen istemci tipleri:

- Windows PC
- Mac
- Tablet
- Panel PC

Not:
Telefon daha önce genel erişim modelinde konuşulmuş olsa da bu resmi connector/support matrisi içinde zorunlu ana istemci olarak ele alınmaz.
Ana odak operasyon ekranları ve saha/masaüstü kullanımıdır.

---

## 9. Edge cihaz resmi destek
İlk resmi desteklenen edge cihazları:

- IPC
- Mini PC

### 9.1 Ana tercih
IPC

### 9.2 İkincil tercih
Mini PC

### 9.3 Neden?
Çünkü ürünün edge-first ve saha çalışmasına uygun olması gerekir.
Raspberry Pi benzeri donanımlar mimari olarak ilerde düşünülebilir ama V1 resmi çekirdekte ana donanım olarak konumlandırılmaz.

---

## 10. Write-back desteği ile connector desteği aynı şey değildir
Bir connector’ın resmi olarak desteklenmesi şu anlama gelmez:
- otomatik olarak write-back desteklidir
- tüm yetenekleri açıktır
- her tenantta aynı hakla çalışır

Write-back:
- ayrı lisansa bağlı olabilir
- connector capability’ye bağlıdır
- tenant güvenlik ayarına bağlıdır

Bu ayrım korunmalıdır.

---

## 11. Capability matrisi fikri
İleride her connector için capability matrisi tutulmalıdır.

Örnek capability alanları:
- read telemetry
- read alarms
- read events
- browse tags/model
- write commands
- test connection
- subscription mode
- polling mode
- health reporting

Bu capability matrisi resmi destek dokümanının alt dokümanı olabilir.

---

## 12. Resmi destekte ne anlıyoruz?
Bir connector veya deployment tipi “resmi destekleniyor” deniyorsa şu anlamlar beklenir:

- mimaride yeri var
- test senaryosu tanımlanmış
- dokümanı hazırlanmış
- canlı kullanım hedefinde kabul edilmiş
- hata/incident olduğunda destek ekibi alanı biliyor

---

## 13. Özellikle kaçınılacak yanlışlar
- mimari olarak açık her şeyi resmi destekli sanmak
- bir connector’ın read desteğini write desteği sanmak
- bir ERP connector’da tüm akışların aynı anda hazır olduğunu varsaymak
- edge cihaz desteğini bütün donanım ailesi için genel kabul etmek

---

## 14. V1 için pratik saha önceliği
Resmi liste geniş olsa da saha başlangıcı için önerilen dar öncelik:

### PLC / saha
- OPC UA
- S7 Native
- Modbus TCP

### ERP
- SAP
- generic REST ERP

### Deployment
- Hybrid
- On-prem

### Edge
- IPC

Bu bölüm resmi destek listesiyle çelişmez; sadece canlı başlangıç önceliğini belirtir.

---

## 15. Son karar
xMachineNema connector ve deployment matrisi:
- mimari genişleme kapasitesi ile resmi destek kapsamını ayıran
- resmi taahhüt alanını görünür kılan
- geliştirme önceliğini netleştiren
bir referans doküman olarak korunacaktır.