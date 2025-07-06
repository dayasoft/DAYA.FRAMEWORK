using System;

namespace DAYA.Cloud.Framework.V2.Domain;

internal static class TypedIdCreator
{
    public static Func<string, Type, object?> Create => (s, type) =>
    {
        var genericType = type.BaseType.GenericTypeArguments[0];
        var typedIdGenericType = typeof(TypedId<>).MakeGenericType(genericType);
        var typedIdObject = Activator.CreateInstance(
            typedIdGenericType,
            new object[] { System.Text.Json.JsonSerializer.Deserialize($"\"{s}\"", genericType) });
        var wrappedTypedIdObj = Activator.CreateInstance(
            type,
            typedIdGenericType.GetProperty("Value").GetValue(typedIdObject, null));

        return wrappedTypedIdObj;
    };
}
