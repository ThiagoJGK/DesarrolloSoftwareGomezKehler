using Xunit;

namespace TourismTracking.EntityFrameworkCore;

[CollectionDefinition(TourismTrackingTestConsts.CollectionDefinitionName)]
public class TourismTrackingEntityFrameworkCoreCollection : ICollectionFixture<TourismTrackingEntityFrameworkCoreFixture>
{

}
