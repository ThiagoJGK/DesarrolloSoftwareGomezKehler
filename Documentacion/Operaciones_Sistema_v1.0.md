# 📋 Matriz de Operaciones del Sistema v1.0

Esta lista de control (checklist) detalla de manera estructurada cada una de las operaciones del sistema, sirviendo como mapa de seguimiento de avance de implementación técnica para la cátedra.

---

## 👤 1. Gestión de Usuarios
*   `[ ]` **1.1. Registrar un nuevo usuario:** Formulario administrativo de alta de usuarios (admin de ABP).
*   `[ ]` **1.2. Iniciar sesión:** Login mediante JWT/OpenIddict integrado en Angular.
*   `[ ]` **1.3. Actualizar perfil:** Modificación de datos (nombre completo, bio, foto, preferencias) en la cuenta activa.
*   `[ ]` **1.4. Cambiar contraseña:** Validación e inyección segura de nuevas contraseñas.
*   `[ ]` **1.5. Eliminar la propia cuenta:** Eliminación física o lógica (`IsDeleted`) del usuario autenticado.
*   `[ ]` **1.6. Consultar perfil público:** Visualización de relatos de viajes e información de otros usuarios de la comunidad.

## 🔐 2. Gestión de Roles y Permisos
*   `[ ]` **2.1. Asignar roles:** Clasificación de usuarios (Administrador / Usuario Estándar).
*   `[ ]` **2.2. Administrar permisos:** Asignación de claims y privilegios por rol (exclusivo Admin).
*   `[ ]` **2.3. CRUD de usuarios:** Listar, editar y eliminar usuarios del sistema (exclusivo Admin).

## 🗺️ 3. Destinos Turísticos
*   `[ ]` **3.1. Buscar por nombre:** Consulta al vuelo a través del cliente HTTP de Open-Meteo API.
*   `[ ]` **3.2. Filtro de búsqueda:** Filtrar resultados por país, provincia o rango de población en la API externa.
*   `[ ]` **3.3. Obtener información detallada:** Ficha técnica de la ciudad seleccionada con coordenadas y población.
*   `[ ]` **3.4. Destinos populares:** Listar destinos más guardados por la comunidad de viajeros.
*   `[ ]` **3.5. Persistir destino:** Guardado y actualización local de la ciudad seleccionada en la base de datos interna.

## ✍️ 4. Experiencias de Viaje
*   `[ ]` **4.1. Crear experiencia:** Redactar relatos con título, texto y tags (keywords) de viaje.
*   `[ ]` **4.2. Editar experiencia propia:** Actualizar el relato propio con validación de autoría en Backend.
*   `[ ]` **4.3. Eliminar experiencia propia:** Borrado físico o lógico del relato con validación de pertenencia.
*   `[ ]` **4.4. Consultar experiencias ajenas:** Muro de relatos de viajeros sobre una ciudad o destino específico.
*   `[ ]` **4.5. Filtrar por valoración:** Filtro de experiencias según sentimiento (positiva / neutral / negativa).
*   `[ ]` **4.6. Buscar experiencias:** Búsqueda textual mediante palabras clave (ej: *"comida"*, *"tranquilidad"*).

## ⭐ 5. Calificaciones y Reseñas
*   `[ ]` **5.1. Calificar destino:** Puntuación entera del 1 al 5 asociada a un destino turístico.
*   `[ ]` **5.2. Agregar reseña:** Comentario textual libre adjunto a la puntuación del destino.
*   `[ ]` **5.3. Editar/Eliminar calificación propia:** Modificación del comentario o puntaje con control de propiedad.
*   `[ ]` **5.4. Consultar promedio:** Cálculo consolidado del promedio matemático de estrellas de cada ciudad.
*   `[ ]` **5.5. Listar comentarios:** Visualización cronológica de opiniones sobre un destino.

## ❤️ 6. Favoritos y Viajes Planeados
*   `[ ]` **6.1. Agregar a favoritos:** Añadir ciudad de interés a la lista de seguimiento personal (Bookmarks).
*   `[ ]` **6.2. Eliminar de favoritos:** Remover ciudad de la lista de seguimiento.
*   `[ ]` **6.3. Consultar lista:** Dashboard con el compendio de destinos preferidos de cada usuario.

## 🔔 7. Notificaciones
*   `[ ]` **7.1. Comentarios en favoritos:** Avisar al usuario sobre nuevas reviews escritas en sus destinos preferidos.
*   `[ ]` **7.2. Cambios relevantes:** Avisar sobre eventos (vía TicketMaster) o cambios de población/región detectados.
*   `[ ]` **7.3. Preferencias de alerta:** Configurar canales de aviso (Pantalla, Email, o Ambos) y periodicidad (Inmediato / Semanal).
*   `[ ]` **7.4. Estado de lectura:** Marcar notificaciones como leídas o no leídas para despejar el buzón.

## 📊 8. Administración y Monitoreo (Exclusivo Admin)
*   `[ ]` **8.1. Métricas de API externa:** Registro de volumen de peticiones y tiempos de respuesta de Open-Meteo.
*   `[ ]` **8.2. Revisar logs:** Visor de auditoría de logs para detectar fallas críticas en llamadas externas.
*   `[ ]` **8.3. Estadísticas de negocio:** Reporte de destinos más comentados, experiencias creadas y usuarios activos.
*   `[ ]` **8.4. Descargar reportes:** Exportar métricas operacionales a archivos CSV o PDF.
