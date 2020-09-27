namespace PrimitiveEngine
{
    using System;
	using System.Collections.Generic;


    /// <summary>
    /// Class IntervalEntityProcessingSystem.
    /// </summary>
    public abstract class IntervalEntityProcessingSystem : IntervalEntitySystem
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalEntityProcessingSystem" /> class.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="aspect">The aspect.</param>
        protected IntervalEntityProcessingSystem(TimeSpan timeSpan, Aspect aspect)
            : base(timeSpan, aspect)
        {
        }
        #endregion


        /// <summary>
        /// Processes the specified entity.
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