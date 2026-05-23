# 📚 Centro de Documentación: TourismTracking

¡Bienvenido al núcleo de conocimiento del proyecto **TourismTracking**! Este directorio centraliza toda la información técnica, académica y organizativa necesaria para la defensa del Trabajo Práctico Final de la materia **Desarrollo de Software (UTN-FRCU, Año 2025)**.

Este espacio está estructurado con formato enriquecido (Markdown) y optimizado para que tú, tu colaborador Exequiel y las inteligencias artificiales de asistencia (**Antigravity**) puedan navegar e interpretar el contexto de desarrollo con absoluta precisión.

---

## 🗺️ Mapa de Documentación

Haz clic en cualquiera de los enlaces para acceder de forma directa a los documentos específicos:

### 🎓 Aspectos Académicos y Requerimientos
*   **[Condiciones de Aprobación 2025](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/Condiciones_Aprobacion_2025.md):** El subconjunto exacto de operaciones de Backend y Frontend requeridas para la promoción de la materia.
*   **[Pliego de Requerimientos TP Final v0.1](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/TP_Final_v0.1.md):** Especificación completa de objetivos, flujos del sistema y tablas detalladas de Requerimientos Funcionales (RF) y No Funcionales (RNF).
*   **[Matriz de Operaciones del Sistema v1.0](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/Operaciones_Sistema_v1.0.md):** Checklist de control detallado de todas las operaciones del sistema de cara al seguimiento del avance de implementación.

### 👥 Onboarding y Colaboración
*   **[Guía de Colaboración Git & IA](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/Guia_Colaboracion_Git_AI.md):** Protocolo de integración continua en local (`dev` branch, rebases y validaciones obligatorias) y protección en producción (`prod` branch, Pull Requests automáticos por IA).
*   **[Hoja de Ruta de Desarrollo (Exequiel)](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/Hoja_Ruta_Desarrollo_Exequiel.md):** Lista de tareas de remediación de Frontend, Backend y Tests con prompts específicos de Antigravity listos para ser ejecutados.
*   **[Guía Maestra de Onboarding (Exequiel)](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/Guia_Onboarding_Exequiel.md):** Manual maestro de integración para configurar y levantar el entorno local de desarrollo de forma automatizada.

### 📐 Arquitectura, UML e Informes Técnicos
*   **[Justificación Arquitectónica y Data Flow](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/Justificacion_Arquitectonica.md):** Reporte exhaustivo que detalla la fundamentación de DDD/ABP, dependencias modulares y diagramas de secuencia de flujo de datos extremo a extremo.
*   **[Diagramas UML & Mermaid](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/diagramas_uml.md):** Modelos visuales que describen el Dominio (Clases), los Casos de Uso, la secuencia de Búsqueda y el comportamiento del Worker periódico de notificaciones.
*   **[Reporte de Auditoría de Brechas y Calidad](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/Reporte_Auditoria_Brechas.md):** Diagnóstico profundo consolidado que detalla las brechas lógicas en Frontend, Backend y Cobertura de Pruebas frente a las condiciones académicas.
*   **[Informe de Cumplimiento Técnico (Anterior)](file:///c:/Users/thiag/Desktop/Files/Facu/FACU%202024/Desarrollo%20de%20Software/Proyecto%20Final/Documentacion/analysis_results.md):** Análisis preliminar que detalla cómo la arquitectura DDD sobre ABP resuelve el 100% de las exigencias teóricas a nivel de servidor.

---

## 🏢 Arquitectura del Proyecto (DDD Layers)

La solución está construida siguiendo los lineamientos de Clean Architecture bajo la metodología DDD (Domain-Driven Design), estructurada en los siguientes directorios clave dentro de `aspnet-core/src/`:

```
TourismTracking/
├── TourismTracking.Domain/             <- Reglas de negocio y entidades puras (Aggregate Roots, Entities).
├── TourismTracking.Domain.Shared/      <- Enums, constantes y clases compartidas por múltiples capas.
├── TourismTracking.Application.Contracts/ <- Interfaces de servicio (AppServices) y DTOs de comunicación.
├── TourismTracking.Application/         <- Lógica e implementación de servicios de aplicación y Workers.
├── TourismTracking.EntityFrameworkCore/<- Infraestructura ORM, DbContext y repositorios concretos.
└── TourismTracking.HttpApi.Host/       <- Servidor Web API principal y punto de arranque del Backend.
```

---

## 🛠️ Buenas Prácticas del Centro de Documentación
1.  **Sincronización:** Cada cambio realizado en la lógica de negocio o en la estructura del código que impacte un requerimiento académico debe verse reflejado en la **Matriz de Operaciones del Sistema** marcando el checkbox correspondiente `[x]`.
2.  **Uso de IA:** Cuando des instrucciones a tu asistente de IA (**Antigravity**), indícale que lea este `README.md` como punto de partida para que sepa exactamente dónde encontrar los requerimientos, diagramas y guías de colaboración.
