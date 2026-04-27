# xMachineNema — Project Status Document

> Paste this file as the first message in every new Claude conversation.
> Last updated: 2026-04-24

This is the **single source of truth** for conversations.
Canonical architecture reference: `docs/00_MASTER_ARCHITECTURE_SNAPSHOT.md`.
Other `docs/0X_*.md` files provide deep-dive details.

---

## 1. Product Identity

### 1.1 What it is
**xMachineNema** is a **sector-agnostic, commercial MES/IIoT/integration/dashboard platform** —
a unified **production operations platform** that sits at the intersection of:

- MES (Manufacturing Execution System)
- IIoT platform
- Dashboard & reporting platform
- Integration platform
- MOM-direction operations layer

### 1.2 Core value proposition
Stands up shop-floor data from heterogeneous PLCs/devices/ERPs into a **standardized**,
real-time, operationally actionable model. Sells as a **modular, licensable product** —
not as one-off custom projects.

Official positioning sentence:
> xMachineNema, üretim sahasındaki farklı PLC ve ERP sistemlerinden gelen verileri standartlaştırıp
> gerçek zamanlı üretim takibi, izlenebilirlik, OEE, kalite ve operasyon yönetimi sağlayan,
> edge destekli modüler üretim operasyon platformudur.

### 1.3 What it is NOT
- ❌ "A dashboard app" — dashboard is only the visible surface
- ❌ "A PLC data collector" — collection is only the ingress
- ❌ "An ERP interface" — ERP integration is one capability, not the whole
- ❌ "A maintenance tool" — maintenance is one optional module
- ❌ "Only for big factories" — architecture must scale small-to-large
- ❌ Sector-specific (no plastics-only / automotive-only logic in core)

### 1.4 Target sectors (all supported by generic core)
Metal · Machinery · Automotive supply · Food · Plastics · Chemistry · Pharma · Energy · General manufacturing

### 1.5 Target customers
- Small businesses (fast setup, live visibility, basic MES)
- Mid-sized factories (ERP integration, lot traceability, quality, multi-line)
- Large / multi-site enterprises (tenant/site model, enterprise deployment, full module suite)

### 1.6 First reference customer
Nema Winkelmann factory. **Zero Nema-specific logic** in the codebase.

### 1.7 Developer & workflow
- **Developer:** Efe (mechatronics engineer, Turkey)
- **Repo:** github.com/granttheft/xMachineNema (public)
- **Loop:** Claude writes prompt → Efe pastes into Cursor → commit via GitHub Desktop

---

## 2. Commercial Model

### 2.1 Billing principles
- **Primary model:** Monthly subscription
- **Primary axes:** Active line × Active modules × Connectors × Deployment type × Support tier
- **NOT user-seat-based** — user count is not the primary pricing lever

### 2.2 Licensing layers
| Layer | Purpose |
|-------|---------|
| **Platform License** | Required base — tenants, auth, org model, config, audit, admin UI |
| **Line License** | Operational use — per active line (data ingest, dashboards, alarms, OEE, MES flow) |
| **Module License** | Add-ons unlocked per tenant (engineering, planning, inventory, HR, etc.) |
| **Connector License** | Premium connectors (SAP, Logo, advanced SQL/REST, custom, write-capable) |
| **Write-back License** | Separate — enables recipe-write, setpoint-write, PLC/ERP outbound actions |

### 2.3 Package families
| Package | Audience | Contents |
|---------|----------|----------|
| **Foundation** | Small / PoC / first reference | Platform + limited line + core dashboards + basic alarms + admin |
| **Core MES** | First real commercial core | Foundation + full MES flow (WO, recipe, lot, QC, OEE, reports, connector mgmt) |
| **Operations Plus** | Factories needing deeper ops | Core MES + advanced workflow, deeper KPI, andon, extended reporting, multi-site |
| **Enterprise** | Large / regulated customers | Ops Plus + dedicated deployment, enterprise SLA, strong isolation, white-label |

### 2.4 Deployment models (priced differently)
| Model | Target | Notes |
|-------|--------|-------|
| **Shared cloud** | SMB / quick start | Lowest entry cost |
| **Dedicated cloud** | Mid-enterprise | Stronger isolation |
| **On-prem** | Security-sensitive / ERP-on-site | Customer infrastructure |
| **Hybrid** | Edge on-site + central portal | Reference architecture |

### 2.5 Support tiers
- **Standard** — tickets/email, standard update cycle
- **Enterprise** — priority support, enterprise SLA, controlled update plan

### 2.6 License types
- **Trial** — time-limited, demo-scoped, reduced capacity
- **Subscription** — standard commercial, monthly renewal
- **Offline** — license file for air-gapped / on-prem, periodic renewal file

