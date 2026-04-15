# xMachineNema Master Architecture Snapshot v1

## 1. Dokümanın amacı
Bu doküman xMachineNema projesinin en üst seviye mimari ve ürün referans dokümanıdır.

Bu dosyanın amacı:
- ürünün ne olduğunu tek yerde tanımlamak
- teknik ve ticari ana kararları sabitlemek
- diğer tüm docs dosyaları için üst referans olmak
- Cursor dahil tüm geliştirme araçlarının aynı temel gerçeklikten hareket etmesini sağlamak

Bu dokümandaki kararlar, alt dokümanlarda detaylandırılır ancak çelişilmez.
Yeni karar gerekiyorsa mevcut karar sessizce değiştirilmez; ilgili docs dosyası açıkça güncellenir.

---

## 2. Ürün özeti
xMachineNema; üretim sahasındaki farklı PLC, cihaz, hat, makine ve ERP sistemlerinden gelen verileri standartlaştıran; bu verileri gerçek zamanlı görünürlük, üretim takibi, OEE, duruş yönetimi, kalite yönetimi, izlenebilirlik ve operasyon yönetimi için kullanan; edge destekli, modüler, web tabanlı bir üretim operasyon platformudur.

Ürün aşağıdaki alanların kesişiminde konumlanır:
- MES
- IIoT platformu
- Dashboard / raporlama platformu
- Entegrasyon platformu
- MOM yönüne büyüyebilen operasyon katmanı

xMachineNema sadece “ekran gösteren bir dashboard sistemi” değildir.
Sadece “PLC veri toplama aracı” da değildir.
Sadece “ERP entegrasyon katmanı” da değildir.

xMachineNema’nın ana değeri:
1. farklı sistemlerden veriyi toplayabilmesi
2. bu veriyi ortak modele dönüştürebilmesi
3. operasyonel iş akışına bağlayabilmesi
4. modüler ve ticari olarak paketlenebilir olmasıdır

---

## 3. Ürün vizyonu
Uzun vadeli vizyon:
- çoklu müşteri destekleyen
- çoklu tesis destekleyen
- edge-first çalışan
- connector-driven büyüyen
- modül bazlı lisanslanan
- farklı sektörlere uyarlanabilen
- ERP’den bağımsız yaşayabilen
- MOM seviyesine büyüyebilen

bir üretim operasyon platformu oluşturmaktır.

Bu vizyonda xMachineNema;
başlangıçta güçlü bir ticari çekirdek olarak kurulacak,
sonrasında modüller, connector’lar ve sektör özel paketlerle genişleyecektir.

---

## 4. Ürünün temel problemi
xMachineNema’nın çözmek istediği temel problemler şunlardır:

### 4.1 Saha verisinin dağınık ve anlamsız olması
Fabrikalarda veriler farklı PLC’lerde, farklı veri yapılarında, farklı isimlerle tutulur.
Bu veri çoğu zaman:
- standart değildir
- doğrudan iş kararına dönüşmez
- yöneticinin anlayacağı biçimde sunulmaz
- ERP ile bağlanamaz

### 4.2 Üretim görünürlüğünün zayıf olması
Birçok işletmede aşağıdaki konular eksiktir:
- anlık hat görünürlüğü
- makine durumu
- duruş nedenleri
- OEE görünürlüğü
- lot / batch takibi
- kalite ile üretim verisinin aynı yerde birleşmesi

### 4.3 Entegrasyon maliyetinin yüksek olması
Her müşteri ve her tesis için sıfırdan entegrasyon yapmak:
- pahalıdır
- uzun sürer
- sürdürülemez
- çekirdek ürünü bozar

### 4.4 Yazılımın ürünleşememesi
Birçok çözüm müşteri özel proje olarak kalır.
xMachineNema’nın hedefi:
- tek seferlik proje değil
- tekrar satılabilir ürün
- modül bazlı genişleyebilen platform
olmasıdır.

---

## 5. Ürün kimliği ve pazardaki duruş
xMachineNema pazarda şu şekilde konumlanacaktır:

> Üretim sahasındaki farklı PLC ve ERP sistemlerinden gelen verileri standartlaştırıp gerçek zamanlı üretim takibi, izlenebilirlik, OEE, kalite ve operasyon yönetimi sağlayan, edge destekli modüler üretim operasyon platformu.

Bu tanımda özellikle korunacak kelimeler:
- standartlaştırma
- gerçek zamanlı görünürlük
- operasyon yönetimi
- edge destekli çalışma
- modüler yapı
- ERP ve PLC bağımsız connector yaklaşımı

---

## 6. Rakiplerden ayrışma eksenleri
xMachineNema’nın farkı tek bir alanda değil, birkaç eksenin birleşiminde kurulacaktır.

### 6.1 Esneklik
Ürün:
- tek marka PLC’ye bağımlı olmayacak
- tek ERP’ye bağımlı olmayacak
- tek deployment modeline bağımlı olmayacak

