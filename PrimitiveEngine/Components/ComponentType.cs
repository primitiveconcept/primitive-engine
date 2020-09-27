namespace PrimitiveEngine
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Numerics;


    /// <summary>
    /// Represents a Component Type.
    /// </summary>
    [DebuggerDisplay("Id:{Id}, Bit:{Bit}")]
    public sealed class ComponentType
    {
        private static BigInteger nextBit;
        private static int nextId;
        private static IList<Type> allTypes;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentType"/> class.
        /// </summary>
        internal ComponentType()
        {
            this.Id = nextId;
            this.Bit = nextBit;

            nextId++;
            nextBit <<= 1;
        }


        /// <summary>
        /// Initializes static members of the <see cref="ComponentType"/> class.
        /// </summary>
        static ComponentType()
        {
            nextBit = 1;
            nextId = 0;
        }
        #endregion


        #region Properties
        public static IList<Type> AllTypes
        {
            get
            {
                if (allTypes == null)
                    allTypes = new List<Type>();
                return allTypes;
            }
        }


        /// <summary>Gets the bit that represents this type of component.</summary>
        /// <value>The bit.</value>
        public BigInteger Bit { get; private set; }


        /// <summary>Gets the bit index that represents this type of component.</summary>
        /// <value>The id.</value>
        public int Id { get; private set; }
        #endregion
    }


    /// <summary>
    /// The component type class.
    /// </summary>
    /// <typeparam name="T">The Type T.</typeparam>
    internal static class ComponentType<T>
        where T : IEntityComponent
    {
        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="ComponentType{T}"/> class.
        /// </summary>
        static ComponentType()
        {
            CType = ComponentTypeManager.GetTypeFor<T>();
            if (CType == null)
            {
                CType = new ComponentType();
                ComponentTypeManager.SetTypeFor<T>(CType);
            }

            ComponentType.AllTypes.Add(
                MethodBase.GetCurrentMethod().DeclaringType);
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets the type of the C.
        /// </summary>
        /// <value>The type of the C.</value>
        // ReSharper disable once StaticMemberInGenericType
        public static ComponentType CType { get; }
        #endregion
    }
}