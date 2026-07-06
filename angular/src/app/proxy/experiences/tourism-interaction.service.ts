import type { DestinationMetricsDto, ExperienceDto, ReviewDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class TourismInteractionService {
  apiName = 'Default';
  

  addReview = (destinationId: string, rating: number, comment: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ReviewDto>({
      method: 'POST',
      url: `/api/app/tourism-interaction/review/${destinationId}`,
      params: { rating, comment },
    },
    { apiName: this.apiName,...config });
  

  addToFavorites = (destinationId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/tourism-interaction/to-favorites/${destinationId}`,
    },
    { apiName: this.apiName,...config });
  

  createExperience = (destinationId: string, title: string, content: string, keywords: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienceDto>({
      method: 'POST',
      url: `/api/app/tourism-interaction/experience/${destinationId}`,
      params: { title, content, keywords },
    },
    { apiName: this.apiName,...config });
  

  deleteExperience = (experienceId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/tourism-interaction/experience/${experienceId}`,
    },
    { apiName: this.apiName,...config });
  

  deleteReview = (reviewId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/tourism-interaction/review/${reviewId}`,
    },
    { apiName: this.apiName,...config });
  

  editExperience = (experienceId: string, title: string, content: string, keywords: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienceDto>({
      method: 'POST',
      url: `/api/app/tourism-interaction/edit-experience/${experienceId}`,
      params: { title, content, keywords },
    },
    { apiName: this.apiName,...config });
  

  editReview = (reviewId: string, rating: number, comment: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ReviewDto>({
      method: 'POST',
      url: `/api/app/tourism-interaction/edit-review/${reviewId}`,
      params: { rating, comment },
    },
    { apiName: this.apiName,...config });
  

  getDestinationAverageRating = (destinationId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, DestinationMetricsDto>({
      method: 'GET',
      url: `/api/app/tourism-interaction/destination-average-rating/${destinationId}`,
    },
    { apiName: this.apiName,...config });
  

  getDestinationReviews = (destinationId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ReviewDto[]>({
      method: 'GET',
      url: `/api/app/tourism-interaction/destination-reviews/${destinationId}`,
    },
    { apiName: this.apiName,...config });
  

  getExperiencesByDestination = (destinationId: string, keywordFilter?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienceDto[]>({
      method: 'GET',
      url: `/api/app/tourism-interaction/experiences-by-destination/${destinationId}`,
      params: { keywordFilter },
    },
    { apiName: this.apiName,...config });
  

  getMyExperiences = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ExperienceDto[]>({
      method: 'GET',
      url: '/api/app/tourism-interaction/my-experiences',
    },
    { apiName: this.apiName,...config });
  

  getMyFavoriteDestinations = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, string[]>({
      method: 'GET',
      url: '/api/app/tourism-interaction/my-favorite-destinations',
    },
    { apiName: this.apiName,...config });
  

  removeFromFavorites = (destinationId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/tourism-interaction/from-favorites/${destinationId}`,
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
