import type { DestinationDto, SaveDestinationInput } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class DestinationService {
  apiName = 'Default';
  

  deleteSavedDestination = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/destination/${id}/saved-destination`,
    },
    { apiName: this.apiName,...config });
  

  getSavedDestinations = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationDto[]>({
      method: 'GET',
      url: '/api/app/destination/saved-destinations',
    },
    { apiName: this.apiName,...config });
  

  saveDestinationToInternalDb = (input: SaveDestinationInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationDto>({
      method: 'POST',
      url: '/api/app/destination/save-destination-to-internal-db',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  searchExternalDestinations = (nameQuery: string, countryCode?: string, regionQuery?: string, minPopulation?: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationDto[]>({
      method: 'POST',
      url: '/api/app/destination/search-external-destinations',
      params: { nameQuery, countryCode, regionQuery, minPopulation },
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
