using DAYA.Cloud.Framework.V2.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace DAYA.Cloud.Framework.V2.Cosmos.Converters;

internal class DateTimeTypedIdValueConverter<TTypedIdValue> : ValueConverter<TTypedIdValue, string>
   where TTypedIdValue : TypedId<DateTime>
{
    public DateTimeTypedIdValueConverter(ConverterMappingHints mappingHints = null)
        : base(id => id.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"), value => Create(DateTime.SpecifyKind(
                DateTime.Parse(value),
                DateTimeKind.Utc)),
              mappingHints)
    {
    }

    private static TTypedIdValue Create(DateTime id) => Activator.CreateInstance(typeof(TTypedIdValue), id) as TTypedIdValue;
}