namespace PrimitiveEngine
{
    using System.Collections.Generic;


    /// <summary>
    /// The entity processing system - a template for processing many entities, tied to components.
    /// </summary>
    public abstract class EntityProcessingSystem : EntitySystem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityProcessingSystem" /> class.
        /// </summary>
        /// <param name="aspect">The aspect.</param>
        protected EntityProcessingSystem(Aspect aspect)
            : base(aspect)
        {
        }
        #endregion


        /// <summary>
        /// Processes the specified entity.
        /// Users might extend this method when they want
        /// to process the specified entities.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public abstract void Process(Entity entity);


        /// <summary>
        /// Processes the entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            foreach (Entity entity in entities.Values)
            {
                Process(entity);
            }
        }
    }
}