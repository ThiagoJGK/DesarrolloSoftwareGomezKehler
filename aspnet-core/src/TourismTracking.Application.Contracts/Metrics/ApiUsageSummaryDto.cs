namespace TourismTracking.Metrics
{
    public class ApiUsageSummaryDto
    {
        public string ApiName { get; set; }
        public int TotalCalls { get; set; }
        public int SuccessCalls { get; set; }
        public int FailedCalls { get; set; }
        public double AverageResponseTimeMs { get; set; }
        public double SuccessRate { get; set; }
    }
}
