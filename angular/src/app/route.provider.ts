import { RoutesService, eLayoutType } from '@abp/ng.core';
import { APP_INITIALIZER, EnvironmentInjector } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  {
    provide: APP_INITIALIZER,
    useFactory: configureRoutes,
    deps: [RoutesService, EnvironmentInjector],
    multi: true,
  },
];

function configureRoutes(routesService: RoutesService, injector: EnvironmentInjector) {
  return () => {
    injector.runInContext(() => {
      routesService.add([
        {
          path: '/',
          name: '::Menu:Home',
          iconClass: 'fas fa-home',
          order: 1,
          layout: eLayoutType.application,
        },
        {
          path: '/tourism',
          name: 'Turismo (Destinos)',
          iconClass: 'fas fa-plane',
          order: 2,
          layout: eLayoutType.application,
        },
        {
          path: '/tourism/dashboard',
          name: 'Mi Tablero',
          iconClass: 'fas fa-user-circle',
          order: 3,
          layout: eLayoutType.application,
        },
      ]);
    });
  };
}