### 2.7 Write-back boundaries
**Allowed:** recipe writes, setpoint writes, work-order parameter sends (licensed, audited, role-gated)
**Forbidden (never in product scope):** Software-initiated emergency stop

---

## 3. Tech Stack

| Layer | Technology |
|-------|------------|
| Backend API | .NET 10, ASP.NET Core Minimal API |
| Frontend | Blazor Server (.NET 10) |
| ORM | EF Core 10 |
| Database (operational) | PostgreSQL |
| Database (time-series) | TimescaleDB (planned, not yet wired) |
| UI Framework | Tailwind CSS (CDN) + Inter font |
| Auth | Cookie auth + bcrypt (dev: plain hash) |
| Orchestration | .NET Aspire (AppHost project) |
| Architecture | Modular Monolith |
| Containers | Docker |
| Edge | Separate runtime/agent (IPC or mini PC primary) |
| Mobile/PWA | Blazor PWA (planned, same codebase) |
| Version control | GitHub Desktop |

**Explicit decisions:**
- Monolith-first, NOT microservices (for dev velocity and productization control)
- Node-RED is NOT the product core
- Transactional data ≠ time-series data (separate storage strategy)

---

## 4. Architecture Principles

The platform's architectural character (from master doc):

- **Modular** — business capabilities live in independently licensable modules
- **Connector-driven** — all external dependencies live in the connector layer, never in the core
- **Edge-first** — edge layer is not optional; it's part of the main architecture
- **Config-first** — sector differences resolved via config/connectors, not hardcoded rules
- **Tenant-aware** — every operational entity is tenant-scoped
- **Audit-friendly** — audit trails mandatory for all mutations
- **Productizable** — no one-off customer logic in core
- **Extensible** — modules + connectors extend without breaking core

### 4.1 Main layers
1. Presentation Layer (Blazor Web + mobile/PWA)
2. Core Application Layer (module business logic)
3. Integration Layer (connectors, mapping, sync)
4. Edge Layer (on-site data collection + buffering)
5. Data Layer (operational + time-series + audit + document)

### 4.2 Organization hierarchy (canonical)
```
Enterprise > Site > Building > Line > Machine > Station
```
This hierarchy underlies: authorization, dashboard scope, connector binding,
licensing, reporting, OEE context, alarm/downtime context, lot/batch visibility.

### 4.3 Tenant strategies (all supported)
- Shared tenant (multi-tenant DB)
- Dedicated database per tenant
- Dedicated deployment per tenant

---

## 5. Solution Structure

```
xMachineNema.slnx          ← active solution (XMachine.slnx is legacy)
src/
  apps/
    XMachine.Api/          ← API (port 5090)
    XMachine.Web/          ← Blazor Server (port 5197)
    XMachine.AppHost/      ← .NET Aspire host
    XMachine.ServiceDefaults/
  building-blocks/
    XMachine.Persistence/  ← EF Core DbContext + migrations (single operational DB)
    XMachine.SharedKernel/ ← base entities, enums, TenantAuditableEntity
    XMachine.Domain/
    XMachine.Application/
    XMachine.Infrastructure/
  modules/
    XMachine.Module.Auth/          ← users, roles, permissions, scopes
    XMachine.Module.Platform/      ← tenant, enterprise, site, line, machine, station
    XMachine.Module.Commercial/    ← license, module activation, packages
    XMachine.Module.MES/           ← orders, recipes, lots, shifts, consumption
    XMachine.Module.Quality/       ← checks, measurements, nonconformance, disposition
    XMachine.Module.Eventing/      ← alarms, downtime, OEE, KPI
    XMachine.Module.Workflow/      ← approval flows
    XMachine.Module.Integration/   ← connector defs/instances, mappings, sync
    XMachine.Module.Engineering/   ← Sprint 3b ✅
    XMachine.Module.Plugin/
    XMachine.Module.Docs/
    ── TO BE ADDED ──
    XMachine.Module.Production/    ← Sprint 4
    XMachine.Module.Planning/      ← Sprint 6
    XMachine.Module.Inventory/     ← Sprint 8
    XMachine.Module.MasterData/    ← Sprint 10
    XMachine.Module.HR/            ← Sprint 11
    XMachine.Module.Reports/       ← Sprint 12
    XMachine.Module.Logistics/     ← Sprint 13
  connectors/
    XMachine.Connectors.Abstractions/
    XMachine.Connectors.Runtime/
    XMachine.Connectors.Contracts/
  edge/                    ← edge agent (separate runtime)
figma-reference/           ← UI source of truth (TSX)
```

---

## 6. Database

