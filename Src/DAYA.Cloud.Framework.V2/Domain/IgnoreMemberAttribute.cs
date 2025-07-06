using System;

namespace DAYA.Cloud.Framework.V2.Domain;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IgnoreMemberAttribute : Attribute
{
}
