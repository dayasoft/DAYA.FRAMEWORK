using DAYA.Cloud.Framework.V2.Domain;

public class CategoryId : TypedId<Guid>
{
    public CategoryId(Guid value) : base(value)
    {
    }
}