### 6.2 Connector-driven mimari
Bağımlılık çekirdekte değil connector katmanında kalacaktır.
Bu sayede:
- çekirdek ürün daha stabil olur
- yeni entegrasyon eklemek daha kolay olur
- müşteri özel entegrasyonlar ürünü bozmaz

### 6.3 Edge + offline çalışma
Ürün sadece cloud mantığında çalışmayacaktır.
Sahadaki edge katmanı sayesinde:
- internet kesintisinde temel çalışma devam edebilir
- veri tamponlanabilir
- write-back ve sync kontrollü şekilde çalışabilir

### 6.4 Modüler ticari model
Bakım, enerji, WIP, planlama, AI gibi alanlar çekirdeğe gömülmeyecek;
modül olarak satılacaktır.

### 6.5 Uygun maliyet ve hızlı kurulum
Amaç, kurumsal ürünlerin her zaman taşıdığı yüksek giriş maliyetini daha kontrollü bir modele çekmektir.

---

## 7. Hedef sektörler
xMachineNema tek sektöre özel kurgulanmayacaktır.
İlk hedef sektörler:
- Metal
- Makine
- Otomotiv yan sanayi
- Gıda
- Plastik
- Kimya
- İlaç
- Enerji
- Genel üretim

Bu kararın anlamı şudur:
çekirdek ürün sektör bağımsız kalmalı,
sektör özel ihtiyaçlar config, modül ve connector seviyesinde çözülmelidir.

---

## 8. Hedef müşteri segmenti
Ürün başlangıçta Türkiye pazarına odaklanacaktır.

Hedef müşteri ölçeği:
- küçük işletme
- orta ölçek fabrika
- büyük fabrika
- çok tesisli kurumsal yapı

Bu yüzden mimari hem küçük kurulumda ayağa kalkabilmeli hem büyük yapıya genişleyebilmelidir.

---

## 9. Ürün erişim modeli
xMachineNema web tabanlı olacaktır.

Desteklenecek istemci tipleri:
- Windows PC
- Mac
- Tablet
- Telefon
- Endüstriyel panel PC

Dış erişim desteklenecektir.

Dış erişim kullanabilecek roller:
- yönetici
- servis ekibi
- satıcı / entegratör
- müşteri IT ekibi

Not:
Mobil saha modülü V1 dışında olsa bile, responsive erişim ve tablet/panel uyumluluğu V1 için önemlidir.

---

## 10. Kurulum modeli
xMachineNema tek kurulum modeline bağlı kalmaz.

Desteklenecek ana modeller:
- Shared cloud
- Dedicated cloud
- On-prem
- Hybrid

### 10.1 Shared cloud
Küçük ve orta ölçek müşteriler için uygundur.

### 10.2 Dedicated cloud
Daha yüksek izolasyon isteyen müşteriler için uygundur.

### 10.3 On-prem
ERP’si içeride çalışan, güvenlik hassas veya internet bağımlılığı istemeyen müşteriler için uygundur.

### 10.4 Hybrid
Sahada edge, merkezde portal/merkezi yönetim yaklaşımı.
Ana ürün tasarımında en güçlü referans model budur.

---

## 11. Edge yaklaşımı
Edge katmanı xMachineNema için opsiyon değil, mimarinin ana parçasıdır.

### 11.1 Edge’in rolü
- PLC ve cihazlardan veri toplamak
- veri dönüşümü yapmak
- lokal tampon oluşturmak
- bağlantı koparsa veri kaybını azaltmak
- merkeze sync yapmak
- bazı projelerde write-back komutlarını kontrollü şekilde uygulamak

### 11.2 Edge cihaz tipi
İlk tercih:
- IPC

İkinci tercih:
- mini PC

Raspberry Pi ve benzeri düşük maliyetli edge cihazlar teorik olarak değerlendirilebilir; ancak V1 resmi çekirdekte ana cihaz tipi olarak konumlandırılmaz.

---

## 12. Tenant ve organizasyon modeli
xMachineNema çoklu müşteri ve çoklu tesis destekler.

### 12.1 Tenant stratejisi
Aşağıdaki üç model desteklenebilir:
- shared tenant
- dedicated database tenant
- dedicated deployment tenant

### 12.2 Resmi organizasyon hiyerarşisi
Firma > Tesis > Bina > Hat > Makine > İstasyon

Bu hiyerarşi aşağıdaki alanlar için ortak omurga olacaktır:
- yetkilendirme
- dashboard scope
- connector bağlama
- lisans
- raporlama
- OEE hesapları
- alarm/duruş bağlamı
- batch/lot görünürlüğü

---

## 13. Ürün teknoloji yönü
İlk ticari çekirdekte önerilen teknoloji omurgası:

- .NET 10
- ASP.NET Core 10
- Blazor Web App
- Aspire
- PostgreSQL
- Docker
- Edge tarafında ayrı runtime / agent yapısı

### 13.1 Uygulama modeli
İlk ticari çekirdek:
- mikroservis olmayacak
- modüler monolith olacak

Sebep:
- geliştirme kontrolü
- daha düşük erken dönem operasyon yükü
- daha kolay debug
- daha hızlı ürünleşme

