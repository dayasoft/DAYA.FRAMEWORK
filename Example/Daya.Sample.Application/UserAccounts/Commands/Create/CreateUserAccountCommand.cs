using Daya.Sample.Domain.Commons;
using Daya.Sample.Domain.UserAccounts;
using DAYA.Cloud.Framework.V2.DirectOperations;

namespace Daya.Sample.Application.UserAccounts.Commands.Create
{
    public record CreateUserAccountCommand(
        UserAccountId UserAccountId,
        TenantId TenantId,
        string FirstName,
        string LastName,
        string EmailAddress) : DirectCommand<Guid>;
}