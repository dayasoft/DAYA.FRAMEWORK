using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace DAYA.Cloud.Framework.V2.Cosmos.Migration;

internal static class JsonUtility
{
    public static bool? IsArray(this JToken jToken)
    {
        const string schemaJson = @"{
            'type': 'array',
        }";
        var schema = JSchema.Parse(schemaJson);
        var isArray = jToken?.IsValid(schema);
        return isArray;
    }

    public static bool? IsArray(this JObject jToken)
    {
        const string schemaJson = @"{
            'type': 'array',
        }";
        var schema = JSchema.Parse(schemaJson);
        var isArray = jToken?.IsValid(schema);
        return isArray;
    }
}
