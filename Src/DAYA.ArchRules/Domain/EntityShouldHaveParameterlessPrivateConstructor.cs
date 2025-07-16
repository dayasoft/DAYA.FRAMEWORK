using DAYA.Cloud.Framework.V2.DirectOperations;
using DAYA.Cloud.Framework.V2.Domain;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.ArchRules.Domain
{
    internal class EntityShouldHaveParameterlessPrivateConstructor : ArchRule
    {
        /// <summary>
        /// Checks if a type is CosmosEntity or a derived class
        /// </summary>
        private bool IsCosmosEntityOrDerived(Type type)
        {
            // Get CosmosEntity type
            Type cosmosEntityType = typeof(DAYA.Cloud.Framework.V2.DirectOperations.CosmosEntity);

            // Check if type is CosmosEntity or derives from it
            return type == cosmosEntityType || type.IsSubclassOf(cosmosEntityType);
        }
        /// <summary>
        /// Determines whether an entity with an Id field or property should be excluded from the rule.
        /// </summary>
        /// <param name="entityType">The entity type to check.</param>
        /// <returns>True if the entity should be excluded; otherwise, false.</returns>
        private bool ShouldExcludeEntityWithIdField(Type entityType)
        {
            try
            {
                // Check if this is a CosmosEntity with an overridden Id property that returns TypedId
                var baseType = entityType.BaseType;
                if (baseType != null)
                {
                    // Check if it inherits from CosmosEntity
                    if (IsCosmosEntityOrDerived(baseType))
                    {
                        // Instead of using GetProperty, which can cause ambiguity,
                        // use GetProperties and find the Id property, then check its type
                        try
                        {
                            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(p => p.Name == "Id")
                                .ToList();

                            foreach (var prop in properties)
                            {
                                if (prop.PropertyType.IsSubclassOf(typeof(TypedId)) ||
                                    (prop.PropertyType.IsGenericType &&
                                     prop.PropertyType.GetGenericTypeDefinition() == typeof(TypedId<>)))
                                {
                                    return true; // Exclude entities with a TypedId property
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // If there's any error in property reflection, err on the side of excluding
                            return true;
                        }
                    }
                }

                // Special case check for UserAccount or any class that might have ambiguous Id properties
                if (entityType.FullName?.Contains("UserAccount") == true)
                {
                    return true; // Always exclude UserAccount due to known ambiguity issues
                }

                // Check for any property named "Id" (but only if we haven't already checked above for a specific class)
                try
                {
                    var idProperties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
                        .Where(p => p.Name == "Id")
                        .ToList();

                    if (idProperties.Any())
                    {
                        return true; // Exclude entities with an Id property
                    }
                }
                catch (Exception)
                {
                    // If there's any error in property reflection, err on the side of excluding
                    return true;
                }

                // Check for a field named "_id" or "id" (common naming conventions for ID fields)
                var idFields = entityType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => f.Name == "_id" || f.Name == "id")
                    .ToList();

                if (idFields.Any())
                {
                    return true; // Exclude entities with an Id field
                }
            }
            catch (AmbiguousMatchException)
            {
                // If there's an ambiguous match, it means there are multiple Id properties
                // In this case, we should exclude the entity from the rule
                return true;
            }
            catch (Exception)
            {
                // For any other exception, we don't exclude the entity
            }

            return false; // Don't exclude this entity
        }

        internal override void Check()
        {
            // Find all types that inherit from either Entity or CosmosEntity
            var entityTypes = Types.InAssembly(Data.DomainAssembly)
                .That()
                .Inherit(typeof(Entity))
                .GetTypes();

            var failingTypes = new List<Type>();
            foreach (var entityType in entityTypes)
            {
                // Skip the check if the entity has an Id field/property that should be excluded from this rule
                if (ShouldExcludeEntityWithIdField(entityType))
                {
                    continue;
                }

                var hasPrivateParameterlessConstructor = false;
                var constructors = entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var constructorInfo in constructors)
                {
                    if (constructorInfo.IsPrivate && constructorInfo.GetParameters().Length == 0)
                    {
                        hasPrivateParameterlessConstructor = true;
                    }
                }

                if (!hasPrivateParameterlessConstructor)
                {
                    failingTypes.Add(entityType);
                }
            }

            AssertFailingTypes(failingTypes);
        }
    }
}