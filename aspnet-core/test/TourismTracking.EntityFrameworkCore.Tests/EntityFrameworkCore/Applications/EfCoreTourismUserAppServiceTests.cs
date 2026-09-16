using TourismTracking.Users;
using Xunit;

namespace TourismTracking.EntityFrameworkCore.Applications
{
    [Collection(TourismTrackingTestConsts.CollectionDefinitionName)]
    public class EfCoreTourismUserAppServiceTests : TourismUserAppService_Tests<TourismTrackingEntityFrameworkCoreTestModule>
    {

    }
}
