using DAYA.Cloud.Framework.V2.Domain;

namespace Daya.Sample.Domain.UserAccounts
{
    public class UserAccountId : TypedId<Guid>
    {
        public UserAccountId(Guid value) : base(value)
        {
        }
    }
}