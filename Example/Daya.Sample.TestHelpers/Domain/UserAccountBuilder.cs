using Daya.Sample.Domain.Commons;
using Daya.Sample.Domain.UserAccounts;
using System;
using System.Threading.Tasks;

namespace Daya.Sample.TestHelpers.Domain
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UserAccountBuilder
    {
        private Guid _id = Guid.NewGuid();
        private bool _idIsSet = false;
        private Guid _tenantId = Guid.NewGuid();
        private bool _tenantIdIsSet = false;
        private string _firstName = Guid.NewGuid().ToString().Substring(0, 18);
        private bool _firstNameIsSet = false;
        private string _lastName = Guid.NewGuid().ToString().Substring(0, 18);
        private bool _lastNameIsSet = false;
        private string _emailAddress = $"pre{Guid.NewGuid()}@somedomain.com";
        private bool _emailAddressIsSet = false;

        public Task<UserAccount> BuildAsync()
        {
            return UserAccount.CreateAsync(
                new UserAccountId(_id),
                new TenantId(_tenantId),
                _firstName,
                _lastName,
                _emailAddress);
        }

        public UserAccountBuilder SetId(Guid id)
        {
            if (_idIsSet)
            {
                throw new System.InvalidOperationException(nameof(_id) + " already initialized");
            }
            _idIsSet = true;
            _id = id;
            return this;
        }

        public UserAccountBuilder SetTenantId(Guid tenantId)
        {
            if (_tenantIdIsSet)
            {
                throw new System.InvalidOperationException(nameof(_tenantId) + " already initialized");
            }
            _tenantIdIsSet = true;
            _tenantId = tenantId;
            return this;
        }

        public UserAccountBuilder SetFirstName(string firstName)
        {
            if (_firstNameIsSet)
            {
                throw new System.InvalidOperationException(nameof(_firstName) + " already initialized");
            }
            _firstNameIsSet = true;
            _firstName = firstName;
            return this;
        }

        public UserAccountBuilder SetLastName(string lastName)
        {
            if (_lastNameIsSet)
            {
                throw new System.InvalidOperationException(nameof(_lastName) + " already initialized");
            }
            _lastNameIsSet = true;
            _lastName = lastName;
            return this;
        }

        public UserAccountBuilder SetEmailAddress(string emailAddress)
        {
            if (_emailAddressIsSet)
            {
                throw new System.InvalidOperationException(nameof(_emailAddress) + " already initialized");
            }
            _emailAddressIsSet = true;
            _emailAddress = emailAddress;
            return this;
        }
    }
}
