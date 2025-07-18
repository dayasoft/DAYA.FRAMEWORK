using Daya.Sample.Application.UserAccounts.Queries.GetUserAccountDetail;
using Daya.Sample.TestHelpers.Domain;
using System;

namespace Daya.Sample.TestHelpers.Application.UserAccounts
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GetUserAccountDetailQueryBuilder
    {
        private Guid _UserId = Guid.NewGuid();
        private bool _UserIdIsSet = false;

        public GetUserAccountDetailQuery Build()
        {
            return new GetUserAccountDetailQuery(
                _UserId);
        }

        public GetUserAccountDetailQueryBuilder SetUserId(Guid UserId)
        {
            if (_UserIdIsSet)
            {
                throw new System.InvalidOperationException(nameof(_UserId) + " already initialized");
            }
            _UserIdIsSet = true;
            _UserId = UserId;
            return this;
        }
    }
}
