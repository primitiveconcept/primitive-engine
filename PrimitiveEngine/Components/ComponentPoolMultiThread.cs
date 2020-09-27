namespace PrimitiveEngine
{
    using System;

    
    /// <summary>
    /// A collection that maintains a set of class instance
    /// to allow for recycling instances and
    /// minimizing the effects of garbage collection.
    /// </summary>
    /// <typeparam name="T">The type of object to store in the Pool. Pools can only hold class types.</typeparam>
    public class ComponentPoolMultiThread<T> : IComponentPool<T>
        where T : ComponentPoolable
    {
        /// <summary>
        /// The pool.
        /// </summary>
        private readonly ComponentPool<T> pool;
        
        /// <summary>
        /// The sync.
        /// </summary>
        private readonly object sync;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentPoolMultiThread{T}"/> class.
        /// </summary>
        /// <param name="initialSize">The initial size.</param>
        /// <param name="resizePool">The resize pool.</param>
        /// <param name="resizes">if set to <see langword="true" /> [resizes].</param>
        /// <param name="allocateFunc">The allocate <see langword="Func" />.</param>
        /// <param name="innerType">Type of the inner.</param>
        public ComponentPoolMultiThread(int initialSize, int resizePool, bool resizes, Func<Type, T> allocateFunc, Type innerType)
        {
            this.pool = new ComponentPool<T>(initialSize, resizePool, resizes, allocateFunc, innerType);
            this.sync = new object();
        }

        /// <summary>
        /// Gets the number of invalid objects in the pool.
        /// </summary>
        /// <value>The invalid count.</value>
        public int InvalidCount
        {
            get
            {
                lock (this.sync) { return this.pool.InvalidCount; }
            }
        }

        /// <summary>
        /// Gets the resize amount.
        /// </summary>
        /// <value>The resize amount.</value>
        public int ResizeAmount
        {
            get
            {
                lock (this.sync) { return this.pool.ResizeAmount; }
            }
        }

        /// <summary>
        /// Gets the number of valid objects in the pool.
        /// </summary>
        /// <value>The valid count.</value>
        public int ValidCount
        {
            get
            {
                lock (this.sync) { return this.pool.ValidCount; }
            }
        }

        /// <summary>
        /// Returns a valid object at the given index. The index must fall in the range of [0, ValidCount].
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A valid object found at the index</returns>
        public T this[int index]
        {
            get
            {
                lock (this.sync) { return this.pool[index]; }
            }
        }

        /// <summary>
        /// Cleans up the pool by checking each valid object to ensure it is still actually
        /// valid. Must be called regularly to free returned Objects.
        /// </summary>
        public void CleanUp()
        {
            lock (this.sync)
            {
                this.pool.CleanUp();
            }
        }

        /// <summary>
        /// Returns a new object from the Pool.
        /// </summary>
        /// <returns>The next object in the pool if available, null if all instances are valid.</returns>
        public T New()
        {
            lock (this.sync) { return this.pool.New(); }
        }

        /// <summary>
        /// Return an object to the pool
        /// </summary>
        /// <param name="component">The component.</param>
        public void ReturnObject(T component)
        {
            lock (this.sync) { this.pool.ReturnObject(component); }
        }
    }
}