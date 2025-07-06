using DAYA.Cloud.Framework.V2.DirectOperations.Attributes;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Helpers
{
    public static class PartitionKeyHelper
    {
        public static PartitionKey GetPartitionKey<T>(T instance)
            where T : class
        {
            var partitionKeyPaths = typeof(T).GetCustomAttribute<PartitionKeyPathAttribute>()?.Paths;

            if (partitionKeyPaths is not { Length: > 0 })
            {
                throw new ArgumentNullException($"'{nameof(PartitionKeyPathAttribute)}' not found for '{nameof(T)}' class or Partition Key '{nameof(PartitionKeyPathAttribute.Paths)}' is null or empty");
            }

            var props = typeof(T)
                .GetProperties()
                .Where(propertyInfo => partitionKeyPaths.Any(partitionKeyPath => string.Equals(propertyInfo.Name, partitionKeyPath, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();

            foreach (var partitionKeyPath in partitionKeyPaths)
            {
                if (props.All(x => string.Equals(x.Name, partitionKeyPath, StringComparison.InvariantCultureIgnoreCase) == false))
                {
                    throw new ArgumentNullException($"'{partitionKeyPath}' defined as partition key path, but not exists in '{nameof(T)}' type");
                }
            }

            Dictionary<string, int> indexMap = partitionKeyPaths
                .Select(x => x.ToLower())
                .Select((value, index) => new { value, index })
                .ToDictionary(x => x.value, x => x.index);

            // Sort listB based on the indices in the dictionary
            props = props
                .OrderBy(item => indexMap[item.Name.ToLower()])
                .ToList();

            var partitionKeyBuilder = new PartitionKeyBuilder();
            foreach (var propertyInfo in props)
            {
                var propValue = propertyInfo.GetValue(instance)?.ToString();
                if (string.IsNullOrWhiteSpace(propValue))
                {
                    throw new ArgumentNullException($"'{propertyInfo.Name}' value is null or empty and can not be used as part of partition key");
                }
                partitionKeyBuilder.Add(propValue);
            }

            return partitionKeyBuilder.Build();
        }

        public static PartitionKey GetPartitionKey(params string[] partitionKeyValues)
        {
            if (partitionKeyValues.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentNullException($"Partition key value can not be null or empty");
            }

            var partitionKeyBuilder = new PartitionKeyBuilder();
            foreach (var partitionKeyValue in partitionKeyValues)
            {
                partitionKeyBuilder.Add(partitionKeyValue);
            }
            return partitionKeyBuilder.Build();
        }

        public static string[] GetPartitionPaths<T>()
            where T : class
        {
            var partitionKeyPaths = typeof(T).GetCustomAttribute<PartitionKeyPathAttribute>()?.Paths;

            if (partitionKeyPaths is not { Length: > 0 })
            {
                throw new ArgumentNullException($"'{nameof(PartitionKeyPathAttribute)}' not found for '{typeof(T).Name}' class or Partition Key '{nameof(PartitionKeyPathAttribute.Paths)}' is null or empty");
            }

            return partitionKeyPaths;
        }

        public static string[] GetPartitionPaths(Type containerType)
        {
            var partitionKeyPaths = containerType.GetCustomAttribute<PartitionKeyPathAttribute>()?.Paths;

            if (partitionKeyPaths is not { Length: > 0 })
            {
                throw new ArgumentNullException($"'{nameof(PartitionKeyPathAttribute)}' not found for '{nameof(containerType)}' class or Partition Key '{nameof(PartitionKeyPathAttribute.Paths)}' is null or empty");
            }

            return partitionKeyPaths;
        }
    }
}