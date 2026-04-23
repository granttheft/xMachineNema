# xMachineNema — Proje Durum Dokümanı

> Bu dosyayı yeni bir Claude sohbetinde ilk mesaj olarak yapıştır.
> Son güncelleme: 2026-04-24

---

## Proje Özeti

**xMachineNema** — Nema Winkelmann fabrikası için geliştirilen,
ileride ticari ürün olarak satılacak MES/IIoT platformu.

**Geliştirici:** Efe (mekatronik mühendisi, Türkiye)
**GitHub Repo:** github.com/granttheft/xMachineNema (private)
**Workflow:** Claude prompt yazar → Efe Cursor'a yapıştırır → GitHub Desktop ile commit

---

## Tech Stack

| Katman | Teknoloji |
|--------|-----------|
| Backend API | .NET 10, ASP.NET Core Minimal API |
| Frontend | Blazor Server (.NET 10) |
| ORM | EF Core 10 |
| Veritabanı | PostgreSQL (TimescaleDB ileride) |
| UI Framework | Tailwind CSS (CDN) + Bootstrap (geçiş döneminde) |
| Auth | Cookie auth + bcrypt (development: plain hash) |
| Mimari | Modular Monolith |
| Versiyon | GitHub Desktop |

---

## Proje Yapısı

```
xMachineNema.slnx  ← aktif solution (XMachine.slnx eski)
src/
  apps/
    XMachine.Api/          ← API (port 5090)
    XMachine.Web/          ← Blazor Server (port 5197)
  modules/
    XMachine.Module.Auth/
    XMachine.Module.MES/
    XMachine.Module.Quality/
    XMachine.Module.Eventing/
    XMachine.Module.Workflow/
    XMachine.Module.Integration/
    XMachine.Module.Platform/
    XMachine.Module.Commercial/
  shared/
    XMachine.Persistence/
    XMachine.SharedKernel/
    XMachine.Connectors.Abstractions/
    XMachine.Connectors.Runtime/
    XMachine.Connectors.Contracts/
```

---

## Veritabanı

- **Host:** localhost:5432
- **DB:** xmachine_nema
- **Seed kullanıcılar:**
  - `operator1 / ChangeMe!`
  - `supervisor1 / ChangeMe!`
  - `devadmin / ChangeMe!` ← SuperAdmin rolü

---

## Mevcut Modüller ve API Endpoint'leri

### MES
- `GET /api/mes/production-orders`
- `GET /api/mes/recipes`
- `GET /api/mes/lots`
- `GET /api/mes/shifts`
- `GET /api/mes/summary`

### Quality
- `GET /api/quality/checks`
- `GET /api/quality/nonconformances`
- `GET /api/quality/summary`

### Eventing
- `GET /api/eventing/alarms`
- `GET /api/eventing/downtimes`
- `GET /api/eventing/oee`
- `GET /api/eventing/kpis`
- `GET /api/eventing/summary`

### Workflow
- `GET /api/workflow/definitions`
- `GET /api/workflow/instances`
- `GET /api/workflow/summary`

### Integration
- `GET /api/integration/connectors`
- `GET /api/integration/mappings`
- `GET /api/integration/health/summary`

### Auth
- `POST /auth/login`
- `GET /auth/logout`

---

## Mevcut UI Sayfaları

| Route | Sayfa | Durum |
|-------|-------|-------|
| /dashboard | Operations Dashboard | ✅ Figma birebir |
| /mes/orders | Production Orders | ✅ Gerçek API |
| /mes/recipes | Recipes | ✅ Gerçek API |
| /mes/lots | Lots | ✅ Gerçek API |
| /mes/shifts | Shifts | ✅ Gerçek API |
| /quality/checks | Quality Checks | ✅ Gerçek API |
| /quality/nonconformances | Nonconformances | ✅ Gerçek API |
| /eventing/alarms | Alarms | ✅ Gerçek API |
| /eventing/downtimes | Downtimes | ✅ Gerçek API |
| /eventing/oee | OEE Snapshots | ✅ Gerçek API |
| /eventing/kpis | KPIs | ✅ Gerçek API |
| /workflow/definitions | Definitions | ✅ Gerçek API |
| /workflow/instances | Instances | ✅ Gerçek API |
| /integration/connectors | Connectors | ✅ Gerçek API |
| /integration/mappings | Mappings | ✅ Gerçek API |
| /integration/health | Health | ✅ Gerçek API |
| /monitoring/live | Live Monitoring | ✅ Mock data |
| /monitoring/machines | Machine Overview | ✅ Mock data |
| /engineering/dashboard | Engineering Dashboard | ✅ Mock data |
| /admin/tenants | Tenants | ✅ |
| /admin/licenses | Licenses | ✅ |

---

## Tamamlanan Sprint'ler

### Sprint 1 — Güvenlik + Stabilite ✅
- Git secrets temizliği
- JSON enum serialization fix
- ICurrentUser service
- Endpoint auth + tenant filter
- Login bug fix (Headers read-only)
- Cookie forwarding fix (SSR/circuit double-render sorunu)
- `Error="@_error"` Blazor binding syntax fix (kritik bug)
- `OnAfterRenderAsync` pattern (tüm liste sayfaları)

