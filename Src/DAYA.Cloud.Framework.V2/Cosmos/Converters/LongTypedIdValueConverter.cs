using DAYA.Cloud.Framework.V2.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace DAYA.Cloud.Framework.V2.Cosmos.Converters;

internal class LongTypedIdValueConverter<TTypedIdValue> : ValueConverter<TTypedIdValue, string>
   where TTypedIdValue : TypedId<long>
{
    public LongTypedIdValueConverter(ConverterMappingHints mappingHints = null)
        : base(id => id.Value.ToString(), value => Create(long.Parse(value)),
              mappingHints)
    {
    }

    private static TTypedIdValue Create(long id) => Activator.CreateInstance(typeof(TTypedIdValue), id) as TTypedIdValue;
}