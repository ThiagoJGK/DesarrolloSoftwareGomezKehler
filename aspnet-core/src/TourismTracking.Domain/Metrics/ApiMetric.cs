using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace TourismTracking.Metrics
{
    public class ApiMetric : CreationAuditedEntity<Guid>
    {
        public string ApiName { get; private set; }
        public string Endpoint { get; private set; }
        public bool IsSuccess { get; private set; }
        public int ResponseTimeMs { get; private set; }
        public string ErrorMessage { get; private set; }

        protected ApiMetric() { }

        public ApiMetric(Guid id, string apiName, string endpoint, bool isSuccess, int responseTimeMs, string errorMessage = null) : base(id)
        {
            ApiName = apiName;
            Endpoint = endpoint;
            IsSuccess = isSuccess;
            ResponseTimeMs = responseTimeMs;
            ErrorMessage = errorMessage ?? string.Empty;
        }
    }
}
