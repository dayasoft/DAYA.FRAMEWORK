using Daya.Sample.Domain.Commons;
using Daya.Sample.Domain.UserAccounts.BusinessRules;
using Daya.Sample.Domain.UserAccounts.DomainEvents;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.DirectOperations.Attributes;
using DAYA.Cloud.Framework.V2.Domain;

namespace Daya.Sample.Domain.UserAccounts
{
    [ContainerName(ServiceDatabaseContainers.UserAccounts)]
    [PartitionKeyPath("partitionKey")]
    public class UserAccount : CosmosEntity
    {
        public override UserAccountId Id { get; } = null!;

        // partitionKey
        public TenantId PartitionKey { get; private set; } = null!;

        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string EmailAddress { get; private set; } = null!;

        private UserAccount(UserAccountId id)
        {
            Id = id;
        }

        public static async Task<UserAccount> CreateAsync(
            UserAccountId id,
            TenantId tenantId,
            string firstName,
            string lastName,
            string emailAddress)
        {
            var isUnique = true; // check if the user with the same email address is uniuqe
            await CheckRuleAsync(new UserAccountShouldBeUniqueRule(isUnique));

            var @event = new UserAccountCreatedDomainEvent(
                id,
                tenantId,
                firstName,
                lastName,
                emailAddress);

            var user = new UserAccount(id);
            user.Apply(@event);
            user.AddDomainEvent(@event);

            return user;
        }

        protected override void Apply(IDomainEvent @event)
        {
            ApplyEvent(@event as dynamic);
        }

        private void ApplyEvent(UserAccountCreatedDomainEvent @event)
        {
            PartitionKey = @event.TenantId;
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            EmailAddress = @event.EmailAddress;
        }
    }
}