### Sprint 2 — UI Polish ✅
- Tailwind CSS + Inter font kurulumu
- Sidebar yenileme (Figma Layout.tsx → NavMenu.razor)
- Dashboard yenileme (Figma ExecutiveDashboard + FactoryDashboard → Dashboard.razor)
- Tüm liste sayfaları Tailwind tablo tasarımı
- StatusBadge pill tasarımı
- Live Monitoring sayfası (yeni)
- Machine Overview sayfası (yeni)
- MONITORING nav section eklendi

### Sprint 3 — Devam Ediyor 🚧
- Engineering Dashboard ✅ (mock data, Figma birebir)
- Myanmar dili kaldırıldı, sadece İngilizce ✅
- **DURAKLATILDI** — proje stratejisi konuşulacak

---

## Kritik Teknik Kararlar

### Cookie Forwarding
Blazor Server'da SSR + circuit phase double-render sorunu:
- `UserCookieStore` (singleton) cookie'yi saklar
- `CookieCaptureMiddleware` login sonrası cookie'yi yakalar
- `ForwardingAuthCookieHandler` 3 strateji: HttpContext → AuthState → MostRecent
- **Tüm sayfalar `OnAfterRenderAsync(bool firstRender)` kullanır**

### Figma → Blazor Dönüşüm Yöntemi
```
Figma TSX dosyası repoya eklendi:
src/apps/XMachine.Web/figma-reference/src/app/components/

Cursor'a: "Bu TSX'i Blazor'a çevir, aynı Tailwind class'ları kullan"
→ Birebir aynı tasarım çıkıyor ✅
```

### Blazor Kuralları
- `Error="_error"` ❌ → `Error="@_error"` ✅ (literal vs binding)
- `OnInitializedAsync` ❌ → `OnAfterRenderAsync(bool firstRender)` ✅
- `StateHasChanged()` her API çağrısı sonrası çağrılır

---

## Açık Sorular / Konuşulacaklar

### Proje Stratejisi (Sprint 3 duraklatıldı)

1. **Ticari ürün hedefi:** Sistem önce Nema Winkelmann'da mı kullanılacak, yoksa direkt dış satışa mı?

2. **Engineering Dashboard içeriği:** Mock data var, ama gerçek kayıtlar nerede tutulacak?
   - `maintenance_records` tablosu mu?
   - `machine_faults` tablosu mu?
   - `work_orders` tablosu mu?

3. **Hangi modüller öncelikli?**
   - Bakım/mühendislik (Efe'nin uzmanlığı)
   - Üretim takibi
   - Kalite kontrol
   - Rulo sac takibi (Nema'ya özgü)

4. **ERP entegrasyonu (SAP):** Ne zaman gerekli?

5. **PLC bağlantısı:** Siemens S7, Beckhoff, Fatek Modbus — hangisi önce?

6. **Dil seçimi:** İngilizce + Türkçe (ileride)

### Mock → Gerçek API Geçişi
Şu an mock data olan sayfalar:
- Live Monitoring → gerçek PLC verisi gerekiyor
- Machine Overview → gerçek makine listesi gerekiyor
- Engineering Dashboard → gerçek bakım kayıtları gerekiyor
- Dashboard KPI'lar → gerçek üretim verisi gerekiyor

---

## Figma Referans Sayfaları (Henüz Eklenmeyenler)

### Yüksek Öncelik
- ProductionControl.tsx → /production/control
- OperatorKiosk.tsx → /production/kiosk
- SupervisorScreen.tsx → /production/supervisor
- PlanningDashboard.tsx → /planning/dashboard
- JobTracker.tsx → /production/jobs

### Orta Öncelik
- WarehouseDashboard.tsx → /inventory/warehouse
- InventoryModule.tsx → /inventory
- RawMaterialWarehouse.tsx → /inventory/raw-materials
- ReportsScreen.tsx → /reports

### Düşük Öncelik
- HR modülleri
- LogisticControl.tsx
- MasterData sayfaları

---

## Commit Geçmişi (Son Sprint'ler)

```
feat: Sprint 2 complete - Tailwind UI redesign + LiveMonitoring + MachineOverview
feat: convert Figma ExecutiveDashboard + FactoryDashboard to Blazor Dashboard.razor
feat: add EngineeringDashboard page + ENGINEERING nav section (Sprint 3)
chore: remove Myanmar/bilingual text, English-only UI
fix: correct Blazor binding syntax Error="@_error" in all list pages
fix: use OnAfterRenderAsync for API calls to avoid SSR phase failures
fix: 3-strategy cookie forwarding for Blazor Server SSR + circuit phases
fix: use singleton UserCookieStore to bridge HTTP and Blazor circuit scopes
feat: add Tailwind CSS CDN + Inter font + design tokens (Sprint 2.1)
feat: redesign sidebar NavMenu with Tailwind CSS + Figma design (Sprint 2.2)
```

---

## Yeni Sohbette Kullanım

Bu dosyayı yeni Claude sohbetine yapıştır ve şunu de:
> "xMachineNema projesinin durum dokümanı bu. Projeyi anlat ve [konu] hakkında konuşalım."
