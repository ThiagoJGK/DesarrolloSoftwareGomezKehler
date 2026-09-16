using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace TourismTracking.Metrics
{
    public interface IAdminMetricsAppService : IApplicationService
    {
        Task<List<ApiUsageSummaryDto>> GetApiUsageSummaryAsync();
        Task<SystemStatisticsDto> GetSystemStatisticsAsync();
        Task<List<ApiMetricDto>> GetRecentApiCallsAsync();
        Task<string> ExportMetricsReportAsync(string format);
    }
}
