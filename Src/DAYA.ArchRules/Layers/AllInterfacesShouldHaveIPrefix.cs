using NetArchTest.Rules;

namespace DAYA.ArchRules.Layers
{
    class AllInterfacesShouldHaveIPrefix : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssemblies(new[] { Data.DomainAssembly, Data.InfrastructureAssembly, Data.ApplicationAssembly })
                .That()
                .AreInterfaces()
                .Should()
                .HaveNameStartingWith("I")
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
