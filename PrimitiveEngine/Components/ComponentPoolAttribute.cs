namespace PrimitiveEngine
{
    using System;


    /// <summary>
    /// Class ArtemisComponentPool.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ComponentPoolAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentPoolAttribute"/> class.
        /// </summary>
        public ComponentPoolAttribute()
        {
            this.InitialSize = 10;
            this.ResizeSize = 10;
            this.IsResizable = true;
            this.IsSupportMultiThread = false;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the initial size of the Pool. Default is 10.
        /// </summary>
        /// <value>The initial size.</value>
        public int InitialSize { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the pool is resizable.
        /// </summary>
        /// <value><see langword="true" /> if the pool is resizable; otherwise, <see langword="false" />.</value>
        public bool IsResizable { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this instance should support
        /// multi thread environment. Default is <see langword="false" />.
        /// </summary>
        /// <value><see langword="true" /> if this instance should support multi thread environment; otherwise, <see langword="false" />.</value>
        public bool IsSupportMultiThread { get; set; }


        /// <summary>
        /// Gets or sets the size of the pool resize. Default is 10.
        /// </summary>
        /// <value>The size of the resize.</value>
        public int ResizeSize { get; set; }
        #endregion
    }
}