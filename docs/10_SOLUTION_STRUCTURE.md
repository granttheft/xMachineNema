# Solution ve Proje İskeleti (Görev 1)

Bu repo V1 için **modüler monolith** yaklaşımıyla tek solution altında çok projeli yapı kullanır.
Edge runtime ayrı tutulur. Connector framework çekirdeğin stratejik parçasıdır.

## Klasörler

- `src/apps/`: Uygulama host projeleri (Web, Api, AppHost, ServiceDefaults)
- `src/building-blocks/`: Çekirdek katmanlar (SharedKernel/Domain/Application/Infrastructure/Persistence)
- `src/modules/`: V1 çekirdek domain modülleri (Platform/Auth/Commercial/Integration/MES/Quality/Eventing/Workflow/Docs/Plugin)
- `src/connectors/`: Connector framework + V1 öncelikli somut connector scaffolding’i
- `src/edge/`: Edge runtime projeleri (Agent, LocalStore)

## Notlar (bilinçli boşluklar)

- `XMachine.AppHost` artık **yerel geliştirme orkestratörü** olarak Api + Web süreçlerini başlatır (Aspire workload gerekmez).
  İleride tam .NET Aspire dağıtım modeli istenirse bu proje genişletilebilir.
- `XMachine.ServiceDefaults` ortak telemetry/health şablonu için ayrı tutulur; AppHost doğrudan bağlanmak zorunda değildir.
- Time-series motoru/storage/schema seçimi **bu aşamada yapılmadı**.
- Modüller/connector’lar **iş mantığı içermez**; sadece scaffolding ve bağımlılık yönü kurulmuştur.

