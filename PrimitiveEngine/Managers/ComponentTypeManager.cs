namespace PrimitiveEngine
{
    using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
    using System.Numerics;
	

    /// <summary>
    /// Class ComponentTypeManager.
    /// </summary>
    public static class ComponentTypeManager
    {
        /// <summary>
        /// The component types.
        /// </summary>
        private static readonly Dictionary<Type, ComponentType> ComponentTypes = 
            new Dictionary<Type, ComponentType>();


        /// <summary>
        /// Gets the bit.
        /// </summary>
        /// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
        /// <returns>The bit flag register.</returns>
        public static BigInteger GetBit<T>() where T : IEntityComponent
        {
            return GetTypeFor<T>().Bit;
        }


        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
        /// <returns>The id.</returns>
        public static int GetId<T>() where T : IEntityComponent
        {
            return GetTypeFor<T>().Id;
        }


        /// <summary>
        /// Get the component type for the given component instance.
        /// </summary>
        /// <typeparam name="T">Component for which you want the component type.</typeparam>
        /// <returns>Component Type.</returns>
        public static ComponentType GetTypeFor<T>() where T : IEntityComponent
        {
            return GetTypeFor(typeof(T));
        }


        /// <summary>
        /// Ensure the given component type [tag] is an "official"
        /// component type for your solution. If it does not already
        /// exist, add it to the bag of available component types.
        /// This is a way you can easily add "official" component
        /// types to your solution.
        /// </summary>
        /// <param name="component">The component type label you want to ensure is an "official" component type</param>
        /// <returns>The specified ComponentType.</returns>
        public static ComponentType GetTypeFor(Type component)
        {
            Debug.Assert(component != null, "Component must not be null.");

            ComponentType result;
            if (!ComponentTypes.TryGetValue(component, out result))
            {
                result = new ComponentType();
                ComponentTypes.Add(component, result);
            }

            return result;
        }


        /// <summary>
        /// Scans assemblies for types implementing <see cref="IEntityComponent"/> interface
        /// and creates a corresponding <see cref="ComponentType"/> for each type found.
        /// </summary>
        /// <param name="assembliesToScan">The assemblies to scan.</param>
        public static void Initialize(params Assembly[] assembliesToScan)
        {
            foreach (Assembly assembly in assembliesToScan)
            {
                IEnumerable<Type> types = assembly.GetTypes();
                Initialize(types, ignoreInvalidTypes: true);
            }
        }


        /// <summary>
        /// Scans the types for types implementing <see cref="IEntityComponent"/> interface
        /// and creates a corresponding Artemis <see cref="ComponentType"/> for each type found.
        /// </summary>
        /// <param name="types">Types to scan</param>
        /// <param name="ignoreInvalidTypes">If set to <see langword="true" />, will not throw Exception</param>
        public static void Initialize(IEnumerable<Type> types, bool ignoreInvalidTypes = false)
        {
            foreach (Type type in types)
            {
                if (typeof(IEntityComponent).IsAssignableFrom(type))
                {
                    if (type.IsInterface)
                        continue;

                    if (type == typeof(ComponentPoolable))
                        continue;

                    GetTypeFor(type);
                }
                else if (!ignoreInvalidTypes)
                {
                    throw new ArgumentException(
                        $"Type {type} does not implement {typeof(IEntityComponent)} interface");
                }
            }
        }


        /// <summary>
        /// Creates an enumerable from a <c>BigIntger</c> which holds type bits.
        /// </summary>
        /// <param name="bits">The BigInteger which holds the type bits.</param>
        /// <returns>An Enumerable of each type the bits has.</returns>
        internal static IEnumerable<Type> GetTypesFromBits(BigInteger bits)
        {
            foreach (KeyValuePair<Type, ComponentType> keyValuePair in ComponentTypes)
            {
                if ((keyValuePair.Value.Bit & bits) != 0)
                    yield return keyValuePair.Key;
            }            
        }


        /// <summary>
        /// Sets the type for specified ComponentType T.
        /// </summary>
        /// <typeparam name="T">The <see langword="Type" /> of T being set.</typeparam>
        /// <param name="componentType">The component type.</param>
        internal static void SetTypeFor<T>(ComponentType componentType)
        {
            ComponentTypes.Add(typeof(T), componentType);
        }


        /// <summary>
        /// Sets the Type for specified ComponentType. 
        /// </summary>
        /// <param name="type">The type being set.</param>
        /// <param name="componentType">The component type.</param>
        internal static void SetTypeFor(Type type, ComponentType componentType)
        {
            ComponentTypes.Add(type, componentType);
        }
    }
}