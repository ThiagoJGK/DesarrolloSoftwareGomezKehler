import type { PublicUserProfileDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TourismUserService {
  apiName = 'Default';
  

  deleteMyAccount = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: '/api/app/tourism-user/my-account',
    },
    { apiName: this.apiName,...config });
  

  getPublicProfile = (userId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PublicUserProfileDto>({
      method: 'GET',
      url: `/api/app/tourism-user/public-profile/${userId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
