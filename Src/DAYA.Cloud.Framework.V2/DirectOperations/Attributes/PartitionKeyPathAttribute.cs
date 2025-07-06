using System;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PartitionKeyPathAttribute : Attribute
    {
        public string[] Paths { get; set; } = null!;

        public PartitionKeyPathAttribute(params string[] paths)
        {
            Paths = paths;
        }
    }
}