- **Host:** localhost:5432
- **DB:** xmachine_nema
- **Connection string key:** `ConnectionStrings:XMachineOperationalDb`
- **Migrate on startup:** `XMachine:Database:MigrateOnStartup`
- **Dev seed toggle:** `XMachine:Seed:Enabled`

### 6.1 Seed users
| Username | Password | Role |
|----------|----------|------|
| `operator1` | `ChangeMe!` | Operator |
| `supervisor1` | `ChangeMe!` | TenantAdmin + ProductionManager |
| `devadmin` | `ChangeMe!` | SuperAdmin |

### 6.2 Current seeded database contents (via `DevSeedHostedService`)
- **Platform:** 1 Tenant · 1 Enterprise · 1 Site · 2 Lines · 4 Machines · 7 Roles · 3 Users
- **Commercial:** 3 Modules · 1 Trial License · 3 Activations · 2 LicensedLines
- **Integration:** 5 ConnectorDefinitions (opcua, s7, modbus_tcp, sap, rest) · 3 Instances · 2 MappingProfiles · 3 MappingRules · 1 AssetTagMap · 1 SyncJob
- **MES:** 2 Recipes · 5 RecipeParameters · 2 ProductionOrders · 4 Operations · 2 OrderRecipeAssignments · 2 LotBatches · 3 MaterialConsumptions · 4 ProductionDeclarations · 1 ScrapDeclaration · 2 InventoryMovements · 2 Shifts · 3 EmployeeAssignments
- **Quality:** 2 QualityChecks · 5 Measurements · 1 Nonconformance · 1 Disposition
- **Eventing:** 3 Alarms · 3 DowntimeRecords · 3 OeeSnapshots · 2 KpiDefinitions · 4 KpiResults
- **Workflow:** 4 Definitions · 8 Steps · 4 Instances · 3 Actions
- **Engineering:** 3 PmSchedules · 3 MaintenanceWorkOrders · 2 MachineFaults



---

## 7. Sprint Convention — Read Phase + Write Phase

**Every sprint follows a two-phase structure:**

Phase 1 — READ
→ Domain entities, EF config, migration
→ GET API endpoints
→ UI pages (list views, dashboards, calendars)
→ Real API wiring for read pages
Phase 2 — WRITE
→ POST/PUT/DELETE API endpoints
→ Create/Edit modals and forms wired to real API
→ Status update flows (approve, close, complete)

Read phase ships first so the UI is visible early.
Write phase completes the sprint before moving to the next sprint.

---

Each module has a license code. Platform is always included.
All other modules require an active `TenantModuleActivation` record.

| # | License Code | Module Name | Backend | UI Read | UI Write | Overall |
|---|-------------|-------------|---------|---------|----------|---------|
| 0 | `platform` | Platform Core | ✅ | ✅ | ⚠️ Partial | ✅ |
| 1 | `mes-core` | MES Core | ✅ | ✅ | 🔴 No create forms | ⚠️ |
| 2 | `quality` | Quality Control | ⚠️ Basic | 🟠 Lists only | 🔴 | 🔴 |
| 3 | `eventing` | Eventing | ✅ | ✅ | 🔴 | ⚠️ |
| 4 | `workflow` | Workflow | ✅ | ✅ | 🔴 | ⚠️ |
| 5 | `integration-core` | Integration | ⚠️ Skeleton | ✅ | 🔴 | ⚠️ |
| 6 | `engineering` | Engineering & Maintenance | ✅ | ✅ | 🚧 Sprint 3b-W | ⚠️ |
| 7 | `production` | Production Control | 🔴 | 🔴 | 🔴 | 🔴 Sprint 4 |
| 8 | `planning` | Planning & Scheduling | 🔴 | 🔴 | 🔴 | 🔴 Sprint 6 |
| 9 | `inventory` | Inventory & Warehouse | 🔴 | 🔴 | 🔴 | 🔴 Sprint 8 |
| 10 | `master-data` | Master Data | 🔴 | 🔴 | 🔴 | 🔴 Sprint 10 |
| 11 | `hr` | Human Resources | 🔴 | 🔴 | 🔴 | 🔴 Sprint 11 |
| 12 | `reports` | Reports & Analytics | 🔴 | 🔴 | 🔴 | 🔴 Sprint 12 |
| 13 | `logistics` | Logistics Control | 🔴 | 🔴 | 🔴 | 🔴 Sprint 13 |

**Premium connectors (separately licensable, Sprint 15-16):**
- `connector-opcua-pro`, `connector-s7-pro`, `connector-modbus-pro`, `connector-sap`, `connector-rest-pro`, `connector-sql-pro`

**Separate license:** `write-back` (Sprint 16+)

---

## 8. Full Figma → Blazor Page Map

All TSX sources: `figma-reference/src/app/components/`

