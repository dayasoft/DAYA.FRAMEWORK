using Daya.Sample.Application.Categories.Commands.Update;
using Daya.Sample.Domain.Categories;
using Daya.Sample.TestHelpers.Domain;
using System;

namespace Daya.Sample.TestHelpers.Application.Categories
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class UpdateCategoryCommandBuilder
    {
        private Guid _CategoryId = Guid.NewGuid();
        private bool _CategoryIdIsSet = false;
        private string _Name = Guid.NewGuid().ToString().Substring(0, 18);
        private bool _NameIsSet = false;
        private string _Description = Guid.NewGuid().ToString().Substring(0, 18);
        private bool _DescriptionIsSet = false;

        public UpdateCategoryCommand Build()
        {
            return new UpdateCategoryCommand(
                new CategoryId(_CategoryId),
                _Name,
                _Description);
        }

        public UpdateCategoryCommandBuilder SetCategoryId(Guid CategoryId)
        {
            if (_CategoryIdIsSet)
            {
                throw new System.InvalidOperationException(nameof(_CategoryId) + " already initialized");
            }
            _CategoryIdIsSet = true;
            _CategoryId = CategoryId;
            return this;
        }

        public UpdateCategoryCommandBuilder SetName(string Name)
        {
            if (_NameIsSet)
            {
                throw new System.InvalidOperationException(nameof(_Name) + " already initialized");
            }
            _NameIsSet = true;
            _Name = Name;
            return this;
        }

        public UpdateCategoryCommandBuilder SetDescription(string Description)
        {
            if (_DescriptionIsSet)
            {
                throw new System.InvalidOperationException(nameof(_Description) + " already initialized");
            }
            _DescriptionIsSet = true;
            _Description = Description;
            return this;
        }
    }
}
