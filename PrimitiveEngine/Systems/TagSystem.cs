namespace PrimitiveEngine
{
    using System.Diagnostics;


    /// <summary>
    /// Tag System does not fire ANY Events of the EntitySystem.
    /// </summary>
    public abstract class TagSystem : ProcessingSystem
    {
        private readonly string tag;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TagSystem"/> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        protected TagSystem(string tag)
        {
            Debug.Assert(!string.IsNullOrEmpty(tag), "Tag must not be null.");

            this.tag = tag;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets the tag.
        /// </summary>
        public string Tag
        {
            get { return this.tag; }
        }
        #endregion


        /// <summary>
        /// Processes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public abstract void Process(Entity entity);


        /// <summary>
        /// Called when [change].
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void OnEntityChanged(Entity entity)
        {
        }


        public override void ProcessSystem()
        {
            Entity entity = this.EntityWorld.TagManager.GetEntity(this.Tag);
            if (entity != null)
            {
                Process(entity);
            }
        }
    }
}