# 🌍 Pliego de Requerimientos: TP Final v0.1
**Tema:** Plataforma de Búsqueda y Seguimiento de Destinos Turísticos  
**Cátedra:** Desarrollo de Software – UTN – FRCU (Año 2025)

---

## 🏛️ Ficha Técnica de la Solución
*   **Framework Base:** ABP .IO (Arquitectura modular y DDD nativo).
*   **Backend:** ASP .NET Core 8 + Entity Framework Core + SQL Server.
*   **Frontend:** Angular (autogenerado e integrado con ABP).
*   **Arquitectura:** DDD (Domain-Driven Design) en capas desacopladas.
*   **API Geográfica Externa:** Open-Meteo Geocoding API (libre) / GeoDB Cities.
*   **API de Eventos Externa:** TicketMaster API.

---

## 🎯 Objetivos del Trabajo
1.  **Modularidad y Seguridad:** Implementar una aplicación web orientada a servicios que resuelva búsquedas geográficas, persistencia interna y envío de alertas.
2.  **Arquitectura DDD:** Utilizar las herramientas del framework ABP (Aggregate Roots, Repositorios, Application Services, DTOs y Unit of Work).
3.  **Frontend Dinámico:** Desarrollar interfaces reactivas con Angular consumiendo servicios de backend.
4.  **Pruebas Automatizadas:** Asegurar la robustez con pruebas unitarias en la capa de Dominio y de Aplicación.

---

## 📋 Requerimientos Funcionales (RF)

| ID | Área / Componente | Descripción Detallada | Rol / Permiso |
| :--- | :--- | :--- | :--- |
| **RF-1** | Gestión de Usuarios | El Administrador da de alta nuevos usuarios (Username único, Email verificado, Rol asignado, Foto de perfil opcional). Los usuarios no pueden auto-registrarse. | Admin |
| **RF-2** | Autenticación (RBAC) | Ingreso mediante usuario y contraseña integrando Identity Management (ABP). Control de accesos a nivel de endpoints y vistas según rol. | Todos |
| **RF-3** | Búsqueda Externa | Buscar ciudades ingresando nombre, región o población mínima consumiendo APIs externas. Mostrar lat/long, población, país e imagen. | Todos |
| **RF-4** | Persistencia (Caché Local) | Guardar un destino consultado en la base de datos interna con marca temporal de última actualización para evitar consultas redundantes a la API. | Estándar / Admin |
| **RF-5** | Lista de Favoritos | Agregar o remover destinos de una lista de seguimiento personal con fecha de adhesión y acceso directo desde el dashboard. | Estándar / Admin |
| **RF-6** | Comunidad e Interacción | CRUD de **Experiencias** de viaje (título, contenido, keywords) asociadas a destinos. Solo el autor puede editar/borrar su experiencia. | Estándar / Admin |
| **RF-7** | Reseñas y Calificaciones | Calificar destinos (1 a 5 estrellas) con comentarios. Las valoraciones son privadas para el autor. El sistema calcula el promedio de satisfacción global. | Estándar / Admin |
| **RF-8** | Alertas y Worker | Worker de segundo plano (cada 24h) que verifica cambios físicos en destinos favoritos o nuevos eventos en TicketMaster, enviando emails o alertas en pantalla. | Sistema |
| **RF-9** | Panel de Monitoreo | Métricas de uso de la API externa (total llamadas, errores, tiempos promedio) y estadísticas de base interna (ciudades populares, usuarios activos). | Admin |
| **RF-10**| Gestión de Logs | Acceso visual a bitácoras de errores de API externa e internamente. Posibilidad de descargar métricas en CSV o PDF. | Admin |
| **RF-11**| Auditoría Completa | Registro automático de actividad crítica: alta/baja de favoritos, modificaciones de reseñas, cambios de roles y creación/eliminación de usuarios. | Sistema |

---

## 🛡️ Requerimientos No Funcionales (RNF)

| ID | Atributo | Criterio de Aceptación Técnica |
| :--- | :--- | :--- |
| **RNF-1** | Arquitectura | Multi-capa desacoplada orientada a DDD usando el framework base ABP .IO. |
| **RNF-2** | Seguridad | Hash criptográfico de contraseñas, validación robusta de entradas contra SQLi/XSS, y protección CSRF nativa habilitada. |
| **RNF-3** | Base de Datos | SQL Server con persistencia ORM gestionada por Entity Framework Core mediante migraciones. |
| **RNF-4** | Escalabilidad | Diseño modular que soporte la incorporación de nuevos proveedores de APIs geográficas o canales de notificación (SMS, Push) sin alterar el core. |
| **RNF-5** | Cobertura de Pruebas| Pruebas unitarias de comportamiento y lógica de negocio implementadas en la capa de Domain y Application mediante xUnit y mocks. |

---

## 📦 Entregables Requeridos para la Defensa
1.  **Código Fuente:** Solución ABP completa compilable (Backend .NET + Frontend Angular).
2.  **Documentación de Diseño:** Diagrama de Clases de Dominio, Casos de Uso y Secuencias (Mermaid/UML).
3.  **Base de Datos:** Script SQL de creación/migración de base de datos.
4.  **Soporte Técnico:** Manual de usuario final y documento técnico que justifique las decisiones de diseño DDD y patrones aplicados.
