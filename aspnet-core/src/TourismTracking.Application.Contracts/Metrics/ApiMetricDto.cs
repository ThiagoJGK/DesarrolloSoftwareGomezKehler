using System;
using Volo.Abp.Application.Dtos;

namespace TourismTracking.Metrics
{
    public class ApiMetricDto : EntityDto<Guid>
    {
        public string ApiName { get; set; }
        public string Endpoint { get; set; }
        public bool IsSuccess { get; set; }
        public int ResponseTimeMs { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
