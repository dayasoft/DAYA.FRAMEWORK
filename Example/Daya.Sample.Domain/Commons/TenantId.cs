using DAYA.Cloud.Framework.V2.Domain;

public class TenantId : TypedId<Guid>
{
    public TenantId(Guid value) : base(value)
    {
    }
}