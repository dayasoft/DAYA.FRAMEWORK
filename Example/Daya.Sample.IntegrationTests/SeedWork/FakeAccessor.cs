using Daya.Sample.Domain.Commons;

namespace Daya.Sample.IntegrationTests.SeedWork
{
    internal class FakeAccessor : IContextAccessor
    {
        private static TenantId _tenantId = null!;

        private FakeAccessor()
        {
        }

        public static FakeAccessor Instance = new();

        public static void ResetTenantId()
        {
            _tenantId = new TenantId(Guid.NewGuid());
        }

        public static void SetTenantId(Guid tenantId)
        {
            _tenantId = new(tenantId);
        }

        public TenantId TenantId => _tenantId;

        public int UserType => throw new NotImplementedException();

        private static Guid _userId;

        public static void ResetUserId()
        {
            _userId = Guid.NewGuid();
        }

        public Guid UserId => _userId;

        public string FullName => "Integration Testuser";

        public string FirstName => "Int-First";

        public string LastName => "Int-Last";

        public string EmailAddress => "EmailAddress";

        public string UserName => throw new NotImplementedException();

        public Guid UserObjectId => throw new NotImplementedException();

        private Guid _storedUserId;

        internal void Destroy()
        {
            _storedUserId = _userId;

            ResetTenantId();
            ResetUserId();
        }

        internal void Rebuild()
        {
            _userId = _storedUserId;
        }
    }
}