namespace PrimitiveEngine
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;


	/// <summary>
	/// Class AttributesProcessor.
	/// </summary>
	public class AttributesProcessor
	{
		/// <summary>
		/// The supported attributes.
		/// </summary>
		public static readonly List<Type> SupportedAttributes =
			new List<Type>
			{
				typeof(EntitySystemAttribute),
				typeof(EntityTemplateAttribute),
				typeof(ComponentPoolAttribute),
				typeof(ComponentCreate)
			};


		/// <summary>
		/// Processes the specified supported attributes.
		/// </summary>
		/// <param name="supportedAttributes">The supported attributes.</param>
		/// <param name="assembliesToScan">The assemblies to scan.</param>
		/// <returns>The Dictionary{TypeList{Attribute}}.</returns>
		public static IDictionary<Type, List<Attribute>> Process(
			List<Type> supportedAttributes,
			IEnumerable<Assembly> assembliesToScan) // Do not double overload "= null)"
		{
			IDictionary<Type, List<Attribute>> attributeTypes = new Dictionary<Type, List<Attribute>>();
			if (assembliesToScan != null)
			{
				foreach (Assembly item in assembliesToScan)
				{
					IEnumerable<Type> types = item.GetTypes();
					foreach (Type type in types)
					{
						object[] attributes = type.GetCustomAttributes(false);
						foreach (object attribute in attributes)
						{
							if (!supportedAttributes.Contains(attribute.GetType()))
								continue;
							
							if (!attributeTypes.ContainsKey(type))
								attributeTypes[type] = new List<Attribute>();
							attributeTypes[type].Add((Attribute)attribute);
						}
					}
				}
			}

			return attributeTypes;
		}
	}
}