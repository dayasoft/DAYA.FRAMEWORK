using Daya.Sample.Domain.Categories;
using System;

namespace Daya.Sample.TestHelpers.Domain
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CategoryTagBuilder
    {
        private string _title = Guid.NewGuid().ToString().Substring(0, 18);
        private bool _titleIsSet = false;

        public CategoryTag Build()
        {
            return CategoryTag.Create(
                _title);
        }

        public CategoryTagBuilder SetTitle(string title)
        {
            if (_titleIsSet)
            {
                throw new System.InvalidOperationException(nameof(_title) + " already initialized");
            }
            _titleIsSet = true;
            _title = title;
            return this;
        }
    }
}
