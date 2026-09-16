# WanderTrack (TourismTracking)

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![ABP Framework](https://img.shields.io/badge/ABP%20Framework-v8.3.4-blue)](https://abp.io/)
[![Angular](https://img.shields.io/badge/Angular-18-DD0031?logo=angular)](https://angular.dev/)
[![Tests](https://img.shields.io/badge/Unit%20Tests-73%2F73%20Passed-success)]()
[![UTN FRCU](https://img.shields.io/badge/UTN-FRCU%202025-006699)](https://www.frcu.utn.edu.ar/)

> **Trabajo Práctico de Cátedra — Desarrollo de Software (Año 2025)**  
> **Institución:** Universidad Tecnológica Nacional — Facultad Regional Concepción del Uruguay (UTN - FRCU)  
> **Docente Titular:** Prof. Enzo Tanga  
> **Alumno:** Thiago Jesús Gómez Kehler  

---

## 🧭 Descripción del Proyecto

**WanderTrack** (TourismTracking) es una plataforma web integral diseñada para la exploración, seguimiento y gestión colaborativa de destinos turísticos internacionales. La solución implementa una arquitectura desacoplada basada en **Domain-Driven Design (DDD)** sobre **ABP Framework v8.3.4**, con un backend en **.NET 8** y una aplicación web interactiva desarrollada en **Angular 18** con la suite Lepton-X Lite UI.

El sistema permite a los viajeros descubrir ciudades y puntos de interés mediante geocodificación externa, guardar destinos en la base de datos relacional local (patrón Write-Through / Cache), redactar bitácoras de viaje (experiencias), calificar destinos con un sistema ponderado de 1 a 5 estrellas, gestionar listas de favoritos y recibir notificaciones asíncronas periódicas ante novedades en sus destinos seguidos.

---

## 🏛️ Arquitectura de la Solución

La solución adopta los principios de **Clean Architecture** y **Domain-Driven Design (DDD)** provistos por el ecosistema ABP, asegurando un desacoplamiento estricto entre la lógica de negocio, la infraestructura relacional y las interfaces de usuario.

```
TourismTracking/
├── aspnet-core/
│   ├── src/
│   │   ├── TourismTracking.Domain/             <- Aggregate Roots (Destination, Experience), Entities (Review, Notification),
│   │   │                                          reglas de negocio, Domain Services y sembrado (TourismDataSeedContributor).
│   │   ├── TourismTracking.Domain.Shared/      <- Enumeraciones, constantes de dominio y localización.
│   │   ├── TourismTracking.Application.Contracts/ <- Interfaces públicas (IAppServices), DTOs y definiciones de permisos.
│   │   ├── TourismTracking.Application/         <- Implementación de casos de uso, AutoMapper y Background Workers.
│   │   ├── TourismTracking.EntityFrameworkCore/<- DbContext relacional, mapeos en Fluent API y migraciones EF Core.
│   │   └── TourismTracking.HttpApi.Host/       <- Endpoints REST API, autenticación OpenIddict (Bearer JWT) y Swagger.
│   └── test/
│       ├── TourismTracking.Domain.Tests/       <- Pruebas unitarias de invariantes de dominio (28 tests).
│       ├── TourismTracking.Application.Tests/  <- Pruebas de lógica de aplicación y flujos de servicio (19 tests).
│       └── TourismTracking.EntityFrameworkCore.Tests/ <- Pruebas de integración con base de datos en memoria (8 tests).
├── angular/                                    <- SPA en Angular 18 (Standalone & Modular Components, Proxies ABP, SCSS).
└── Documentacion/                              <- Pliegos de cátedra, matrices de operaciones, manuales y guías de arquitectura.
```

---

## 📋 Matriz de Requerimientos Académicos Implementados

| Requerimiento | Descripción Técnica | Estado |
| :--- | :--- | :---: |
| **RF 1: Gestión de Identidad** | Registro de usuarios, autenticación Bearer mediante OpenIddict, edición de perfiles, perfiles públicos y eliminación controlada de cuentas. | 🟢 100% |
| **RF 2: Catálogo de Destinos** | Búsqueda geográfica consumiendo Open-Meteo Geocoding API, persistencia Write-Through en base de datos local y ficha detallada de destino. | 🟢 100% |
| **RF 3: Filtros Avanzados** | Panel reactivo en Angular para filtrar destinos por país, región/provincia y umbral de población mínima. | 🟢 100% |
| **RF 4: Experiencias de Viaje** | CRUD completo de relatos y bitácoras de viaje asociadas a destinos, con validación de autoría y lectura pública. | 🟢 100% |
| **RF 5: Calificaciones y Reseñas** | Calificación con estrellas (1 a 5), comentarios descriptivos, cálculo aritmético de promedio y resolución dinámica del autor de la reseña. | 🟢 100% |
| **RF 6: Destinos Favoritos** | Marcado reactivo de destinos favoritos desde la vista de detalle y administración desde "Mi Tablero". | 🟢 100% |
| **RF 7: Notificaciones Asíncronas** | Background Worker periódico (`DailyDestinationUpdateWorker`) para monitorización de novedades en destinos y bandeja de notificaciones. | 🟢 100% |
| **RF 8: Trazabilidad y Auditoría** | Registro estructurado de eventos en bitácoras (`ILogger`), métricas de llamadas a servicios y manejo de excepciones controladas. | 🟢 100% |

---

## 💾 Persistencia y Sembrado de Datos (Database Seeding)

El sistema cuenta con un contributor de sembrado relacional (`TourismDataSeedContributor`) que se ejecuta durante la inicialización de base de datos vía `DbMigrator`, asegurando que el entorno cuente de forma inmediata con datos persistidos reales (0 mocks o hardcodes en el frontend):

* **8 Destinos Emblemáticos:** Mendoza, San Carlos de Bariloche, Ushuaia, Cataratas del Iguazú, Salta, París, Roma y Tokio, persistidos con coordenadas geográficas reales, demografía y fotografías en alta definición de Wikimedia Commons.
* **32 Reseñas Reales:** Evaluaciones persistidas con calificaciones consistentes (promedio estable de 4.8 ⭐) y contenido detallado de autores.
* **8 Crónicas de Viaje:** Experiencias redactadas vinculadas a cada destino.
* **Favoritos y Notificaciones Demo:** Registros iniciales asociados a la cuenta de administración para validar de inmediato la funcionalidad de "Mi Tablero".

---

## 🚀 Guía de Instalación y Puesta en Marcha

### Prerrequisitos de Software
* **.NET 8.0 SDK** ([Descargar](https://dotnet.microsoft.com/download/dotnet/8.0))
* **Node.js 18 o 20 LTS** y npm ([Descargar](https://nodejs.org/))
* **SQL Server** (SQL Server Express, Developer Edition o LocalDB `(localdb)\mssqllocaldb`)
* **ABP CLI**:
  ```powershell
  dotnet tool install -g Volo.Abp.Cli
  ```

---

### Paso 1: Clonar y Ubicarse en la Rama Activa
```powershell
git clone https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler.git
cd "Proyecto Final"
git checkout dev
```

---

### Paso 2: Crear y Sembrar la Base de Datos Relacional
Ejecutar el proyecto migrador de ABP para aplicar las migraciones de Entity Framework Core y persistir las semillas:
```powershell
dotnet run --project aspnet-core/src/TourismTracking.DbMigrator/TourismTracking.DbMigrator.csproj
```

---

### Paso 3: Iniciar el Backend (.NET 8 Web API)
```powershell
dotnet run --project aspnet-core/src/TourismTracking.HttpApi.Host/TourismTracking.HttpApi.Host.csproj
```
* **Documentación Interactiva Swagger:** [https://localhost:44305/swagger](https://localhost:44305/swagger)

---

### Paso 4: Iniciar el Frontend (Angular 18)
En una nueva consola:
```powershell
cd angular
npm install
npm start
```
* **Acceso a la Aplicación Web:** [http://localhost:4200](http://localhost:4200)

> **Nota:** También se dispone de scripts automatizados en la raíz: `Iniciar_Entorno.bat` e `Iniciar_Entorno_Ligero.bat`.

---

## 🔐 Seguridad y Gestión de Credenciales (User Secrets)

En cumplimiento de las buenas prácticas de seguridad y las directivas académicas de la cátedra, **no se almacenan contraseñas ni claves de API privadas en los repositorios públicos de Git**.

Las contraseñas de las cuentas de prueba sembradas en la base de datos y la API Key de TicketMaster se gestionan mediante el almacén seguro de **.NET User Secrets** en desarrollo local o mediante **variables de entorno** en entornos de integración continua / producción.

### ⚙️ Configuración Rápida en Desarrollo Local:

Ejecutar los siguientes comandos en PowerShell o Terminal para definir las credenciales locales en el almacén de usuario del sistema operativo:

```powershell
# Definir contraseña para el usuario activo principal (ThiagoJGK)
dotnet user-secrets set "SeedPasswords:Thiago" "TuPasswordSegura1*" --project aspnet-core/src/TourismTracking.HttpApi.Host

# Definir contraseña para los usuarios de la comunidad (@lucas.aventura, @sofia.viajera, etc.)
dotnet user-secrets set "SeedPasswords:Community" "TuPasswordComunidad1*" --project aspnet-core/src/TourismTracking.HttpApi.Host

# Definir la API Key de TicketMaster (Discovery API v2)
dotnet user-secrets set "TicketMaster:ApiKey" "TuApiKeyTicketMaster" --project aspnet-core/src/TourismTracking.HttpApi.Host
```

> **Nota:** `TourismTracking.DbMigrator` comparte el mismo `UserSecretsId` (`TourismTracking-4681b4fd-151f-4221-84a4-929d86723e4c`), por lo que al ejecutar la migración y el sembrado de datos leerá automáticamente estas mismas claves.

### 🌐 Equivalencia en Variables de Entorno (CI/CD o Docker):
| Clave de Configuración | Variable de Entorno | Propósito |
| :--- | :--- | :--- |
| `SeedPasswords:Thiago` | `SeedPasswords__Thiago` | Contraseña para la cuenta activa demo `ThiagoJGK` |
| `SeedPasswords:Community` | `SeedPasswords__Community` | Contraseña para cuentas de la comunidad |
| `TicketMaster:ApiKey` | `TicketMaster__ApiKey` | Clave de acceso a la API externa de TicketMaster |

### 👥 Cuentas de Demostración Sembradas en Base de Datos:
| Usuario | Nombre Completo | Propósito en la Evaluación | Contraseña |
| :--- | :--- | :--- | :--- |
| `ThiagoJGK` | Thiago Gómez Kehler | **Usuario Principal:** tablero precargado de favoritos, bitácoras editables y notificaciones | Configurada vía User Secrets (`SeedPasswords:Thiago`) |
| `admin` | Administrador WanderTrack | **Administración:** gestión de permisos ABP, roles y Swagger UI | Configurada vía inicialización ABP / User Secrets |
| `lucas.aventura` | Lucas Benítez | Viajero de comunidad: alta montaña y trekking | Configurada vía User Secrets (`SeedPasswords:Community`) |
| `sofia.viajera` | Sofía Martínez | Viajera de comunidad: fotografía y patrimonio histórico | Configurada vía User Secrets (`SeedPasswords:Community`) |
| `elena.patagonia` | Elena Rossi | Viajera de comunidad: enoturismo y gastronomía | Configurada vía User Secrets (`SeedPasswords:Community`) |
| `martin.turismo` | Martín Albarracín | Viajero de comunidad: ecoturismo y circuitos culturales | Configurada vía User Secrets (`SeedPasswords:Community`) |

---

## 🧪 Verificación Automatizada (Suite de Pruebas Unitarias)

La solución cuenta con **73 pruebas unitarias e integración** que certifican el cumplimiento de las reglas de negocio en la capa de dominio y de aplicación (incluyendo pruebas con simulación de eventos de TicketMaster):

```powershell
dotnet test aspnet-core/TourismTracking.sln
```

### Resumen de Resultados de Pruebas:
```
Total tests: 73
     Passed: 73
     Failed: 0
    Skipped: 0
```
* `TourismTracking.Domain.Tests`: 28 pruebas unitarias aprobadas.
* `TourismTracking.Application.Tests`: 37 pruebas de servicios, métricas y worker en segundo plano aprobadas.
* `TourismTracking.EntityFrameworkCore.Tests`: 8 pruebas de persistencia y relaciones relacionales aprobadas.
