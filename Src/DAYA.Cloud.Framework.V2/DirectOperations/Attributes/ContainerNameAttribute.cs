using System;

namespace DAYA.Cloud.Framework.V2.DirectOperations.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ContainerNameAttribute : Attribute
    {
        public string ContainerName { get; set; } = null!;

        public ContainerNameAttribute(string containerName)
        {
            ContainerName = containerName;
        }
    }
}