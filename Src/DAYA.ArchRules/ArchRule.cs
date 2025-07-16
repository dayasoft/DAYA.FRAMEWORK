using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Azure.Core.GeoJson;
using DAYA.ArchRules.Utilities;
using DAYA.Cloud.Framework.V2.Application.Configuration.Commands;
using DAYA.Cloud.Framework.V2.Application.Configuration.Notifications;
using DAYA.Cloud.Framework.V2.Application.Configuration.Queries;
using DAYA.Cloud.Framework.V2.Application.Contracts;
using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.DirectOperations.Contracts;
using DAYA.Cloud.Framework.V2.Domain;
using DAYA.Cloud.Framework.V2.Infrastructure.EventBus;
using FluentValidation;
using NetArchTest.Rules;

namespace DAYA.ArchRules
{
    internal abstract class ArchRule
    {
        internal abstract void Check();

        internal virtual string Message => string.Empty;

        protected PredicateList DomainEvents =>
            Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(DomainEventBase))
                .Or()
                .Inherit(typeof(IDomainEvent));

        protected PredicateList ValueObjects =>
            Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(ValueObject));

        protected PredicateList TypedIds =>
            Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(TypedId));

        protected PredicateList Entities =>
            Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(Entity));

        protected HashSet<Type> DomainDefinedTypes =>
                Types.InAssembly(Data.DomainAssembly)
                   .GetTypes()
                   .Except(DomainEvents.GetTypes())
                   .Except(Entities.GetTypes())
                   .ToHashSet();

        protected PredicateList Queries => Types.InAssembly(Data.ApplicationAssembly)
            .That()
            .Inherit(typeof(Query<>)).Or()
            .Inherit(typeof(PageableQuery<>)).Or()
            .ImplementInterface(typeof(IQuery<>));

        protected PredicateList Commands => Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .Inherit(typeof(Command<>)).Or()
                .Inherit(typeof(Command)).Or()
                .Inherit(typeof(DirectCommand)).Or()
                .Inherit(typeof(DirectCommand<>)).Or()
                .ImplementInterface(typeof(ICommand)).Or()
                .ImplementInterface(typeof(ICommand<>)).Or()
                .ImplementInterface(typeof(IDirectCommand)).Or()
                .ImplementInterface(typeof(IDirectCommand<>));

        protected PredicateList CommandsWithResults => Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .Inherit(typeof(Command<>)).Or()
                .Inherit(typeof(DirectCommand<>)).Or()
                .ImplementInterface(typeof(ICommand<>)).Or()
                .ImplementInterface(typeof(IDirectCommand<>));

        protected IEnumerable<Type> QueryHandlersTypes => Types.InAssembly(Data.ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>)).Or()
            .ImplementInterface(typeof(IPageableQueryHandler<,>))
            .GetTypes();

        protected PredicateList QueryHandlers => Types.InAssembly(Data.ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IQueryHandler<,>)).Or()
            .ImplementInterface(typeof(IPageableQueryHandler<,>));

        protected IEnumerable<Type> NotificationHandlersTypes => Types.InAssembly(Data.ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IDomainNotificationHandler<>))
            .GetTypes();

        protected PredicateList NotificationHandlers => Types.InAssembly(Data.ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(IDomainNotificationHandler<>));

        protected IEnumerable<Type> QueryResults => GetQueryResultFromHandlers(QueryHandlersTypes);
        protected IEnumerable<Type> CommandResults => GetCommandsResults(CommandsWithResults.GetTypes());

        protected IEnumerable<Type> CommandHandlersTypes => Types.InAssembly(Data.ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Or()
            .ImplementInterface(typeof(IDirectCommandHandler<>))
            .Or()
            .ImplementInterface(typeof(IDirectCommandHandler<,>))
            .GetTypes();

        protected PredicateList CommandHandlers => Types.InAssembly(Data.ApplicationAssembly)
            .That()
            .ImplementInterface(typeof(ICommandHandler<>))
            .Or()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Or()
            .ImplementInterface(typeof(IDirectCommandHandler<>))
            .Or()
            .ImplementInterface(typeof(IDirectCommandHandler<,>));

        protected IEnumerable<Type> Validators => Types.InAssembly(Data.ApplicationAssembly)
                .That()
                .Inherit(typeof(AbstractValidator<>))
                .GetTypes();

        protected static IEnumerable<Type> GetCommandsResults(IEnumerable<Type> handlers)
        {
            if (!handlers.Any())
            {
                return Enumerable.Empty<Type>();
            }
            var queryInterfaces = new[]
            {
                typeof(ICommand<>),
                typeof(IDirectCommand<>),
            };
            var han = handlers.First().GetInterfaces().ToArray();
            return handlers
                .Select(x => x.GetInterfaces()
                    .First(x => queryInterfaces.Any(y => x.Name.Contains(y.Name)))
                    .GenericTypeArguments[0]);
        }

        protected static IEnumerable<Type> GetQueryResultFromHandlers(IEnumerable<Type> handlers)
        {
            var queryInterfaces = new[]
            {
                typeof(IQueryHandler<,>), typeof(IPageableQueryHandler<,>),
            };
            return handlers
                .Select(x => x.GetInterfaces()
                    .First(x => queryInterfaces.Any(y => x.Name.Contains(y.Name)))
                    .GenericTypeArguments[1]);
        }

        protected HashSet<Type> ApplicationDefinedTypes =>
             Types.InAssembly(Data.ApplicationAssembly)
                .GetTypes()
                .Except(Queries.GetTypes())
                .Except(QueryHandlersTypes)
                .Except(Commands.GetTypes())
                .Except(CommandHandlersTypes)
                .Except(Validators)
                .ToHashSet();

        protected PredicateList IntegrationEvents => Types.InAssembly(Data.IntegrationEventsAssembly)
                .That()
                .Inherit(typeof(IntegrationEvent));

        protected static Type[] AllTypes = Types.InAssemblies(new[]
            {
                Data.ApplicationAssembly,
                Data.DomainAssembly,
                Data.InfrastructureAssembly
            })
            .GetTypes()
            .ToArray();

        protected static IEnumerable<Type> GetQueriesFromHandlers(IEnumerable<Type> handlers)
        {
            var queryInterfaces = new[]
            {
                typeof(IQueryHandler<,>), typeof(IPageableQueryHandler<,>),
            };
            return handlers
                .Select(x => x.GetInterfaces()
                    .First(x => queryInterfaces.Any(y => x.Name.Contains(y.Name)))
                    .GenericTypeArguments[0]);
        }

        protected void AssertAreImmutable(IEnumerable<Type> types)
        {
            IList<Type> failingTypes = new List<Type>();
            foreach (var type in types)
            {
                if (!AssertIsImmutable(type))
                {
                    failingTypes.Add(type);
                    break;
                }
            }

            AssertFailingTypes(failingTypes);
        }

        private static bool AssertIsImmutable(Type type, int depth = 0)
        {
            if (depth >= 10)
            {
                throw new StackOverflowException();
            }
            if (type.IsPrimitive())
            {
                return true;
            }
            if (type.GetFields().Any(x => !x.IsInitOnly) || type.GetProperties().Any(x => x.CanWrite))
            {
                return false;
            }

            var ok = true;
            foreach (var item in type.GetFields().SelectMany(x => x.FieldType.GetUnderlyingTypes()))
            {
                ok &= AssertIsImmutable(item, depth + 1);
            }
            foreach (var item in type.GetProperties().SelectMany(x => x.PropertyType.GetUnderlyingTypes()))
            {
                ok &= AssertIsImmutable(item, depth + 1);
            }
            return ok;
        }

        protected Type GetArrayElementType(Type type)
        {
            if (type.Equals(typeof(string)))
            {
                return typeof(string);
            }
            return type.GetElementType();
        }

        protected bool IsNonStringEnumerable(Type type)
        {
            if (type == null)
            {
                return false;
            }
            return typeof(IEnumerable).IsAssignableFrom(type) ||
                typeof(IEnumerable<>).IsAssignableFrom(type);
        }

        protected Type GetCollectionElementType(Type type)
        {
            if (type.Equals(typeof(string)))
            {
                return typeof(string);
            }
            return type.GetGenericArguments()[0];
        }

        protected bool IsSimple(Type type)
        {
            if (type.IsArray)
            {
                return IsSimple(type.GetElementType());
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsSimple(type.GetGenericArguments()[0]);
            }
            if (type.IsGenericType && typeof(ICollection).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                bool ok = true;
                foreach (var genericType in type.GetGenericArguments())
                {
                    ok &= IsSimple(genericType);
                }
                return ok;
            }

            return type.IsPrimitive
                   || type.IsEnum
                   || type.Equals(typeof(string))
                   || type.Equals(typeof(DateTime))
                   || type.Equals(typeof(DateTimeOffset))
                   || type.Equals(typeof(int))
                   || type.Equals(typeof(long))
                   || type.Equals(typeof(byte))
                   || type.Equals(typeof(short))
                   || type.Equals(typeof(double))
                   || type.Equals(typeof(float))
                   || type.Equals(typeof(Guid))
                   || type.Equals(typeof(decimal));
        }

        protected bool IsSimpleOrUserDefined(Type type, HashSet<Type> typesSet)
        {
            if (type.IsArray)
            {
                return IsSimpleOrUserDefined(type.GetElementType(), typesSet);
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsSimpleOrUserDefined(type.GetGenericArguments()[0], typesSet);
            }

            if (type.IsGenericType && (typeof(ICollection).IsAssignableFrom(type.GetGenericTypeDefinition()) ||
                                       typeof(ICollection<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
            {
                bool ok = true;
                foreach (var genericType in type.GetGenericArguments())
                {
                    ok &= IsSimpleOrUserDefined(genericType, typesSet);
                }
                return ok;
            }
            if (type.IsGenericType && typeof(IReadOnlyCollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                bool ok = true;
                foreach (var genericType in type.GetGenericArguments())
                {
                    ok &= IsSimpleOrUserDefined(genericType, typesSet);
                }
                return ok;
            }

            return typesSet.Contains(type) ||
                type.IsPrimitive ||
                type.IsEnum ||
                type.Equals(typeof(string)) ||
                type.Equals(typeof(DateTime)) ||
                type.Equals(typeof(Guid)) ||
                type.Equals(typeof(decimal));
        }

        protected bool ContainsSimpleOrDefinedPropertiesOrFields(Type type, HashSet<Type> definedTypes, int depth = 0)
        {
            if (depth == 10)
            {
                throw new StackOverflowException();
            }

            if (type == typeof(GeoPoint))
            {
                return true;
            }

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;
            bool ok = true;
            var properties = type.GetProperties(bindingFlags);
            foreach (var property in properties)
            {
                if (property.Name == "EqualityContract")
                {
                    continue;
                }
                if (definedTypes.Contains(property.PropertyType))
                {
                    ok &= ContainsSimpleOrDefinedPropertiesOrFields(property.PropertyType, definedTypes, depth + 1);
                }
                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
                {
                    ok &= ContainsSimpleOrDefinedPropertiesOrFields(property.PropertyType, definedTypes, depth + 1);
                }
                else if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    ok &= ContainsSimpleOrDefinedPropertiesOrFields(property.PropertyType, definedTypes, depth + 1);
                }
                else
                {
                    ok &= IsSimpleOrUserDefined(property.PropertyType, definedTypes);
                }
            }
            var fields = type.GetFields(bindingFlags);
            foreach (var field in fields)
            {
                if (definedTypes.Contains(field.FieldType))
                {
                    ok &= ContainsSimpleOrDefinedPropertiesOrFields(field.FieldType, definedTypes, depth + 1);
                }
                else if (field.FieldType == typeof(GeoPoint))
                {
                    continue;
                }
                else
                {
                    ok &= IsSimpleOrUserDefined(field.FieldType, definedTypes);
                }
            }

            return ok;
        }

        protected bool ContainsFields(Type type, int depth = 0)
        {
            if (depth == 10)
            {
                throw new StackOverflowException();
            }
            if (!Types.InAssemblies(new[] { Data.ApplicationAssembly, Data.DomainAssembly, Data.InfrastructureAssembly })
                .GetTypes()
                .Any(x => x == type))
            {
                return false;
            }
            bool contains = false;

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;
            var enumerable = type.GetFields(bindingFlags)
                            .Where(x => x.Attributes == FieldAttributes.Private)
                            .Where(x => !x.CustomAttributes.Any())
                            .ToArray();
            if (enumerable.Any())
            {
                return true;
            }

            FieldInfo[] fieldInfos = type.GetFields().ToArray();
            if (fieldInfos.Any())
            {
                return true;
            }
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (!IsSimple(property.PropertyType))
                {
                    contains |= ContainsFields(property.PropertyType, depth + 1);
                }
            }
            return contains;
        }

        protected bool ContainsNonPublicPropertySetter(Type type, int depth = 0)
        {
            if (depth == 10)
            {
                throw new StackOverflowException();
            }
            if (!Types.InAssemblies(new[] { Data.ApplicationAssembly, Data.DomainAssembly, Data.InfrastructureAssembly })
                .GetTypes()
                .Any(x => x == type))
            {
                return false;
            }
            bool contains = false;

            const BindingFlags bindingFlags = BindingFlags.DeclaredOnly |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Public |
                                              BindingFlags.Instance;

            var publicProperties = type.GetProperties(bindingFlags);

            if (publicProperties.Any(x => !x.GetSetMethod(true).IsPublic))
            {
                return true;
            }

            var properties = type.GetProperties();
            var vos = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(ValueObject))
                .GetTypes();

            foreach (var property in properties)
            {
                if (vos.Contains(property.PropertyType))
                {
                    continue;
                }
                if (!IsSimple(property.PropertyType))
                {
                    contains |= ContainsNonPublicPropertySetter(property.PropertyType, depth + 1);
                }
            }
            return contains;
        }

        protected Graph BuildGraph(List<Type> types)
        {
            Graph graph = new Graph(types.Count);
            int from = 0;
            foreach (var type in types)
            {
                if (types.Contains(type.BaseType))
                {
                    int to = types.IndexOf(type.BaseType);
                    graph.AddEdge(from, to);
                }
                PropertyInfo[] ownedProperties = type.GetProperties(BindingFlags.Public |
                              BindingFlags.NonPublic |
                              BindingFlags.Instance |
                              BindingFlags.DeclaredOnly);
                foreach (var item in ownedProperties)
                {
                    var propertyType = item.PropertyType;
                    if (propertyType.IsArray)
                    {
                        propertyType = GetArrayElementType(propertyType);
                    }
                    else if (IsNonStringEnumerable(propertyType))
                    {
                        propertyType = GetCollectionElementType(propertyType);
                    }
                    if (types.Contains(propertyType))
                    {
                        int to = types.IndexOf(propertyType);
                        graph.AddEdge(from, to);
                    }
                }
                from++;
            }

            return graph;
        }

        protected void AssertFailingTypes(IEnumerable<Type> types)
        {
            var matchTypes = types?.Where(x => !RuleEngine.IgnoredRules.Any(i => i.ClassType == x.Name && i.RuleType == GetType().Name));
            if (matchTypes?.Any() ?? false)
            {
                throw new ArchitectureException("[" + string.Join(", ", matchTypes.Select(x => x.Name).Distinct()) + "]");
            }
        }

        protected void AssertArchTestResult(TestResult result)
        {
            AssertFailingTypes(result.FailingTypes);
        }

        protected void AssertArchTestResult(ConditionList conditionList)
        {
            AssertArchTestResult(conditionList.GetResult());
        }
    }

    internal static class StringUtility
    {
        public static string FieldName(this string field)
        {
            if (field.IndexOf('<') < 0)
            {
                return field;
            }
            return field.Substring(1, field.IndexOf('>') - 1);
        }
    }
}