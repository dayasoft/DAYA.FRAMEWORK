using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Daya.CodeGenerator;

	internal static class TypeExtender
	{
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
				   || type.Equals(typeof(decimal))
				   || typeof(IFormFile).IsAssignableFrom(type)
				   || typeof(IList).IsAssignableFrom(type)
				   || typeof(IDictionary).IsAssignableFrom(type);
		}

		public static Type GetUnderlyingType(this Type type)
		{
			if (type.IsArray)
			{
				return type.GetElementType();
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return type.GetGenericArguments()[0];
			}
			if (type.IsGenericType && typeof(ICollection).IsAssignableFrom(type.GetGenericTypeDefinition()))
			{
				bool ok = true;
				foreach (var genericType in type.GetGenericArguments())
				{
					ok &= genericType.IsSimple();
				}

				try
				{
					return type.GetGenericArguments().Single();
				}
				catch (Exception e)
				{
					Console.WriteLine("Exception (Handled):");
					Console.WriteLine(e.ToString());
					var types = type.GetGenericArguments().ToList();
					return types.First();
				}
			}
			if (type.IsGenericType && typeof(IReadOnlyCollection<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
			{
				bool ok = true;
				foreach (var genericType in type.GetGenericArguments())
				{
					ok &= genericType.IsSimple();
				}
				return type.GetGenericArguments().Single();
			}
			return type;
		}

		public static bool IsSimple(this Type type)
		{
			if (type.IsArray)
			{
				return type.GetElementType().IsSimple();
			}

			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return type.GetGenericArguments()[0].IsSimple();
			}
			if (type.IsGenericType && typeof(ICollection).IsAssignableFrom(type.GetGenericTypeDefinition()))
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
	}