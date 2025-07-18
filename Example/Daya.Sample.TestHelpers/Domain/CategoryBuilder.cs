using Daya.Sample.Domain.Categories;
using System;
using System.Threading.Tasks;

namespace Daya.Sample.TestHelpers.Domain
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class CategoryBuilder
    {
        private Guid _id = Guid.NewGuid();
        private bool _idIsSet = false;
        private string _partitionKey = Guid.NewGuid().ToString().Substring(0, 18);
        private bool _partitionKeyIsSet = false;
        private string _name = Guid.NewGuid().ToString().Substring(0, 18);
        private bool _nameIsSet = false;
        private string? _description = Guid.NewGuid().ToString().Substring(0, 18);
        private bool _descriptionIsSet = false;

        public Task<Category> BuildAsync()
        {
            return Category.CreateAsync(
                new CategoryId(_id),
                _partitionKey,
                _name,
                _description);
        }

        public CategoryBuilder SetId(Guid id)
        {
            if (_idIsSet)
            {
                throw new System.InvalidOperationException(nameof(_id) + " already initialized");
            }
            _idIsSet = true;
            _id = id;
            return this;
        }

        public CategoryBuilder SetPartitionKey(string partitionKey)
        {
            if (_partitionKeyIsSet)
            {
                throw new System.InvalidOperationException(nameof(_partitionKey) + " already initialized");
            }
            _partitionKeyIsSet = true;
            _partitionKey = partitionKey;
            return this;
        }

        public CategoryBuilder SetName(string name)
        {
            if (_nameIsSet)
            {
                throw new System.InvalidOperationException(nameof(_name) + " already initialized");
            }
            _nameIsSet = true;
            _name = name;
            return this;
        }

        public CategoryBuilder SetDescription(string? description)
        {
            if (_descriptionIsSet)
            {
                throw new System.InvalidOperationException(nameof(_description) + " already initialized");
            }
            _descriptionIsSet = true;
            _description = description;
            return this;
        }
    }
}
