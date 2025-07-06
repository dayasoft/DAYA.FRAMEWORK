using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DAYA.Cloud.Framework.V2.Domain;

public abstract class ValueObject : IEquatable<ValueObject>
{
    private List<PropertyInfo> _properties;
    private List<FieldInfo> _fields;

    public static bool operator ==(ValueObject obj1, ValueObject obj2)
    {
        if (object.Equals(obj1, null))
        {
            if (object.Equals(obj2, null))
            {
                return true;
            }
            return false;
        }
        return obj1.Equals(obj2);
    }

    public static bool operator !=(ValueObject obj1, ValueObject obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(ValueObject obj)
    {
        return Equals(obj as object);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        return GetProperties().All(p => PropertiesAreEqual(obj, p))
            && GetFields().All(f => FieldsAreEqual(obj, f));
    }

    private bool PropertiesAreEqual(object obj, PropertyInfo p)
    {
        var type = p.PropertyType;
        if (type.IsArray)
        {
            var firstArray = p.GetValue(this, null) as Array;
            var secondArray = p.GetValue(obj, null) as Array;
            return CheckEquality(firstArray, secondArray);
        }
        if (type.IsGenericType && (typeof(ICollection).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                                   typeof(ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
        {
            var firstCollection = p.GetValue(this, null) as ICollection;
            var secondCollection = p.GetValue(obj, null) as ICollection;
            return CheckEquality(firstCollection, secondCollection);
        }
        return object.Equals(p.GetValue(this, null), p.GetValue(obj, null));
    }

    private static bool CheckEquality(ICollection first, ICollection second)
    {
        if (ReferenceEquals(first, second))
        {
            return true;
        }

        if (first == null || second == null || (first.Count != second.Count))
        {
            return false;
        }

        var firstEnumerator = first.GetEnumerator();
        var secondEnumerator = second.GetEnumerator();
        while (firstEnumerator.MoveNext() && secondEnumerator.MoveNext())
        {
            if (!object.Equals(firstEnumerator.Current, secondEnumerator.Current))
            {
                return false;
            }
        }

        return true;
    }

    private static bool CheckEquality(Array first, Array second)
    {
        if (ReferenceEquals(first, second))
        {
            return true;
        }

        if (first == null || second == null || (first.Length != second.Length))
        {
            return false;
        }

        for (int i = 0; i < first.Length; i++)
        {
            if (!object.Equals(first.GetValue(i), second.GetValue(i)))
            {
                return false;
            }
        }

        return true;
    }

    private bool FieldsAreEqual(object obj, FieldInfo f)
    {
        var type = f.FieldType;
        if (type.IsArray)
        {
            var firstArray = f.GetValue(this) as Array;
            var secondArray = f.GetValue(obj) as Array;
            return CheckEquality(firstArray, secondArray);
        }
        if (type.IsGenericType && (typeof(ICollection).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                                   typeof(ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
        {
            var firstCollection = f.GetValue(this) as ICollection;
            var secondCollection = f.GetValue(obj) as ICollection;
            return CheckEquality(firstCollection, secondCollection);
        }
        return object.Equals(f.GetValue(this), f.GetValue(obj));
    }

    private IEnumerable<PropertyInfo> GetProperties()
    {
        if (this._properties == null)
        {
            this._properties = GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.GetCustomAttribute(typeof(IgnoreMemberAttribute)) == null)
                .ToList();
        }

        return this._properties;
    }

    private IEnumerable<FieldInfo> GetFields()
    {
        if (this._fields == null)
        {
            this._fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.GetCustomAttribute(typeof(IgnoreMemberAttribute)) == null)
                .ToList();
        }

        return this._fields;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (var prop in GetProperties())
            {
                var value = prop.GetValue(this, null);
                hash = HashValue(hash, value);
            }

            foreach (var field in GetFields())
            {
                var value = field.GetValue(this);
                hash = HashValue(hash, value);
            }

            return hash;
        }
    }

    private static int HashValue(int seed, object value)
    {
        var currentHash = value?.GetHashCode() ?? 0;

        return seed * 23 + currentHash;
    }

    protected static async Task CheckRule(IBusinessRule rule)
    {
        if (await rule.IsBrokenAsync())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }
}
