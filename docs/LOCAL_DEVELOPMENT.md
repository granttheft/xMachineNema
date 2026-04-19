# Local development (Web + Api + PostgreSQL)



This document is the source of truth for running **xMachineNema** on a developer machine: ports, configuration, migrations, seed, and health checks.



## Prerequisites



- [.NET SDK 10](https://dotnet.microsoft.com/download) (matching repo `TargetFramework`)

- [PostgreSQL](https://www.postgresql.org/download/) reachable from your machine (sample: `localhost:5432`)



Docker is **not** required for the core loop.



---



## Default ports (sample configuration)



| Service | URL (HTTP) | Notes |

|--------|------------|--------|

| **XMachine.Api** | `http://localhost:5090` | `Properties/launchSettings.json` (`http` profile) |

| **XMachine.Web** | `http://localhost:5197` | Blazor shell; must use **`XMachine:Api:BaseUrl`** pointing at Api |

| **PostgreSQL** | `localhost:5432` | Repo sample DB name: `xmachine_nema` (same as Api/Web `appsettings.Development.json`) |



If you change the Api port, set **`XMachine:Api:BaseUrl`** on Web to the same base URL (no trailing slash required).

## Visual Studio ile çalışma

**Çözüm dosyası:** repo kökünde **`xMachineNema.slnx`** — Visual Studio 2022+ ile açın (filtrelenmiş klasör görünümü: `src/apps`, `src/modules`, …).

### Ön koşul: Docker veya yerel PostgreSQL

- Örnek Development connection string’leri **`xmachine_nema`** veritabanına ve `localhost:5432`’ye göre hizalıdır.
- Postgres ayakta değilse **Api** migration aşamasında düşer; logda migration hatası ve `ConnectionStrings:XMachineOperationalDb` kontrol edin.

### İlk kontrol (Api çalışırken)

| Kontrol | URL / komut |
|--------|-------------|
| Liveness | `GET http://localhost:5090/health/live` |
| Dev bayrakları (yalnız Development) | `GET http://localhost:5090/health/dev-summary` (migrate/seed/connection string var mı) |
| DB erişimi | `GET http://localhost:5090/health/ready` |

### XMachine.Api — tek başına

1. Solution Explorer’da **XMachine.Api** → sağ tık **Set as Startup Project**.
2. Üst açılır listeden launch profile **`http`** (varsayılan URL: `http://localhost:5090`).
3. **F5** / Start. Konsolda migration/seed log akışını izleyin.

**Api açılmazsa:** `ConnectionStrings:XMachineOperationalDb` (host, port, DB adı, kullanıcı, parola); Postgres; ardından `/health/ready` ve migration log mesajı.

### XMachine.Web — tek başına

1. **XMachine.Web** → **Set as Startup Project**.
2. Profile **`http`** önerilir (`http://localhost:5197`) — örnek **`XMachine:Api:BaseUrl`** `http://localhost:5090` ile uyumlu. **`https`** profili farklı şema/port kullanır; Api adresini buna göre güncellemediyseniz API çağrıları şaşabilir.
3. **F5**. Web logunda: çözümlenen Api URL ve `/health/live` ipucu.

**Web / Blazor API hataları:** Api’nin ayakta olduğundan emin olun; `GET {ApiBaseUrl}/health/live` → 200; **`XMachine:Api:BaseUrl`**; shell’deki “API unreachable” bandı; **`XMachine:DevAuth`** (development giriş).

### Multiple Startup Projects (Web + Api)

1. Solution’a sağ tık → **Properties** → **Startup Project** → **Multiple startup projects**.
2. **XMachine.Api** ve **XMachine.Web** için **Start** (AppHost isteğe bağlı ayrı senaryo).
3. Visual Studio her iki süreci de başlatır; **sıra garanti değildir**. Web önce açılırsa üstte sarı uyarı çıkabilir — Api ayağa kalkınca tarayıcıda **yenileyin**.

### XMachine.AppHost — tek komut (Visual Studio veya CLI)

- Startup project olarak **XMachine.AppHost** seçip **F5** verebilirsiniz (konsol: önce Api child, `/health/live` bekler, sonra Web child).
- CLI ile aynı davranış: `dotnet run --project src/apps/XMachine.AppHost/XMachine.AppHost.csproj`

---

## AppHost ile çalışma



Tek süreç: önce Api, **`GET /health/live`** başarılı olunca Web.



Repository kökünden:



```bash

dotnet run --project src/apps/XMachine.AppHost/XMachine.AppHost.csproj

```



AppHost sınırı (bilinçli olarak minimal):



- Api’yi başlatır (`http://localhost:5090`)

- **`GET http://localhost:5090/health/live`** başarılı olana kadar bekler (varsayılan zaman aşımı ~90s)

- Web’i başlatır; Web ortam değişkeni **`XMachine__Api__BaseUrl`** ile Api adresini verir (`http://localhost:5197`)

- **Ctrl+C** ile alt dotnet süreçleri kapatılır (Windows’ta process tree)



**Edge.Agent** AppHost’ta başlatılmaz (isteğe bağlı sonraki adımlar).



---



## İki terminal ile çalışma



1. PostgreSQL çalışıyor ve veritabanı oluşturulmuş olmalı (ör. `CREATE DATABASE xmachine_nema;` — Development şablonundaki connection string ile aynı ad).

2. **Api**, ardından **Web** (Api `GET /health/live` döndükten sonra Web açmak en sorunsuz yoldur).



Terminal 1:



```bash

dotnet run --project src/apps/XMachine.Api/XMachine.Api.csproj

```



Terminal 2:



```bash

dotnet run --project src/apps/XMachine.Web/XMachine.Web.csproj

```



`http` launch profili veya `ASPNETCORE_URLS` ile URL’leri sabitleyebilirsiniz.



---



## Local PostgreSQL ile çalışma



- **Api** ve **Web** için `ConnectionStrings:XMachineOperationalDb` aynı veritabanına işaret etmeli (cookie auth Web üzerinden; aynı DB).

- Örnek connection string şablonu: `Host=localhost;Port=5432;Database=xmachine_nema;Username=...;Password=...`
- **Api ve Web Development:** `ConnectionStrings:XMachineOperationalDb` aynı veritabanına işaret etmeli (cookie auth + aynı seed’li demo DB). Şablonda ikisi de Docker örneğiyle hizalıdır; kendi Postgres’inizde değerleri birlikte güncelleyin.



### XMachine.Api (`appsettings.json` / `appsettings.Development.json`)



| Key | Purpose |

|-----|---------|

| `ConnectionStrings:XMachineOperationalDb` | Npgsql connection string |

| `XMachine:Database:MigrateOnStartup` | `true` ise startup’ta `Database.Migrate()` |

| `XMachine:Seed:Enabled` | Development’ta tenant yoksa `DevSeedHostedService` demo veri |

| `XMachine:DevAuth:SharedPassword` | Web ile uyumlu dokümantasyon aynası |



### XMachine.Web



| Key | Purpose |

|-----|---------|

| **`XMachine:Api:BaseUrl`** | **Tek resmi** Api taban URL’i (boşsa yerel varsayılan `http://localhost:5090`) |

| `ConnectionStrings:XMachineOperationalDb` | Cookie sign-in için (`UserAccount` / roller) |

| `XMachine:DevAuth:Enabled` / `SharedPassword` | Development ortak parola |



Web açılışında log: çözümlenen Api URL’i ve resmi canlılık ucu **`/health/live`**.



---



## Docker PostgreSQL ile çalışma (kısa not)



Bu repoda compose görevi yok. İsterseniz ayrı bir `docker-compose.yml` ile Postgres çalıştırıp `XMachineOperationalDb` içinde **host adını** konteyner hostname’ine (ör. `postgres`) çevirin; migration/seed akışı aynı kalır.



---



## Migration



EF Core migration’larını PostgreSQL’e uygulama:



```bash

dotnet ef database update --project src/building-blocks/XMachine.Persistence/XMachine.Persistence.csproj --startup-project src/apps/XMachine.Api/XMachine.Api.csproj --context XMachineDbContext

```



Yeni migration (model değişikliğinden sonra):



```bash

dotnet ef migrations add YourMigrationName --project src/building-blocks/XMachine.Persistence/XMachine.Persistence.csproj --startup-project src/apps/XMachine.Api/XMachine.Api.csproj --context XMachineDbContext --output-dir Operational/Migrations

```



`XMachine.Api`, `dotnet ef` için `Microsoft.EntityFrameworkCore.Design` içerir.



---



## Seed



- **`XMachine:Seed:Enabled`** (Development şablonunda genelde `true`).

- Yalnızca **Development** ortamında ve **henüz tenant yoksa** çalışır (mevcut DB’lerde idempotent atlama).

- Migration uygulanmış ve geçerli connection string gerekir.



Temiz demo DB: veritabanını drop/recreate → `database update` → Api’yi seed açık başlatın.



---



## Health kontrolü



**Orkestrasyon ve dokümantasyon için resmi uç:** **`GET /health/live`** (process ayakta).



AppHost yalnızca buna bakar. Ek uçlar kalabilir:



| Endpoint | Meaning |

|----------|---------|

| `GET /health` | Özet / linkler |

| **`GET /health/live`** | **Resmi liveness** (AppHost bekleme) |

| `GET /health/ready` | DB `CanConnect` |

| `GET /health/dev-summary` | **Development only**: migrate/seed/bağlantı bayrakları (secret yok) |



Hızlı kontrol:



```bash

curl -s -o NUL -w "%{http_code}" http://localhost:5090/health/live

```



“Web açılıyor ama API çağrıları hata veriyor” senaryosunda çoğunlukla **`/health/ready`** ve PostgreSQL / connection string bakın.



---



## Local auth (kısa)



- Giriş **Web** üzerinde (`/login`). Api, aynı makine anahtarlarıyla iletilen auth cookie’yi kabul eder (`.xmachine-auth-keys`).

- Development kullanıcı/parola: seed + Web `XMachine:DevAuth`.



---



## Troubleshooting



| Symptom | Check |

|--------|--------|

| Api startup’ta çıkıyor | Migration hatası logları; PostgreSQL; connection string |

| Web boş / API hataları | Web log (Api URL); `curl http://localhost:5090/health/live` |

| Seed çalışmıyor | `XMachine:Seed:Enabled`; tenant zaten var (tasarım gereği atlanır) |

| AppHost Api için zaman aşımı | Postgres + migration; Api logları |
| Visual Studio’da `MSB3027` / DLL kopyalanamıyor | Çalışan debug’ı durdurun (**Shift+F5**); hâlâ kilit varsa Task Manager ile ilgili `dotnet` sürecini sonlandırın; sonra rebuild. Neden: Api/Web hâlâ `bin\Debug\net10.0` altından çalışıyorken aynı klasöre kopyalama yapılamaz. |


