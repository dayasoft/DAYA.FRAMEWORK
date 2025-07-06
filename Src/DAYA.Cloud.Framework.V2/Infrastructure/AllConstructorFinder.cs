using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace DAYA.Cloud.Framework.V2.Infrastructure;

public class AllConstructorFinder
{
	private static readonly ConcurrentDictionary<Type, ConstructorInfo[]> _cache =
		new();

	public static ConstructorInfo[] FindConstructors(Type targetType)
	{
		var constructors = _cache.GetOrAdd(targetType,
			t => t.GetTypeInfo().DeclaredConstructors.ToArray());

		return constructors.Length > 0 ? constructors : throw new NoConstructorsFoundException(targetType);
	}
}

public class NoConstructorsFoundException : Exception
{
	public NoConstructorsFoundException(Type targetType)
		: base($"No constructors found for type {targetType.FullName}")
	{
		TargetType = targetType;
	}

	public Type TargetType { get; }
}