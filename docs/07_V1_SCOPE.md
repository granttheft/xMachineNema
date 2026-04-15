# xMachineNema V1 Kapsamı

## 1. Dokümanın amacı
Bu doküman xMachineNema’nın ilk ticari sürümünde hangi alanların kesin yapılacağını, hangilerinin add-on olarak kalacağını, hangilerinin bilinçli biçimde dışarıda bırakılacağını tanımlar.

Bu dokümanın amacı:
- kapsamın kontrolsüz büyümesini önlemek
- çekirdek ile modül ayrımını sabitlemek
- Cursor ve geliştiricilerin “hazır başlamışken bunu da ekleyelim” yaklaşımıyla ürünü dağıtmasını engellemek
- ilk ticari sürümün sahada canlıya alınabilecek kadar kontrollü kalmasını sağlamak

---

## 2. V1’in ana hedefi
V1’in amacı bütün vizyonu bitirmek değildir.

V1’in amacı:
- ürünün ticari çekirdeğini ayağa kaldırmak
- sahada gerçek değer gösterebilmek
- ürünleşebilir bir omurga kanıtlamak
- çekirdek modül + connector + lisans mantığını çalışır hale getirmektir

Bu yüzden V1:
- dar değil
- ama sınırsız da değil
olacaktır.

---

## 3. V1’de kesin olacak çekirdek modüller
Aşağıdaki alanlar V1 çekirdekte sabittir:

- Dashboard
- Alarm / Event
- OEE
- Duruş yönetimi
- Üretim emri
- Operatör ekranı
- Reçete / parametre yönetimi
- İzlenebilirlik
- Batch / lot
- Kalite
- Raporlama
- KPI tanımı
- Audit
- Connector yönetimi
- Lisans yönetimi

Bu liste V1 çekirdeğin resmi sınırıdır.

---

## 4. V1 çekirdeğin anlamı
Bu modüller bir araya geldiğinde şu iş değeri ortaya çıkmalıdır:

- sahadan veri alınabilmeli
- hat ve makine görünürlüğü sağlanabilmeli
- alarm ve duruşlar görülebilmeli
- iş emri akışı yürüyebilmeli
- reçete bağlanabilmeli
- lot/batch görünürlüğü oluşmalı
- kalite ile üretim bağı kurulmalı
- OEE görülebilmeli
- lisans ve modül mantığı çalışmalı
- audit ve onay omurgası hazır olmalı

---

## 5. V1’de zorunlu ekranlar
V1’de aşağıdaki ekranlar zorunludur:

### 5.1 Yönetim ekranları
- Yönetici ana dashboard
- Tenant / lisans yönetimi
- Connector yönetimi

### 5.2 Operasyon ekranları
- Hat detay ekranı
- Makine detay ekranı
- Alarm ekranı

### 5.3 Üretim ekranları
- İş emri ekranı
- Operatör ekranı
- Reçete ekranı

### 5.4 Kalite ekranları
- Kalite ekranı

Bu liste V1’in “ekransız çekirdek” olmamasını garanti eder.

---

## 6. V1 add-on modüller
Aşağıdaki modüller ürün ailesinde yer alır ancak V1 çekirdek içinde zorunlu değildir:

- Bakım
- Enerji
- Andon
- Doküman yönetimi
- WIP / Depo
- Planlama

### 6.1 Anlamı
Bu modüller:
- tasarımda düşünülür
- lisans modelinde yer alır
- veri modelinde zemin bırakılabilir
- ama ilk ticari çekirdeğin ana bitirme kriteri değildir

---

## 7. V1 dışında kalan modüller
Aşağıdaki modüller V1’in bilinçli olarak dışındadır:

- AI
- Mobil saha

### 7.1 Neden?
Çünkü bu alanlar:
- daha fazla veri olgunluğu ister
- daha fazla UX ve saha denemesi ister
- çekirdeğin canlıya çıkışını yavaşlatır

---

## 8. V1’de write-back kararı
Write-back V1 içinde mümkündür ama çekirdeğin doğal ve zorunlu parçası değildir.

### 8.1 Kural
- write-back ayrı lisansla açılır
- çekirdekte varlığı düşünülür
- ama tüm kurulumlarda varsayılan açık değildir