### Platform Core (always licensed)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| Layout.tsx | NavMenu + MainLayout | ✅ Done |
| ExecutiveDashboard.tsx | /dashboard | ✅ Done (merged into Dashboard.razor) |
| FactoryDashboard.tsx | /dashboard | ✅ Done (merged into Dashboard.razor) |
| — | /admin/tenants | ✅ Done |
| — | /admin/licenses | ✅ Done |

### MES Core (`mes-core`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| — | /mes/orders | ✅ Real API |
| — | /mes/recipes | ✅ Real API |
| — | /mes/lots | ✅ Real API |
| — | /mes/shifts | ✅ Real API |

### Quality (`quality`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| QualityControl.tsx | /quality/control | 🔴 Not built |
| DefectHandling.tsx | /quality/defects | 🔴 Not built |
| ScrapManagement.tsx | /quality/scrap | 🔴 Not built |
| RejectManagement.tsx | /quality/rejects | 🔴 Not built |
| WasteManagement.tsx | /quality/waste | 🔴 Not built |
| — | /quality/checks | ✅ Real API (basic list) |
| — | /quality/nonconformances | ✅ Real API (basic list) |

### Eventing (`eventing`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| LiveMonitoring.tsx | /monitoring/live | ⚠️ Mock data |
| MachineOverviewDashboard.tsx | /monitoring/machines | ⚠️ Mock data |
| — | /eventing/alarms | ✅ Real API |
| — | /eventing/downtimes | ✅ Real API |
| — | /eventing/oee | ✅ Real API |
| — | /eventing/kpis | ✅ Real API |

### Engineering & Maintenance (`engineering`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| EngineeringDashboard.tsx | /engineering/dashboard | ✅ Real API |
| Engineering.tsx | /engineering | ✅ Built (mock data · write phase pending) |
| EngineeringModule.tsx | /engineering | ✅ Merged into Engineering.razor |
| MachineCalendarView.tsx | /engineering/calendar | ✅ Built (simplified · mock) |
| MoldChangeRequests.tsx | /engineering/mold-changes | ✅ Built (mock state · write phase pending) |
| AssignmentCalendarModal.tsx | modal component | ⏳ Deferred to Planning sprint |
| EngineeringMobile.tsx | /mobile/engineering | 🔴 Sprint 14 (PWA) |
| EngineeringMobilePWA.tsx | /mobile/engineering | 🔴 Sprint 14 (PWA) |

### Production Control (`production`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| ProductionControl.tsx | /production/control | 🔴 Not built |
| ProductionControlMCR.tsx | /production/mcr | 🔴 Not built |
| ProductionControlFixed.tsx | — (variant, evaluate) | 🔴 Not built |
| ProductionModule.tsx | /production | 🔴 Not built |
| OperatorDashboard.tsx | /production/operator | 🔴 Not built |
| OperatorKiosk.tsx | /production/kiosk | 🔴 Not built |
| SupervisorScreen.tsx | /production/supervisor | 🔴 Not built |
| JobTracker.tsx | /production/jobs | 🔴 Not built |
| SplitJobManagement.tsx | /production/split-jobs | 🔴 Not built |
| LiveSchedule.tsx | /production/live | 🔴 Not built |
| LiveProductionSchedule.tsx | /production/live-schedule | 🔴 Not built |

### Planning (`planning`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| PlanningDashboard.tsx | /planning/dashboard | 🔴 Not built |
| JobPlanningSchedule.tsx | /planning/schedule | 🔴 Not built |
| JobPlanningScheduleEnhanced.tsx | /planning/schedule (enhanced) | 🔴 Not built |
| CreatePlanForm.tsx | /planning/create | 🔴 Not built |
| PlanningCalendar.tsx | /planning/calendar | 🔴 Not built |
| PlanningMachineCalendarView.tsx | /planning/machine-calendar | 🔴 Not built |
| PlanningReports.tsx | /planning/reports | 🔴 Not built |
| JobScheduleContext.tsx | shared context | 🔴 Not built |

