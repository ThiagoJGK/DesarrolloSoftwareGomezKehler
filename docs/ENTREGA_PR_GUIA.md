# Guía Operativa de Entrega Formal y Pull Request (`dev` → `prod`)

Este documento detalla el procedimiento obligatorio para formalizar la entrega del **Proyecto Final de Desarrollo de Software (Gomez Kehler)** en GitHub, garantizando los permisos requeridos para la evaluación docente y la correcta integración de ramas.

---

## 1. Requisitos Previos de Cuenta y Permisos

> **ADVERTENCIA CRÍTICA:** El otorgamiento del permiso **`Write`** al docente es un **requisito excluyente** previo al pedido de revisión. Si la cuenta del docente figura únicamente como `Reviewer` sin permiso de escritura (`Write`), GitHub no le permitirá emitir una aprobación formal de merge (`Approve`) ni gestionar el Pull Request en repositorios privados/académicos según las directivas del curso.

### Pasos en la UI web de GitHub

1. Ingresar al repositorio en GitHub: `https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler`
2. Navegar a la pestaña **Settings → Collaborators → Add people** (`Settings` → menú lateral `Collaborators` → botón `Add people`).
3. Buscar el usuario o correo institucional del docente en el cuadro de búsqueda.
4. Seleccionar estrictamente el nivel de permiso **`Write`** (NO dejar en `Read` ni limitarse a asignarlo luego solo como `Reviewer`).
5. Confirmar la invitación haciendo clic en **Add [usuario] to this repository**.
6. Verificar que el docente acepte la invitación (o que su estado figure activo) antes de solicitar la revisión formal del Pull Request.

---

## 2. Sincronización de Ramas de Trabajo

El repositorio utiliza la siguiente estructura de ramas para la entrega:
- **Rama base de desarrollo:** `dev` (`origin/dev`)
- **Rama de producción / entrega:** `prod` (`origin/prod`)
- **Rama de correcciones:** `teamwork-lean/correcciones-docente`

Antes de abrir el Pull Request, asegurarse de que `dev` contenga todos los cambios aprobados (incluyendo el `aspnet-core/NuGet.Config` corregido y la compilación limpia de los 14 proyectos) y esté sincronizada con `origin/dev`:

```powershell
# 1. Actualizar referencias remotas y verificar estado de ramas
git fetch origin
git branch -a

# 2. Cambiar a la rama dev y actualizarla con el remoto
git checkout dev
git pull origin dev

# 3. Integrar los cambios aprobados desde la rama de correcciones
git merge teamwork-lean/correcciones-docente

# 4. Verificar que la solución restaura y compila limpiamente antes de subir
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/verify_clean_restore.ps1

# 5. Subir la rama dev actualizada a origin/dev
git push origin dev
```

*(Opcional)* Verificar que `origin/prod` esté actualizada respecto al estado previo a la entrega (`git log origin/prod -n 3`).

---

## 3. Apertura Formal del Pull Request (PR)

1. En GitHub, ir a la pestaña **Pull requests** → **New pull request**, o ingresar directamente a:
   `https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler/compare/prod...dev`
2. Configurar exactamente las ramas de comparación:
   - **Base branch (`base`):** `prod`
   - **Compare branch (`compare`):** `dev`
3. Ingresar el **Título formal** del PR:
   ```text
   Entrega Final - Proyecto de Software [Gomez Kehler]
   ```
4. Completar la **Descripción del PR** utilizando la siguiente plantilla con el checklist de entrega y referencias al feedback del docente:

```markdown
## Entrega Final - Proyecto de Software [Gomez Kehler]

### Resumen de cambios y correcciones aplicadas
- Corrección y aislamiento de `aspnet-core/NuGet.Config` con `<clear />` y fuente oficial `https://api.nuget.org/v3/index.json` (resolución determinista sin fuentes locales/privadas).
- Validación de restauración y compilación limpia de los 14 proyectos de `aspnet-core/TourismTracking.sln`.
- Inclusión de script de verificación E2E (`scripts/verify_clean_restore.ps1`) y guía formal de entrega (`docs/ENTREGA_PR_GUIA.md`).

### Checklist de Entrega
- [x] **Backend ABP (.NET):** Solución `aspnet-core/TourismTracking.sln` restaurada y compilada sin errores (14 proyectos compilados).
- [x] **Frontend Angular:** Aplicación cliente (`angular/`) integrada y alineada con el backend.
- [x] **NuGet.Config corregido:** Configurado en `aspnet-core/NuGet.Config` con `<clear />` y feed oficial NuGet v3.
- [x] **14 proyectos compilados:** Verificado mediante `dotnet restore --configfile aspnet-core/NuGet.Config` y `dotnet build --no-restore`.
- [x] **Permisos del docente:** Colaborador agregado con permiso `Write` en `Settings → Collaborators → Add people`.

### Referencias y Verificación
- **Rama origen (`compare`):** `dev`
- **Rama destino (`base`):** `prod`
- **Feedback docente atendido:** Aislamiento de dependencias NuGet y formalización del flujo de entrega `dev` → `prod`.
```

5. Hacer clic en **Create pull request**.

---

## 4. Asignación de Revisor (`Reviewer`) y Enlace de Entrega

Una vez creado el Pull Request:

1. En la barra lateral derecha de la vista del PR, ubicar la sección **Reviewers** (hacer clic en el ícono de engranaje o en el selector).
2. Seleccionar la cuenta del docente para asignarlo como **`Reviewer`**.
3. Confirmar que el docente figure listado bajo **Reviewers** (y que previamente cuente con permiso **`Write`** en el repositorio).
4. Copiar la URL directa del PR creado desde la barra de direcciones del navegador (formato: `https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler/pull/<NUMERO_PR>`) o consultar el listado de PRs activos en:
   - **URL directa de PRs:** https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler/pulls
   - **URL directa de creación `dev` → `prod`:** https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler/compare/prod...dev
5. Enviar el enlace directo del PR por el canal de entrega oficial de la cátedra.
