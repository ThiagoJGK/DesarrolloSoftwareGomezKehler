import type { ApiMetricDto, ApiUsageSummaryDto, SystemStatisticsDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AdminMetricsService {
  apiName = 'Default';
  

  exportMetricsReport = (format: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, string>({
      method: 'POST',
      responseType: 'text',
      url: '/api/app/admin-metrics/export-metrics-report',
      params: { format },
    },
    { apiName: this.apiName,...config });
  

  getApiUsageSummary = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiUsageSummaryDto[]>({
      method: 'GET',
      url: '/api/app/admin-metrics/api-usage-summary',
    },
    { apiName: this.apiName,...config });
  

  getRecentApiCalls = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ApiMetricDto[]>({
      method: 'GET',
      url: '/api/app/admin-metrics/recent-api-calls',
    },
    { apiName: this.apiName,...config });
  

  getSystemStatistics = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, SystemStatisticsDto>({
      method: 'GET',
      url: '/api/app/admin-metrics/system-statistics',
    },
    { apiName: this.apiName,...config });

  constructor(private restService: RestService) {}
}
