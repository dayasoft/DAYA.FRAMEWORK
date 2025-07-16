namespace DAYA.ArchRules.IntegrationEvents
{
    class IntegrationEventShouldHaveIntegrationEventPostfix : ArchRule
    {
        internal override void Check()
        {
            var result = IntegrationEvents
                .Should()
                .HaveNameEndingWith("IntegrationEvent")
                .GetResult();

            AssertArchTestResult(result);
        }
    }
}
