using DAYA.Cloud.Framework.V2.DirectOperations.Repositories;
using NetArchTest.Rules;

namespace DAYA.ArchRules.Infrastructure
{
    internal class ClassesWhichHaveNameEndingWithRepositoryShouldImplementIRepository : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.InfrastructureAssembly)
                .That()
                .HaveNameEndingWith("Repository")
                .And()
                .DoNotHaveNameEndingWith("AuditMessageRepository")
                .And()
                .DoNotHaveNameEndingWith("InternalCommandRepository")
                .And()
                .DoNotHaveNameEndingWith("OutboxRepository")
                .Should()
                .ImplementInterface(typeof(ICosmosRepository<,>))
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}