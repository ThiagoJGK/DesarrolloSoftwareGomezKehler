# Instrucciones de Auditoría en Navegador (Front-End)

El objetivo de esta auditoría es verificar interactivamente el comportamiento, la visualización y la consistencia del frontend de la aplicación (Angular), garantizando que no existan diseños rotos, botones inactivos o errores en la consola (F12).

---

## 🧭 Plan de Navegación y Verificaciones

### 1. Búsqueda y Filtros de Destinos (Buscador Principal)
*   **Acción:**
    - Entrar a la página de inicio de la aplicación (`http://localhost:4200` o la URL del dev server).
    - Buscar un destino ingresando texto en el buscador (ej. "Mendoza").
    - Hacer clic en **"Filtros Avanzados"** y verificar que el acordeón se despliegue suavemente y muestre los tres inputs (País, Región y Población).
    - Ingresar un filtro (ej. País: "AR", Población: "50000") y presionar Buscar.
    - Confirmar que los resultados filtrados aparezcan de forma premium.
    - Validar que no haya errores de consola (F12) en las llamadas de red.

### 2. Detalle del Destino y Acciones (Reviews y Diarios)
*   **Acción:**
    - Hacer clic en un destino guardado para ver sus detalles (debe redirigir a `/tourism/destination/:id`).
    - **Calificaciones (Reseñas):**
        - Interactuar con la botonera de filtrado de reseñas por estrellas (Todas, Positivas, Neutrales, Negativas) y verificar que el listado se filtre en el cliente instantáneamente.
        - Dejar una reseña ingresando un comentario y seleccionando estrellas.
        - Verificar que aparezcan los botones de **Editar** y **Eliminar** en tu propia reseña, y que al hacer clic funcionen de forma fluida.
        - Verificar que para reseñas de otros usuarios **NO aparezcan** los botones de edición y eliminación.
    - **Diarios de Viaje (Experiencias):**
        - Escribir una nueva experiencia con título, historia y palabras clave (ej: "comida, museos"), y publicarla.
        - Utilizar la barra de búsqueda de palabras clave de experiencias (ej. buscar "comida") y verificar que se filtre el listado al hacer clic en **Filtrar**. Hacer clic en **Limpiar** para restaurar.
    - **Favoritos:**
        - Hacer clic en "Agregar a Favoritos". Verificar que el botón cambie a "Quitar de Favoritos" y cambie de color.
        - Hacer clic de nuevo para quitarlo y verificar la integridad.

### 3. Dashboard del Usuario (`/tourism/dashboard`)
*   **Acción:**
    - Navegar al Dashboard del usuario.
    - **Notificaciones (Fase 4):**
        - Ir a la pestaña **Notificaciones**.
        - Verificar que se muestre el badge con el conteo de notificaciones no leídas en el tab.
        - Hacer clic en **"Marcar como leída"** en una notificación y verificar que cambie su estado visual y el badge disminuya.
        - Hacer clic en **"Marcar todas como leídas"** y verificar que todas se actualicen correctamente.
    - **Métricas API (Solo Admin):**
        - Ir a la pestaña **"Métricas API (Admin)"** (si el usuario autenticado tiene rol admin).
        - Validar que los tres paneles (Total de usuarios, experiencias creadas y destinos guardados) tengan datos reales.
        - Validar que se muestre la tabla de consumo de API (GeoDB, Ticketmaster, etc.) con sus tasas de éxito y tiempos de respuesta.
        - Validar que se liste la tabla de destinos más reseñados con estrellas.
        - Validar que se muestre la bitácora de llamadas recientes.
        - Hacer clic en **"Descargar Reporte CSV"** y certificar que descargue físicamente el archivo `.csv`.
    - **Gestión de Perfil:**
        - Intentar subir una foto de perfil mayor a 1MB y verificar que el sistema muestre la alerta correspondiente sin colapsar el almacenamiento.
        - Subir una imagen JPG/PNG menor a 1MB y verificar que se actualice el avatar.

---

## 🚨 Criterios de Rechazo (Debe Reportarse)
*   Cualquier excepción de JavaScript en la consola (F12).
*   Botones que no realicen ninguna acción o que lleven a páginas de error (`404` / `500`).
*   Elementos visuales desalineados, texto superpuesto, o responsive roto en pantallas móviles.
