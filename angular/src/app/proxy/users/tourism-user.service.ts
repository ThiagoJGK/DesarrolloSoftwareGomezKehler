import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TourismUserService {
  constructor(private restService: RestService) {}

  getPublicProfile(userId: string): Observable<any> {
    return this.restService.request<any, any>({
      method: 'GET',
      url: `/api/app/tourism-user/public-profile`,
      params: { userId }
    });
  }

  deleteMyAccount(): Observable<void> {
    return this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/tourism-user/my-account`
    });
  }
}
