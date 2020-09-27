namespace PrimitiveEngine
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;


	public static class Attributes
	{
		public static IList<object> GetAttributes(this Type type)
		{
			List<object> attributes = new List<object>();
			foreach (object attribute in type.GetCustomAttributes(true))
			{
				attributes.Add(attribute);
			}

			return attributes;
		}


		public static IList<Type> GetTypesWithAttribute(Assembly assembly, Type attributeType)
		{
			List<Type> types = new List<Type>();
			foreach (Type type in assembly.GetTypes())
			{
				if (type.HasAttribute(attributeType))
					types.Add(type);
			}

			return types;
		}


		public static bool HasAttribute(this Type type, Type attribute)
		{
			return type.IsDefined(attribute, true);
		}
	}
}