# xMachineNema

Modular edge-first MES, IIoT, dashboard and integration platform.

## Repo layout

- **Docs (source of truth):** `docs/`
- **Solution layout overview:** `docs/10_SOLUTION_STRUCTURE.md`
- **Local run (Web + Api + Postgres + migrations + seed):** [`docs/LOCAL_DEVELOPMENT.md`](docs/LOCAL_DEVELOPMENT.md)
- **Visual Studio:** open **`xMachineNema.slnx`** at repo root; see **Visual Studio ile çalışma** in [`docs/LOCAL_DEVELOPMENT.md`](docs/LOCAL_DEVELOPMENT.md) for single project, multiple startup projects, and AppHost.

## Quick start (local)

1. Install **.NET 10** and **PostgreSQL**.
2. Create database (e.g. `xmachine_nema`) and align `ConnectionStrings:XMachineOperationalDb` in `appsettings.Development.json` for **Api** and **Web** (use the same values on both). Set Web **`XMachine:Api:BaseUrl`** if Api is not on the default `http://localhost:5090`.
3. Either:
   - **`dotnet run --project src/apps/XMachine.AppHost/XMachine.AppHost.csproj`** (starts Api then Web), or  
   - Run **Api** and **Web** in two terminals (see `docs/LOCAL_DEVELOPMENT.md`).

Default sample URLs: Api `http://localhost:5090`, Web `http://localhost:5197`.

**Build error `MSB3027` / DLL locked by `XMachine.Api`:** stop debugging (**Shift+F5**), then rebuild. See troubleshooting in [`docs/LOCAL_DEVELOPMENT.md`](docs/LOCAL_DEVELOPMENT.md).
