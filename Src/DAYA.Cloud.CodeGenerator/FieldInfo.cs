using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Daya.CodeGenerator;

internal class FieldInfo
{
    public string Type { get; set; }
    public string Name { get; set; }
    public bool IsTypedId { get; set; }
    public bool IsArray { get; set; }
    public bool IsList { get; set; }
    public bool IsNullable { get; set; }
    public string TypedIdName { get; set; }
    public List<string> Namespaces { get; set; }
    private static int _count = 0;

    public string GetDefinition()
    {
        _count = 0;
        try
        {
            if (Name.ToLower().Contains("email") && !IsList && !IsArray && Type == "String")
            {
                return $"private {GetTypeName()} _{Name} = $\"pre{{Guid.NewGuid()}}@somedomain.com\";";
            }
            return $"private {GetTypeName()} _{Name} = {DefaultValue()};";
        }
        catch (StackOverflowException)
        {
            return $"private {GetTypeName()} _{Name} = null;";
        }
    }

    private string DefaultValue()
    {
        if (IsNullable)
        {
            if (IsTypedId)
            {
                return $"new {TypedIdName}Builder().Build()";
            }
        }
        if (IsArray || IsList)
        {
            return $"new {GetTypeName()}()";
        }
        return Type switch
        {
            "Int32" => "2",
            "DateTime" => "DateTime.UtcNow",
            "DateTimeOffset" => "DateTimeOffset.UtcNow",
            "Int64" => "2L",
            "Decimal" => "2.0m",
            "Double" => "2.0",
            "Boolean" => "false",
            "Guid" => "Guid.NewGuid()",
            "String" => "Guid.NewGuid().ToString().Substring(0, 18)",
            _ => DefaultValue(Type),
        };
    }

    internal string GetTypeName()
    {
        if (IsNullable)
        {
            if (IsTypedId)
            {
                return TypedIdName + "?";
            }
        }
        var underLyingTypeName = GetUnderLyingTypeName();
        if (IsArray || IsList)
        {
            return $"List<{underLyingTypeName}>";
        }
        return underLyingTypeName + (IsNullable ? "?" : "");
    }

    public string GetUnderLyingTypeName()
    {
        return Type switch
        {
            "Int32" => "int",
            "DateTime" => "DateTime",
            "Int64" => "long",
            "Double" => "double",
            "Boolean" => "bool",
            "Guid" => "Guid",
            "Decimal" => "decimal",
            "String" => "string",
            _ => Type,
        };
    }

    private static string DefaultValue(string type)
    {
        var domainTypes = Generator.DomainAssembly.GetTypes().Select(x => x.Name);
        if (domainTypes.Contains(type))
        {
            var underlyingType = Generator.DomainAssembly.GetTypes().First(x => x.Name == type);
            if (underlyingType.IsEnum)
            {
                return $"({underlyingType.Name})1";
            }
            if (underlyingType.IsInterface)
            {
                return "null";
            }
            if (Generator.AggregateType.IsAssignableFrom(underlyingType))
            {
                return $"new {underlyingType.Name}Builder().Build()";
            }
            if (Generator.ValueObjectType.IsAssignableFrom(underlyingType))
            {
                return $"new {underlyingType.Name}Builder().Build()";
            }
            var factoryMethod = underlyingType.GetMethods(BindingFlags.Public | BindingFlags.Static).First(x => !x.IsSpecialName);

            var paramNames = Generator.GetFieldNameTypes(factoryMethod.GetParameters());

            string v = $"{type}.{factoryMethod.Name}({string.Join(", ", paramNames.Select(x => x.GetDefaultValue()))})";
            return v;
        }
        if (Generator.ApplicationAssembly.GetTypes().Select(x => x.Name).Contains(type))
        {
            var underlyingType = Generator.ApplicationAssembly.GetTypes().First(x => x.Name == type);
            if (underlyingType.IsEnum)
            {
                return $"({underlyingType.Name})1";
            }
            if (underlyingType.IsInterface)
            {
                return "null";
            }
            return $"new {underlyingType.Name}Builder().Build()";
        }
        if (type.Contains("[]"))
        {
            return $"new {type} {{ }}";
        }

        throw new NotSupportedException();
    }

    public string PascalName()
    {
        return Name.Substring(0, 1).ToUpper() + Name[1..];
    }

    public string GetParameterDefinition()
    {
        if (IsNullable)
        {
            return "_" + Name;
        }
        if (IsTypedId)
        {
            return $"new {TypedIdName}(_{Name})";
        }
        return "_" + Name;
    }

    public string GetDefaultValue()
    {
        if (_count++ >= 100)
        {
            throw new StackOverflowException();
        }
        if (IsNullable)
        {
            return "null";
        }
        if (IsTypedId)
        {
            return $"new {TypedIdName}({DefaultValue()})";
        }
        return DefaultValue();
    }

    public override string ToString()
    {
        return $"{Type} {Name} " + (IsTypedId ? "(TypedId)" : "");
    }
}