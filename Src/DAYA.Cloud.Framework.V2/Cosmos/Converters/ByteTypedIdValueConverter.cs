using DAYA.Cloud.Framework.V2.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace DAYA.Cloud.Framework.V2.Cosmos.Converters;

internal class ByteTypedIdValueConverter<TTypedIdValue> : ValueConverter<TTypedIdValue, string>
   where TTypedIdValue : TypedId<byte>
{
    public ByteTypedIdValueConverter(ConverterMappingHints mappingHints = null)
        : base(id => id.Value.ToString(), value => Create(byte.Parse(value)),
              mappingHints)
    {
    }

    private static TTypedIdValue Create(byte id) => Activator.CreateInstance(typeof(TTypedIdValue), id) as TTypedIdValue;
}