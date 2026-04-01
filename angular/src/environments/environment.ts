import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'TourismTracking',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44305/',
    redirectUri: baseUrl,
    clientId: 'TourismTracking_App',
    responseType: 'code',
    scope: 'offline_access TourismTracking',
    requireHttps: true,
  },
  apis: {
    default: {
      url: 'https://localhost:44305',
      rootNamespace: 'TourismTracking',
    },
  },
} as Environment;
