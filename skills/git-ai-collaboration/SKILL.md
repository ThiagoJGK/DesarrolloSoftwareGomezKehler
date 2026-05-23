---
name: git-ai-collaboration
description: Protocolo automatizado para la integración continua local y colaboración segura de agentes de IA en repositorios compartidos de Git sobre la rama dev.
---

# 🚀 Protocolo de Colaboración Git-AI (Versión 1.0)

Este documento es una **Skill** ejecutable y de lectura obligatoria para cualquier agente de inteligencia artificial (incluyendo **Antigravity**) que trabaje en este proyecto. Define la disciplina de Git y los estándares que deben cumplirse para permitir que múltiples humanos y sus respectivas IAs colaboren sobre la rama `dev` sin provocar colisiones ni sobreescrituras destructivas de código.

---

## 🏁 1. Reglas de Oro para la IA
1.  **Validación antes de Push:** La IA tiene prohibido subir cambios a `dev` que no compilen o rompan las pruebas existentes.
2.  **Sincronización Obligatoria:** Siempre se debe ejecutar `git pull --rebase` antes de cualquier integración.
3.  **Documentación Externa:** Se debe priorizar la justificación técnica en documentos Markdown independientes en lugar de saturar el código con comentarios excesivos.
4.  **Uso de Commits Semánticos:** Seguir de forma estricta la convención de commits descriptivos.

---

## 🛠️ 2. Flujo de Trabajo en la Rama `dev` (Paso a Paso)

El agente de IA debe seguir este algoritmo secuencial exacto para cada tarea que le sea asignada:

### Paso 2.1: Sincronización Inicial
Antes de escribir cualquier línea de código, la IA debe sincronizar su espacio de trabajo local con el repositorio remoto:
```powershell
git checkout dev
git pull --rebase origin dev
```

### Paso 2.2: Implementación y Validación Local
Una vez realizados los cambios lógicos, la IA debe validar físicamente la estabilidad de la solución:
*   **Compilar el Backend:**
    ```powershell
    dotnet build aspnet-core/TourismTracking.sln
    ```
*   **Ejecutar la suite de pruebas unitarias:**
    ```powershell
    dotnet test aspnet-core/TourismTracking.sln
    ```
*   **Compilar el Frontend (si aplica):**
    ```powershell
    cd angular; npm run build; cd ..
    ```
> [!CAUTION]
> **Abortar si hay Errores:** Si alguno de los comandos anteriores falla, la IA debe abortar el proceso de integración, diagnosticar la falla en el entorno local, corregir el bug y volver al Paso 2.2. **NUNCA se debe subir código roto a `dev`.**

### Paso 2.3: Integración de Cambios Remotos
Inmediatamente antes de subir sus cambios, la IA debe verificar si el colaborador subió código nuevo a la rama:
```powershell
git pull --rebase origin dev
```
*   **Si no hay conflictos:** Continuar al Paso 2.4.
*   **Si hay conflictos:** 
    1.  Parsear los bloques en conflicto (`<<<<<<<`, `=======`, `>>>>>>>`).
    2.  Analizar el histórico de Git (`git log -n 5`) para entender la intención del colaborador.
    3.  Resolver el conflicto de manera lógica (fusión de funcionalidades, no borrado ciego).
    4.  Regresar al **Paso 2.2** para re-validar que la compilación y los tests siguen pasando tras la resolución.

### Paso 2.4: Creación del Commit Semántico
La IA debe realizar el commit utilizando la nomenclatura semántica (Conventional Commits):
*   `feat(...)`: Nueva funcionalidad.
*   `fix(...)`: Corrección de un bug.
*   `docs(...)`: Cambios en la documentación externa.
*   `style(...)`: Formateo de código sin cambios lógicos.
*   `refactor(...)`: Mejora interna de la estructura de código.
*   `test(...)`: Adición de pruebas unitarias.

Ejemplo:
```powershell
git add .
git commit -m "feat(destination): cache popular destinations to local database"
```

### Paso 2.5: Push Remoto
Subir los cambios a la rama compartida:
```powershell
git push origin dev
```

---

## 🔒 3. Protocolo de Despliegue en Producción (`prod`)

La rama de entrega académica `prod` se encuentra protegida contra empujes directos. La IA debe automatizar su promoción mediante los siguientes pasos en su entorno local cuando el desarrollador humano lo solicite:

1.  **Chequeo de Integridad Final:**
    ```powershell
    git checkout dev
    git pull
    dotnet build aspnet-core/TourismTracking.sln
    dotnet test aspnet-core/TourismTracking.sln
    ```
2.  **Creación de Pull Request en GitHub:**
    *   La IA utilizará la API de integración de GitHub para crear un Pull Request desde la rama `dev` hacia la rama `prod`.
    *   El título del PR debe seguir el formato: `release: Delivery Milestone [Número/Fecha]`.
    *   El cuerpo del PR debe incluir un **Changelog Académico** estructurado con:
        *   Funcionalidades agregadas y justificadas.
        *   Evidencia técnica de que las pruebas unitarias pasaron al 100%.
        *   Matriz de operaciones impactadas.

---

## 📝 4. Justificación Técnica y Documentación

Teniendo en cuenta que el proyecto será defendido ante la cátedra, cada decisión técnica relevante (ej. estructuración de entidades, queries complejas o jobs en segundo plano) debe ser registrada de forma meticulosa.

*   **Sin Bloat de Código:** Dejar las clases limpias, legibles y descriptivas (Clean Code).
*   **Markdown Externos:** Registrar las justificaciones de diseño en archivos Markdown dentro del directorio `/Documentacion` (ej. diagramas de secuencia de flujos de datos, justificación del uso de Aggregate Roots, etc.).
