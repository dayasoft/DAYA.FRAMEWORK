using DAYA.Cloud.Framework.V2.Domain;

namespace Daya.Sample.Domain.Categories
{
    public class CategoryId : TypedId<Guid>
    {
        public CategoryId(Guid value) : base(value)
        {
        }
    }
}