using DAYA.Cloud.Framework.V2.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace DAYA.Cloud.Framework.V2.Cosmos.Converters;

internal class UIntTypedIdValueConverter<TTypedIdValue> : ValueConverter<TTypedIdValue, string>
   where TTypedIdValue : TypedId<ulong>
{
    public UIntTypedIdValueConverter(ConverterMappingHints mappingHints = null)
        : base(id => id.Value.ToString(), value => Create(uint.Parse(value)),
              mappingHints)
    {
    }

    private static TTypedIdValue Create(uint id) => Activator.CreateInstance(typeof(TTypedIdValue), id) as TTypedIdValue;
}