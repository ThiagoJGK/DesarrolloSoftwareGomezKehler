# 🌍 Guía Maestra de Onboarding: Proyecto TourismTracking

**Bienvenido, Exequiel.**  
**Proyecto:** TourismTracking / WanderTrack  
**Cátedra:** Desarrollo de Software 2025 - UTN FRCU | Prof. Enzo Tanga  
**Colaboradores:** Thiago JGK & Exequiel  

Este documento es una guía exhaustiva diseñada para la incorporación al entorno de desarrollo local. Aquí se detallan la arquitectura del sistema, los prerrequisitos técnicos, la configuración paso a paso de la base de datos y los comandos para poner en marcha la solución completa.

---

## 1. ¿Qué es TourismTracking? (Contexto del Sistema)

TourismTracking (WanderTrack) es una plataforma web profesional desarrollada sobre arquitectura **Domain-Driven Design (DDD)** con **ABP Framework v8.3.4** en el backend (.NET 8) y **Angular 18** en el frontend (Lepton-X Lite UI). Centraliza la búsqueda de destinos turísticos, la gestión de itinerarios y una comunidad de viajeros mediante reseñas con estrellas y experiencias compartidas.

### Estructura de Módulos y Arquitectura
* **Capa de Dominio (`TourismTracking.Domain`)**: Entidades raíz (`Destination`, `Experience`), entidades secundarias (`Review`, `Notification`), reglas de negocio e inicialización de datos vía `IDataSeedContributor`.
* **Capa de Contratos (`TourismTracking.Application.Contracts`)**: DTOs fuertemente tipados, interfaces de servicios y permisos de seguridad.
* **Capa de Aplicación (`TourismTracking.Application`)**: Lógica de aplicación, casos de uso, servicios de destinos, reseñas y tablero de usuario.
* **Capa de Infraestructura (`TourismTracking.EntityFrameworkCore`)**: Mapeo relacional con EF Core, configuración de Fluent API y migraciones de esquema en SQL Server.
* **Capa Web API (`TourismTracking.HttpApi.Host`)**: Endpoints REST, autenticación OpenIddict y documentación Swagger interactiva.
* **Capa de Presentación (`angular/`)**: SPA en Angular 18, temas responsivos, componentes de destinos, reseñas y dashboard de usuario.

---

## 2. Configuración Manual del Entorno de Desarrollo (Paso a Paso)

Para poner en marcha la aplicación de forma local, sigue la siguiente secuencia de instalación y configuración:

### Paso 1: Prerrequisitos de Software
Asegúrate de contar con las siguientes herramientas instaladas en tu sistema Windows:
1. **.NET 8 SDK**: Versión 8.0.x o superior (`dotnet --version`).
2. **Node.js**: Versión 18 o 20 LTS (`node --version`) con npm (`npm --version`).
3. **SQL Server**: Instancia local (SQL Server Developer, Express o LocalDB `(localdb)\mssqllocaldb`).
4. **ABP CLI**: Herramienta global de desarrollo de ABP Framework. Instalar mediante:
   ```powershell
   dotnet tool install -g Volo.Abp.Cli
   ```

### Paso 2: Clonado y Sincronización de Git
Clonar el repositorio y ubicarse en la rama activa de desarrollo:
```powershell
git clone https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler.git
cd "Proyecto Final"
git checkout dev
git pull --rebase origin dev
```

### Paso 3: Configuración de Base de Datos y Sembrado Inicial (DbMigrator)
El proyecto cuenta con un migrador automático que crea la base de datos relacional, aplica las migraciones de Entity Framework Core e inserta los datos semilla institucionales:
```powershell
dotnet run --project aspnet-core/src/TourismTracking.DbMigrator/TourismTracking.DbMigrator.csproj
```
*Resultado esperado:* Salida por consola indicando ejecución exitosa de migraciones y mensaje `Successfully completed database migrations.`.

### Paso 4: Instalación de Dependencias del Frontend
Ingresar a la carpeta de Angular e instalar las librerías requeridas:
```powershell
cd angular
npm install
```

---

## 3. Puesta en Marcha del Entorno Local

Puedes iniciar los servicios mediante los scripts automatizados de la raíz o mediante consolas independientes:

### Opción A: Inicio con Scripts Automatizados
En la raíz del repositorio se dispone de ejecutables batch preparados para levantar todos los módulos:
* `Iniciar_Entorno.bat`: Inicia SQL Server (si aplica), ejecuta el Web API y levanta el servidor de desarrollo de Angular.
* `Iniciar_Entorno_Ligero.bat`: Variante optimizada para equipos con recursos acotados.

### Opción B: Inicio Manual por Terminales
1. **Backend Web API (.NET 8)**:
   ```powershell
   cd aspnet-core/src/TourismTracking.HttpApi.Host
   dotnet run
   ```
   *Acceso Swagger:* `https://localhost:44305/swagger`
2. **Frontend Angular 18**:
   ```powershell
   cd angular
   npm start
   ```
   *Acceso Interfaz:* `http://localhost:4200`

---

## 4. Credenciales de Acceso Demo y Verificación

Para ingresar al sistema y validar la operatividad de los módulos:

* **Usuario Administrador:** `admin`
* **Contraseña:** `1q2w3E*`

### Recorrido de Verificación Rápida:
1. **Inicio de Sesión:** Acceder a `http://localhost:4200` e iniciar sesión con las credenciales demo.
2. **Catálogo de Destinos:** Explorar los destinos sembrados con imágenes de alta resolución y calificaciones reales.
3. **Filtros Avanzados:** Probar el acordeón de filtros por país, región y población.
4. **Mi Tablero:** Acceder a "Mi Tablero" para interactuar con las crónicas de viaje persistidas, destinos favoritos y notificaciones del usuario.
5. **Ejecución de Pruebas Unitarias:** Ejecutar `dotnet test aspnet-core/TourismTracking.sln` para comprobar que la totalidad de los 55 tests unitarios pasan exitosamente.

---

**¡Bienvenido al equipo de desarrollo, Exequiel!**
