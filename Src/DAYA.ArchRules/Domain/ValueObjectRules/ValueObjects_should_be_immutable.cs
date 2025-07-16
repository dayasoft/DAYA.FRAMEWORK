namespace DAYA.ArchRules.Domain.ValueObjectRules
{
    class ValueObjects_should_be_immutable : ArchRule
    {
        internal override void Check()
        {
            var types =
                ValueObjects
                .GetTypes();

            AssertAreImmutable(types);
        }
    }
}
