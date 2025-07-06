using DAYA.Cloud.Framework.V2.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace DAYA.Cloud.Framework.V2.Cosmos.Converters;

internal class GuidTypedIdValueConverter<TTypedIdValue> : ValueConverter<TTypedIdValue, string>
   where TTypedIdValue : TypedId<Guid>
{
    public GuidTypedIdValueConverter(ConverterMappingHints mappingHints = null)
        : base(id => id.Value.ToString(), value => Create(Guid.Parse(value)),
              mappingHints)
    {
    }

    private static TTypedIdValue Create(Guid id) => Activator.CreateInstance(typeof(TTypedIdValue), id) as TTypedIdValue;
}