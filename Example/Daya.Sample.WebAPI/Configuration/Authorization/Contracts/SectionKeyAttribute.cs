using System;

namespace Daya.Sample.API.Configuration.Authorization.Contracts;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SectionKeyAttribute : Attribute
{
	public string Key { get; set; }

	public SectionKeyAttribute(string key)
	{
		Key = key;
	}
}