using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.Domain;

namespace DAYA.ArchRules.Utilities
{
    internal static class TypeExtender
    {
        public static bool IsSimple(this Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType().IsSimple();
            }

            if (type.IsGenericType)
            {
                bool ok = true;
                foreach (var genericType in type.GetGenericArguments())
                {
                    ok &= genericType.IsSimple();
                }
                return ok;
            }

            return type.IsPrimitive();
        }

        public static bool IsPrimitive(this Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   || type.Equals(typeof(string))
                   || type.Equals(typeof(DateTime))
                   || type.Equals(typeof(DateTimeOffset))
                   || type.Equals(typeof(TimeSpan))
                   || type.Equals(typeof(Guid))
                   || type.Equals(typeof(double))
                   || type.Equals(typeof(int))
                   || type.Equals(typeof(short))
                   || type.Equals(typeof(ushort))
                   || type.Equals(typeof(long))
                   || type.Equals(typeof(ulong))
                   || type.Equals(typeof(uint))
                   || type.Equals(typeof(byte))
                   || type.Equals(typeof(decimal));
        }

        public static Type[] GetUnderlyingTypes(this Type type)
        {
            if (type.IsArray)
            {
                return new[] { type.GetElementType() };
            }

            if (type.IsGenericType)
            {
                return type.GetGenericArguments();
            }
            return new[] { type };
        }

        public static bool IsSimpleOrUserDefined(this Type type, HashSet<Type> typesSet)
        {
            if (type == typeof(TypedId))
            {
                return true;
            }
            if (type.IsArray)
            {
                return type.GetElementType().IsSimpleOrUserDefined(typesSet);
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0].IsSimpleOrUserDefined(typesSet);
            }

