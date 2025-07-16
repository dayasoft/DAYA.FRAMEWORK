using NetArchTest.Rules;

namespace DAYA.ArchRules.Layers
{
    class ApplicationLayerDoesNotHaveDependencyToInfrastructureLayer : ArchRule
    {
        internal override void Check()
        {
            var result = Types.InAssembly(Data.ApplicationAssembly)
                .Should()
                .NotHaveDependencyOn(Data.InfrastructureAssembly.GetName().Name)
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
