# 📚 Centro de Documentación: TourismTracking / WanderTrack

¡Bienvenido al repositorio documental del proyecto **TourismTracking (WanderTrack)**! Este directorio centraliza toda la información técnica, de arquitectura y de seguimiento requerida para la defensa del Trabajo Práctico Final de la materia **Desarrollo de Software (UTN-FRCU, Ciclo Lectivo 2025 - Cátedra Prof. Enzo Tanga)**.

Este espacio está organizado en Markdown para facilitar la lectura, el control de versiones y el seguimiento del desarrollo por parte del alumno **Thiago Jesús Gómez Kehler**.

---

## 🗺️ Mapa de Documentación

A continuación se indexan los documentos técnicos y normativos del proyecto:

### 🎓 Aspectos Académicos y Requerimientos
* **[Condiciones de Aprobación 2025](./Condiciones_Aprobacion_2025.md):** El subconjunto exacto de operaciones de Backend y Frontend requeridas para la promoción de la materia.
* **[Pliego de Requerimientos TP Final v0.1](./TP_Final_v0.1.md):** Especificación completa de objetivos, flujos del sistema y tablas detalladas de Requerimientos Funcionales (RF) y No Funcionales (RNF).
* **[Matriz de Operaciones del Sistema v1.0](./Operaciones_Sistema_v1.0.md):** Matriz de trazabilidad y control de avance de todas las operaciones del sistema.
* **[Informe Técnico de Estado Avanzado](./REVISION_PREVIA_ACADEMICA.md):** Documento técnico exhaustivo con la evidencia de pruebas, arquitectura y estado de avance presentado para revisión previa.

### 📐 Arquitectura, UML e Informes Técnicos
* **[Justificación Arquitectónica y Data Flow](./Justificacion_Arquitectonica.md):** Fundamentación del diseño DDD sobre ABP Framework v8.3.4, desacoplamiento modular y diagramas de flujo de datos extremo a extremo.
* **[Diagramas UML & Mermaid](./diagramas_uml.md):** Modelos visuales que describen el Dominio (Diagrama de Clases), Casos de Uso, secuencia de Búsqueda y comportamiento del Worker periódico de notificaciones.
* **[Reporte de Auditoría de Brechas y Calidad](./Reporte_Auditoria_Brechas.md):** Diagnóstico técnico consolidado que detalla las brechas resueltas en Frontend, Backend y Cobertura de Pruebas frente a las condiciones de la cátedra.
* **[Informe de Cumplimiento Técnico](./analysis_results.md):** Análisis de cobertura funcional que detalla cómo la arquitectura DDD sobre ABP resuelve las exigencias de la cátedra.

---

## 🏢 Arquitectura del Proyecto (DDD Layers)

La solución está construida siguiendo los lineamientos de Clean Architecture bajo la metodología DDD (Domain-Driven Design), estructurada en los siguientes proyectos dentro de `aspnet-core/src/`:

```
TourismTracking/
├── TourismTracking.Domain/             <- Reglas de negocio puras, entidades raíz (Destination, Experience) y semillas (IDataSeedContributor).
├── TourismTracking.Domain.Shared/      <- Enums, constantes, tipos compartidos y localización.
├── TourismTracking.Application.Contracts/ <- Interfaces de servicio (AppServices), permisos y DTOs fuertemente tipados.
├── TourismTracking.Application/         <- Casos de uso, servicios de aplicación, mapeos con AutoMapper y Workers de fondo.
├── TourismTracking.EntityFrameworkCore/<- Infraestructura ORM, DbContext relacional, mapeo Fluent API y migraciones.
└── TourismTracking.HttpApi.Host/       <- Servidor Web API REST, autenticación OpenIddict y endpoints para Angular.
```

---

## 🛠️ Buenas Prácticas y Mantenimiento Documental
1. **Sincronización:** Cada cambio realizado en la lógica de negocio o en la estructura del código que impacte un requerimiento académico debe verse reflejado en la **Matriz de Operaciones del Sistema** marcando el checkbox correspondiente `[x]`.
2. **Trazabilidad de Commits:** Utilizar mensajes bajo el estándar Conventional Commits referenciando la sección o funcionalidad impactada.
3. **Validación Cruzada:** Antes de fusionar a la rama `prod`, validar que la documentación se encuentre alineada con el comportamiento real del sistema verificado mediante la suite de pruebas unitarias.
