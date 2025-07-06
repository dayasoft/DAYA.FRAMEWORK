public class LookUpPermissions
{
    public static Type[]? GetPermissionTypeFromUserType(IContextAccessor contextAccessor)
    {
        if (contextAccessor.UserType == (int)UserType.None)
        {
            return null;
        }

        //if (contextAccessor.TenantId.Value.ToString() == PlatformStatics.PlatformTenantId)
        //{
        //    return [typeof(PlatformPermission)];
        //}

        //if (contextAccessor.TenantId.Value.ToString() != PlatformStatics.PlatformTenantId)
        //{
        //    var iB2BPermissionDefinitionType = typeof(IB2bPermissionDefinition);
        //    return iB2BPermissionDefinitionType
        //        .Assembly
        //        .GetTypes()
        //        .Where(t => t.IsAssignableTo(iB2BPermissionDefinitionType))
        //        .ToArray();
        //}

        return null;
    }
}