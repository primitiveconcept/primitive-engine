namespace PrimitiveEngine
{
    using System;


    /// <summary>
    /// Class EntityTemplateAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class EntityTemplateAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityTemplateAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public EntityTemplateAttribute(string name)
        {
            this.Name = name;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }
        #endregion
    }
}