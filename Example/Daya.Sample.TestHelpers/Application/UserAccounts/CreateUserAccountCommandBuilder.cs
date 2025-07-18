using Daya.Sample.Application.UserAccounts.Commands.Create;
using Daya.Sample.Domain.Commons;
using Daya.Sample.Domain.UserAccounts;
using Daya.Sample.TestHelpers.Domain;
using System;

namespace Daya.Sample.TestHelpers.Application.UserAccounts
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CreateUserAccountCommandBuilder
    {
        private Guid _UserAccountId = Guid.NewGuid();
        private bool _UserAccountIdIsSet = false;
        private Guid _TenantId = Guid.NewGuid();
        private bool _TenantIdIsSet = false;
        private string _FirstName = Guid.NewGuid().ToString().Substring(0, 18);
        private bool _FirstNameIsSet = false;
        private string _LastName = Guid.NewGuid().ToString().Substring(0, 18);
        private bool _LastNameIsSet = false;
        private string _EmailAddress = $"pre{Guid.NewGuid()}@somedomain.com";
        private bool _EmailAddressIsSet = false;

        public CreateUserAccountCommand Build()
        {
            return new CreateUserAccountCommand(
                new UserAccountId(_UserAccountId),
                new TenantId(_TenantId),
                _FirstName,
                _LastName,
                _EmailAddress);
        }

        public CreateUserAccountCommandBuilder SetUserAccountId(Guid UserAccountId)
        {
            if (_UserAccountIdIsSet)
            {
                throw new System.InvalidOperationException(nameof(_UserAccountId) + " already initialized");
            }
            _UserAccountIdIsSet = true;
            _UserAccountId = UserAccountId;
            return this;
        }

        public CreateUserAccountCommandBuilder SetTenantId(Guid TenantId)
        {
            if (_TenantIdIsSet)
            {
                throw new System.InvalidOperationException(nameof(_TenantId) + " already initialized");
            }
            _TenantIdIsSet = true;
            _TenantId = TenantId;
            return this;
        }

        public CreateUserAccountCommandBuilder SetFirstName(string FirstName)
        {
            if (_FirstNameIsSet)
            {
                throw new System.InvalidOperationException(nameof(_FirstName) + " already initialized");
            }
            _FirstNameIsSet = true;
            _FirstName = FirstName;
            return this;
        }

        public CreateUserAccountCommandBuilder SetLastName(string LastName)
        {
            if (_LastNameIsSet)
            {
                throw new System.InvalidOperationException(nameof(_LastName) + " already initialized");
            }
            _LastNameIsSet = true;
            _LastName = LastName;
            return this;
        }

        public CreateUserAccountCommandBuilder SetEmailAddress(string EmailAddress)
        {
            if (_EmailAddressIsSet)
            {
                throw new System.InvalidOperationException(nameof(_EmailAddress) + " already initialized");
            }
            _EmailAddressIsSet = true;
            _EmailAddress = EmailAddress;
            return this;
        }
    }
}
