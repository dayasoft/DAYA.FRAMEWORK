using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DAYA.Cloud.Framework.V2.Cosmos.Abstractions;

internal class PrimitiveCollectionJsonConverter : JsonConverter
{
    public PrimitiveCollectionJsonConverter()
    {
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var t = value.GetType();
        var val = t.GetProperty("Items").GetValue(value, null);
        serializer.Serialize(writer, val);
        //string value1 = JsonConvert.SerializeObject(val);
        //writer.WriteRawValue(value1);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        string s = reader?.Value?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(s))
        {
            return default;
        }

        var genericType = objectType.GetGenericArguments();
        var genericListType = typeof(List<>).MakeGenericType(genericType);

        var list = JsonConvert.DeserializeObject(s, genericListType);

        var bindingFlags = BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance;
        var constructors = objectType.GetConstructors(bindingFlags);
        return constructors[0].Invoke(new[] { list });
    }

    public override bool CanConvert(Type objectType)
    {
        throw new NotImplementedException();
    }

    public override bool CanRead
    {
        get { return true; }
    }
}