### Inventory & Warehouse (`inventory`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| Inventory.tsx | /inventory | 🔴 Not built |
| InventoryModule.tsx | /inventory | 🔴 Not built |
| InventoryModuleNew.tsx | /inventory (evaluate vs Module) | 🔴 Not built |
| InventoryWarehouse.tsx | /inventory/warehouse | 🔴 Not built |
| WarehouseDashboard.tsx | /inventory/warehouse-dashboard | 🔴 Not built |
| WarehouseManagement.tsx | /inventory/warehouse-management | 🔴 Not built |
| RawMaterialWarehouse.tsx | /inventory/raw-materials | 🔴 Not built |
| RawMaterialRegistration.tsx | /inventory/raw-material-registration | 🔴 Not built |
| RawMaterialProductMapping.tsx | /inventory/raw-material-mapping | 🔴 Not built |
| FinishedGoodsReceiving.tsx | /inventory/finished-goods-receiving | 🔴 Not built |
| FinishedGoodsTransfer.tsx | /inventory/finished-goods-transfer | 🔴 Not built |
| FinishedGoodsTransferNew.tsx | (variant, evaluate) | 🔴 Not built |
| GlueFillOperator.tsx | /inventory/operations/glue-fill | 🔴 Not built |
| CutGlueResidueManagement.tsx | /inventory/operations/cut-glue-residue | 🔴 Not built |
| BorrowModule.tsx | /inventory/borrow | 🔴 Not built (definition TBD) |
| MovementConfirmationModal.tsx | modal component | 🔴 Not built |

### Master Data (`master-data`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| MasterData.tsx | /master-data | 🔴 Not built |
| MasterDataModule.tsx | /master-data (module host) | 🔴 Not built |
| ProductMaster.tsx | /master-data/products | 🔴 Not built |
| ProductMasterEnhanced.tsx | /master-data/products (enhanced) | 🔴 Not built |
| ProductTab.tsx | tab component | 🔴 Not built |
| ColorTab.tsx | /master-data/colors | 🔴 Not built |
| ProductColorMapping.tsx | /master-data/product-colors | 🔴 Not built |
| ProductColorMappingTab.tsx | tab component | 🔴 Not built |

### Human Resources (`hr`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| HR.tsx | /hr | 🔴 Not built |
| HRModule.tsx | /hr (module host) | 🔴 Not built |
| hr/ subfolder | /hr/... | 🔴 Not built |

### Logistics (`logistics`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| LogisticControl.tsx | /logistics/control | 🔴 Not built |

### Reports & Analytics (`reports`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| ReportsScreen.tsx | /reports | 🔴 Not built |
| AdvancedReporting.tsx | /reports/advanced | 🔴 Not built |
| reports/ subfolder | /reports/... | 🔴 Not built |

### Integration (`integration-core` + premium connectors)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| — | /integration/connectors | ✅ Real API |
| — | /integration/mappings | ✅ Real API |
| — | /integration/health | ✅ Real API |

### Workflow (`workflow`)
| Figma TSX | Blazor Route | Status |
|-----------|-------------|--------|
| — | /workflow/definitions | ✅ Real API |
| — | /workflow/instances | ✅ Real API |

---

## 9. Sprint Roadmap

### ✅ Sprint 1 — Security & Stability
- Git secrets cleanup, JSON enum serialization fix, ICurrentUser service
- Endpoint auth + tenant filter
- Cookie forwarding (SSR / circuit phase double-render)
- `Error="@_error"` Blazor binding fix
- `OnAfterRenderAsync` pattern established across list pages

### ✅ Sprint 2 — UI Foundation
- Tailwind CSS + Inter font via CDN
- Sidebar redesign (Layout.tsx → NavMenu.razor)
- Dashboard (ExecutiveDashboard + FactoryDashboard → Dashboard.razor)
- All list pages Tailwind tables + StatusBadge pills
- LiveMonitoring + MachineOverview pages (mock data)
- MONITORING nav section added

### ✅ Sprint 3a — Engineering Dashboard UI (Mock)
- EngineeringDashboard.razor (Figma match, mock data, English-only)

### ✅ Sprint 3b — Engineering Module — Read Phase
**Backend:**
- `XMachine.Module.Engineering` — MaintenanceWorkOrder, MachineFault, PreventiveMaintenanceSchedule
- `Machine.OperationalStatus` enum (Running/Idle/Down/PmDue/WaitingParts) added to Platform
- EF config, `engineering` schema, migration: `AddEngineeringModule`
- API: GET machine-status, work-orders, faults, pm-schedules, summary + POST work-orders
- Engineering seed data added to DevSeedHostedService

**UI pages built:**
- `/engineering/dashboard` → ✅ Real API (EngineeringDashboard.razor)
- `/engineering` → ✅ Tabbed: Breakdowns · PM · Spare Parts · Process Setup · Reports (mock)
- `/engineering/calendar` → ✅ Maintenance calendar, month/day view (mock)
- `/engineering/mold-changes` → ✅ MCR lifecycle with 3 modals (mock state)

### 🚧 Sprint 3b-W — Engineering Module — Write Phase (CURRENT)
**New API endpoints:**
- `POST /api/engineering/pm-schedules` — create PM schedule
- `PUT /api/engineering/work-orders/{id}/status` — update WO status (open→in-progress→done→cancelled)
- `PUT /api/engineering/machine-status/{id}` — manually override machine operational status
- `POST /api/engineering/faults` — report machine fault

