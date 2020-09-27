namespace PrimitiveEngine
{
    using System.Collections.Generic;
	using System.Threading;


    /// <summary>
    /// Class QueueManager.
    /// </summary>
    internal class QueueManager
    {
        private readonly object lockObject = new object();


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueManager"/> class.
        /// </summary>
        public QueueManager()
        {
            this.EntitiesToProcessEachFrame = 50;
            this.Queue = new Queue<Entity>();
            this.RefCount = 0;

            AcquireLock();
            ++this.RefCount;
            ReleaseLock();
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the entities to process each frame.
        /// </summary>
        /// <value>The entities to process each frame.</value>
        public int EntitiesToProcessEachFrame { get; set; }


        /// <summary>
        /// Gets or sets the queue.
        /// </summary>
        /// <value>The queue.</value>
        public Queue<Entity> Queue { get; set; }


        /// <summary>
        /// Gets or sets the ref count.
        /// </summary>
        /// <value>The ref count.</value>
        public int RefCount { get; set; }
        #endregion


        /// <summary>
        /// Acquires the lock.
        /// </summary>
        public void AcquireLock()
        {
            Monitor.Enter(this.lockObject);
        }


        /// <summary>
        /// Releases the lock.
        /// </summary>
        public void ReleaseLock()
        {
            Monitor.Exit(this.lockObject);
        }
    }

    /// <summary>
    /// Class QueueManager that is independent of the entity concept.
    /// </summary>
    /// <typeparam name="T">The Type T.</typeparam>
    internal class QueueManager<T>
    {
        private readonly object lockObject = new object();


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueManager{T}"/> class.
        /// </summary>
        public QueueManager()
        {
            this.EntitiesToProcessEachFrame = 50;
            this.Queue = new Queue<T>();
            this.RefCount = 0;

            AcquireLock();
            ++this.RefCount;
            ReleaseLock();
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the entities to process each frame.
        /// </summary>
        /// <value>The entities to process each frame.</value>
        public int EntitiesToProcessEachFrame { get; set; }


        /// <summary>
        /// Gets or sets the queue.
        /// </summary>
        /// <value>The queue.</value>
        public Queue<T> Queue { get; set; }


        /// <summary>
        /// Gets or sets the ref count.
        /// </summary>
        /// <value>The ref count.</value>
        public int RefCount { get; set; }
        #endregion


        /// <summary>
        /// Acquires the lock.
        /// </summary>
        public void AcquireLock()
        {
            Monitor.Enter(this.lockObject);
        }


        /// <summary>
        /// Releases the lock
        /// .</summary>
        public void ReleaseLock()
        {
            Monitor.Exit(this.lockObject);
        }
    }
}