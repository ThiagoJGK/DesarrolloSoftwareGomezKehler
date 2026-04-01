using TourismTracking.Samples;
using Xunit;

namespace TourismTracking.EntityFrameworkCore.Domains;

[Collection(TourismTrackingTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<TourismTrackingEntityFrameworkCoreTestModule>
{

}
