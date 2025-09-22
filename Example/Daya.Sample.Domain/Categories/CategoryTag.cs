using DAYA.Cloud.Framework.V2.Domain;
using Newtonsoft.Json;

namespace Daya.Sample.Domain.Categories
{
    public class CategoryTag : ValueObject
    {
        public string Title { get; } = null!;

        private CategoryTag()
        {
        }

        [JsonConstructor]
        private CategoryTag(string title)
        {
            Title = title;
        }

        public static CategoryTag Create(string title)
        {
            return new CategoryTag(title);
        }
    }
}