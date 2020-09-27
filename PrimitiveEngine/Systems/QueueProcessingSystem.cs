namespace PrimitiveEngine
{
    using System.Collections.Generic;


    /// <summary>
    /// System not based on Components.
    /// It processes ONCE everything you explicitly add to it using the method AddToQueue.
    /// Use <see cref="EntitiesToProcessEachFrame"/> property to set processing batch size.
    /// </summary>
    public abstract class QueueProcessingSystem : ProcessingSystem
    {
        private readonly Queue<Entity> queue;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueProcessingSystem"/> class.
        /// </summary>
        public QueueProcessingSystem()
        {
            this.EntitiesToProcessEachFrame = 50;
            this.queue = new Queue<Entity>();
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the entities to process each frame.
        /// </summary>
        /// <value>The entities to process each frame.</value>
        public int EntitiesToProcessEachFrame { get; set; }


        /// <summary>
        /// Gets the queue count.
        /// </summary>
        /// <value>The queue count.</value>
        public int QueueCount
        {
            get
            {
                return this.queue.Count;
            }
        }
        #endregion


        /// <summary>
        /// Processes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public abstract void Process(Entity entity);


        /// <summary>
        /// Adds to queue.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void AddToQueue(Entity entity)
        {
            this.queue.Enqueue(entity);
        }


        public override void ProcessSystem()
        {
            int size = this.queue.Count > this.EntitiesToProcessEachFrame ? this.EntitiesToProcessEachFrame : this.queue.Count;
            for (int index = 0; index < size; ++index)
            {
                Process(this.queue.Dequeue());
            }
        }
    }
}