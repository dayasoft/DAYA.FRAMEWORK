using Daya.Sample.Domain.Commons;
using DAYA.Cloud.Framework.V2.Domain;

namespace Daya.Sample.Domain.UserAccounts.DomainEvents
{
    public record UserAccountCreatedDomainEvent(
        UserAccountId UserAccountId,
        TenantId TenantId,
        string FirstName,
        string LastName,
        string EmailAddress) : DomainEventBase(UserAccountId);
}