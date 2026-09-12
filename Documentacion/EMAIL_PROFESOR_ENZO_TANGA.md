# ✉️ Borrador Formal de Correo para el Prof. Enzo Tanga
## Entrega Final de Trabajo Práctico — WanderTrack

**Para:** Prof. Enzo Tanga <docente.desarrollosoftware@frcu.utn.edu.ar>  
**De:** Thiago Jesús Gómez Kehler <thiagojgk@wander-track.com> / Exequiel  
**Asunto:** `[Desarrollo de Software 2025] Entrega Final de Trabajo Práctico - WanderTrack (Gómez Kehler / Exequiel)`  

---

### Cuerpo del Mensaje

Estimado Profesor Enzo Tanga,

Esperamos que se encuentre muy bien.

Nos dirigimos a usted con el agrado de presentarle la entrega formal y completa del Trabajo Práctico Final de la materia **Desarrollo de Software** (ciclo lectivo 2025), denominado **"WanderTrack - Plataforma de Monitoreo y Recomendación Turística"**.

La solución ha sido desarrollada en estricto apego a las directivas arquitectónicas y funcionales de la cátedra, implementando Domain-Driven Design (DDD) sobre .NET 8 con ABP Framework v8.3.4 en el backend, y una interfaz de usuario interactiva y modular construida en Angular 18 con diseño responsivo.

#### 📌 Cumplimiento de Requerimientos y Evidencia de Calidad:
1. **Requerimientos Funcionales (RF 1 al RF 8):** Se ha alcanzado el **100% de cobertura** en todas las operaciones del sistema solicitadas en el pliego de condiciones de aprobación, abarcando gestión de usuarios, perfiles públicos de viajeros, geocodificación y persistencia local (patrón Write-Through) con Open-Meteo, bitácoras de viaje con control estricto de autoría, calificaciones ponderadas con sentimiento de reseñas, favoritos reactivos, notificaciones asíncronas periódicas y monitoreo administrativo.
2. **Pruebas Unitarias Automatizadas:** La solución cuenta con una batería de **55 pruebas unitarias e integración** que certifican el 100% de aprobación sin fallos (**55/55 Passed**) distribuidas en las capas de Dominio, Aplicación y Entity Framework Core.
3. **Compilación de Producción:** El empaquetado del frontend en Angular compila de manera limpia con **0 advertencias y 0 errores** mediante `npm run build`.
4. **Sembrado Relacional Idempotente:** Se incluye un contributor de base de datos (`DbMigrator`) que puebla 8 destinos emblemáticos con fotos en alta definición, 32 reseñas comunitarias, 8 crónicas de viaje, perfiles de usuarios auténticos y estados iniciales completos.

#### 🔗 Enlaces al Repositorio y Pull Request de Entrega:
* **Repositorio GitHub:** https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler
* **Pull Request Formal de Entrega:** https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler/pull/new/dev (Rama `dev` hacia `prod`)
* **Documento Completo de Entrega y Changelog:** Archivo `docs/ENTREGA_FINAL_ACADEMICA.md` en el repositorio.

#### 🔑 Credenciales Sugeridas para la Evaluación Interactiva:
* **Usuario de evaluación activa:** `ThiagoJGK` | **Contraseña:** `Thiago123*`  
  *(Cuenta recomendada: cuenta con tablero precargado de destinos favoritos, bitácoras propias editables/eliminables y bandeja de notificaciones en vivo).*
* **Usuario administrador:** `admin` | **Contraseña:** `1q2w3E*`  
  *(Acceso a módulos de administración y Swagger UI).*
* **Usuarios de la comunidad:** `lucas.aventura`, `sofia.viajera`, `elena.patagonia`, `martin.turismo` | **Contraseña:** `Comunidad2024*`  
  *(Para evaluar la navegación de perfiles públicos y autoría de opiniones).*

Quedamos a su entera disposición para coordinar la fecha de defensa del proyecto ante la cátedra y atender cualquier consulta técnica sobre la solución.

Agradeciéndole cordialmente por su acompañamiento docente durante el cursado de la materia, lo saludamos con nuestra consideración más distinguida.

Atentamente,

**Thiago Jesús Gómez Kehler & Exequiel**  
Estudiantes de Desarrollo de Software — UTN FRCU
