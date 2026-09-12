using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourismTracking.Destinations;
using TourismTracking.Experiences;
using TourismTracking.Notifications;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace TourismTracking.Data
{
    public class TourismDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Destination, Guid> _destinationRepository;
        private readonly IRepository<Review, Guid> _reviewRepository;
        private readonly IRepository<Experience, Guid> _experienceRepository;
        private readonly IRepository<FavoriteListItem, Guid> _favoriteRepository;
        private readonly IRepository<Notification, Guid> _notificationRepository;
        private readonly IRepository<IdentityUser, Guid> _userRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IdentityUserManager _identityUserManager;

        public TourismDataSeedContributor(
            IRepository<Destination, Guid> destinationRepository,
            IRepository<Review, Guid> reviewRepository,
            IRepository<Experience, Guid> experienceRepository,
            IRepository<FavoriteListItem, Guid> favoriteRepository,
            IRepository<Notification, Guid> notificationRepository,
            IRepository<IdentityUser, Guid> userRepository,
            IGuidGenerator guidGenerator,
            IdentityUserManager identityUserManager)
        {
            _destinationRepository = destinationRepository;
            _reviewRepository = reviewRepository;
            _experienceRepository = experienceRepository;
            _favoriteRepository = favoriteRepository;
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
            _guidGenerator = guidGenerator;
            _identityUserManager = identityUserManager;
        }

        [UnitOfWork]
        public virtual async Task SeedAsync(DataSeedContext context)
        {
            // 1. Seed / Ensure Community Users and Active User (ThiagoJGK)
            var lucasUser = await EnsureUserAsync(
                context,
                "lucas.aventura",
                "Lucas",
                "Benítez",
                "lucas.aventura@wander-track.com",
                "Comunidad2024*",
                "https://api.dicebear.com/7.x/avataaars/svg?seed=lucas.aventura",
                "Amante del trekking, escalada y naturaleza"
            );

            var sofiaUser = await EnsureUserAsync(
                context,
                "sofia.viajera",
                "Sofía",
                "Martínez",
                "sofia.viajera@wander-track.com",
                "Comunidad2024*",
                "https://api.dicebear.com/7.x/avataaars/svg?seed=sofia.viajera",
                "Fotografía de viajes y turismo histórico"
            );

            var elenaUser = await EnsureUserAsync(
                context,
                "elena.patagonia",
                "Elena",
                "Rossi",
                "elena.patagonia@wander-track.com",
                "Comunidad2024*",
                "https://api.dicebear.com/7.x/avataaars/svg?seed=elena.patagonia",
                "Rutas gastronómicas y enoturismo regional"
            );

            var martinUser = await EnsureUserAsync(
                context,
                "martin.turismo",
                "Martín",
                "Albarracín",
                "martin.turismo@wander-track.com",
                "Comunidad2024*",
                "https://api.dicebear.com/7.x/avataaars/svg?seed=martin.turismo",
                "Ecoturismo y circuitos culturales"
            );

            var thiagoUser = await EnsureUserAsync(
                context,
                "ThiagoJGK",
                "Thiago",
                "Gómez Kehler",
                "thiagojgk@wander-track.com",
                "Thiago123*",
                "https://api.dicebear.com/7.x/avataaars/svg?seed=ThiagoJGK",
                "Aventuras al aire libre, fotografía paisajística y senderismo de alta montaña"
            );

            var adminUser = await _identityUserManager.FindByNameAsync("admin") 
                            ?? await _userRepository.FirstOrDefaultAsync(u => u.UserName == "admin");
            var adminId = adminUser?.Id ?? _guidGenerator.Create();

            var communityUsers = new[] { lucasUser, sofiaUser, elenaUser, martinUser }
                .Where(u => u != null)
                .ToArray();

            // 2. Destinations and Review/Experience Data Definition
            var destinations = new List<(string Name, string Country, long Pop, double Lat, double Lng, string ImageUrl, List<(int Stars, string Comment, string UserTag)> Reviews, (string Title, string Story, string Tags) Experience)>
            {
                (
                    "Mendoza",
                    "Provincia de Mendoza, Argentina",
                    115041,
                    -32.8908,
                    -68.8272,
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/d/d4/Monumento_al_Ej%C3%A9rcito_de_Los_Andes_-_Cerro_de_La_Gloria_-_Mendoza_02.jpg/800px-Monumento_al_Ej%C3%A9rcito_de_Los_Andes_-_Cerro_de_La_Gloria_-_Mendoza_02.jpg",
                    new List<(int, string, string)>
                    {
                        (5, "Increíble experiencia recorriendo las bodegas en el Valle de Uco con la cordillera de fondo.", "Martín Rossi"),
                        (5, "El rafting en Potrerillos y la vista del Cerro de la Gloria son imperdibles.", "Lucía Fernández"),
                        (5, "Excelente oferta gastronómica y vinos de primer nivel mundial.", "Esteban Gomez"),
                        (4, "La ciudad es muy limpia y arbolada. Recomiendo visitarla en época de vendimia.", "Valeria Paz")
                    },
                    ("Cata de Vinos y Aventura en Mendoza", "Recorrimos las bodegas de Luján de Cuyo y terminamos la jornada con cabalgata al atardecer frente al Cordón del Plata.", "gastronomia, enoturismo, cordillera")
                ),
                (
                    "San Carlos de Bariloche",
                    "Río Negro, Argentina",
                    112887,
                    -41.1335,
                    -71.3103,
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/8/87/Catedral_de_Bariloche_2.jpg/800px-Catedral_de_Bariloche_2.jpg",
                    new List<(int, string, string)>
                    {
                        (5, "Subir al Cerro Campanario y disfrutar de la vista 360 del Nahuel Huapi es una postal única.", "Santiago Diaz"),
                        (5, "Los chocolates artesanales y el cordero patagónico son espectaculares.", "Camila Benitez"),
                        (5, "Paseo en barco a la Isla Victoria y el Bosque de Arrayanes totalmente recomendado.", "Joaquín Alvarez"),
                        (4, "Excelente infraestructura turística y excursiones para toda la familia.", "Mariana Lopez")
                    },
                    ("Travesía de Trekking en los Cerros de Bariloche", "Caminata inolvidable hacia el Refugio Frey bordeando la Laguna Toncek entre agujas de granito.", "trekking, naturaleza, lagos")
                ),
                (
                    "Ushuaia",
                    "Tierra del Fuego, Argentina",
                    82615,
                    -54.8019,
                    -68.3030,
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/5/52/Ushuaia_-_Bahia_Encerrada.jpg/800px-Ushuaia_-_Bahia_Encerrada.jpg",
                    new List<(int, string, string)>
                    {
                        (5, "Navegar por el Canal Beagle y ver el Faro Les Éclaireurs con leones marinos es mágico.", "Facundo Morales"),
                        (5, "El Tren del Fin del Mundo y el Parque Nacional Tierra del Fuego son de visita obligada.", "Agustina Ramos"),
                        (5, "La centolla fueguina es un manjar inigualable.", "Nicolás Gimenez"),
                        (4, "Ciudad pintoresca y única. Mucho viento, llevar abrigo técnico adecuado.", "Elena Torres")
                    },
                    ("Exploración al Fin del Mundo", "Día completo de navegación austral por el Canal Beagle y visita a la pingüinera en Isla Martillo.", "austral, glaciares, fauna")
                ),
                (
                    "Puerto Iguazú",
                    "Misiones, Argentina",
                    82000,
                    -25.6953,
                    -54.4367,
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/1/15/Cataratas_del_Iguaz%C3%BA%2C_Argentina_01.jpg/800px-Cataratas_del_Iguaz%C3%BA%2C_Argentina_01.jpg",
                    new List<(int, string, string)>
                    {
                        (5, "La Garganta del Diablo te deja sin palabras. El poder del agua es sobrecogedor.", "Gonzalo Varela"),
                        (5, "Recomiendo hacer el paseo náutico de la Gran Aventura bajo los saltos.", "Paula Cabrera"),
                        (5, "La selva paranaense y la diversidad de tucanes y mariposas son fascinantes.", "Matías Romero"),
                        (4, "El parque está muy bien señalizado y accesible mediante el tren ecológico.", "Daniela Ortiz")
                    },
                    ("Sendero Macuco y Garganta del Diablo", "Caminata por la selva subtropical misionera y experiencia inmersiva frente al salto más imponente del planeta.", "cataratas, selva, aventura")
                ),
                (
                    "Salta",
                    "Provincia de Salta, Argentina",
                    535303,
                    -24.7859,
                    -65.4117,
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/c/cb/Catedral_de_Salta_al_atardecer.jpg/800px-Catedral_de_Salta_al_atardecer.jpg",
                    new List<(int, string, string)>
                    {
                        (5, "La arquitectura colonial, las peñas folklóricas y las empanadas salteñas son inmejorables.", "Emiliano Vega"),
                        (5, "El Tren a las Nubes cruzando el viaducto La Polvorilla a más de 4.200 msnm es inolvidable.", "Cecilia Medina"),
                        (5, "La Quebrada de las Conchas rumbo a Cafayate tiene formaciones rocosas rojizas de ensueño.", "Rodrigo Herrera"),
                        (4, "Hermosa ciudad conocida con justicia como 'La Linda'. Clima agradable todo el año.", "Florencia Soria")
                    },
                    ("Ruta del Vino y Quebrada de las Flechas", "Recorrido por los Valles Calchaquíes con degustación de Torrontés de altura en Cafayate.", "cultura, tradicion, paisajes")
                ),
                (
                    "París",
                    "Île-de-France, Francia",
                    2161000,
                    48.8566,
                    2.3522,
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/4/4b/La_Tour_Eiffel_vue_de_la_Tour_Saint-Jacques%2C_Paris_mai_2014_%282%29.jpg/800px-La_Tour_Eiffel_vue_de_la_Tour_Saint-Jacques%2C_Paris_mai_2014_%282%29.jpg",
                    new List<(int, string, string)>
                    {
                        (5, "Caminar por Montmartre y ver el atardecer junto a la Torre Eiffel iluminada supera cualquier expectativa.", "Lucas Mendez"),
                        (5, "El Museo del Louvre y Orsay albergan las obras maestras más trascendentales de la historia.", "Sofía Carrizo"),
                        (4, "La pastelería francesa y los bistrós parisinos en Le Marais son excepcionales.", "Andrés Nuñez"),
                        (4, "Excelente red de metro para moverse rápidamente por toda la ciudad.", "Julieta Roldan")
                    },
                    ("Paseo Cultural por el Sena", "Caminata histórica desde la Catedral de Notre-Dame hasta el Puente Alejandro III al caer la noche.", "arte, historia, arquitectura")
                ),
                (
                    "Roma",
                    "Lazio, Italia",
                    2873000,
                    41.9028,
                    12.4964,
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/d/de/Colosseo_2020.jpg/800px-Colosseo_2020.jpg",
                    new List<(int, string, string)>
                    {
                        (5, "Estar dentro del Coliseo y caminar por el Foro Romano te transporta milenios atrás.", "Federico Rios"),
                        (5, "La Fontana di Trevi y el Panteón de Agripa tienen una belleza arquitectónica inigualable.", "Bárbara Ponce"),
                        (5, "La pasta carbonara auténtica y el gelato italiano en Trastevere son insuperables.", "Cristian Molina"),
                        (4, "Museo a cielo abierto. Conviene reservar entradas con anticipación para los Museos Vaticanos.", "Noelia Bravo")
                    },
                    ("Crónica por la Roma Imperial", "Recorrido a pie por las siete colinas explorando las ruinas clásicas y los templos del Imperio Romano.", "antiguedad, gastronomia, monumentos")
                ),
                (
                    "Tokio",
                    "Kanto, Japón",
                    13960000,
                    35.6762,
                    139.6503,
                    "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b2/Skyscrapers_of_Shinjuku_2009_January.jpg/800px-Skyscrapers_of_Shinjuku_2009_January.jpg",
                    new List<(int, string, string)>
                    {
                        (5, "El cruce de Shibuya, los templos milenarios de Asakusa y la tecnología de Akihabara forman un contraste único.", "Guillermo Luna"),
                        (5, "La gastronomía es de otro nivel: ramen artesanal, sushi fresco y cortes de wagyu.", "Romina Ibarra"),
                        (5, "El sistema de transporte público y la seguridad son impecables y asombrosos.", "Tomás Peralta"),
                        (4, "Una metrópolis fascinante que nunca duerme. Las vistas desde Tokyo Skytree son infinitas.", "Patricia Arce")
                    },
                    ("Contrastes de Tradición y Futuro en Tokio", "Visita matutina al Santuario Meiji entre bosques de cedro y tarde inmersiva en los rascacielos de Shinjuku.", "vanguardia, cultura, metropolis")
                )
            };

            // 3. Idempotently Seed Destinations
            var destinationMap = new Dictionary<string, Destination>();

            foreach (var item in destinations)
            {
                var destination = await _destinationRepository.FirstOrDefaultAsync(d => d.Name == item.Name);
                if (destination == null)
                {
                    destination = new Destination(
                        _guidGenerator.Create(),
                        item.Name,
                        item.Country,
                        item.Pop,
                        item.Lat,
                        item.Lng,
                        item.ImageUrl,
                        DateTime.UtcNow
                    );
                    await _destinationRepository.InsertAsync(destination);
                }
                destinationMap[item.Name] = destination;
            }

            // 4. Idempotently Seed 32 Reviews Distributed across Community Users
            int reviewIndex = 0;
            foreach (var item in destinations)
            {
                var destination = destinationMap[item.Name];
                foreach (var rev in item.Reviews)
                {
                    var authorId = communityUsers.Length > 0
                        ? communityUsers[reviewIndex % communityUsers.Length].Id
                        : adminId;
                    reviewIndex++;

                    var reviewExists = await _reviewRepository.AnyAsync(r => r.DestinationId == destination.Id && r.Comment == rev.Comment);
                    if (!reviewExists)
                    {
                        var review = new Review(
                            _guidGenerator.Create(),
                            destination.Id,
                            authorId,
                            rev.Stars,
                            rev.Comment
                        );
                        await _reviewRepository.InsertAsync(review);
                    }
                }
            }

            // 5. Idempotently Seed 8 Destination Diaries (Experiences) Distributed across Community Users
            int expIndex = 0;
            foreach (var item in destinations)
            {
                var destination = destinationMap[item.Name];
                var authorId = communityUsers.Length > 0
                    ? communityUsers[expIndex % communityUsers.Length].Id
                    : adminId;
                expIndex++;

                var expExists = await _experienceRepository.AnyAsync(e => e.DestinationId == destination.Id && e.Title == item.Experience.Title);
                if (!expExists)
                {
                    var experience = new Experience(
                        _guidGenerator.Create(),
                        destination.Id,
                        authorId,
                        item.Experience.Title,
                        item.Experience.Story,
                        item.Experience.Tags
                    );
                    await _experienceRepository.InsertAsync(experience);
                }
            }

            // 6. Idempotently Seed Active User (ThiagoJGK) Data
            if (thiagoUser != null)
            {
                // 3-4 Favorite Destinations
                var favoriteNames = new[] { "Mendoza", "San Carlos de Bariloche", "Ushuaia", "París" };
                foreach (var name in favoriteNames)
                {
                    if (destinationMap.TryGetValue(name, out var dest))
                    {
                        var favExists = await _favoriteRepository.AnyAsync(f => f.DestinationId == dest.Id && f.UserId == thiagoUser.Id);
                        if (!favExists)
                        {
                            var fav = new FavoriteListItem(_guidGenerator.Create(), dest.Id, thiagoUser.Id);
                            await _favoriteRepository.InsertAsync(fav);
                        }
                    }
                }

                // 2-3 Owned Experiences for Edit/Delete Testing in Mi Tablero
                var thiagoExperiences = new List<(string DestName, string Title, string Story, string Tags)>
                {
                    (
                        "Mendoza",
                        "Travesía por los Viñedos del Valle de Uco",
                        "Pedaleamos entre viñedos centenarios de Tupungato con vista directa al Cordón del Plata. Cerramos la jornada con una degustación exclusiva al atardecer y cata guiada.",
                        "enoturismo, mendoza, cordillera, vinos"
                    ),
                    (
                        "San Carlos de Bariloche",
                        "Ascenso y Noche en el Refugio Frey",
                        "Iniciamos la caminata desde la base del cerro Catedral. El sendero por el bosque de lengas y la llegada a la laguna Toncek con las agujas graníticas nevadas superaron toda expectativa.",
                        "trekking, patagonia, refugio, lagos"
                    ),
                    (
                        "Ushuaia",
                        "Navegación Austral por el Canal Beagle",
                        "Zarpamos desde el muelle turístico hacia el Faro Les Éclaireurs con avistaje de lobos marinos de dos pelos y cormoranes imperiales en un día diáfano inolvidable.",
                        "austral, fin-del-mundo, faro, glaciares"
                    )
                };

                foreach (var tExp in thiagoExperiences)
                {
                    if (destinationMap.TryGetValue(tExp.DestName, out var dest))
                    {
                        var expExists = await _experienceRepository.AnyAsync(e => e.UserId == thiagoUser.Id && e.Title == tExp.Title);
                        if (!expExists)
                        {
                            var exp = new Experience(
                                _guidGenerator.Create(),
                                dest.Id,
                                thiagoUser.Id,
                                tExp.Title,
                                tExp.Story,
                                tExp.Tags
                            );
                            await _experienceRepository.InsertAsync(exp);
                        }
                    }
                }

                // 4 Notifications in AppNotifications (2 unread, 2 read)
                var thiagoNotifications = new List<(string Title, string Message, bool Read)>
                {
                    (
                        "Bienvenido a WanderTrack, Thiago",
                        "Tu cuenta está configurada y lista para explorar destinos, interactuar con la comunidad y gestionar tus bitácoras de viaje.",
                        false
                    ),
                    (
                        "Alerta de eventos en Bariloche",
                        "Se ha registrado una nueva travesía de alta montaña y condiciones meteorológicas ideales para senderismo en tu destino favorito.",
                        false
                    ),
                    (
                        "Actualización de tu favorito: Mendoza",
                        "Se han sincronizado las fechas oficiales de la Fiesta Nacional de la Vendimia y apertura de nuevos circuitos de enoturismo.",
                        true
                    ),
                    (
                        "Bitácora publicada en la comunidad",
                        "Tu crónica 'Travesía por los Viñedos del Valle de Uco' ya está visible para todos los viajeros de la plataforma.",
                        true
                    )
                };

                foreach (var notif in thiagoNotifications)
                {
                    var notifExists = await _notificationRepository.AnyAsync(n => n.UserId == thiagoUser.Id && n.Title == notif.Title);
                    if (!notifExists)
                    {
                        var notification = new Notification(
                            _guidGenerator.Create(),
                            thiagoUser.Id,
                            notif.Title,
                            notif.Message
                        );
                        if (notif.Read)
                        {
                            notification.MarkAsRead();
                        }
                        await _notificationRepository.InsertAsync(notification);
                    }
                }
            }

            // 7. Also Seed Favorites & Notifications for Admin Demo User (idempotent fallback)
            if (adminUser != null)
            {
                var adminFavoriteNames = new[] { "Mendoza", "San Carlos de Bariloche", "Puerto Iguazú" };
                foreach (var name in adminFavoriteNames)
                {
                    if (destinationMap.TryGetValue(name, out var dest))
                    {
                        var favExists = await _favoriteRepository.AnyAsync(f => f.DestinationId == dest.Id && f.UserId == adminId);
                        if (!favExists)
                        {
                            var fav = new FavoriteListItem(_guidGenerator.Create(), dest.Id, adminId);
                            await _favoriteRepository.InsertAsync(fav);
                        }
                    }
                }

                var adminNotifications = new List<(string Title, string Message, bool Read)>
                {
                    ("Bienvenido a WanderTrack", "Tu cuenta de administración ha sido configurada con éxito. Ya puedes explorar y registrar destinos en tu bitácora personal.", false),
                    ("Nuevo evento en tu destino favorito: Mendoza", "Se ha registrado la Fiesta Nacional de la Vendimia para las fechas de tu viaje planificado.", false),
                    ("Nueva reseña destacada en Bariloche", "Un viajero ha calificado con 5 estrellas el recorrido por el Cerro Campanario.", false),
                    ("Actualización de métricas geográficas", "Los datos de población y georreferencia han sido sincronizados con las bases cartográficas oficiales.", true)
                };

                foreach (var notif in adminNotifications)
                {
                    var notifExists = await _notificationRepository.AnyAsync(n => n.UserId == adminId && n.Title == notif.Title);
                    if (!notifExists)
                    {
                        var notification = new Notification(
                            _guidGenerator.Create(),
                            adminId,
                            notif.Title,
                            notif.Message
                        );
                        if (notif.Read)
                        {
                            notification.MarkAsRead();
                        }
                        await _notificationRepository.InsertAsync(notification);
                    }
                }
            }
        }

        private async Task<IdentityUser> EnsureUserAsync(
            DataSeedContext context,
            string userName,
            string name,
            string surname,
            string email,
            string password,
            string avatarUrl,
            string preferences)
        {
            var user = await _identityUserManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new IdentityUser(_guidGenerator.Create(), userName, email, context.TenantId)
                {
                    Name = name,
                    Surname = surname
                };
                user.SetProperty("Photo", avatarUrl);
                user.SetProperty("Preferences", preferences);
                var result = await _identityUserManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    user = await _identityUserManager.FindByEmailAsync(email)
                           ?? await _identityUserManager.FindByNameAsync(userName);
                }
            }
            else
            {
                // Ensure password matches the required seed password
                await _identityUserManager.RemovePasswordAsync(user);
                await _identityUserManager.AddPasswordAsync(user, password);

                bool modified = false;
                if (string.IsNullOrWhiteSpace(user.Name) || user.Name != name)
                {
                    user.Name = name;
                    modified = true;
                }
                if (string.IsNullOrWhiteSpace(user.Surname) || user.Surname != surname)
                {
                    user.Surname = surname;
                    modified = true;
                }
                if (user.GetProperty<string>("Photo") != avatarUrl)
                {
                    user.SetProperty("Photo", avatarUrl);
                    modified = true;
                }
                if (user.GetProperty<string>("Preferences") != preferences)
                {
                    user.SetProperty("Preferences", preferences);
                    modified = true;
                }
                if (modified)
                {
                    await _identityUserManager.UpdateAsync(user);
                }
            }

            return user;
        }
    }
}
