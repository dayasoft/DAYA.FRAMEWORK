using DAYA.Cloud.Framework.V2.Application.Contracts;

namespace Daya.Sample.Application.UserAccounts.Queries.GetUserAccountDetail
{
    public record GetUserAccountDetailQuery(Guid UserId) : Query<UserAccountDetailQueryDto>();
}