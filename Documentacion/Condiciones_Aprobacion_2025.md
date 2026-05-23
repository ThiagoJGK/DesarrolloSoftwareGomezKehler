# 🎓 Condiciones de Aprobación del Práctico Final 2025

Este documento establece el subconjunto de funcionalidades y características particulares que el Trabajo Práctico Final debe cumplir para la aprobación de la materia **Desarrollo de Software**. Estas condiciones modifican y complementan el documento de requerimientos general.

---

## 🛠️ Funcionalidades de Backend

Las siguientes operaciones del sistema deben ser implementadas a nivel de servidor (API) y deben contar con **pruebas unitarias y/o de integración**:

### 1. Gestión de Usuarios
*   **1.1.** Registrar un nuevo usuario.
*   **1.2.** Iniciar sesión con usuario y contraseña.
*   **1.3.** Actualizar datos de perfil (nombre, email, foto, preferencias).
*   **1.4.** Cambiar contraseña.
*   **1.5.** Eliminar la propia cuenta.
*   **1.6.** Consultar perfil público de otros usuarios.

### 3. Destinos Turísticos (Integración Open-Meteo / GeoDB)
*   **3.1.** Buscar ciudades por nombre.
*   **3.2.** Buscar ciudades filtrando por país, región o población mínima.
*   **3.3.** Obtener información detallada de una ciudad.
*   **3.5.** Guardar destinos en la base interna de la aplicación (caché local).

### 4. Experiencias de Viaje
*   **4.1.** Crear una nueva experiencia en un destino.
*   **4.2.** Editar una experiencia propia (control de autoría).
*   **4.3.** Eliminar una experiencia propia (control de autoría).
*   **4.4.** Consultar experiencias de otros usuarios en un destino.
*   **4.5.** Filtrar experiencias por valoración (positiva / negativa / neutral).
*   **4.6.** Buscar experiencias por palabras clave (ej: *"gastronomía"*, *"seguridad"*).

### 5. Calificaciones y Reseñas
*   **5.1.** Calificar un destino (de 1 a 5 estrellas).
*   **5.2.** Agregar comentario descriptivo junto con la calificación.
*   **5.3.** Editar o eliminar calificación propia.
*   **5.4.** Consultar promedio de calificaciones consolidado de un destino.
*   **5.5.** Listar comentarios de un destino.

### 6. Favoritos y Viajes Planeados
*   **6.1.** Agregar destino a la lista de favoritos.
*   **6.2.** Eliminar destino de favoritos.
*   **6.3.** Consultar lista personal de favoritos.

### 7. Notificaciones
*   **7.2.** Notificar sobre cambios relevantes en destinos (ej: eventos turísticos).
*   **7.4.** Marcar notificaciones como leídas/no leídas.

### 8. Administración y Monitoreo (Exclusivo Admin)
*   **8.1.** Consultar métricas de uso de la API externa (llamadas, tiempos de respuesta).

---

## 💻 Funcionalidades de Frontend (Angular)

Las siguientes operaciones del sistema deben estar implementadas e integradas en la interfaz de usuario en Angular:

### 1. Gestión de Usuarios
*   **1.1.** Registrar un nuevo usuario.
*   **1.2.** Iniciar sesión con usuario y contraseña.
*   **1.3.** Actualizar datos de perfil (nombre, email, foto, preferencias).
*   **1.4.** Cambiar contraseña.
*   **1.5.** Eliminar la propia cuenta.
*   **1.6.** Consultar perfil público de otros usuarios.

### 3. Destinos Turísticos
*   **3.1.** Buscar ciudades por nombre.
*   **3.2.** Buscar ciudades filtrando por país, región o población mínima.
*   **3.3.** Obtener información detallada de una ciudad.
*   **3.4.** Listar ciudades/destinos populares.
*   **3.5.** Guardar destinos en la base interna de la app.

### 5. Calificaciones y Reseñas
*   **5.1.** Calificar un destino (1 a 5 estrellas).
*   **5.2.** Agregar comentario junto con la calificación.
*   **5.3.** Editar o eliminar calificación propia.
*   **5.4.** Consultar promedio de calificaciones de un destino.
*   **5.5.** Listar comentarios de un destino.

---

## 📈 Condiciones Académicas de Aprobación

### 📋 Regularización de la Materia
*   Tener entregados y aprobados la totalidad de los trabajos prácticos previos (**TP1 a TP8**) antes del final de la semana de recuperación (**2025-11-28**).

### 🏆 Promoción de la Materia (Aprobación Directa)
*   Implementar la **totalidad** de los métodos y operaciones descritos en este documento tanto a nivel de Frontend como de Backend.
*   Realizar un **Pull Request** limpio hacia la rama `prod` del repositorio GitHub del grupo.
*   Solicitar la preaprobación del cuerpo docente.
*   Realizar la **defensa grupal** del TP y aprobarla antes del **2026-02-09**.

### 📝 Aprobación con Regularidad (Examen Final)
*   Aquellos alumnos que regularicen pero no promocionen deberán inscribirse en una mesa de examen final de la materia y realizar la defensa del trabajo bajo las mismas condiciones exigidas para la promoción.
