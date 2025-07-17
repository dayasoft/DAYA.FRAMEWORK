using DAYA.Cloud.Framework.V2.Domain;

namespace Daya.Sample.Domain.Commons
{
    public class TenantId : TypedId<Guid>
    {
        public TenantId(Guid value) : base(value)
        {
        }
    }
}