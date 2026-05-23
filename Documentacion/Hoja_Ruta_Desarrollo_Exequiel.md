# 🚀 Hoja de Ruta de Desarrollo para Exequiel
**Proyecto:** TourismTracking (Entrega y Defensa Académica 2025)

¡Hola Exequiel! Este documento es una hoja de ruta práctica y estructurada diseñada para coordinar nuestro trabajo y el de nuestras respectivas inteligencias artificiales (**Antigravity**).

Recientemente hemos ejecutado una auditoría profunda multi-agente en el proyecto. El backend está en un estado excelente (compila con **0 errores/advertencias** y pasa el 100% de los tests existentes), pero detectamos varias brechas lógicas y maquetas en la UI que debemos corregir para asegurar la promoción directa de la materia.

Para que no pisemos nuestro trabajo, hemos instalado y configurado **dos nuevas Skills en el repositorio** que nuestras IAs leerán y obedecerán automáticamente: `git-ai-collaboration` (para integración segura en `dev` mediante rebase y pre-push checks locales) y `spec-driven-development` (para sincronizar APIs en .NET y proxies de Angular sin picar código a mano).

---

## 🛠️ Matriz de Tareas de Remediación

A continuación, se detallan las tareas específicas que debemos completar. Puedes pedirle a tu instancia de **Antigravity** que ejecute cualquiera de estas tareas copiando y pegando el prompt sugerido.

### 1. Conectar Botones y Eventos Huérfanos en Angular (Frontend)
*   **Brecha:** El botón de favoritos (corazón) en el detalle del destino y las acciones de "Editar/Eliminar Experiencias" en tu tablero de usuario son estéticos y no ejecutan ninguna función en Angular (carecen de directivas `(click)`).
*   **Archivos a Modificar:** 
    *   `angular/src/app/tourism/destination-detail/destination-detail.component.html` (y `.ts`)
    *   `angular/src/app/tourism/user-dashboard/user-dashboard.component.html` (y `.ts`)
*   **🚀 Prompt para tu Antigravity:**
    > *"Hola Antigravity. Por favor, lee la skill `git-ai-collaboration`. Necesito que vincules el evento `(click)` del botón de Favoritos en la vista detallada del destino para que llame a `interactionService.addToFavorites(destinationId)`. También, vincula las acciones de Editar y Eliminar experiencias en la tabla de UserDashboard para que ejecuten las llamadas correspondientes en el proxy de interactividad de ABP. Valida que el frontend compile sin errores antes de finalizar."*

---

### 2. Eliminar Valores Hardcodeados en Angular (Frontend)
*   **Brecha:** Las tarjetas de destinos populares muestran un rating promedio falso de `⭐ 4.5` escrito a fuego en el HTML, y los autores de las reseñas aparecen siempre como `"Usuario Anónimo"` estático.
*   **Archivos a Modificar:**
    *   `angular/src/app/tourism/list-destinations/list-destinations.component.html` (y `.ts`)
    *   `angular/src/app/tourism/destination-detail/destination-detail.component.html` (y `.ts`)
*   **🚀 Prompt para tu Antigravity:**
    > *"Antigravity, lee la skill `spec-driven-development`. En las tarjetas de destinos populares de Angular, reemplaza el rating promedio estático de 4.5 por una llamada dinámica a la API de métricas de calificaciones. Además, modifica la lista de comentarios para que en lugar de mostrar siempre 'Usuario Anónimo', resuelva dinámicamente el nombre del creador a partir de su ID usando el servicio de perfiles."*

---

### 3. Corregir Inconsistencia de Seguridad en Reseñas (Backend)
*   **Brecha:** En `Hito3_Services.cs`, la edición de reseñas valida la autoría con `review.UserId != _currentUser.Id`, pero la eliminación valida contra `review.CreatorId != _currentUser.Id`. Esto es inconsistente y rompe las pruebas unitarias donde `CreatorId` no se persiste físicamente.
*   **Archivo a Modificar:** `aspnet-core/src/TourismTracking.Application/Experiences/Hito3_Services.cs` (Línea 73).
*   **🚀 Prompt para tu Antigravity:**
    > *"Antigravity, lee la skill `git-ai-collaboration`. Modifica el método `DeleteReviewAsync` en `Hito3_Services.cs` para que valide consistentemente el propietario contra `review.UserId` en lugar de `review.CreatorId`. Compila el backend con `dotnet build` y asegúrate de que las pruebas unitarias en `TourismTracking.Application.Tests` sigan pasando."*

---

### 4. Habilitar Bitácoras y Logs en Consumo de API Externa (Backend)
*   **Brecha:** El bloque `catch` del buscador en `DestinationAppService.cs` está totalmente vacío. Si Open-Meteo falla, el sistema no registra el error en las bitácoras, lo cual viola el requerimiento académico de auditoría **8.2**.
*   **Archivo a Modificar:** `aspnet-core/src/TourismTracking.Application/Destinations/DestinationAppService.cs`.
*   **🚀 Prompt para tu Antigravity:**
    > *"Antigravity, edita el catch de excepciones en `SearchExternalDestinationsAsync` dentro de `DestinationAppService.cs` para inyectar un log de error descriptivo usando el `Logger` del servicio de ABP. Asegúrate de compilar la solución después para garantizar la coherencia."*

---

### 5. Incrementar Cobertura de Pruebas Unitarias
*   **Brecha:** La capa de dominio tiene cobertura de 0% (invariantes de Review, Destination y Experience sin testear) y la de servicios de aplicación tiene solo 15% de cobertura.
*   **Proyectos a Modificar:** 
    *   `TourismTracking.Domain.Tests`
    *   `TourismTracking.Application.Tests`
*   **🚀 Prompt para tu Antigravity:**
    > *"Antigravity, lee la skill `git-ai-collaboration`. Necesito que creemos una suite de pruebas para validar las invariantes críticas de negocio en `TourismTracking.Domain.Tests`. Escribe pruebas unitarias para `Review` (validando que lance error si las estrellas no están entre 1 y 5), `Destination` (validando que los nombres no sean vacíos) y `Experience`. Ejecuta `dotnet test` y asegúrate de que el 100% de los tests pasen exitosamente."*

---

## 🧭 ¿Cómo Colaborar con Git de forma Segura?

Para evitar conflictos y asegurar que no pisemos nuestro trabajo:
1.  **Mantente en `dev`:** Trabajaremos directamente sobre la rama `dev`.
2.  **Sincronización:** Cada vez que inicies o termines una sesión, dile a tu IA que haga `git pull --rebase origin dev`.
3.  **Compilación Obligatoria:** Tu IA tiene prohibido hacer `git push` si `dotnet build` o `dotnet test` fallan localmente.
4.  **Changelog de PR:** Cuando consideremos que el conjunto de cambios está listo, le ordenaremos a una de las IAs crear el Pull Request automático a `prod` para empaquetar la entrega de forma limpia.

¡Cualquier duda, puedes consultar el **[README.md](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/README.md)** del Centro de Documentación que centraliza todas las guías y pliegos!
