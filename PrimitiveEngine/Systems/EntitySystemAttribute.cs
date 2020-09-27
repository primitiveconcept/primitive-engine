namespace PrimitiveEngine
{
	using System;


    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class EntitySystemAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntitySystemAttribute"/> class.
        /// </summary>
        public EntitySystemAttribute()
        {
            this.UpdateType = UpdateType.FrameUpdate;
            this.Layer = 0;
            this.ExecutionType = ExecutionType.Synchronous;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the type of the execution.
        /// </summary>
        /// <value>The type of the execution.</value>
        public ExecutionType ExecutionType { get; set; }


        /// <summary>
        /// Gets or sets the type of the game loop.
        /// </summary>
        /// <value>The type of the game loop.</value>
        public UpdateType UpdateType { get; set; }


        /// <summary>Gets or sets the layer.</summary>
        /// <value>The layer.</value>
        public int Layer { get; set; }
        #endregion
    }
}