            if (type.IsCollection())
            {
                bool ok = true;
                foreach (var genericType in type.GetGenericArguments())
                {
                    ok &= genericType.IsSimpleOrUserDefined(typesSet);
                }
                return ok;
            }
            if (type.IsGenericType && (typeof(IReadOnlyCollection<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
            {
                bool ok = true;
                foreach (var genericType in type.GetGenericArguments())
                {
                    ok &= genericType.IsSimpleOrUserDefined(typesSet);
                }
                return ok;
            }

            return typesSet.Contains(type) ||
                type.IsPrimitive();
        }

        public static bool IsCollection(this Type type)
        {
            return type.IsGenericType && (typeof(ICollection).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                                       typeof(ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()));
        }

        public static bool IsList(this Type type)
        {
            return type.IsGenericType && (typeof(IList).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                                       typeof(IList<>).IsAssignableFrom(type.GetGenericTypeDefinition()));
        }

        public static bool ContainsSimpleOrDefinedProperties(this Type type, HashSet<Type> definedTypes, int depth = 0)
        {
            if (depth == 10)
            {
                throw new StackOverflowException();
            }

            if (definedTypes.Contains(type) || type.FullName.Contains("PagedSearchResult") || type.FullName.Contains("SuggestResultDto"))
            {
                return true;
            }

            if (type.IsArray)
            {
                return type.GetElementType().ContainsSimpleOrDefinedProperties(definedTypes, depth + 1);
            }

            bool ok = true;
            var properties = type.GetProperties();
            foreach (var property in properties.SelectMany(x => x.PropertyType.GetUnderlyingTypes()))
            {
                if (definedTypes.Contains(property))
                {
                    ok &= property.ContainsSimpleOrDefinedProperties(definedTypes, depth + 1);
                }
                else
                {
                    ok &= property.IsSimpleOrUserDefined(definedTypes);
                }
            }
            return ok;
        }

        public static bool IsInitOnly(this Type type, int depth = 0)
        {
            if (depth == 10)
            {
                throw new StackOverflowException();
            }
            if (type.IsPrimitive())
            {
                return true;
            }
            var ok = type
                .GetFields()
                .Where(x => !x.FieldType.IsPrimitive())
                .SelectMany(x => x.FieldType.GetUnderlyingTypes())
                .All(x => x.IsInitOnly(depth + 1));
            ok &= type
                .GetProperties()
                .Where(x => !x.PropertyType.IsPrimitive())
                .SelectMany(x => x.PropertyType.GetUnderlyingTypes())
                .All(x => x.IsInitOnly(depth + 1));

            return ok & !(type.GetFields().Any(x => !x.IsInitOnly) || type.GetProperties().Any(x => x.CanWrite && !x.IsInitOnly()));
        }

        public static bool IsInitOnly(this PropertyInfo property)
        {
            if (!property.CanWrite)
            {
                return false;
            }

            var setMethod = property.SetMethod;

            // Get the modifiers applied to the return parameter.
            var setMethodReturnParameterModifiers = setMethod.ReturnParameter.GetRequiredCustomModifiers();

            // Init-only properties are marked with the IsExternalInit type.
            return setMethodReturnParameterModifiers.Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
        }

        public static bool HaveAConstructorMatchesWithFieldsAndPropsNames(this Type type, BindingFlags constructorBinding = BindingFlags.Default, int depth = 0)
        {
            if (depth == 10)
            {
                throw new StackOverflowException();
            }
            var ok = type
                .GetFields()
                .Where(x => !x.FieldType.IsSimple())
                .SelectMany(x => x.FieldType.GetUnderlyingTypes())
                .All(x => x.HaveAConstructorMatchesWithFieldsAndPropsNames(constructorBinding, depth + 1));

            ok &= type
                .GetProperties()
                .Where(x => !x.PropertyType.IsSimple())
                .SelectMany(x => x.PropertyType.GetUnderlyingTypes())
                .All(x => x.HaveAConstructorMatchesWithFieldsAndPropsNames(constructorBinding, depth + 1));

            if (!ok)
            {
                return false;
            }

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;
            var names = type.GetFields(bindingFlags).Select(x => x.Name.ToLower()).ToList();
            var propertyNames = type.GetProperties(bindingFlags).Select(x => x.Name.ToLower()).ToList();
            names.AddRange(propertyNames);
            ConstructorInfo[] constructors;
            if (constructorBinding == BindingFlags.Default)
            {
                constructors = type.GetConstructors();
            }
            else
            {
                constructors = type.GetConstructors(constructorBinding);
            }
            foreach (var constructorInfo in constructors)
            {
                var parameters = constructorInfo.GetParameters().Select(x => x.Name.ToLower()).ToList();

                if (names.Intersect(parameters).Count() == names.Count)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HaveListProperty(this Type type, BindingFlags bindingFlags)
        {
            return type
                .GetProperties(bindingFlags)
                .Any(x => x.PropertyType.IsList());
        }

        public static bool HavePropertyMoreThan(this Type type, BindingFlags bindingFlags, int max)
        {
            return type
                .GetProperties(bindingFlags)
                .Count() > max;
        }

        public static bool HaveField(this Type type, BindingFlags bindingFlags)
        {
            return type.GetFields(bindingFlags).Any();
        }

        public static bool HavePropertyWithName(this Type type, BindingFlags bindingFlags, string name)
        {
            return type
                .GetProperties(bindingFlags)
                .Any(x => x.Name.ToLower() == name);
        }

        public static bool HavePropertyWithCondition(this Type type, BindingFlags bindingFlags, Func<PropertyInfo, bool> condition)
        {
            return type
                .GetProperties(bindingFlags)
                .Any(x => condition(x));
        }

        public static bool HaveDuplicateFieldType(this Type type, BindingFlags bindingFlags)
        {
            return type.GetFields(bindingFlags)
                .GroupBy(x => x.FieldType)
                .Any(x => x.Count() > 1);
        }

        public static bool ComplexPropertiesAndFieldsInheritDefiendTypes(this Type type, List<Type> definedTypes, BindingFlags bindingFlags, int depth = 0)
        {
            bool ok = true;
            if (depth == 10)
            {
                throw new StackOverflowException();
            }

            var fields = type.GetFields(bindingFlags).Where(x => !x.FieldType.IsSimple());
            var properties = type.GetProperties(bindingFlags).Where(x => !x.PropertyType.IsSimple());

            foreach (var field in fields)
            {
                if (definedTypes.Contains(field.FieldType))
                {
                    ok &= field.FieldType.ComplexPropertiesAndFieldsInheritDefiendTypes(definedTypes, bindingFlags, depth + 1);
                }
                else
                {
                    ok &= field.FieldType.CheckType(definedTypes, bindingFlags, depth + 1);
                }
            }

            foreach (var property in properties)
            {
                if (definedTypes.Contains(property.PropertyType))
                {
                    ok &= property.PropertyType.ComplexPropertiesAndFieldsInheritDefiendTypes(definedTypes, bindingFlags, depth + 1);
                    if (!ok)
                    {
                        Console.WriteLine("");
                    }
                }
                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
                {
                    ok &= property.PropertyType.ComplexPropertiesAndFieldsInheritDefiendTypes(definedTypes, bindingFlags, depth + 1);
                }
                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    ok &= property.PropertyType.ComplexPropertiesAndFieldsInheritDefiendTypes(definedTypes, bindingFlags, depth + 1);
                }
                else
                {
                    ok &= property.PropertyType.CheckType(definedTypes, bindingFlags, depth + 1);
                }
            }
            return ok;
        }

        public static bool CheckType(this Type type, List<Type> definedTypes, BindingFlags bindingFlags, int depth = 0)
        {
            if (type.IsArray)
            {
                if (definedTypes.Contains(type.GetElementType()))
                {
                    return type.GetElementType().ComplexPropertiesAndFieldsInheritDefiendTypes(definedTypes, bindingFlags, depth + 1);
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (definedTypes.Contains(type.GetGenericArguments()[0]))
                {
                    return type.GetGenericArguments()[0].ComplexPropertiesAndFieldsInheritDefiendTypes(definedTypes, bindingFlags, depth + 1);
                }
            }
            else if (type.IsGenericType && typeof(ICollection).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                foreach (var genericType in type.GetGenericArguments())
                {
                    if (definedTypes.Contains(genericType))
                    {
                        return genericType.ComplexPropertiesAndFieldsInheritDefiendTypes(definedTypes, bindingFlags, depth + 1);
                    }
                }
            }
            else if (type.IsGenericType && typeof(IReadOnlyCollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                foreach (var genericType in type.GetGenericArguments())
                {
                    if (definedTypes.Contains(genericType))
                    {
                        return genericType.ComplexPropertiesAndFieldsInheritDefiendTypes(definedTypes, bindingFlags, depth + 1);
                    }
                }
            }
            else
            {
                return false;
            }

            return false;
        }

        public static bool IsUpdateSearchCommandHandler(this Type type)
        {
            var commandHandler = type
                .GetInterfaces()
                .FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));

            return (commandHandler is not null) &&
                IsSubclassOfRawGeneric(typeof(UpdateSearchCommand<>), commandHandler.GetGenericArguments()[0]);
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}