Bu karar ileride bazı alanların servisleştirilmesine engel değildir.

---

## 14. Mimari yaklaşım
xMachineNema’nın temel mimari karakteri şudur:

- connector-driven
- edge-first
- config-first
- tenant-aware
- audit-friendly
- modüler
- ürünleşebilir
- genişletilebilir

### 14.1 Ana katmanlar
1. Presentation Layer
2. Core Application Layer
3. Integration Layer
4. Edge Layer
5. Data Layer

---

## 15. Veri yaklaşımı
xMachineNema tek tip veri sistemi değildir.
Aşağıdaki veri aileleri birlikte yaşar:

- platform verisi
- ticari/lisans verisi
- organizasyon ve varlık verisi
- MES işlem verisi
- telemetry / zaman serisi verisi
- alarm / event verisi
- audit verisi
- doküman / görsel verisi
- connector / mapping verisi

Ana ilke:
işlem verisi ile zaman serisi veri aynı mantıkta saklanmaz.

---

## 16. Lisans omurgası
Ana ticari model:
- aylık abonelik
- hat bazlı lisans
- modül bazlı lisans
- demo / trial
- offline lisans

### 16.1 Platform lisansı
Ürünün açılması için temel lisans katmanı

### 16.2 Hat lisansı
Operasyonel kullanım lisansı

### 16.3 Modül lisansı
Bakım, enerji, planlama vb. genişleme katmanı

### 16.4 Connector lisansı
Özellikle premium connector’lar için kullanılabilir

### 16.5 Write-back lisansı
PLC’ye yazma ayrı lisans katmanı olarak değerlendirilir

---

## 17. V1 çekirdek kapsamı
V1 çekirdekte sabit olarak bulunan alanlar:

- Dashboard
- Alarm / Event
- OEE
- Duruş yönetimi
- Üretim emri
- Operatör ekranı
- Reçete / parametre
- İzlenebilirlik
- Batch / lot
- Kalite
- Raporlama
- KPI tanımı
- Audit
- Connector yönetimi
- Lisans yönetimi

---

## 18. V1 add-on modüller
V1 içinde çekirdeğe dahil edilmeyecek ama ürün ailesinde yer alacak modüller:

- Bakım
- Enerji
- Andon
- Doküman yönetimi
- WIP / Depo
- Planlama

---

## 19. V1 dışında kalan modüller
Bu modüller ilk ticari çekirdeğe alınmayacaktır:
- AI
- Mobil saha

---

## 20. Write-back politikası
Write-back desteklenebilir ancak bu alan kontrollü olacaktır.

### 20.1 Desteklenebilecek alanlar
- reçete yazma
- setpoint yazma
- iş emri parametresi gönderme

### 20.2 Kurallar
- ayrı lisansla açılır
- yetki kontrolüne tabidir
- kritik komutlarda onay gerekir
- tenant bazlı daha sıkı kurallar tanımlanabilir

### 20.3 Ürün kapsamı dışında kalan konu
Emergency stop yazılım komutu olarak ürün kapsamına alınmaz.

---

## 21. Güvenlik yaklaşımı
İlk sürümde en sade ama güçlü yaklaşım tercih edilir.

Net kararlar:
- kullanıcı adı / şifre giriş
- audit trail zorunlu
- rol bazlı yetki
- scope bazlı yetki
- onay akışları

Dış servis kullanıcıları:
- sadece izleme
- alarm görme
- connector health görme
- log görme

---

## 22. Geliştirme sırası
Resmi geliştirme sırası:

1. Platform
2. Integration
3. MES
4. Modüller

### 22.1 Platform
Tenant, auth, rol, site/hat/makine yapısı, lisans/modül aktivasyonu

### 22.2 Integration
Connector contracts, runtime, mapping, health, sync

### 22.3 MES
İş emri, reçete, batch/lot, kalite, alarm, duruş, OEE, KPI, audit

### 22.4 Modüller
Bakım, enerji, andon, WIP, planlama, vb.

---

## 23. İlk canlı hedef
İlk canlı hedef kontrollü ve yönetilebilir olacaktır.

Önerilen ilk canlı kurulum:
- 1 tenant
- 1 site
- 1–2 hat
- OPC UA + S7 Native
- SAP veya generic REST ERP
- çekirdek dashboard ve MES akışı

---

## 24. Bu dokümanda özellikle korunacak kurallar
- Mimariyi Cursor kendi varsayımlarıyla değiştirmeyecek
- Ürün mikroservise erken zorlanmayacak
- Connector katmanı çekirdek ürünün yerini almayacak
- Node-RED ürün çekirdeği olmayacak
- Çekirdek ürün müşteri özel çözümlerle bozulmayacak
- Time-series katmanı ileride ayrıca detaylandırılacak

---

## 25. Son not
Bu dosya özet değil, referans dokümandır.
Alt dokümanlar bu dosyayı detaylandırır.
Buradaki kararlar değişirse, diğer docs dosyaları da kontrollü güncellenmelidir.