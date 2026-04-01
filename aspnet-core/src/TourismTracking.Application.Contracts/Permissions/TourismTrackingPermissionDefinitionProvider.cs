using TourismTracking.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace TourismTracking.Permissions;

public class TourismTrackingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(TourismTrackingPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(TourismTrackingPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TourismTrackingResource>(name);
    }
}
