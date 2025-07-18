using Daya.Sample.Application.Categories.Queries.GetCategories;
using Daya.Sample.TestHelpers.Domain;
using System;

namespace Daya.Sample.TestHelpers.Application.Categories
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class GetCategoriesQueryBuilder
    {
        private int _PageNumber = 2;
        private bool _PageNumberIsSet = false;
        private int _PageSize = 2;
        private bool _PageSizeIsSet = false;

        public GetCategoriesQuery Build()
        {
            return new GetCategoriesQuery(
                _PageNumber,
                _PageSize);
        }

        public GetCategoriesQueryBuilder SetPageNumber(int PageNumber)
        {
            if (_PageNumberIsSet)
            {
                throw new System.InvalidOperationException(nameof(_PageNumber) + " already initialized");
            }
            _PageNumberIsSet = true;
            _PageNumber = PageNumber;
            return this;
        }

        public GetCategoriesQueryBuilder SetPageSize(int PageSize)
        {
            if (_PageSizeIsSet)
            {
                throw new System.InvalidOperationException(nameof(_PageSize) + " already initialized");
            }
            _PageSizeIsSet = true;
            _PageSize = PageSize;
            return this;
        }
    }
}
