# ✉️ Borrador de Correo para el Prof. Enzo Tanga
## Solicitud de Revisión de Pull Request a Prod — WanderTrack

**Para:** Prof. Enzo Tanga <eftanga@gmail.com> *(o el correo institucional del docente)*  
**De:** Thiago Jesús Gómez Kehler <thiagojgk@gmail.com>  
**Asunto:** `[Desarrollo de Software] Solicitud de revisión de PR a prod para examen final - Thiago Gómez Kehler`  

---

### Cuerpo del Mensaje

Hola Enzo,

Espero que estés muy bien.

Siguiendo lo que me indicaste por correo para poder coordinar la inscripción a la mesa de examen, ya generé el Pull Request desde la rama `dev` hacia `prod` en el repositorio con el estado avanzado del Trabajo Práctico (**WanderTrack - Turismo**).

Te comparto los enlaces y la información de acceso para que puedas revisarlo cuando tengas oportunidad:

* **Repositorio en GitHub:** https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler
* **Pull Request (dev ➔ prod):** https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler/pull/1
* **Informe Técnico & Matriz de Requerimientos:** Se encuentra documentado en detalle dentro del archivo [`Documentacion/REVISION_PREVIA_ACADEMICA.md`](https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler/blob/dev/Documentacion/REVISION_PREVIA_ACADEMICA.md) en el repositorio.

#### 🔑 Credenciales para la navegación interactiva:
* **Usuario de prueba sugerido:** `ThiagoJGK` | **Contraseña:** Configurada vía User Secrets (ver `README.md`)  
  *(Cuenta recomendada: ya cuenta con tablero precargado de destinos favoritos, bitácoras personales editables y bandeja de notificaciones en tiempo real).*
* **Usuario Administrador:** `admin` | **Contraseña:** Configurada vía User Secrets / Inicialización ABP (ver `README.md`)  
  *(Para consultar métricas de consumo de API externa y Swagger UI).*
* **Usuarios de la comunidad:** `lucas.aventura`, `sofia.viajera`, `elena.patagonia` | **Contraseña:** Configurada vía User Secrets (ver `README.md`)  
  *(Para evaluar la navegación de opiniones y perfiles públicos de otros viajeros).*

#### 🛠️ Resumen de lo implementado:
* **Backend (.NET 8 / ABP Framework v8.3.4):** Arquitectura Domain-Driven Design (DDD), 55 pruebas unitarias e integración aprobadas (55/55 Passed) en capas de Dominio, Aplicación y EF Core. Sembrado relacional de datos vía `DbMigrator` con fotos reales y coordenadas geográficas.
* **Frontend (Angular 18):** Búsqueda reactiva de destinos con Open-Meteo, acceso directo a la ficha del destino con calificaciones y reseñas comunitarias, guardado rápido en favoritos, redacción de diarios de viaje con palabras clave, y buzón de notificaciones con marcado de lectura.

Quedo a tu entera disposición por cualquier observación, sugerencia de mejora o ajuste que consideres necesario tras la revisión, para dejarlo listo de cara a la defensa en la mesa de examen.

Saludos cordiales,

**Thiago Jesús Gómez Kehler**  
Alumno de Desarrollo de Software — UTN FRCU  
Legajo / Contacto: thiagojgk@gmail.com
