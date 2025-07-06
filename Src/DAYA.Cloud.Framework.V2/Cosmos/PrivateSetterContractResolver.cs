using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DAYA.Cloud.Framework.V2.Cosmos
{
    public class PrivateSetterContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            // Check if the property is writable
            if (!property.Writable)
            {
                var propertyInfo = member as PropertyInfo;
                if (propertyInfo != null)
                {
                    // Allow private setters to be writable
                    var hasPrivateSetter = propertyInfo.GetSetMethod(true) != null;
                    property.Writable = hasPrivateSetter;
                }
            }

            return property;
        }
    }
}