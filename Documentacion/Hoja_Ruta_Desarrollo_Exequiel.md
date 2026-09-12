# 📋 Hoja de Ruta de Desarrollo Técnico - Plan de Trabajo

**Proyecto:** TourismTracking / WanderTrack  
**Cátedra:** Desarrollo de Software 2025 - UTN FRCU | Prof. Enzo Tanga  
**Colaboradores:** Thiago JGK & Exequiel  

Este documento establece la planificación técnica de ingeniería de software para coordinar las tareas pendientes entre Thiago y Exequiel, garantizando la resolución de brechas funcionales, la integridad de la persistencia y la cobertura de pruebas requerida para la aprobación y promoción de la asignatura.

---

## 🛠️ Matriz de Tareas de Implementación y Remediación

A continuación se detallan las especificaciones técnicas y los criterios de aceptación para cada módulo del sistema:

### 1. Conexión de Eventos y Manejo Reactivo en Angular (Frontend)
* **Objetivo:** Vincular los eventos de interacción de usuario en los componentes Angular con los proxies del backend.
* **Archivos Afectados:**
  * `angular/src/app/tourism/destination-detail/destination-detail.component.html` (y `.ts`)
  * `angular/src/app/tourism/user-dashboard/user-dashboard.component.html` (y `.ts`)
* **Especificación Técnica:**
  * Enlazar el botón de favoritos (ícono de corazón) en la vista de detalle del destino mediante la directiva reactiva `(click)="toggleFavorite(destination.id)"`, invocando `TourismInteractionService.addToFavorites()` o `removeFromFavorites()`.
  * Vincular las acciones de edición y eliminación de experiencias en el tablero de usuario (`UserDashboardComponent`) con los métodos correspondientes del servicio de interacción.
* **Criterios de Aceptación:**
  * Interfaz responsiva con actualización inmediata del estado en la UI tras emitir la solicitud HTTP.
  * Compilación exitosa del frontend (`npm run build`) sin advertencias ni tipos incompatibles.

---

### 2. Eliminación de Datos Hardcodeados y Enlace a Métricas Reales en Angular (Frontend)
* **Objetivo:** Erradicar valores fijos en el marcado HTML y consumir métricas reales calculadas por el backend relacional.
* **Archivos Afectados:**
  * `angular/src/app/tourism/list-destinations/list-destinations.component.html` (y `.ts`)
  * `angular/src/app/tourism/destination-detail/destination-detail.component.html` (y `.ts`)
* **Especificación Técnica:**
  * Sustituir las calificaciones fijas por el consumo dinámico del endpoint `TourismInteractionService.getDestinationAverageRating(id)`.
  * En la lista de reseñas del destino, resolver dinámicamente la identidad del autor consultando el perfil público del usuario (`TourismUserService.getPublicProfile(creatorId)`) en lugar de etiquetas estáticas.
* **Criterios de Aceptación:**
  * Cero datos mockeados o valores simulados en la capa de presentación.
  * Manejo seguro de destinos sin calificaciones previas (despliegue de estado inicial sobrio sin romper la vista).

---

### 3. Corrección de Inconsistencia de Autoría en Reseñas (Backend)
* **Objetivo:** Estandarizar la validación de propiedad y permisos en la manipulación de entidades `Review`.
* **Archivo Afectado:**
  * `aspnet-core/src/TourismTracking.Application/Experiences/Hito3_Services.cs`
* **Especificación Técnica:**
  * En el método `DeleteReviewAsync`, unificar la comprobación de autoría contra `review.UserId == CurrentUser.GetId()`, alineándola con la regla implementada en `UpdateReviewAsync` y respetando el modelo de entidades del dominio.
* **Criterios de Aceptación:**
  * Rechazo con excepción de autorización (`AbpAuthorizationException`) si un usuario intenta modificar o borrar una reseña de otro usuario.
  * Pasaje exitoso de las pruebas de integración en `TourismTracking.Application.Tests`.

---

### 4. Bitácoras y Manejo Estructurado de Excepciones en Integración Externa (Backend)
* **Objetivo:** Cumplir el requerimiento institucional de auditoría y trazabilidad ante fallos de conectividad con APIs de terceros.
* **Archivo Afectado:**
  * `aspnet-core/src/TourismTracking.Application/Destinations/DestinationAppService.cs`
* **Especificación Técnica:**
  * En el bloque `catch` del método `SearchExternalDestinationsAsync`, registrar eventos y errores de comunicación con el servicio meteorológico y de geocodificación mediante `ILogger<DestinationAppService>` (`Logger.LogError(ex, "Error al consultar API externa...")`).
* **Criterios de Aceptación:**
  * Inyección del log en las bitácoras del sistema con detalles del endpoint fallido.
  * Respuesta controlada (colección vacía o fallback) hacia el frontend sin propagar excepciones 500 no controladas.

---

### 5. Cobertura Exhaustiva de Pruebas Unitarias (Domain & Application)
* **Objetivo:** Garantizar la estabilidad de las reglas de negocio mediante una suite integral de pruebas automatizadas.
* **Proyectos Afectados:**
  * `aspnet-core/test/TourismTracking.Domain.Tests/`
  * `aspnet-core/test/TourismTracking.Application.Tests/`
* **Especificación Técnica:**
  * Implementar casos de prueba bajo el patrón Arrange-Act-Assert para invariantes de dominio en `Destination` (validación de nombre, coordenadas geográficas válidas), `Review` (rango de calificación entre 1 y 5 estrellas) y `Experience`.
* **Criterios de Aceptación:**
  * 100% de pruebas unitarias exitosas (55 de 55 pruebas pasando sin errores ni omisiones).
  * Ejecución reproducible con el comando estándar `dotnet test aspnet-core/TourismTracking.sln`.

---

## 🧭 Flujo de Trabajo y Colaboración en Git

Para asegurar la convergencia ordenada del proyecto en el repositorio:
1. **Rama de Trabajo:** Todo el desarrollo activo se realiza sobre la rama de integración `dev`.
2. **Sincronización Previa:** Antes de comenzar una sesión de trabajo y antes de subir código, ejecutar `git pull --rebase origin dev`.
3. **Verificación Pre-Push Obligatoria:** Ejecutar siempre `dotnet build` y `dotnet test` localmente. No se autoriza el push si existen errores de compilación o fallos en las pruebas.
4. **Pull Request de Entrega a `prod`:** Finalizada la implementación y verificada la persistencia, se genera el Pull Request hacia `prod` adjuntando el Changelog Académico y la evidencia de pruebas para la corrección del docente.

Para consultar el índice general y las especificaciones de la cátedra, acceder al [Centro de Documentación](./README.md).
