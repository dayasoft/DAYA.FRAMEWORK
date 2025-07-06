using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DAYA.Cloud.Framework.V2.Cosmos.Abstractions;

namespace DAYA.Cloud.Framework.V2.Domain
{
    public class PrimitiveCollectionConverter<T> : JsonConverter<PrimitiveCollection<T>>
    {
        public override PrimitiveCollection<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Deserialize JSON array into an array of T
            var items = JsonSerializer.Deserialize<T[]>(ref reader, options);

            // Return a new instance of PrimitiveCollection<T> initialized with the array
            return new PrimitiveCollection<T>(items);
        }

        public override void Write(Utf8JsonWriter writer, PrimitiveCollection<T> value, JsonSerializerOptions options)
        {
            // Write the Items array to JSON
            JsonSerializer.Serialize(writer, value.Items.ToArray(), options);
        }
    }
}