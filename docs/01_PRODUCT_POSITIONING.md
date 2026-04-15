# xMachineNema Ürün Konumlandırması v1

## 1. Dokümanın amacı
Bu doküman xMachineNema’nın pazarda nasıl konumlandırılacağını, müşteriye nasıl anlatılacağını ve hangi yanlış anlatımlardan kaçınılacağını tanımlar.

Bu dosya teknik ekip için de önemlidir.
Çünkü ürünün konumlandırması:
- hangi modüllerin öncelikli olacağını
- hangi özelliklerin çekirdeğe alınacağını
- hangi ekranların demo değeri taşıdığını
- fiyatlandırmanın hangi eksende kurulacağını
etkiler.

---

## 2. Ürünün kısa tanımı
xMachineNema; üretim sahasındaki farklı PLC, makine ve ERP sistemlerinden gelen verileri standartlaştırıp bunları operasyonel görünürlük, üretim takibi, OEE, kalite, izlenebilirlik ve entegrasyon için kullanan modüler bir üretim operasyon platformudur.

Bu tanımda özellikle vurgulanması gerekenler:
- standartlaştırma
- gerçek zamanlı görünürlük
- üretim operasyonu
- modülerlik
- entegrasyon
- edge desteği

---

## 3. Ürünün sınıfı
xMachineNema tek kutuya sığan bir ürün değildir.
Aşağıdaki alanların kesişiminde durur:

- MES
- IIoT platformu
- Dashboard ve raporlama platformu
- Entegrasyon platformu
- MOM yönüne büyüyebilen operasyon katmanı

### 3.1 Neden sadece MES demiyoruz?
Çünkü ürün:
- veri toplar
- dönüştürür
- entegrasyon yapar
- dashboard üretir
- operasyon akışı çalıştırır

Yani klasik “yalnızca üretim iş emri ekranı” tanımından daha geniştir.

### 3.2 Neden sadece IIoT platformu demiyoruz?
Çünkü ürün:
- sadece veri toplamaz
- iş emri, kalite, reçete, lot ve onay akışını da yönetir

### 3.3 Neden sadece dashboard demiyoruz?
Çünkü ürün:
- işlemsel veri taşır
- üretim operasyonuna etkide bulunur
- audit ve workflow içerir

---

## 4. Ana satış cümlesi
Önerilen ana satış cümlesi:

> xMachineNema, üretim sahasındaki farklı PLC ve ERP sistemlerinden gelen verileri standartlaştırıp gerçek zamanlı üretim takibi, izlenebilirlik, OEE, kalite ve operasyon yönetimi sağlayan, edge destekli modüler üretim operasyon platformudur.

Bu cümle:
- teknik ekip için doğru
- satış dili için yeterince güçlü
- ürünü daraltmadan anlatan
bir ana cümledir.

---

## 5. Müşteriye sağlanan temel değerler

### 5.1 Canlı üretim görünürlüğü
Müşteri:
- hat durumunu
- makine durumunu
- aktif iş emirlerini
- kritik alarmları
- üretim performansını
tek yerde görebilir.

### 5.2 OEE ve duruş görünürlüğü
Müşteri:
- kullanılabilirlik
- performans
- kalite
- plansız duruşlar
- duruş nedenleri
alanlarında anlamlı görünürlük elde eder.

### 5.3 İzlenebilirlik
Müşteri:
- lot / batch takibi
- üretim geçmişi
- kalite ile üretim ilişkisi
- reçete ile üretim sonucu ilişkisi
kurabilir.

### 5.4 ERP entegrasyonu
Müşteri:
- iş emirlerini içeri alabilir
- üretim sonuçlarını geri gönderebilir
- tüketim / hurda / kalite sonucu bağlayabilir

### 5.5 Saha verisinin standartlaşması
Farklı PLC ve protokollerden gelen veriler ortak modele çevrilir.
Bu ürünün en stratejik farklarından biridir.

### 5.6 Modüler büyüme
Müşteri tüm sistemi bir anda almak zorunda kalmaz.
Çekirdekle başlayıp:
- bakım
- enerji
- WIP
- planlama
- diğer modüller
ile büyüyebilir.

---

## 6. Rakiplerden ayrışma stratejisi
xMachineNema tek bir sloganla değil, çoklu eksenle ayrışacaktır.

### 6.1 Esneklik
Çekirdek ürün:
- tek PLC markasına bağlı değildir
- tek ERP’ye bağlı değildir
- tek deployment modeline bağlı değildir

### 6.2 Çok protokollü saha uyumluluğu
Farklı tesislerde farklı cihaz ve protokoller desteklenebilir.
Bu, ürünün tekrar satılabilirliğini artırır.

### 6.3 ERP bağımsız connector mimarisi
ERP entegrasyonu ürünün çekirdeğini bozmaz.
Bağımlılık connector katmanında tutulur.

