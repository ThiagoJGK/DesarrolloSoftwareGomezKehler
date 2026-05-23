# 🌍 Guía Maestra de Onboarding: Proyecto TourismTracking
**Bienvenido, Exequiel.** 

Este documento es una guía exhaustiva diseñada para alguien que se incorpora desde cero. Aquí desglosamos la arquitectura, las funcionalidades y los pasos técnicos para que puedas operar el sistema hoy mismo de la mano de **Antigravity**.

---

## 1. ¿Qué es TourismTracking? (Contexto)
TourismTracking es una plataforma web profesional que centraliza la búsqueda de destinos turísticos, permite el seguimiento de ciudades de interés y fomenta una comunidad de viajeros mediante reseñas y experiencias compartidas.

### Explicación de los "Hitos" (Milestones)
El proyecto se dividió en fases lógicas para asegurar una arquitectura robusta:
*   **Hito 1: Cimientos y Arquitectura**: Configuración de **ABP.IO**, un framework basado en **Domain-Driven Design (DDD)**. Se definió la estructura de capas (Dominio, Aplicación, Infraestructura y Web).
*   **Hito 2: Identidad y Seguridad**: Implementación de la gestión de usuarios, roles (Admin/Usuario) y autenticación segura mediante JWT.
*   **Hito 3: Integración Geográfica**: Conexión con la API externa **GeoDB Cities**. Permite buscar ciudades reales en todo el mundo y persistir sus datos técnicos en nuestra base local.
*   **Hito 4: Interacción Social**: Creación de los sistemas de **Experiencias** (blogs de viajes), **Reviews** (calificaciones 1-5 estrellas) y **Favoritos**.
*   **Hito 5: Inteligencia y Notificaciones**: Desarrollo de un **Worker (Job de fondo)** que se ejecuta cada 24 horas para revisar cambios en destinos favoritos y preparar notificaciones.

---

## 2. Configuración del Entorno con Antigravity

Como ya tienes **Antigravity** instalado, no necesitas realizar las instalaciones manualmente. Simplemente copia y pega el siguiente prompt en tu chat de Antigravity para que él se encargue de preparar todo por ti.

### 🚀 Prompt de Configuración Automática
> [!TIP]
> **Copia el texto de abajo y pégalo en Antigravity:**
>
> "Hola Antigravity, soy Exequiel y me estoy incorporando al proyecto TourismTracking. Por favor, ayúdame a configurar mi entorno de desarrollo desde cero en esta máquina. Realiza las siguientes tareas:
> 1. **Recuperar Proyecto**: Clona el repositorio `https://github.com/ThiagoJGK/DesarrolloSoftwareGomezKehler.git` en una carpeta de mi elección.
> 2. **Verificar Dependencias**: Revisa si tengo instalados .NET 8 SDK, Node.js (LTS) y SQL Server. Si falta algo, indícame el link de descarga o ayúdame a instalarlo.
> 3. **Instalar ABP CLI**: Ejecuta el comando `dotnet tool install -g Volo.Abp.Cli` para instalar la herramienta global de ABP.
> 4. **Base de Datos**: Ejecuta el proyecto `TourismTracking.DbMigrator` dentro de `aspnet-core/src` para crear la base de datos local y cargar los datos iniciales.
> 5. **Frontend**: Entra a la carpeta `angular`, ejecuta `npm install` para las dependencias y luego prepárame el comando para iniciar el sistema."

---

## 3. Catálogo Exhaustivo de Funcionalidades
Actualmente, el sistema cuenta con las siguientes capacidades operativas:

### 👤 Gestión de Identidad
*   **Seguridad**: Encriptación de contraseñas y manejo de sesiones.
*   **Perfiles**: Cada usuario puede editar su bio y preferencias de notificación.
*   **Roles**: Los Administradores pueden gestionar a otros usuarios.

### 🗺️ Exploración de Destinos
*   **Búsqueda Global**: Filtra por nombre, país o población mínima consumiendo datos de GeoDB.
*   **Caché Inteligente**: Cuando un usuario interactúa con una ciudad, esta se guarda en nuestra DB para acceso rápido.

### ✍️ Comunidad de Viajeros
*   **Experiencias**: Un muro donde los usuarios redactan relatos de sus viajes con etiquetas.
*   **Reseñas y Rating**: Sistema de 1 a 5 estrellas. El sistema calcula automáticamente el promedio de satisfacción de cada destino.
*   **Favoritos**: Botón de "Seguir" para añadir destinos a una lista personal.

---

## 4. Cómo Ejecutar el Sistema (Una vez configurado)

Una vez que Antigravity termine la configuración, estos son tus comandos diarios:

1.  **Backend**: 
    ```bash
    cd aspnet-core/src/TourismTracking.HttpApi.Host
    dotnet run
    ```
2.  **Frontend**: 
    ```bash
    cd angular
    npm start
    ```
    *Abre tu navegador en `http://localhost:4200`.*

---

## 5. Guía de Pruebas (Verification Flow)
Para confirmar que todo funciona, realiza este recorrido:

1.  **Login**: Usa el usuario `admin` y la contraseña `1q2w3E*`.
2.  **Buscar**: Ve al buscador, escribe "Paris" y presiona Enter.
3.  **Seguir**: Entra al detalle y presiona el corazón de favoritos.
4.  **Reseñar**: Deja una calificación de 5 estrellas.
5.  **Verificar**: Comprueba que el destino aparece ahora en tu dashboard personal.

---
> [!IMPORTANT]
> **Tu Próximo Desafío**: Tu prioridad será ayudarnos a migrar el sistema de notificaciones de un estado "simulado" a un envío de correos electrónicos real (SMTP).

**¡Bienvenido al equipo, Exequiel!**
