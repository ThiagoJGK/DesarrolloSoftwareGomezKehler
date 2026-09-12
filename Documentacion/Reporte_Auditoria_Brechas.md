# 🔍 Reporte de Auditoría de Brechas y Calidad de Software
**Proyecto:** TourismTracking  
**Objetivo:** Consolidación de hallazgos del sistema frente a los requerimientos de promoción de la cátedra (Año 2025).

Este reporte unifica las auditorías profundas realizadas por el equipo de desarrollo mediante inspección estática, matrices de requerimientos y pruebas de integración en las capas de **Backend (.NET)**, **Frontend (Angular)** y **Pruebas Unitarias**. Su propósito es servir de mapa de ruta definitivo para remediar las falencias técnicas del software antes de la defensa final de la materia.

---

## 🛠️ 1. Matriz de Cobertura de Requerimientos Académicos

A continuación se detalla el estado real de integración de cada operación exigida por la cátedra, cruzando el estado del Backend (Servidor) y Frontend (Interfaz de Angular):

| ID | Operación Requerida | Estado Backend | Estado Frontend | Brecha / Hallazgo Crítico |
| :--- | :--- | :---: | :---: | :--- |
| **1.1** | Registrar nuevo usuario | 🟢 100% | 🟢 100% | Resuelto nativamente por `@abp/ng.account` y OpenIddict. |
| **1.2** | Iniciar sesión | 🟢 100% | 🟢 100% | Resuelto nativamente mediante flujo OIDC Bearer. |
| **1.3** | Actualizar perfil de usuario | 🟢 100% | 🟡 Parcial | Los campos de foto y preferencias no están binding en Angular ni guardados en el perfil personalizado. |
| **1.4** | Cambiar contraseña | 🟢 100% | 🟢 100% | Soportado nativamente por la UI de cuenta de ABP. |
| **1.5** | Eliminar la propia cuenta | 🔴 **No** | 🔴 **No** | **Faltante total.** No hay endpoint ni interfaz Angular. Se debe implementar `IdentityUserManager.DeleteAsync` en el backend. |
| **1.6** | Consultar perfil de otros | 🔴 **No** | 🔴 **No** | **Faltante total.** Las vistas renderizan rigidamente "Usuario Anónimo" y los endpoints de usuarios restringen el listado a administradores. |
| **3.1** | Buscar ciudades por nombre | 🟢 100% | 🟢 100% | Resuelto consumiendo Open-Meteo Geocoding API sin clave. |
| **3.2** | Filtrar ciudades (país/pob.) | 🟡 Parcial | 🔴 **No** | En backend solo se filtra por país en memoria. En frontend el formulario solo tiene un input de texto básico. |
| **3.3** | Detalle de ciudad | 🟢 100% | 🟢 100% | Ficha detallada funcional (`DestinationDetailComponent`). |
| **3.4** | Listar destinos populares | 🟢 100% | 🟡 Parcial | En Angular, las tarjetas populares tienen escrito el promedio de calificación **hardcodeado en el HTML** (`⭐ 4.5`). |
| **3.5** | Persistir destino (Caché local) | 🟢 100% | 🟢 100% | Patrón Write-Through funcional en `DestinationAppService`. |
| **4.1** | Crear experiencia de viaje | 🟢 100% | 🟢 100% | CRUD funcional a través de diarios de viaje. |
| **4.2** | Editar experiencia propia | 🟢 100% | 🔴 **No** | El backend valida la autoría, pero en Angular los botones de edición **carecen de bindings `(click)`** en el dashboard. |
| **4.3** | Eliminar experiencia propia | 🟢 100% | 🔴 **No** | El backend valida autoría, pero el frontend tiene el botón huérfano de acción **sin bindings**. |
| **4.4** | Consultar experiencias ajenas | 🟢 100% | 🟢 100% | Listado dinámico en la ficha del destino. |
| **4.5** | Filtrar experiencias (valoración)| 🔴 **No** | 🔴 **No** | **Faltante.** La entidad `Experience` no tiene campo de calificación o sentimiento, impidiendo este filtrado. |
| **4.6** | Buscar por palabras clave | 🟢 100% | 🔴 **No** | El backend soporta `keywordFilter`, pero no hay campos de entrada de tags en la UI de Angular. |
| **5.1** | Calificar un destino (1-5) | 🟢 100% | 🟢 100% | Validado a nivel de entidad de dominio. Selector estrella funcional. |
| **5.2** | Agregar comentario en reseña | 🟢 100% | 🟢 100% | Formulario funcional de reseña. |
| **5.3** | Editar/borrar reseña propia | 🟢 100% | 🔴 **No** | Soportado en backend, pero **no existen botones ni vistas** en Angular para modificar reseñas escritas. |
| **5.4** | Promedio de calificaciones | 🟢 100% | 🟢 100% | Cálculo aritmético dinámico redondeado a un decimal. |
| **5.5** | Listar comentarios de destino | 🟢 100% | 🟡 Parcial | Renderizado cronológico funcional, pero los autores aparecen siempre como **"Usuario Anónimo"** estático en el HTML. |
| **6.1** | Agregar destino a favoritos | 🟢 100% | 🔴 **No** | **Fallo estético.** El botón de corazón en Angular **no tiene evento `(click)`** programado. |
| **6.2** | Eliminar de favoritos | 🟢 100% | 🟢 100% | Botón funcional en el panel del usuario. |
| **6.3** | Consultar favoritos | 🟢 100% | 🟢 100% | Dashboard del usuario despliega la lista. |
| **7.2** | Notificar cambios y eventos | 🟡 Parcial | 🔴 **No** | El worker periódico está programado, pero las llamadas a APIs externas y notificaciones reales están comentadas en un `TODO`. |
| **7.4** | Notificaciones leídas/no leídas| 🔴 **No** | 🔴 **No** | **Faltante total.** No hay entidades de notificación de usuario ni panel de buzón en el frontend. |
| **8.1** | Métricas de uso de API (Admin) | 🔴 **No** | 🔴 **No** | **Faltante total.** La pestaña "Métricas" en Angular es **100% estática (fake)** con números hardcodeados en el HTML. |

