# Diagramas UML - Tourism Tracking Platform

A continuación se presentan los diagramas fundamentales que describen la arquitectura, el comportamiento y el modelo de dominio de la plataforma, listos para ser adjuntados en la entrega final de la materia.

## 1. Diagrama de Clases (Domain Model)

Muestra las entidades principales del dominio, sus propiedades (obligatorias por DDD) y las relaciones entre ellas.

```mermaid
classDiagram
    class AggregateRoot {
        <<abstract>>
        +Guid Id
    }
    
    class FullAuditedAggregateRoot {
        <<abstract>>
        +Guid? CreatorId
        +DateTime CreationTime
        +Guid? LastModifierId
        +DateTime? LastModificationTime
        +bool IsDeleted
    }

    class Destination {
        +string Name
        +string Country
        +string Region
        +double Latitude
        +double Longitude
        +UpdateDetails(name, country, region)
    }

    class Experience {
        +Guid DestinationId
        +Guid UserId
        +string Title
        +string Content
        +string Keywords
        +UpdateDetails(title, content, keywords)
    }

    class Review {
        +Guid DestinationId
        +Guid UserId
        +int Rating
        +string Comment
        +UpdateReview(rating, comment)
    }

    class FavoriteListItem {
        +Guid DestinationId
        +Guid UserId
    }

    class IdentityUser {
        +Guid Id
        +string UserName
        +string Email
    }

    AggregateRoot <|-- FavoriteListItem
    FullAuditedAggregateRoot <|-- Destination
    FullAuditedAggregateRoot <|-- Experience
    FullAuditedAggregateRoot <|-- Review

    IdentityUser "1" <-- "0..*" Experience : Creates
    IdentityUser "1" <-- "0..*" Review : Writes
    IdentityUser "1" <-- "0..*" FavoriteListItem : Has

    Destination "1" <-- "0..*" Experience : Belongs To
    Destination "1" <-- "0..*" Review : Belongs To
    Destination "1" <-- "0..*" FavoriteListItem : Bookmarks
```

---

## 2. Diagrama de Casos de Uso

Describe las interacciones de los distintos actores (Turista/Usuario y Administrador) con el sistema.

```mermaid
usecaseDiagram
    actor "Turista (Usuario Registrado)" as User
    actor "Administrador" as Admin
    
    package "Módulo de Destinos" {
        usecase "Buscar Destino (GeoDB)" as UC1
        usecase "Guardar Destino Internamente" as UC2
        usecase "Gestionar Favoritos" as UC3
    }
    
    package "Módulo de Interacción" {
        usecase "Publicar Experiencia" as UC4
        usecase "Calificar Destino (1-5)" as UC5
        usecase "Leer Promedio Público" as UC6
    }
    
    package "Módulo de Administración" {
        usecase "Ver Métricas Externas" as UC7
        usecase "Consultar Logs de Auditoría" as UC8
    }

    User --> UC1
    User --> UC2
    User --> UC3
    User --> UC4
    User --> UC5
    User --> UC6
    
    Admin --> UC7
    Admin --> UC8
    Admin --> UC6
    
    UC1 ..> UC2 : <<include>>
```

---

## 3. Diagrama de Secuencia: Búsqueda y Guardado de Destino

Ilustra el flujo arquitectónico en capas (Web -> AppService -> Repositorio/API) cuando un usuario busca y guarda una nueva ciudad.

```mermaid
sequenceDiagram
    actor U as Turista (Angular Web)
    participant API as DestinationAppService
    participant HTTP as HttpClient (GeoDB)
    participant DB as IRepository~Destination~
    
    U->>API: SearchCityAsync("Bariloche")
    activate API
    API->>HTTP: GET /v1/geo/cities?name=Bariloche
    activate HTTP
    HTTP-->>API: JSON Response (City Data)
    deactivate HTTP
    API-->>U: List~DestinationDto~
    deactivate API
    
    U->>API: GetOrSaveDestinationAsync("Bariloche_ID")
    activate API
    API->>DB: FindAsync("Bariloche_ID")
    activate DB
    DB-->>API: null (No existe)
    deactivate DB
    
    API->>HTTP: GET /v1/geo/cities/Bariloche_ID
    activate HTTP
    HTTP-->>API: JSON Detail
    deactivate HTTP
    
    API->>DB: InsertAsync(new Destination(...))
    activate DB
    DB-->>API: Success
    deactivate DB
    
    API-->>U: DestinationDto (Guardado local)
    deactivate API
```

---

## 4. Diagrama de Secuencia: Worker de Notificaciones Diarias

Muestra el proceso en segundo plano (Requisito 7) que cruza los Favoritos con Eventos Externos (TicketMaster).

```mermaid
sequenceDiagram
    participant W as DailyDestinationUpdateWorker
    participant DBFav as IRepository~FavoriteListItem~
    participant API as HttpClient (Eventos)
    participant Mail as IEmailSender (Notificador)

    Note over W: Ejecución Cíclica (Cada 24h)
    activate W
    
    W->>DBFav: GetListAsync()
    activate DBFav
    DBFav-->>W: [ Fav(UserId: 1, Dest: A), Fav(UserId: 2, Dest: A) ]
    deactivate DBFav
    
    loop Por cada destino con fans
        W->>API: Consultar Nuevos Eventos(Destino A)
        activate API
        API-->>W: Eventos Encontrados
        deactivate API
        
        opt Hay Cambios / Eventos
            W->>Mail: SendEmailAsync(User 1, "Novedades en Destino A")
            W->>Mail: SendEmailAsync(User 2, "Novedades en Destino A")
        end
    end
    
    deactivate W
```
