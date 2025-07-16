using NetArchTest.Rules;

namespace DAYA.ArchRules.Layers
{
    class DomainLayerDoesNotHaveDependencyToInfrastructureLayer : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.DomainAssembly)
                .Should()
                .NotHaveDependencyOn(Data.ApplicationAssembly.GetName().Name)
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