**UI wiring:**
- Engineering.razor → "Schedule PM" button → real `POST /api/engineering/pm-schedules`
- Engineering.razor → "Report New Breakdown" modal → real `POST /api/engineering/work-orders`
- Engineering.razor → Breakdowns tab status buttons → real `PUT` endpoint
- MoldChangeRequests.razor → approve/reject/start/complete → real API

### ✅ Sprint 3b — Engineering/Maintenance Module (TAMAMLANDI)
- XMachine.Module.Engineering oluşturuldu (domain entities, EF config, migration)
- Machine.OperationalStatus eklendi (Platform modülü)
- API endpoints: machine-status, work-orders (GET/POST), faults, pm-schedules, summary
- DevSeedHostedService engineering seed data eklendi
- EngineeringDashboard.razor → mock'tan gerçek API'ye geçirildi
- EngineeringApiDtos.cs + XMachineApiClient güncellemesi

### 📋 Sprint 4 — Production Control — Read Phase
- `XMachine.Module.Production` backend, GET endpoints
- ProductionControl.razor, ProductionControlMCR.razor
- OperatorKiosk.razor (touch-optimized)
- SupervisorScreen.razor, JobTracker.razor, SplitJobManagement.razor
- LiveSchedule.razor, LiveProductionSchedule.razor

### 📋 Sprint 4-W — Production Control — Write Phase
- Start/stop/pause job endpoints
- Operator declaration forms (production qty, scrap qty)
- Supervisor approval flows
- Real-time status updates from operator kiosk

### 📋 Sprint 5 — Live Monitoring (Real Data)
- LiveMonitoring.razor → real DowntimeRecord + OEE
- MachineOverview.razor → real Machine + OperationalStatus
- OperatorDashboard.razor
- SignalR hub for real-time dashboard tiles

### 📋 Sprint 5-W — Live Monitoring Write
- Downtime entry form (operator reports stop reason)
- Alarm acknowledgement endpoint + UI button
- OEE manual correction form

### 📋 Sprint 6 — Planning — Read Phase
- `XMachine.Module.Planning` backend, GET endpoints
- PlanningDashboard.razor
- JobPlanningScheduleEnhanced.razor (full drag-drop Gantt from MachineCalendarView.tsx)
- PlanningCalendar.razor, PlanningMachineCalendarView.razor

### 📋 Sprint 6-W — Planning Write
- CreatePlanForm.razor → POST new production plan
- Drag-drop reschedule → PUT plan schedule
- Plan approval flow (draft → final → approved)
- Cross-machine transfer API


### 📋 Sprint 7 — Quality Control — Read Phase
- QualityControl.razor (full Figma)
- DefectHandling.razor, ScrapManagement.razor
- RejectManagement.razor, WasteManagement.razor

### 📋 Sprint 7-W — Quality Write
- New quality check form → POST /api/quality/checks
- Measurement recording form
- Nonconformance creation + disposition form (rework/scrap/accept)

### 📋 Sprint 8–9 — Inventory & Warehouse — Read Phase
- `XMachine.Module.Inventory` backend
- InventoryModule.razor, WarehouseManagement.razor
- RawMaterialWarehouse + Registration + ProductMapping
- FinishedGoodsReceiving + Transfer flows
- BorrowModule.razor (definition clarified during sprint)
- GlueFillOperator.razor, CutGlueResidueManagement.razor

### 📋 Sprint 8-W — Inventory Write
- Stock movement entry forms (issue, receipt, transfer)
- Raw material registration form
- Warehouse location management
- Finished goods transfer confirmation modal

### 📋 Sprint 10 — Master Data — Read + Write (combined)
- `XMachine.Module.MasterData` backend
- ProductMasterEnhanced.razor (CRUD)
- ColorTab, ProductColorMapping, RawMaterialProductMapping (CRUD)
- Machine master CRUD (add machines, assign lines)

### 📋 Sprint 11 — HR — Read + Write (combined)
- `XMachine.Module.HR` backend (employee, attendance, leave, training)
- HR.razor + HRModule.razor + all hr/ sub-pages
- Employee CRUD, attendance recording, leave requests

### 📋 Sprint 12 — Reports & Advanced Analytics
- `XMachine.Module.Reports` backend
- ReportsScreen.razor, AdvancedReporting.razor
- reports/ sub-pages, export (PDF/XLSX)

### 📋 Sprint 13 — Logistics
- `XMachine.Module.Logistics` backend
- LogisticControl.razor

### 📋 Sprint 14 — Mobile / PWA
- Blazor PWA configuration (manifest, service worker, offline shell)
- EngineeringMobile.razor, EngineeringMobilePWA.razor
- Mobile-responsive OperatorKiosk variant
- Push notifications strategy

