# Project: WanderTrack - Polish, UX Overhaul & Academic Delivery

## Architecture
- **Backend**: ABP Framework on .NET 8, DDD Architecture:
  - `aspnet-core/src/TourismTracking.Domain`: Aggregate roots, Domain services, Data seeding (`TourismDataSeedContributor.cs`)
  - `aspnet-core/src/TourismTracking.Domain.Shared`: Enums, extra property configurations (`TourismTrackingModuleExtensionConfigurator.cs`)
  - `aspnet-core/src/TourismTracking.Application.Contracts`: DTOs (`ReviewDto.cs`, `ExperienceDto.cs`, `NotificationDto.cs`), service interfaces (`ITourismInteractionAppService.cs`, `INotificationAppService.cs`)
  - `aspnet-core/src/TourismTracking.Application`: Application services (`TourismInteractionAppService.cs`, `NotificationAppService.cs`, `TourismUserAppService.cs`)
  - `aspnet-core/src/TourismTracking.EntityFrameworkCore`: EF Core DbContext and repositories
  - `aspnet-core/test/*`: 55 unit tests across Domain, Application, and EF Core
- **Frontend**: Angular 17/18, TypeScript, Bootstrap, ABP UI / Proxies:
  - `angular/src/app/tourism/list-destinations`: Search cards, "Ver Destino", direct heart favorite, search results preservation
  - `angular/src/app/tourism/destination-detail`: Detail view, coordinates, photo fallback, sentiment filters, experience form with optional keywords, author name/avatar display
  - `angular/src/app/tourism/user-dashboard`: Tablero personal, favorites, owned experiences (edit/delete), live notifications (unread badge, mark read, test trigger)
  - `angular/src/app/home`: Clean home landing page without academic banners or demo credentials

## Feature Inventory
| # | Feature | Description | Milestone | Source |
|---|---------|-------------|-----------|--------|
| 1 | R1.1 Community User Profiles | Seed @lucas.aventura, @sofia.viajera, @elena.patagonia, @martin.turismo with bios/preferences/avatars | M1 (DONE) | Survey/R1 |
| 2 | R1.2 Authentic Authorship | Link 32 reviews & 8 diaries to community users for RF 1.6 modal | M1 (DONE) | Survey/R1 |
| 3 | R1.3 Active User (ThiagoJGK) Data | Seed 3-4 favorites, 2-3 owned experiences for edit/delete, 4 notifications | M1 (DONE) | Survey/R1 |
| 4 | R1.4 Idempotent Seeder Execution | Replace global count check with per-entity checks in TourismDataSeedContributor | M1 (DONE) | Survey/R1 |
| 5 | R2.1 Search UX & "Ver Destino" | Add "Ver Destino" (auto-persists & navigates), direct heart on card, preserve searchResults | M2 (DONE) | Survey/R2 |
| 6 | R2.2 Destination Detail View | Show latitude/longitude coordinates, photos, reactive favorites, sentiment filters | M2 (DONE) | Survey/R2 |
| 7 | R3.1 Experience Validation Fix | Make keywords optional with default tags ("viajes, turismo") in backend and frontend | M1 (DONE), M2 (DONE) | Survey/R3 |
| 8 | R3.2 Eradicate Raw GUIDs | Replace Usuario a00f69da... and ID: a00f69da with author name, username, and avatar | M1 (DONE), M2 (DONE) | Survey/R3 |
| 9 | R4 Academic Banner Eradication | Remove "Modo Evaluación Activo", demo credentials, and academic demo texts from home and dashboard | M3 (DONE) | Survey/R4 |
| 10| R5 Live Dashboard Notifications | Unread badge, notification listing, mark single/all as read, simulate alert in real time | M1 (DONE), M3 (DONE) | Survey/R5 |
| 11| R6.1 Test & Build Verification | 55/55 unit tests pass, clean Angular build | M4 (DONE) | Survey/R6 |
| 12| R6.2 Forensic Audit & Quality Gate | Forensic integrity auditor verification (clean, no slop/cheating) | M4 (DONE) | Survey/R6 |
| 13| R6.3 Git Sync & PR dev -> prod | Ask user confirmation for git push, branch prod, open PR with Academic Changelog | M5 (DONE) | Survey/R6 |
| 14| R6.4 Email Draft for Prof. Enzo Tanga | Academic formal email draft with PR link and testing evidence | M5 (DONE) | Survey/R6 |

## Milestones
| # | Name | Scope | Dependencies | Status |
|---|------|-------|-------------|--------|
| M1 | Backend Community Seeding & Authorship DTOs | R1 (Community profiles, ThiagoJGK state, idempotent seeder), R3.1 (Keywords nullable/default), R3.2 (Author DTOs in InteractionAppService), R5 (SendTestNotification) | none | DONE |
| M2 | Frontend UX Overhaul & Experience Validation | R2 (Search card "Ver Destino", direct heart, detail coordinates), R3.1 (Keywords validation fix & chips), R3.2 (Author rendering & profile modal) | M1 | DONE |
| M3 | Banner Eradication & Live Dashboard Notifications | R4 (Clean home & dashboard banners), R5 frontend (Unread badge, mark read, test notification trigger) | M1, M2 | DONE |
| M4 | Verification & Forensic Audit | R6 (dotnet test 55/55, npm run build, forensic auditor) | M1, M2, M3 | DONE |
| M5 | Academic Delivery & Git PR | R6 (Git push confirmation, prod branch, PR with Changelog, email draft) | M4 | DONE |

## Interface Contracts
### Backend ↔ Frontend DTOs
- `ReviewDto`:
  - `Guid Id, int Rating, string Comment, Guid UserId, Guid DestinationId, DateTime CreationTime`
  - `string? AuthorName, string? AuthorUsername, string? AuthorAvatar`
- `ExperienceDto`:
  - `Guid Id, Guid DestinationId, Guid UserId, string Title, string Content, string Keywords, DateTime CreationTime`
  - `string? AuthorName, string? AuthorUsername, string? AuthorAvatar`
- `ITourismInteractionAppService`:
  - `Task<ExperienceDto> CreateExperienceAsync(Guid destinationId, string title, string content, string? keywords = null);`
  - `Task<ExperienceDto> EditExperienceAsync(Guid id, string title, string content, string? keywords = null);`
- `INotificationAppService`:
  - `Task<NotificationDto> SendTestNotificationAsync();`

## Code Layout
- Backend Source: `aspnet-core/src/`
- Backend Tests: `aspnet-core/test/`
- Frontend Source: `angular/src/app/`
- Documentation: `docs/`, `Documentacion/`
- Metadata: `.agents/`
