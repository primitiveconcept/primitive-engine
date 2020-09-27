namespace PrimitiveEngine
{
    /// <summary>
    /// Class ComponentPool-able.
    /// </summary>
    public abstract class ComponentPoolable : IEntityComponent
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentPoolable"/> class.
        /// </summary>
        public ComponentPoolable()
        {
            this.PoolId = 0;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the pool id.
        /// </summary>
        /// <value>The pool id.</value>
        internal int PoolId { get; set; }
        #endregion


        /// <summary>
        /// Cleans up.
        /// </summary>
        public virtual void CleanUp()
        {
        }


        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
        }
    }
}