### 📋 Sprint 15 — Real PLC Integration
**Goal:** Live machine data flows into the platform.
- OPC-UA connector (real, replace placeholder)
- Siemens S7 connector (native)
- Modbus TCP connector (read + write)
- Beckhoff ADS connector (optional)
- Fatek connector (optional)
- Live tag subscription engine → writes to DowntimeRecord + AlarmEvent
- Tag browser UI in /integration/connectors
- Real-time signal mapping configuration
- Edge agent coordination

### 📋 Sprint 16 — ERP Integration
**Goal:** Bidirectional data exchange with customer's ERP.
- SAP connector (real, via BAPI/RFC or S/4 REST)
  - **Inbound:** Pull ProductionOrders, BOM, material master
  - **Outbound:** Push production confirmations, goods movements (consumption, FG receipt)
- Generic ERP REST connector (non-SAP customers)
  - Configurable field mapping via MappingProfile
  - Scheduler: periodic SyncJob
- Logo ERP connector (Türkiye pazarı)
- ERP sync status dashboard in /integration/health
- Conflict resolution strategy (ERP wins vs. xMachine wins)
- Error log + retry queue for failed sync jobs
- **Write-back license** enforcement layer activated here

### 📋 Sprint 17 — Commercial & Licensing UI
- Customer self-service tenant onboarding
- License management UI (customer-facing)
- Module marketplace view (what's activated, what can be added)
- Usage & seat dashboards
- Offline license file generation/validation
- Branding / white-label self-service

### 📋 Sprint 18+ — Optional AI Module (Not V1 Core)
- Anomaly detection
- Predictive maintenance
- Quality prediction
- Recommendation engines

---

## 10. API Endpoints (Current)

### Auth
- `POST /auth/login` · `GET /auth/logout`

### MES (`mes-core`)
- `GET /api/mes/production-orders` · `GET /api/mes/recipes`
- `GET /api/mes/lots` · `GET /api/mes/shifts` · `GET /api/mes/summary`

### Quality (`quality`)
- `GET /api/quality/checks` · `GET /api/quality/nonconformances` · `GET /api/quality/summary`

### Eventing (`eventing`)
- `GET /api/eventing/alarms` · `GET /api/eventing/downtimes`
- `GET /api/eventing/oee` · `GET /api/eventing/kpis` · `GET /api/eventing/summary`

### Engineering (`engineering`) ← NEW
- `GET /api/engineering/machine-status`
- `GET /api/engineering/work-orders` · `POST /api/engineering/work-orders`
- `GET /api/engineering/faults`
- `GET /api/engineering/pm-schedules`
- `GET /api/engineering/summary`

### Workflow (`workflow`)
- `GET /api/workflow/definitions` · `GET /api/workflow/instances` · `GET /api/workflow/summary`

### Integration (`integration-core`)
- `GET /api/integration/connectors` · `GET /api/integration/mappings`
- `GET /api/integration/health/summary`

### Platform / Commercial
- `GET /api/platform/tenants` · `GET /api/platform/licenses`

### Health
- `GET /health` · `GET /health/live` · `GET /health/ready` · `GET /health/dev-summary`

---

## 11. Critical Technical Patterns

### 11.1 Cookie Forwarding (Blazor Server SSR + Circuit)
Blazor Server renders in two phases: initial SSR, then circuit handoff.
Cookies set during login don't persist across this boundary without custom handling.

- `UserCookieStore` (singleton) caches cookies after login
- `CookieCaptureMiddleware` captures cookies on login response
- `ForwardingAuthCookieHandler` tries 3 strategies: HttpContext → AuthState → MostRecent
- **All pages use `OnAfterRenderAsync(bool firstRender)` for API calls — NEVER `OnInitializedAsync`**
- `StateHasChanged()` called after every API response

### 11.2 Figma → Blazor Conversion
```
1. Open TSX from figma-reference/src/app/components/
2. Tell Cursor: "Convert this TSX to Blazor Server razor, keep all Tailwind classes identical"
3. Result matches Figma pixel-for-pixel — manual drift is discouraged
```

### 11.3 Blazor Binding Rules (Hard Won)
- `Error="_error"` ❌ → `Error="@_error"` ✅ (literal vs. binding)
- `OnInitializedAsync` ❌ → `OnAfterRenderAsync(bool firstRender)` ✅
- `StateHasChanged()` after every async API call

### 11.4 Module Licensing Pattern
```
Backend (middleware): TenantModuleActivation gate before endpoint executes
  → 402 Payment Required if module not licensed

Frontend: conditionally render nav items + pages
  → unlicensed module routes redirect to /upgrade with upsell
```

### 11.5 Entity Base Class
All tenant-owned entities derive from `TenantAuditableEntity`, which provides:
- `Id` (Guid), `TenantId` (Guid)
- `Status` (EntityStatus: Active/Inactive)
- `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`

### 11.6 Write Operations Pattern ← NEW
- **Operational data** → PostgreSQL (current)
- **Time-series data** → TimescaleDB (planned, not yet wired)
- **Audit data** → separate tables/schema (planned)
- **Document/image data** → blob storage (planned)
POST endpoint → validate TenantId → create entity → SaveChangesAsync → return 201 Created
PUT endpoint  → validate TenantId + entity ownership → update → SaveChangesAsync → return 200 OK
UI form       → HttpClient.PostAsJsonAsync / PutAsJsonAsync → handle response → StateHasChanged
---

## 12. Known Tensions & Open Questions

### 12.1 V1 scope tension
`docs/07_V1_SCOPE.md` describes a conservative V1 where Maintenance / Energy / Andon / Documents / WIP / Planning are **add-ons, NOT in V1 core** — and AI / Mobile are **out of V1 entirely**.

The current direction (as of 2026-04-24) is: **all 60+ Figma components will be built and shipped in the commercial platform, gated by module licenses.** Mobile/PWA is IN scope. HR is IN scope. Only AI is still future.

**Action item:** When next reviewing `docs/07_V1_SCOPE.md`, reconcile or explicitly supersede with this new direction.

### 12.2 Write-back activation timing
Write-back is a separate license (per `06_LICENSING_AND_PACKAGING.md`). It enters the product seriously in Sprint 16 (ERP) and Sprint 15 (PLC). Before then, all connectors are read-only.

### 12.3 Mobile PWA as separate app vs. responsive Blazor
Sprint 14 assumes same-codebase PWA. If Blazor Server PWA proves impractical (long circuits over mobile networks), a separate Blazor WASM or MAUI companion app becomes the fallback.

### 12.4 BorrowModule semantic still undefined
Meaning will be clarified during Sprint 8–9 (Inventory). Current best guess: inter-line/shift material or tool borrowing.

### 12.5 TimescaleDB migration trigger
Currently all eventing data goes to regular PostgreSQL tables. Once alarm/downtime/OEE volume exceeds ~100k rows/day per tenant, migrate to TimescaleDB hypertables.

### 12.6 MachineCalendarView full drag-drop
`/engineering/calendar` is a simplified maintenance calendar (no drag-drop).
The full Gantt drag-drop production planning calendar from MachineCalendarView.tsx belongs to Sprint 6 (Planning module).

---

## 13. How to Use in a New Conversation

**Paste this file as the first message**, then state the topic:

> "This is the xMachineNema project status. Let's work on [topic]."

**For Sprint 3b-W (current — write phase):**
> "We're on Sprint 3b-W — Engineering write phase. Add POST/PUT endpoints for pm-schedules and work-order status, then wire the UI buttons to real API."

**For Figma → Blazor conversion work:**
> "Convert figma-reference/src/app/components/[X].tsx to Blazor Server razor. Keep all Tailwind classes. Use OnAfterRenderAsync pattern."

**For architecture questions:**
> Refer first to `docs/00_MASTER_ARCHITECTURE_SNAPSHOT.md` — it overrides this file where they disagree.

---

## 14. Reference Documents in Repo

| File | Purpose |
|------|---------|
| `docs/00_MASTER_ARCHITECTURE_SNAPSHOT.md` | **Canonical** architecture reference (top authority) |
| `docs/01_PRODUCT_POSITIONING.md` | How to position and sell the product |
| `docs/02_SYSTEM_ARCHITECTURE.md` | System-level architecture detail |
| `docs/03_MODULE_BOUNDARIES.md` | Module-by-module boundary definitions |
| `docs/04_DATABASE_DESIGN.md` | DB schema design principles |
| `docs/05_CONNECTOR_FRAMEWORK.md` | Connector abstraction & runtime |
| `docs/06_LICENSING_AND_PACKAGING.md` | Commercial licensing model |
| `docs/07_V1_SCOPE.md` | V1 scope (⚠️ see §12.1 — tension with current direction) |
| `docs/08_SUPPORTED_CONNECTORS_MATRIX.md` | Connector coverage matrix |
| `docs/09_DEVELOPMENT_SEQUENCE.md` | Original dev sequence guide |
| `docs/10_SOLUTION_STRUCTURE.md` | Solution folder/project layout |
| `docs/LOCAL_DEVELOPMENT.md` | Local dev setup |
| `docs/PROJECT_STATUS.md` | **This file** — living status |
