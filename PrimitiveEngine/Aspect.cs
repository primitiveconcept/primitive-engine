namespace PrimitiveEngine
{
    using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Numerics;
	using System.Text;
    

    /// <summary>
    /// Specify a Filter class to filter what Entities (with what Components)
    /// an EntitySystem will Process.
    /// </summary>
    public class Aspect
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Aspect"/> class.
        /// </summary>
        protected Aspect()
        {
            this.OneTypesMap = 0;
            this.ExcludeTypesMap = 0;
            this.ContainsTypesMap = 0;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the contains types map.
        /// </summary>
        /// <value>The contains types map.</value>
        protected BigInteger ContainsTypesMap { get; set; }


        /// <summary>
        /// Gets or sets the exclude types map.
        /// </summary>
        /// <value>The exclude types map.</value>
        protected BigInteger ExcludeTypesMap { get; set; }


        /// <summary>
        /// Gets or sets the one types map.
        /// </summary>
        /// <value>The one types map.</value>
        protected BigInteger OneTypesMap { get; set; }
        #endregion


        /// <summary>
        /// All the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>The specified Aspect.</returns>
        public static Aspect All(params Type[] types)
        {
            return new Aspect().GetAll(types);
        }


        /// <summary>
        /// Returns an Empty Aspect (does not filter anything - i.e. rejects everything).
        /// </summary>
        /// <returns>The Aspect.</returns>
        public static Aspect Empty()
        {
            return new Aspect();
        }


        /// <summary>
        /// Excludes the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>The specified Aspect.</returns>
        public static Aspect Exclude(params Type[] types)
        {
            return new Aspect().GetExclude(types);
        }


        /// <summary>
        /// Ones the specified types.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>The specified Aspect.</returns>
        public static Aspect One(params Type[] types)
        {
            return new Aspect().GetOne(types);
        }


        /// <summary>
        /// Gets all.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>The specified Aspect.</returns>
        public Aspect GetAll(params Type[] types)
        {
            Debug.Assert(types != null, "Types must not be null.");

            foreach (ComponentType componentType in types.Select(ComponentTypeManager.GetTypeFor))
            {
                this.ContainsTypesMap |= componentType.Bit;
            }

            return this;
        }


        /// <summary>
        /// Gets the exclude.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>The specified Aspect.</returns>
        public Aspect GetExclude(params Type[] types)
        {
            Debug.Assert(types != null, "Types must not be null.");

            foreach (ComponentType componentType in types.Select(ComponentTypeManager.GetTypeFor))
            {
                this.ExcludeTypesMap |= componentType.Bit;
            }

            return this;
        }


        /// <summary>
        /// Gets the one.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns>The specified Aspect.</returns>
        public Aspect GetOne(params Type[] types)
        {
            Debug.Assert(types != null, "Types must not be null.");

            foreach (ComponentType componentType in types.Select(ComponentTypeManager.GetTypeFor))
            {
                this.OneTypesMap |= componentType.Bit;
            }

            return this;
        }


        /// <summary>
        /// Called by the EntitySystem to determine if the system is interested in the passed Entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public virtual bool Interests(Entity entity)
        {
            Debug.Assert(entity != null, "Entity must not be null.");
            
            if (!(this.ContainsTypesMap > 0 || this.ExcludeTypesMap > 0 || this.OneTypesMap > 0))
            {
                return false;
            }

            ////Little help
            ////10010 & 10000 = 10000
            ////10010 | 10000 = 10010
            ////10010 | 01000 = 11010

            ////1001 & 0000 = 0000 OK
            ////1001 & 0100 = 0000 NOK           
            ////0011 & 1001 = 0001 Ok

            return ((this.OneTypesMap      & entity.TypeBits) != 0                     || this.OneTypesMap      == 0) &&
                   ((this.ContainsTypesMap & entity.TypeBits) == this.ContainsTypesMap || this.ContainsTypesMap == 0) &&
                   ((this.ExcludeTypesMap  & entity.TypeBits) == 0                     || this.ExcludeTypesMap  == 0);
        }


        /// <summary>
        /// Creates a string that displays all the type names of the components
        /// that interests this Aspect.
        /// </summary>
        /// <returns>A string displaying all the type names that interests this Aspect.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(1024);

            builder.AppendLine("Aspect :");
            AppendTypes(builder, " Requires the components : ", this.ContainsTypesMap);
            AppendTypes(builder, " Has none of the components : ", this.ExcludeTypesMap);
            AppendTypes(builder, " Has atleast one of the components : ", this.OneTypesMap);

            return builder.ToString();
        }


        /// <summary>
        /// Appends the types.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="headerMessage">The header message.</param>
        /// <param name="typeBits">The type bits.</param>
        private static void AppendTypes(StringBuilder builder, string headerMessage, BigInteger typeBits)
        {
            if (typeBits != 0)
            {
                builder.AppendLine(headerMessage);
                foreach (Type type in ComponentTypeManager.GetTypesFromBits(typeBits))
                {
                    builder.Append(", ");
                    builder.AppendLine(type.Name);
                }
            }
        }
    }
}
