import type { EntityDto } from '@abp/ng.core';

export interface ApiMetricDto extends EntityDto<string> {
  apiName?: string;
  endpoint?: string;
  isSuccess: boolean;
  responseTimeMs: number;
  errorMessage?: string;
  creationTime?: string;
}

export interface ApiUsageSummaryDto {
  apiName?: string;
  totalCalls: number;
  successCalls: number;
  failedCalls: number;
  averageResponseTimeMs: number;
  successRate: number;
}

export interface MostReviewedDestinationDto {
  destinationName?: string;
  country?: string;
  reviewsCount: number;
  averageRating: number;
}

export interface SystemStatisticsDto {
  totalUsers: number;
  totalExperiences: number;
  totalSavedDestinations: number;
  mostReviewedDestinations: MostReviewedDestinationDto[];
}