### 8.2 Desteklenebilecek alanlar
- reçete yazma
- setpoint yazma
- iş emri parametresi gönderme

### 8.3 Kurallar
- yetki kontrolü gerekir
- kritik komutlarda onay gerekir
- tenant bazlı daha sıkı kural uygulanabilir

### 8.4 Yasak alan
- emergency stop yazılımsal komut olarak ürün kapsamına alınmaz

---

## 9. V1’de resmi güvenlik seviyesi
V1’de aşağıdaki güvenlik omurgası zorunludur:

- kullanıcı adı / şifre
- rol bazlı yetki
- scope bazlı yetki
- audit trail
- onay akışları
- şifre politikası
- oturum zaman aşımı

Dış servis kullanıcıları yalnızca:
- izleme
- alarm görme
- connector health görme
- log görme
alanlarına sahip olur.

---

## 10. V1’de ilk canlı hedef
İlk canlı kapsam kontrollü tutulacaktır.

Önerilen hedef:
- 1 tenant
- 1 site
- 1–2 hat
- OPC UA + S7 Native
- SAP veya generic REST ERP
- Dashboard + Alarm + OEE + Duruş + İş emri + Reçete + Kalite + Audit + Connector yönetimi

Bu hedef ilk sahaya çıkış için yeterli değeri üretir.

---

## 11. V1’de neyin özellikle yapılmaması gerekir
Aşağıdaki genişlemeler V1’de kontrolsüz şekilde çekirdeğe alınmamalıdır:

- müşteri özel bakım ekranları
- tüm sektörlere özel kural setleri
- AI özellikleri
- mobil saha akışları
- geniş rapor motoru karması
- çok ağır SSO / kurumsal kimlik entegrasyonları
- henüz ihtiyaç doğrulanmamış özel connector’lar

---

## 12. V1 kabul kriteri nasıl düşünülmeli
V1 başarı kriteri şu olmalıdır:

### 12.1 Teknik başarı
- çekirdek mimari ayağa kalkmış olmalı
- tenant/lisans/modül mantığı çalışmalı
- connector framework omurgası çalışmalı
- çekirdek ekranlar açılmalı
- DB migration ve temel veri modeli kararlı olmalı

### 12.2 İş başarısı
- hat görünürlüğü sağlanmalı
- OEE / duruş çalışmalı
- iş emri + reçete + kalite zinciri kurulmalı
- lot/batch görünürlüğü çalışmalı
- müşteri ürünü “canlı sahaya yakın” görmelidir

### 12.3 Ürün başarısı
- ek modül takılabilir olmalı
- yeni connector eklenebilir olmalı
- lisansla özellik aç/kapat mantığı çalışmalı

---

## 13. V1’de demo için minimum akış
V1 demo akışı aşağıdaki zinciri gösterebilmelidir:

1. PLC / kaynak sistemden veri gelişi
2. Hat ve makine görünürlüğü
3. Alarm / duruş görünürlüğü
4. ERP’den iş emri gelişi
5. İş emrine reçete bağlanması
6. Batch / lot oluşumu
7. Kalite kaydı
8. OEE / KPI görünümü
9. Connector health görünürlüğü
10. Lisans / modül etkisinin görünmesi

---

## 14. V1’de opsiyonel ama zorunlu olmayan derinlikler
Aşağıdaki alanlarda minimal başlanabilir:
- doküman yönetimi
- batch genealogy derinliği
- gelişmiş workflow varyasyonları
- çok detaylı dashboard kişiselleştirme
- gelişmiş export seçenekleri

Bu alanlar “yok” olmak zorunda değildir ama çekirdeğin bitiş kriteri bunlara bağlanmamalıdır.

---

## 15. Özellikle kaçınılacak kapsam hataları
- add-on modülleri çekirdeğe yığmak
- her müşteri isteğini V1’e almak
- “ileride lazım olur” diye AI ve mobili öne çekmek
- resmi destek kapsamı ile mimari genişleme kapasitesini karıştırmak
- write-back’i her müşteride açık varsaymak

---

## 16. Son karar
V1;
- sahada gerçek değer gösterecek kadar güçlü
- ürünleşmeyi bozmayacak kadar kontrollü
- modüllerle genişlemeye açık
bir ticari çekirdek olarak korunacaktır.