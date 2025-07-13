using Daya.Sample.Domain.Commons;
using DAYA.Cloud.Framework.V2.Domain;

namespace Daya.Sample.Domain.UserAccounts.BusinessRules
{
    public class UserAccountShouldBeUniqueRule : IBusinessRule
    {
        private readonly bool _isUnique;

        public UserAccountShouldBeUniqueRule(bool isUnique)
        {
            _isUnique = isUnique;
        }

        public Task<bool> IsBrokenAsync()
        {
            return Task.FromResult(!_isUnique);
        }

        public string Message => ErrorList.UserAccountAlreadyExist;
    }
}