using Daya.Sample.Domain.UserAccounts;
using Daya.Sample.TestHelpers.Domain;
using System;

namespace Daya.Sample.TestHelpers.Domain
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UserAccountIdBuilder
    {
        private Guid _value = Guid.NewGuid();
        private bool _valueIsSet = false;

        public UserAccountId Build()
        {
            return new UserAccountId(
                _value);
        }

        public UserAccountIdBuilder SetValue(Guid value)
        {
            if (_valueIsSet)
            {
                throw new System.InvalidOperationException(nameof(_value) + " already initialized");
            }
            _valueIsSet = true;
            _value = value;
            return this;
        }
    }
}
