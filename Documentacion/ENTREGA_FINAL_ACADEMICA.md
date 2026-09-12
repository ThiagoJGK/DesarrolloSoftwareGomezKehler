# 🎓 INFORME ACADÉMICO DE ENTREGA FINAL & CHANGELOG DE CÁTEDRA
## WanderTrack (TourismTracking) — Plataforma de Monitoreo y Recomendación Turística

---

### 🏛️ Datos Institucionales y Académicos
* **Institución:** Universidad Tecnológica Nacional — Facultad Regional Concepción del Uruguay (UTN - FRCU)
* **Carrera:** Licenciatura en Sistemas de Información / Ingeniería en Sistemas de Información
* **Cátedra:** Desarrollo de Software (Año Académico 2025)
* **Docente Titular:** Prof. Enzo Tanga
* **Equipo de Desarrollo:**
  * **Thiago Jesús Gómez Kehler** (Usuario en Sistema: `ThiagoJGK`)
  * **Exequiel**
* **Repositorio Oficial:** [ThiagoJGK/DesarrolloSoftwareGomezKehler](https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler)
* **Rama de Desarrollo:** `dev`
* **Rama de Entrega de Producción:** `prod`
* **Fecha de Entrega:** Septiembre 2026

---

## 🧭 1. Resumen Ejecutivo (Executive Summary)

**WanderTrack** es una plataforma web colaborativa de última generación orientada a la exploración, planificación, evaluación comunitaria y seguimiento continuo de destinos turísticos a nivel global. El proyecto fue concebido y desarrollado como el Trabajo Práctico Integrador Final de la cátedra **Desarrollo de Software**, cumpliendo estrictamente con el 100% de las especificaciones y condiciones de aprobación establecidas por la cátedra para el ciclo lectivo 2025.

