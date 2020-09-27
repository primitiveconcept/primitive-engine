namespace PrimitiveEngine
{
    using System.Collections.Generic;


    /// <summary>
    /// Class DelayedEntityProcessingSystem.
    /// </summary>
    public abstract class DelayedEntityProcessingSystem : DelayedEntitySystem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedEntityProcessingSystem" /> class.
        /// </summary>
        /// <param name="aspect">The aspect.</param>
        protected DelayedEntityProcessingSystem(Aspect aspect)
            : base(aspect)
        {
        }
        #endregion


        /// <summary>
        /// Process an entity this system is interested in.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="accumulatedDelta">The entity to process.</param>
        public abstract void Process(Entity entity, long accumulatedDelta);


        /// <summary>
        /// Process all entities with the delayed Entity processing system.
        /// </summary>
        /// <param name="entities">Entities to process</param>
        /// <param name="accumulatedDelta">Total Delay</param>
        public override void ProcessEntities(IDictionary<int, Entity> entities, long accumulatedDelta)
        {
            foreach (Entity item in entities.Values)
            {
                Process(item, accumulatedDelta);
            }
        }
    }
}