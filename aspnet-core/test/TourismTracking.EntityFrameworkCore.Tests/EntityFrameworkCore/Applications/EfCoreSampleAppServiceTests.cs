using TourismTracking.Samples;
using Xunit;

namespace TourismTracking.EntityFrameworkCore.Applications;

[Collection(TourismTrackingTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<TourismTrackingEntityFrameworkCoreTestModule>
{

}