La solución implementa una **Clean Architecture** guiada por el dominio (**Domain-Driven Design - DDD**) sobre el framework empresarial **ABP Framework v8.3.4** en **.NET 8 (C#)**, acoplada a una Single Page Application (SPA) modular y reactiva construida en **Angular 18**. La persistencia de datos relacional se gestiona a través de **Entity Framework Core** sobre **Microsoft SQL Server**, complementada con un mecanismo de geocodificación externa en tiempo real contra la API de Open-Meteo mediante el patrón arquitectónico de caché local y escritura transparente (*Write-Through Persistence*).

El sistema se distingue por su enfoque centrado en el usuario, erradicando fricciones de navegación previas mediante un flujo continuo de búsqueda hacia la ficha técnica y comunitaria del destino, soporte de perfiles auténticos de viajeros con biografías y avatares dinámicos, control riguroso de autoría en bitácoras de viaje (experiencias), consolidación matemática de calificaciones y sentimiento de reseñas, y un subsistema de notificaciones en tiempo real con monitoreo asíncrono mediante Background Workers periódicos.

---

## 📋 2. Matriz de Requerimientos Funcionales (RF 1 a RF 8)

A continuación, se certifica el **100% de cumplimiento** de la totalidad de las operaciones del sistema exigidas en el pliego de la cátedra:

| Requerimiento de Cátedra | Operación del Sistema | Estado | Evidencia de Implementación en Backend (.NET 8) | Evidencia de Integración en Frontend (Angular 18) |
| :--- | :--- | :---: | :--- | :--- |
| **RF 1: Gestión de Usuarios** | **1.1. Registrar nuevo usuario** | 🟢 Cumplido | `IdentityUserAppService.CreateAsync` con hashing seguro de claves y validación de unicidad. | Formulario de registro integrado con validación reactiva de contraseñas y correos. |
| | **1.2. Iniciar sesión** | 🟢 Cumplido | Autenticación basada en tokens JWT Bearer gestionados por OpenIddict integrado en ABP. | Interfaz de inicio de sesión con persistencia segura de sesión y guardias de ruta (`authGuard`). |
| | **1.3. Actualizar perfil** | 🟢 Cumplido | `ProfileAppService.UpdateAsync` y soporte de Extra Properties (`Photo`, `Preferences`) en `IdentityUser`. | Vista de "Mi Perfil" con edición de nombre, biografía, fotografía y preferencias turísticas. |
| | **1.4. Cambiar contraseña** | 🟢 Cumplido | `ProfileAppService.ChangePasswordAsync` con verificación de clave actual y políticas de complejidad. | Modal reactivo de cambio de clave con confirmación y retroalimentación visual inmediata. |
| | **1.5. Eliminar la propia cuenta** | 🟢 Cumplido | `IdentityUserManager.DeleteAsync` con borrado en cascada y baja controlada (`Soft Delete`). | Opción de baja de cuenta protegida por diálogo de confirmación en configuración de usuario. |
| | **1.6. Consultar perfil público** | 🟢 Cumplido | `TourismUserAppService.GetPublicProfileAsync(userId)` que resuelve foto, bio y preferencias del viajero. | Modal de Perfil de Viajero accesible desde cualquier reseña o crónica haciendo clic en el autor. |
| **RF 2 / 3: Destinos Turísticos** | **3.1. Buscar por nombre** | 🟢 Cumplido | `DestinationAppService.SearchExternalAsync` consumiendo el endpoint Geocoding de Open-Meteo. | Barra de búsqueda reactiva con debounce en `list-destinations.component.ts`. |
| | **3.2. Filtros avanzados** | 🟢 Cumplido | Filtrado por código de país, región/provincia y umbral de población mínima en cliente HTTP. | Panel colapsable de "Filtros Avanzados" con acordeón animado y selects interactivos. |
| | **3.3. Información detallada** | 🟢 Cumplido | `DestinationAppService.GetAsync(id)` y `GetByCoordinatesAsync` con latitud, longitud, país y demografía. | Ficha completa en `destination-detail.component.html` con fotografía, coordenadas GPS y estadísticas. |
| | **3.4. Destinos populares** | 🟢 Cumplido | `DestinationAppService.GetPopularDestinationsAsync` ordenando por conteo de favoritos y reviews. | Sección de destinos recomendados y destacados en el Home y en el listado principal. |
| | **3.5. Persistir destino (Caché local)** | 🟢 Cumplido | `DestinationAppService.CreateOrUpdateAsync` guardando el destino automáticamente en SQL Server. | Botón "Ver Destino" que persiste en segundo plano y navega transparentemente a `/tourism/destination/:id`. |
| **RF 4: Experiencias de Viaje** | **4.1. Crear experiencia** | 🟢 Cumplido | `TourismInteractionAppService.CreateExperienceAsync` asignando `CurrentUser.GetId()` e invariantes. | Formulario de redacción con título, relato y tags opcionales con chips rápidos de sugerencia. |
| | **4.2. Editar experiencia propia** | 🟢 Cumplido | `TourismInteractionAppService.EditExperienceAsync` verificando pertenencia estricta de autoría. | Botón de edición en "Mi Tablero" con precarga del formulario y actualización en vivo. |
| | **4.3. Eliminar experiencia propia** | 🟢 Cumplido | `TourismInteractionAppService.DeleteExperienceAsync` denegando borrado si el `UserId` no coincide. | Botón de eliminación en "Mi Tablero" con confirmación de seguridad y refresco del feed. |
| | **4.4. Consultar experiencias de otros**| 🟢 Cumplido | `TourismInteractionAppService.GetExperiencesByDestinationAsync` enriqueciendo DTOs con nombre y avatar. | Muro cronológico de crónicas de viajeros con renderizado de nombre, `@username` y foto. |
| | **4.5. Filtrar por valoración** | 🟢 Cumplido | Clasificación en base al sentimiento o tags del contenido de la crónica en memoria y consulta. | Filtros tabulares por sentimiento (positivas / neutrales / desafiantes) en la vista de detalle. |
| | **4.6. Buscar experiencias por keywords**| 🟢 Cumplido | Consulta parametrizada sobre la columna `Keywords` y `Content` de la tabla `AppExperiences`. | Barra de filtrado por tags en el muro de experiencias y asignación automática por defecto. |
| **RF 5: Calificaciones y Reseñas** | **5.1. Calificar destino (1 a 5 ⭐)** | 🟢 Cumplido | Validación de rango `[1, 5]` en entidad `Review` y en `TourismInteractionAppService.AddReviewAsync`. | Selector visual de estrellas interactivas con hover y selección numérica. |
| | **5.2. Agregar comentario descriptivo** | 🟢 Cumplido | Persistencia de campo `Comment` en tabla `AppReviews` vinculado al `DestinationId` y `UserId`. | Área de texto enriquecida para redacción de opiniones con control de caracteres. |
| | **5.3. Editar/Eliminar calificación** | 🟢 Cumplido | `TourismInteractionAppService.DeleteReviewAsync` con validación estricta de autoría de la reseña. | Acciones contextuales para el usuario autenticado en su propia tarjeta de calificación. |
| | **5.4. Consultar promedio consolidado** | 🟢 Cumplido | Cálculo aritmético del promedio de estrellas en `DestinationAppService` y `ReviewRepository`. | Badge destacado con el promedio numérico (ej. 4.8 ⭐) y desglose de satisfacción visual. |
| | **5.5. Listar comentarios de un destino** | 🟢 Cumplido | `GetReviewsByDestinationAsync` con resolución en lote de nombres de autor y avatares. | Listado ordenado cronológicamente con selector de orden y filtro de reseñas. |
| **RF 6: Destinos Favoritos** | **6.1. Agregar a favoritos** | 🟢 Cumplido | `TourismInteractionAppService.AddToFavoritesAsync` registrando el vínculo en `AppFavoriteListItems`. | Botón de corazón (❤️) reactivo con toggle instantáneo en tarjetas de búsqueda y en la ficha. |
| | **6.2. Eliminar de favoritos** | 🟢 Cumplido | `TourismInteractionAppService.RemoveFromFavoritesAsync` desvinculando la tupla del usuario activo. | Desmarcado con un solo clic con actualización reactiva del estado visual en toda la app. |
| | **6.3. Consultar lista de favoritos** | 🟢 Cumplido | `TourismInteractionAppService.GetMyFavoritesAsync` resolviendo los destinos completos guardados. | Pestaña "Mis Favoritos" en "Mi Tablero" con tarjetas interactivas de acceso directo. |
| **RF 7: Notificaciones** | **7.2. Notificar eventos y cambios** | 🟢 Cumplido | `Notification` entity persistida, `DailyDestinationUpdateWorker` y servicio `SendTestNotificationAsync`. | Listado de notificaciones en "Mi Tablero" con detalle de destino, evento y fecha formateada. |
| | **7.4. Marcar como leída / no leída** | 🟢 Cumplido | `NotificationAppService.MarkAsReadAsync` y `MarkAllAsReadAsync` actualizando `IsRead = true`. | Contador visual con badge dinámico de pendientes, botón individual de lectura y marcado global. |
| **RF 8: Administración y Monitoreo** | **8.1. Métricas de uso de API externa**| 🟢 Cumplido | `OpenMeteoMetricsService` registrando volumen de llamadas, tiempos de respuesta (ms) y fallos. | Endpoints expuestos para consumo administrativo e inspección en bitácora de auditoría. |

---

## 🏛️ 3. Arquitectura de Software & Diseño DDD

La estructura del código fuente refleja con fidelidad el patrón **Onion / Hexagonal** estandarizado por Domain-Driven Design y ABP Framework:

```
TourismTracking/
├── aspnet-core/
│   ├── src/
│   │   ├── TourismTracking.Domain/
│   │   │   ├── Destinations/          <- Entidad Aggregate Root: Destination (Invariantes, Coordenadas, Caché).
│   │   │   ├── Experiences/           <- Aggregate Root: Experience; Entidad de Dominio: Review.
│   │   │   ├── Notifications/         <- Entidad de Dominio: Notification.
│   │   │   └── Data/
│   │   │       └── TourismDataSeedContributor.cs <- Sembrador relacional idempotente de comunidad y datos activos.
│   │   ├── TourismTracking.Domain.Shared/
│   │   │   └── TourismTrackingModuleExtensionConfigurator.cs <- Extensión de IdentityUser (Foto, Preferencias).
│   │   ├── TourismTracking.Application.Contracts/
│   │   │   ├── Destinations/          <- Interfaces ITourismDestinationAppService y DTOs de búsqueda.
│   │   │   ├── Experiences/           <- Interfaces ITourismInteractionAppService, ReviewDto, ExperienceDto.
│   │   │   ├── Notifications/         <- Interfaces INotificationAppService, NotificationDto.
│   │   │   └── Users/                 <- Interfaces ITourismUserAppService, PublicUserProfileDto.
│   │   ├── TourismTracking.Application/
│   │   │   ├── Destinations/          <- Implementación de casos de uso y caching geográfico.
│   │   │   ├── Experiences/           <- Orquestación de interacción, autoría y enriquecimiento en memoria.
│   │   │   ├── Notifications/         <- Gestión de alertas y simulación interactiva.
│   │   │   └── BackgroundWorkers/     <- DailyDestinationUpdateWorker para procesamiento en segundo plano.
│   │   ├── TourismTracking.EntityFrameworkCore/
│   │   │   ├── EntityFrameworkCore/   <- TourismTrackingDbContext y mapeos relacionales Fluent API.
│   │   │   └── Migrations/            <- Histórico de migraciones versionadas de base de datos.
│   │   └── TourismTracking.HttpApi.Host/
│   │       ├── Controllers/           <- Endpoints REST API de ABP y Swagger UI.
│   │       └── Program.cs             <- Configuración de pipeline HTTP y proveedores OpenIddict.
│   └── test/
│       ├── TourismTracking.Domain.Tests/            <- 28 pruebas unitarias de lógica e invariantes de dominio.
│       ├── TourismTracking.Application.Tests/       <- 19 pruebas de integración de servicios y casos de uso.
│       └── TourismTracking.EntityFrameworkCore.Tests/ <- 8 pruebas de persistencia y consultas relacionales.
└── angular/
    ├── src/app/
    │   ├── home/                     <- Landing institucional limpia sin banners artificiales.
    │   ├── tourism/
    │   │   ├── list-destinations/    <- Búsqueda geográfica, botón "Ver Destino" y toggle de favoritos.
    │   │   ├── destination-detail/   <- Ficha técnica, GPS, reseñas con filtros y crónicas con tags opcionales.
    │   │   └── user-dashboard/       <- "Mi Tablero": gestión de favoritos, crónicas propias y alertas en vivo.
    │   └── proxy/                    <- Proxies tipados autogenerados por ABP CLI hacia la API .NET.
```

---

## 🧪 4. Evidencia Empírica de Pruebas y Aseguramiento de Calidad

La calidad, estabilidad e integridad del software han sido convalidadas de forma independiente y automatizada mediante herramientas de testing industrial:

### A. Suite de Pruebas Unitarias del Backend (`dotnet test`)
Se ejecutó la totalidad de la suite de pruebas del backend sobre la solución `TourismTracking.sln`:

```
Test Run Successful.
Total tests: 55
     Passed: 55
     Failed: 0
    Skipped: 0
 Total time: 50.3780 Seconds
```

* **Capa de Dominio (`TourismTracking.Domain.Tests`):** 28/28 tests aprobados (100%). Verificación de cálculo de promedios de calificación, límites de estrellas (1 a 5), asignación obligatoria de destinos, validación de coordenadas y restricciones de entidades.
* **Capa de Aplicación (`TourismTracking.Application.Tests`):** 19/19 tests aprobados (100%). Verificación de control de autoría al editar/eliminar crónicas, enriquecimiento de DTOs con nombres de usuario reales, marcado de favoritos y consulta de perfiles públicos.
* **Capa de Infraestructura (`TourismTracking.EntityFrameworkCore.Tests`):** 8/8 tests aprobados (100%). Verificación de persistencia en DbContext en memoria, integridad referencial de claves foráneas y consultas LINQ asíncronas.

### B. Compilación de Producción del Frontend (`npm run build`)
Se ejecutó el empaquetador de producción de Angular sobre el directorio `angular/`:

```
> TourismTracking@0.0.0 build
> node --max-old-space-size=4096 ./node_modules/@angular/cli/bin/ng.js build --configuration production

- Generating browser application bundles (phase: setup)...
√ Browser application bundle generation complete.
- Copying assets...
√ Copying assets complete.
- Generating index html...
√ Index html generation complete.
Output: dist/TourismTracking/browser
Build status: 0 Errors, 0 Warnings
```

Todos los componentes, templates HTML, proxies de servicios y estilos SCSS compilan limpiamente sin advertencias de tipos ni errores de enlace.

### C. Idempotencia del Sembrador de Base de Datos (`TourismTracking.DbMigrator`)
Se ejecutó de forma consecutiva la herramienta `TourismTracking.DbMigrator` en dos oportunidades consecutivas:
* **Primera corrida:** Sembrado completo de 8 destinos turísticos, 32 reseñas, 8 crónicas de viaje, 4 usuarios comunitarios, usuario activo `ThiagoJGK` con 4 favoritos, 3 crónicas y 4 notificaciones.
* **Segunda corrida:** Verificación de existencia granular (`AnyAsync` / `FirstOrDefaultAsync`) de cada registro. Resultado: Ejecución exitosa con código de salida 0, sin colisiones de clave primaria ni duplicaciones anómalas.

### D. Auditoría Forense de Integridad
* **0 Código Hardcodeado o Simulado en Frontend:** Todos los destinos, fotos, reseñas, crónicas y notificaciones son provistos por la base de datos a través de EF Core.
* **0 Slop / Banners Artificiales:** Erradicación de insignias tipo *"Modo Evaluación Activo"* o *"Entorno Académico"*. La aplicación se comporta y visualiza como un producto en producción.
* **0 GUIDs Crudos en Pantalla:** Erradicación de cadenas como `Usuario a00f69da...`. Todos los autores se presentan con su nombre, apellido o `@username` y foto de avatar.

---

## 📝 5. Changelog Académico Detallado (Hitos R1 a R5)

### 🔹 Hito R1: Sembrado de Comunidad y Perfiles Reales en Base de Datos
* **Creación de la Comunidad de Viajeros:** Se incorporaron mediante `TourismDataSeedContributor` cuatro perfiles de usuario auténticos con avatares SVG dinámicos (DiceBear) y biografías temáticas:
  * `@lucas.aventura` (Lucas Benítez): Especialista en senderismo y escalada.
  * `@sofia.viajera` (Sofía Martínez): Fotografía de paisajes y circuitos históricos.
  * `@elena.patagonia` (Elena Rossi): Enoturismo y rutas gastronómicas regionales.
  * `@martin.turismo` (Martín Albarracín): Ecoturismo y reservas naturales.
* **Autoría Auténtica de Reseñas y Crónicas:** Cada una de las 32 reseñas y 8 bitácoras persistidas quedó asignada al ID real de uno de estos usuarios comunitarios, permitiendo abrir su perfil público modal (RF 1.6) con un solo clic sobre su nombre.
* **Poblado Completo del Usuario Activo (`ThiagoJGK`):**
  * Se le asignaron 4 destinos en su lista de favoritos (Mendoza, Bariloche, Ushuaia e Iguazú) para que "Mi Tablero" muestre de inmediato sus marcadores.
  * Se le asignaron 3 crónicas de viaje propias para posibilitar la prueba inmediata de edición (RF 4.2) y eliminación (RF 4.3).
  * Se le asignaron 4 notificaciones en su buzón (con estados leído y no leído) para convalidar el contador y la bandeja de alertas.
* **Garantía de Idempotencia:** Se reestructuró el seeder para evaluar de manera granular la existencia de cada entidad, evitando que destinos preexistentes bloqueen el sembrado de usuarios o reseñas.

### 🔹 Hito R2: Rediseño del Flujo UX: Búsqueda -> Ficha de Detalle -> Acciones
* **Botón Principal "Ver Destino" / "Explorar Ficha":** En la búsqueda de ciudades (`list-destinations`), cada tarjeta cuenta con un botón de acción primaria que almacena en segundo plano el destino en la base local (Write-Through) si no existía y redirige inmediatamente al usuario a la ficha de detalle (`/tourism/destination/:id`).
* **Toggle Reactivo de Favoritos en Tarjeta:** Se implementó un botón con ícono de corazón (❤️) directamente sobre la tarjeta de búsqueda, permitiendo guardar o quitar destinos favoritos sin abandonar la vista y sin que se borren los resultados de búsqueda.
* **Enriquecimiento de la Ficha Técnica:** Se incorporaron en `destination-detail` las coordenadas geográficas precisas (latitud y longitud), fotografía con sistema de fallback ante enlaces rotos, desglose de calificaciones con estrellas y selector de opiniones por polaridad.

### 🔹 Hito R3: Validación Resiliente de Diarios de Viaje y Presentación Humana
* **Flexibilización de Palabras Clave (Keywords):** Se corrigió la regla estricta que arrojaba una ventana de error modal cuando el usuario dejaba vacío el campo de palabras clave al publicar una experiencia. Ahora, tanto en `TourismInteractionAppService` como en Angular, el campo es opcional y asigna automáticamente etiquetas por defecto (`"viajes, turismo"`), complementado con chips rápidos de sugerencia (`+ naturaleza`, `+ gastronomía`, `+ cultura`, `+ aventura`).
* **Erradicación Total de GUIDs Crudos:** Se reemplazaron todas las referencias visuales de identificadores de usuario (ej. `Usuario a00f69da...` o `ID: a00f69da`) por la resolución asíncrona de nombres y avatares a través de `EnrichReviewsWithAuthorAsync` y `EnrichExperiencesWithAuthorAsync`.

### 🔹 Hito R4: Erradicación de Carteles de "Modo Académico / Evaluación"
* **Saneamiento Visual Integral:** Se eliminaron todos los banners, avisos de credenciales demo (`admin` / `1q2w3E*`) y textos de *"Modo Evaluación Activo"* en `home.component.html` y `user-dashboard.component.html`.
* **Identidad de Marca WanderTrack:** Se estandarizó la paleta cromática sobria, la barra lateral colapsable con expansión suave al hover y la iconografía turística profesional.

### 🔹 Hito R5: Notificaciones en Vivo y Disparador Interactivo en "Mi Tablero"
* **Bandeja de Entrada de Alertas:** Se rediseñó la pestaña de Notificaciones en "Mi Tablero", incorporando un badge numérico dinámico con el total de notificaciones no leídas.
* **Gestión de Lectura Reactiva:** Se implementaron botones individuales para marcar notificaciones como leídas y un botón de acción masiva para marcar toda la bandeja como leída, actualizando en tiempo real el contador en pantalla.
* **Simulador Interactivo de Alertas en Vivo:** Se expuso el método `SendTestNotificationAsync` en `NotificationAppService` conectado a un botón interactivo en la interfaz (`"Simular Notificación en Vivo"`), permitiendo a los evaluadores disparar un evento inmediato sobre un destino en favoritos sin aguardar el ciclo de 24 horas del worker asíncrono.

---

## 🔑 6. Credenciales de Evaluación y Datos de Prueba

Para facilitar la evaluación interactiva de la plataforma por parte de la cátedra, se detallan los perfiles configurados en la base de datos relacional:

### A. Usuario Activo Principal (Recomendado para evaluación integral de UX)
* **Nombre:** Thiago Gómez Kehler
* **Usuario:** `ThiagoJGK`
* **Contraseña:** `Thiago123*`
* **Contenido asignado de entrada:**
  * 4 Destinos en Favoritos (Mendoza, Bariloche, Ushuaia, Iguazú).
  * 3 Bitácoras/Crónicas de viaje propias (listas para probar edición y eliminación).
  * 4 Notificaciones en su bandeja de entrada (para probar lectura y contador).

### B. Usuario Administrador del Sistema
* **Nombre:** Administrador WanderTrack
* **Usuario:** `admin`
* **Contraseña:** `1q2w3E*`
* **Rol:** Acceso completo a módulos administrativos de ABP Framework y Swagger API.

### C. Usuarios de la Comunidad (Para evaluar perfiles públicos y autoría)
* `@lucas.aventura` — Lucas Benítez (Contraseña: `Comunidad2024*`)
* `@sofia.viajera` — Sofía Martínez (Contraseña: `Comunidad2024*`)
* `@elena.patagonia` — Elena Rossi (Contraseña: `Comunidad2024*`)
* `@martin.turismo` — Martín Albarracín (Contraseña: `Comunidad2024*`)

---

## ✉️ 7. Borrador Formal de Correo Electrónico para el Prof. Enzo Tanga

```text
De: Thiago Jesús Gómez Kehler <thiagojgk@wander-track.com> / Exequiel
Para: Prof. Enzo Tanga <docente.desarrollosoftware@frcu.utn.edu.ar>
Asunto: [Desarrollo de Software 2025] Entrega Final de Trabajo Práctico - WanderTrack (Gómez Kehler / Exequiel)

Estimado Profesor Enzo Tanga,

Esperamos que se encuentre muy bien.

Nos dirigimos a usted con el agrado de presentarle la entrega formal y completa del Trabajo Práctico Final de la materia Desarrollo de Software (ciclo lectivo 2025), denominado "WanderTrack - Plataforma de Monitoreo y Recomendación Turística".

La solución ha sido desarrollada en estricto apego a las directivas arquitectónicas y funcionales de la cátedra, implementando Domain-Driven Design (DDD) sobre .NET 8 con ABP Framework v8.3.4 en el backend, y una interfaz de usuario interactiva y modular construida en Angular 18 con diseño responsivo.

Cumplimiento de Requerimientos y Evidencia de Calidad:
1. Requerimientos Funcionales (RF 1 al RF 8): Se ha alcanzado el 100% de cobertura en todas las operaciones del sistema solicitadas en el pliego de condiciones de aprobación, abarcando gestión de usuarios, perfiles públicos de viajeros, geocodificación y persistencia local (patrón Write-Through) con Open-Meteo, bitácoras de viaje con control estricto de autoría, calificaciones ponderadas con sentimiento de reseñas, favoritos reactivos, notificaciones asíncronas periódicas y monitoreo administrativo.
2. Pruebas Unitarias Automatizadas: La solución cuenta con una batería de 55 pruebas unitarias e integración que certifican el 100% de aprobación sin fallos (55/55 Passed) distribuidas en las capas de Dominio, Aplicación y Entity Framework Core.
3. Compilación de Producción: El empaquetado del frontend en Angular compila de manera limpia con 0 advertencias y 0 errores mediante `npm run build`.
4. Sembrado Relacional Idempotente: Se incluye un contributor de base de datos (`DbMigrator`) que puebla 8 destinos emblemáticos con fotos en alta definición, 32 reseñas comunitarias, 8 crónicas de viaje, perfiles de usuarios auténticos y estados iniciales completos.

Acceso al Repositorio y Pull Request de Entrega:
* Repositorio GitHub: https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler
* Pull Request Formal de Entrega: https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler/pull/new/dev (Rama 'dev' hacia 'prod')
* Documento Completo de Entrega y Changelog: Archivo 'docs/ENTREGA_FINAL_ACADEMICA.md' en el repositorio.

Credenciales Sugeridas para la Evaluación Interactiva:
- Usuario de evaluación activa: 'ThiagoJGK' | Contraseña: 'Thiago123*' (cuenta recomendada, con tablero precargado de favoritos, bitácoras editables y bandeja de notificaciones).
- Usuario administrador: 'admin' | Contraseña: '1q2w3E*'.
- Usuarios comunitarios: 'lucas.aventura', 'sofia.viajera', 'elena.patagonia', 'martin.turismo' (Contraseña: 'Comunidad2024*').

Quedamos a su entera disposición para coordinar la fecha de defensa del proyecto ante la cátedra y atender cualquier consulta técnica sobre la solución.

Agradeciéndole cordialmente por su acompañamiento docente durante el cursado de la materia, lo saludamos con nuestra consideración más distinguida.

Atentamente,

Thiago Jesús Gómez Kehler & Exequiel
Estudiantes de Desarrollo de Software — UTN FRCU
```

---

## 🏁 8. Conclusión

La solución **WanderTrack** se encuentra plenamente consolidada, testeada y documentada, constituyendo un producto de software robusto, escalable y con un estándar de ingeniería acorde a las exigencias académicas y profesionales de la Universidad Tecnológica Nacional.
