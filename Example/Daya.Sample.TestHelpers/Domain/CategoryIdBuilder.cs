using Daya.Sample.Domain.Categories;
using Daya.Sample.TestHelpers.Domain;
using System;

namespace Daya.Sample.TestHelpers.Domain
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CategoryIdBuilder
    {
        private Guid _value = Guid.NewGuid();
        private bool _valueIsSet = false;

        public CategoryId Build()
        {
            return new CategoryId(
                _value);
        }

        public CategoryIdBuilder SetValue(Guid value)
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