---

## 🚨 2. Hallazgos Críticos de Calidad y Seguridad

### A. Inconsistencia de Seguridad en Reseñas (Backend)
En la clase de servicios del backend `Hito3_Services.cs` se detectó una discrepancia severa en las validaciones de autoría:
*   En **`EditReviewAsync`** (Línea 63): Se valida el propietario contra `review.UserId != _currentUser.Id` (Entidad de Negocio).
*   En **`DeleteReviewAsync`** (Línea 73): Se valida contra `review.CreatorId != _currentUser.Id` (Propiedad de auditoría de infraestructura de ABP).

> [!CAUTION]
> **Consecuencias:** Esta inconsistencia fragiliza la aplicación. En entornos de pruebas o migraciones donde `CreatorId` pueda no registrarse o ser nulo, los usuarios no podrán eliminar sus propias reseñas aunque les pertenezcan legítimamente. Además, obliga a que las pruebas unitarias dependan del framework.  
> **Acción recomendada:** Modificar `DeleteReviewAsync` para que valide consistentemente contra `review.UserId`.

### B. Ocultamiento de Errores de Red (Backend)
El método de búsqueda externa en `DestinationAppService.cs` silencia de forma absoluta las excepciones:
```csharp
catch(Exception ex)
{
    // Return an empty list or handle error if API fails
}
```
Si la API de Open-Meteo sufre de indisponibilidad o latencia excesiva, el sistema no registrará ningún log y retornará un array vacío, imposibilitando el diagnóstico administrativo (exigido en el requerimiento **8.2**).

---

## 🧪 3. Diagnóstico de la Suite de Pruebas Unitarias
*   **Domain Layer (`TourismTracking.Domain.Tests`):** Cobertura del **0%**. No existen pruebas para validar las invariantes críticas de negocio (rango 1-5 de estrellas en `Review`, requerimientos de nombres no vacíos en `Destination` o `Experience`).
*   **Application Layer (`TourismTracking.Application.Tests`):** Cobertura aproximada del **15%**. Se usan buenas prácticas con mocks de `NSubstitute`, pero solo se testean tres escenarios: el happy path del promedio de calificaciones y dos excepciones de seguridad. Quedan sin probar las lógicas CRUD completas, los flujos alternativos, los workers y la persistencia.

---

## 🛠️ 4. Plan de Remediación y Ruta Tecnológica

Para asegurar una entrega de altísima calidad profesional ante la cátedra y cumplir con el 100% de la promoción, el equipo de desarrollo debe abordar las siguientes prioridades:

### Fase I: Remediación Inmediata de "Mocks" en Angular
1.  **Vincular Enlaces Huérfanos:**
    *   Programar el `(click)="addToFavorites()"` en el botón de favoritos de la vista de destinos.
    *   Programar `(click)="deleteExperience(id)"` y `(click)="editExperience(id)"` en los botones de la tabla del dashboard de usuario.
2.  **Eliminar Hardcodes Estéticos:**
    *   Reemplazar el promedio `⭐ 4.5` hardcodeado en la grilla de destinos populares con una llamada al servicio `getDestinationAverageRatingAsync`.
    *   Resolver el nombre del autor en las reseñas de la vista de detalle mapeando la ID del creador, en lugar de mostrar siempre "Usuario Anónimo" en el HTML.

### Fase II: Correcciones Críticas de Backend
1.  **Uniformar Seguridad:** Cambiar la validación de `DeleteReviewAsync` a `review.UserId`.
2.  **Activar Logger de Excepciones:** Inyectar la llamada `Logger.LogError(ex, ...)` en el catch de `DestinationAppService` para cumplir con las bitácoras de monitoreo.
3.  **Completar Filtros de Búsqueda:** Adaptar el backend para filtrar por región y población mínima al consultar la geolocalización.

### Fase III: Elevación de Cobertura de Tests
1.  **Crear Pruebas de Invariantes de Dominio:** Escribir tests en `TourismTracking.Domain.Tests` para certificar que las entidades lanzan excepciones ante datos inconsistentes (Ratings fuera de rango, títulos vacíos).
2.  **Completar Casos de Prueba en Aplicación:** Testear los flujos válidos de inserción, edición y borrado para experiencias y favoritos.
3.  **Agregar Pruebas de Integración con SQLite:** Correr flujos transaccionales completos sobre base de datos en memoria para validar la integridad del Unit of Work y EF Core.
