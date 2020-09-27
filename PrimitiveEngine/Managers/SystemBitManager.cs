namespace PrimitiveEngine
{
	using System.Collections.Generic;
    using System.Numerics;


    /// <summary>
    /// Class SystemBitManager.
    /// </summary>
    internal class SystemBitManager
    {
        /// <summary>
        /// The system bits.
        /// </summary>
        private readonly Dictionary<EntitySystem, BigInteger> systemBits = new Dictionary<EntitySystem, BigInteger>();

        /// <summary>
        /// The position.
        /// </summary>
        private int position;


        /// <summary>
        /// Gets the bit-register for the specified entity system.
        /// </summary>
        /// <param name="entitySystem">The entity system.</param>
        /// <returns>The bit flag register for the specified system.</returns>
        public BigInteger GetBitFor(EntitySystem entitySystem)
        {
            BigInteger bit;
            if (this.systemBits.TryGetValue(entitySystem, out bit) == false)
            {
                bit = new BigInteger(1) << this.position;

                this.position++;
                this.systemBits.Add(entitySystem, bit);
            }

            return bit;
        }
    }
}