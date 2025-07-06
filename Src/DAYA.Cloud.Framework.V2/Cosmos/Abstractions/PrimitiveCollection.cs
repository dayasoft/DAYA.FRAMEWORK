using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.Cloud.Framework.V2.Cosmos.Abstractions;

[JsonConverter(typeof(PrimitiveCollectionConverter<>))]
public class PrimitiveCollection<T>
{
    public ArraySegment<T> Items { get; private init; }

    public PrimitiveCollection(IEnumerable<T> iItems)
    {
        Items = iItems.ToArray();
    }
}