# Project: WanderTrack (TourismTracking) — Academic Review Fixes

## Architecture
- **Framework**: ABP Framework v8.3.4 (.NET 8) with Angular 18 Frontend.
- **Layers**:
  - `TourismTracking.Domain`: Core entities (`Destination`, `Notification`, `ApiMetric`, `FavoriteListItem`), Data Seeder (`TourismDataSeedContributor`).
  - `TourismTracking.Application.Contracts`: DTOs, interfaces (`ITicketmasterService` or within Application).
  - `TourismTracking.Application`: Background worker (`DailyDestinationUpdateWorker`), typed service (`TicketmasterService`).
  - `TourismTracking.HttpApi.Host`: Host module with background worker lifecycle registration, configuration (`appsettings.json`, User Secrets).
  - `TourismTracking.DbMigrator`: Database migration and seeding console application.
  - `TourismTracking.Application.Tests`: xUnit + Shouldly + NSubstitute unit test suite.

## Feature Inventory
| # | Feature | Description | Milestone | Source |
|---|---------|-------------|-----------|--------|
| 1 | Credential Sanitization | Remove all plaintext passwords (`Comunidad2024*`, `Thiago123*`, `1q2w3E*`) from `.cs`, `.json`, `.md` | M1 | ORIGINAL_REQUEST R1 |
| 2 | Seeder Configuration Injection | `TourismDataSeedContributor` injects `IConfiguration` to read `SeedPasswords` securely with dynamic fallback | M1 | ORIGINAL_REQUEST R1 |
| 3 | Shared User Secrets | Add shared `UserSecretsId` to `DbMigrator.csproj` matching `HttpApi.Host.csproj` | M1 | ORIGINAL_REQUEST R1 |
| 4 | Configuration Placeholders | Add `SeedPasswords` and `TicketMaster:ApiKey` sections to Host and DbMigrator `appsettings.json` | M1 | ORIGINAL_REQUEST R1, R3 |
| 5 | Documentation Updates | Update `README.md`, `EMAIL_PROFESOR_ENZO_TANGA.md`, and `REVISION_PREVIA_ACADEMICA.md` in both `Documentacion/` and `docs/` | M1 | ORIGINAL_REQUEST R1 |
| 6 | Worker Registration | Register `DailyDestinationUpdateWorker` via `context.AddBackgroundWorkerAsync` in `TourismTrackingHttpApiHostModule` | M1 | ORIGINAL_REQUEST R2 |
| 7 | Background Workers Module Dependency | Add `typeof(AbpBackgroundWorkersModule)` to `[DependsOn]` in `TourismTrackingHttpApiHostModule` | M1 | ORIGINAL_REQUEST R2 |
| 8 | TicketMaster API Key Config & Fallback | Read `TicketMaster:ApiKey` via `IConfiguration`, eliminate `DUMMY_KEY`, gracefully warn on missing/empty key | M1 | ORIGINAL_REQUEST R3 |
| 9 | Typed TicketMaster Service & DTOs | Implement `ITicketmasterService` and `TicketmasterResponseDto` hierarchy with `System.Text.Json` deserialization | M1 | ORIGINAL_REQUEST R4 |
| 10 | Real Event Querying & Notifications | Worker queries TicketMaster, skips destinations with 0 events, emits notifications with real event name and date | M1 | ORIGINAL_REQUEST R4 |
| 11 | Sanitized ApiMetric Logging | Worker logs `ApiMetric` without exposing the API key in the recorded URL endpoint | M1 | ORIGINAL_REQUEST R4 |
| 12 | Worker Unit Tests | Implement unit tests for `DailyDestinationUpdateWorker` covering events, 0 events, and missing API key | M1 | ORIGINAL_REQUEST R5 |
| 13 | Full Suite & Build Verification | Verify `dotnet test` (all 55+ tests pass), `dotnet build` (0 errors), `npm run build` (0 errors), and `git grep` checks | M1 | ORIGINAL_REQUEST R6 |

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| M1 | Complete Academic Review Implementation & Verification | All requirements R1-R5: credentials externalization, worker registration, TicketMaster service & deserialization, unit tests, full verification | none | DONE |

## Interface Contracts
### ITicketmasterService
```csharp
namespace TourismTracking.Ticketmaster;

public interface ITicketmasterService : Volo.Abp.DependencyInjection.ITransientDependency
{
    Task<List<TicketmasterEventDto>> GetEventsByCityAsync(string cityName, CancellationToken cancellationToken = default);
}

public class TicketmasterEventDto
{
    public string Name { get; set; } = string.Empty;
    public string FormattedDate { get; set; } = string.Empty;
    public string? Url { get; set; }
}
```

### DailyDestinationUpdateWorker
```csharp
public class DailyDestinationUpdateWorker : AsyncPeriodicBackgroundWorkerBase
{
    // Public testable entry point:
    public async Task ProcessDestinationUpdatesAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
}
```

## Code Layout
- `aspnet-core/src/TourismTracking.Domain/Data/TourismDataSeedContributor.cs`: Seeder credential externalization.
- `aspnet-core/src/TourismTracking.HttpApi.Host/appsettings.json`: Configuration placeholders.
- `aspnet-core/src/TourismTracking.DbMigrator/appsettings.json`: Configuration placeholders.
- `aspnet-core/src/TourismTracking.DbMigrator/TourismTracking.DbMigrator.csproj`: Shared UserSecretsId.
- `aspnet-core/test/TourismTracking.HttpApi.Client.ConsoleTestApp/appsettings.json`: Remove plaintext admin password.
- `aspnet-core/src/TourismTracking.HttpApi.Host/TourismTrackingHttpApiHostModule.cs`: Worker registration.
- `aspnet-core/src/TourismTracking.Application/Ticketmaster/`: `ITicketmasterService.cs`, `TicketmasterDtos.cs`, `TicketmasterService.cs`.
- `aspnet-core/src/TourismTracking.Application/Workers/DailyDestinationUpdateWorker.cs`: Worker logic.
- `aspnet-core/test/TourismTracking.Application.Tests/Workers/DailyDestinationUpdateWorker_Tests.cs`: Unit tests.
- `README.md`, `Documentacion/`, `docs/`: User Secrets documentation.