### 6.4 Edge + offline yaklaşımı
Sadece cloud tabanlı olmayan, saha koşullarına uygun bir yapı sunar.

### 6.5 Modül bazlı ürünleşme
Her müşteri için sıfırdan proje yapmak yerine,
ortak çekirdek + modül yaklaşımı benimsenir.

### 6.6 Uygun maliyet hedefi
Amaç kurumsal ürünlerin her zaman taşıdığı yüksek giriş bariyerini kırmaktır.
Bu “ucuz ürün” demek değildir.
“Daha erişilebilir ürünleşme” demektir.

---

## 7. Hedef sektör mantığı
xMachineNema sektör bağımsız çekirdeğe sahip olacaktır.

İlk hedef sektörler:
- metal
- makine
- otomotiv yan sanayi
- gıda
- plastik
- kimya
- ilaç
- enerji
- genel üretim

### 7.1 Bunun mimariye etkisi
- ürün sektör özel kural yığınına dönüşmemeli
- sektör farklılıkları config, connector ve modül seviyesinde çözülmeli
- çekirdek akışlar mümkün olduğunca ortak tutulmalı

---

## 8. Hedef müşteri tipi
Ürün hem küçük hem büyük müşteriye satılabilir şekilde tasarlanır.

### 8.1 Küçük işletme için değer
- hızlı kurulum
- canlı görünürlük
- temel iş emri + OEE + alarm

### 8.2 Orta ölçek fabrika için değer
- ERP bağlantısı
- lot takibi
- kalite ve reçete akışları
- çok hat yönetimi

### 8.3 Büyük fabrika / çok tesis için değer
- tenant / site modeli
- multi-site görünürlük
- kurumsal deployment
- geniş connector ekosistemi
- modül bazlı ölçeklenme

---

## 9. Yanlış konumlandırmalar
xMachineNema aşağıdaki yanlış anlatımlarla sunulmamalıdır:

### 9.1 “Bu bir dashboard yazılımı”
Yanlış.
Dashboard ürünün sadece görünen katmanıdır.

### 9.2 “Bu bir PLC veri toplama uygulaması”
Yanlış.
Veri toplama ürünün yalnızca giriş kapısıdır.

### 9.3 “Bu bir ERP ara yüzü”
Yanlış.
ERP entegrasyonu vardır ama ürün sadece buna indirgenemez.

### 9.4 “Bu bir bakım yazılımı”
Yanlış.
Bakım modülü olabilir ama ürünün tamamı değildir.

### 9.5 “Bu sadece büyük fabrikalar içindir”
Yanlış.
Mimari küçükten büyüğe ölçeklenebilir olmalıdır.

---

## 10. Demo dili nasıl olmalı
Demo sırasında ürün şu sırayla anlatılmalıdır:

1. Sahadan veri alıyoruz
2. Bu veriyi standartlaştırıyoruz
3. Hat ve makine görünürlüğü sağlıyoruz
4. İş emri ve reçeteyi bağlıyoruz
5. OEE / duruş / kalite / izlenebilirliği gösteriyoruz
6. ERP ile bağlantıyı kuruyoruz
7. Aynı çekirdeği modüllerle büyütebiliyoruz

Bu anlatım, ürünü sadece “ekran” değil “operasyon omurgası” olarak gösterir.

---

## 11. Satışta öne çıkacak ana başlıklar
Satış sunumlarında şu başlıklar korunmalıdır:

- canlı üretim takibi
- OEE ve duruş yönetimi
- ERP entegrasyonu
- izlenebilirlik
- çok protokollü yapı
- edge destekli kurulum
- modüler büyüme
- farklı fabrikalara uyarlanabilirlik

---

## 12. Ticari anlatımda kaçınılacak teknik detaylar
Müşteri seviyesinde ilk görüşmede aşağıdaki teknik detaylar öne çıkarılmamalıdır:
- .NET sürümü
- PostgreSQL detayı
- Aspire
- iç proje ayrımı
- sınıf/katman isimleri

Bunlar iç mimari detaylarıdır.
Müşteri dilinde sonuç anlatılmalıdır.

---

## 13. İç ekip için konumlandırma notu
Teknik ekip her yeni özelliği şu filtreyle değerlendirmelidir:

Bu özellik:
1. çekirdek ürün konumlandırmasını güçlendiriyor mu?
2. ürünün tekrar satılabilirliğini artırıyor mu?
3. sektör bağımsız çekirdeği bozuyor mu?
4. modül veya connector olarak çözülebilir mi?
5. sadece tek müşteriye özel proje detayına mı dönüşüyor?

Bu filtre ürünün dağılmasını engeller.

---

## 14. Son karar
xMachineNema’nın resmi konumlandırması:

- modüler
- edge destekli
- connector-driven
- MES + IIoT + dashboard + entegrasyon odaklı
- üretim operasyon platformu

Bu ifade gelecekteki bütün ürün, satış ve mimari kararlarının üst çerçevesidir.