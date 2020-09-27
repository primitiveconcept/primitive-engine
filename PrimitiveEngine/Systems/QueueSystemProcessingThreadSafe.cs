namespace PrimitiveEngine
{
    using System;
	using System.Collections.Generic;


    /// <summary>
    /// Queue system not based on components.
    /// It Process ONCE everything you explicitly add to it
    /// using the method AddToQueue.
    /// </summary>
    public abstract class QueueSystemProcessingThreadSafe : EntitySystem
    {
        private static readonly Dictionary<Type, QueueManager> QueuesManager = new Dictionary<Type, QueueManager>();

        /// <summary>
        /// The id.
        /// </summary>
        public readonly Type Id;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueSystemProcessingThreadSafe"/> class.
        /// </summary>
        protected QueueSystemProcessingThreadSafe()
        {
            this.Id = GetType();
            if (!QueuesManager.ContainsKey(this.Id))
            {
                QueuesManager[this.Id] = new QueueManager();
            }
            else
            {
                QueuesManager[this.Id].AcquireLock();
                ++QueuesManager[this.Id].RefCount;
                QueuesManager[this.Id].ReleaseLock();
            }
        }
        #endregion


        /// <summary>
        /// Adds to queue.
        /// </summary>
        /// <param name="ent">The entity.</param>
        /// <param name="entitySystemType">Type of the entity system.</param>
        public static void AddToQueue(Entity ent, Type entitySystemType)
        {
            QueueManager queueManager = QueuesManager[entitySystemType];
            queueManager.AcquireLock();
            queueManager.Queue.Enqueue(ent);
            queueManager.ReleaseLock();
        }


        /// <summary>
        /// Gets the queue processing limit.
        /// </summary>
        /// <param name="entitySystemType">Type of the entity system.</param>
        /// <returns>The limit.</returns>
        public static int GetQueueProcessingLimit(Type entitySystemType)
        {
            QueueManager queueManager = QueuesManager[entitySystemType];
            queueManager.AcquireLock();
            int result = queueManager.EntitiesToProcessEachFrame;
            queueManager.ReleaseLock();
            return result;
        }


        /// <summary>
        /// Queues the count.
        /// </summary>
        /// <param name="entitySystemType">Type of the entity system.</param>
        /// <returns>The number of queues.</returns>
        public static int QueueCount(Type entitySystemType)
        {
            QueueManager queueManager = QueuesManager[entitySystemType];
            queueManager.AcquireLock();
            int result = queueManager.Queue.Count;
            queueManager.ReleaseLock();
            return result;
        }


        /// <summary>
        /// Sets the queue processing limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="entitySystemType">Type of the entity system.</param>
        public static void SetQueueProcessingLimit(int limit, Type entitySystemType)
        {
            QueueManager queueManager = QueuesManager[entitySystemType];
            queueManager.AcquireLock();
            queueManager.EntitiesToProcessEachFrame = limit;
            queueManager.ReleaseLock();
        }


        /// <summary>
        /// Override to implement code that gets executed when systems are initialized.
        /// </summary>
        public override void LoadContent()
        {
        }


        /// <summary>
        /// Called when [added].
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void OnEntityAdded(Entity entity)
        {
        }


        /// <summary>
        /// Called when [change].
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void OnEntityChanged(Entity entity)
        {
        }


        /// <summary>
        /// Called when [removed].
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void OnEntityRemoved(Entity entity)
        {
        }


        /// <summary>
        /// Processes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Process(Entity entity)
        {
        }


        /// <summary>
        /// Processes this instance.
        /// </summary>
        public override void Process()
        {
            Entity[] entities;
            QueueManager queueManager = QueuesManager[this.Id];
            queueManager.AcquireLock();
            {
                int count = queueManager.Queue.Count;
                if (count > queueManager.EntitiesToProcessEachFrame)
                {
                    entities = new Entity[queueManager.EntitiesToProcessEachFrame];
                    for (int index = 0; index < queueManager.EntitiesToProcessEachFrame; ++index)
                    {
                        entities[index] = queueManager.Queue.Dequeue();
                    }
                }
                else
                {
                    entities = queueManager.Queue.ToArray();
                    queueManager.Queue.Clear();
                }
            }

            queueManager.ReleaseLock();

            foreach (Entity item in entities)
            {
                Process(item);
            }
        }


        /// <summary>
        /// Override to implement code that gets executed when systems are terminated.
        /// </summary>
        public override void UnloadContent()
        {
        }


        /// <summary>
        /// Finalizes an instance of the <see cref="QueueSystemProcessingThreadSafe"/> class.
        /// </summary>
        ~QueueSystemProcessingThreadSafe()
        {
            QueueManager queueManager = QueuesManager[this.Id];
            queueManager.AcquireLock();
            --queueManager.RefCount;
            if (queueManager.RefCount == 0)
            {
                QueuesManager.Remove(this.Id);
            }

            queueManager.ReleaseLock();
        }
    }

    
    /// <summary>
    /// Queue system not based on entities and components.
    /// It Process ONCE everything you explicitly add to it.
    /// Use the static method AddToQueue (second parameter is the type of your specialization of this class).
    /// </summary>
    /// <typeparam name="T">The Type T.</typeparam>
    public abstract class QueueSystemProcessingThreadSafe<T> : EntitySystem
    {
        private static readonly Dictionary<Type, QueueManager<T>> QueuesManager = new Dictionary<Type, QueueManager<T>>();

        /// <summary>
        /// The id.
        /// </summary>
        public readonly Type Id;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueSystemProcessingThreadSafe{T}"/> class.
        /// </summary>
        protected QueueSystemProcessingThreadSafe()
        {
            this.Id = GetType();
            if (!QueuesManager.ContainsKey(this.Id))
            {
                QueuesManager[this.Id] = new QueueManager<T>();
            }
            else
            {
                QueuesManager[this.Id].AcquireLock();
                ++QueuesManager[this.Id].RefCount;
                QueuesManager[this.Id].ReleaseLock();
            }
        }
        #endregion


        /// <summary>
        /// Adds to queue.
        /// </summary>
        /// <param name="ent">The entity.</param>
        /// <param name="entitySystemType">Type of the entity system.</param>
        public static void AddToQueue(T ent, Type entitySystemType)
        {
            QueueManager<T> queueManager = QueuesManager[entitySystemType];
            queueManager.AcquireLock();
            queueManager.Queue.Enqueue(ent);
            queueManager.ReleaseLock();
        }


        /// <summary>
        /// Gets the queue processing limit.
        /// </summary>
        /// <param name="entitySystemType">Type of the entity system.</param>
        /// <returns>The limit.</returns>
        public static int GetQueueProcessingLimit(Type entitySystemType)
        {
            QueueManager<T> queueManager = QueuesManager[entitySystemType];
            queueManager.AcquireLock();
            int result = queueManager.EntitiesToProcessEachFrame;
            queueManager.ReleaseLock();
            return result;
        }


        /// <summary>
        /// Queues the count.
        /// </summary>
        /// <param name="entitySystemType">Type of the entity system.</param>
        /// <returns>The number of queues.</returns>
        public static int QueueCount(Type entitySystemType)
        {
            QueueManager<T> queueManager = QueuesManager[entitySystemType];
            queueManager.AcquireLock();
            int result = queueManager.Queue.Count;
            queueManager.ReleaseLock();
            return result;
        }


        /// <summary>
        /// Sets the queue processing limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="entitySystemType">Type of the entity system.</param>
        public static void SetQueueProcessingLimit(int limit, Type entitySystemType)
        {
            QueueManager<T> queueManager = QueuesManager[entitySystemType];
            queueManager.AcquireLock();
            queueManager.EntitiesToProcessEachFrame = limit;
            queueManager.ReleaseLock();
        }


        /// <summary>
        /// Override to implement code that gets executed when systems are initialized.
        /// </summary>
        public override void LoadContent()
        {
        }


        /// <summary>
        /// Called when [added].
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void OnEntityAdded(Entity entity)
        {
        }


        /// <summary>
        /// Called when [change].
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void OnEntityChanged(Entity entity)
        {
        }


        /// <summary>
        /// Called when [removed].
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void OnEntityRemoved(Entity entity)
        {
        }


        /// <summary>
        /// Processes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Process(Entity entity)
        {
        }


        /// <summary>
        /// Processes this instance.
        /// </summary>
        public override void Process()
        {
            T[] entities;
            QueueManager<T> queueManager = QueuesManager[this.Id];
            queueManager.AcquireLock();
            {
                int count = queueManager.Queue.Count;
                if (count > queueManager.EntitiesToProcessEachFrame)
                {
                    entities = new T[queueManager.EntitiesToProcessEachFrame];
                    for (int index = 0; index < queueManager.EntitiesToProcessEachFrame; ++index)
                    {
                        entities[index] = queueManager.Queue.Dequeue();
                    }
                }
                else
                {
                    entities = queueManager.Queue.ToArray();
                    queueManager.Queue.Clear();
                }
            }

            queueManager.ReleaseLock();

            foreach (T item in entities)
            {
                Process(item);
            }
        }


        /// <summary>
        /// Processes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Process(T entity)
        {
        }


        /// <summary>
        /// Override to implement code that gets executed when systems are terminated.
        /// </summary>
        public override void UnloadContent()
        {
        }


        /// <summary>
        /// Finalizes an instance of the <see cref="QueueSystemProcessingThreadSafe{T}"/> class.
        /// </summary>
        ~QueueSystemProcessingThreadSafe()
        {
            QueueManager<T> queueManager = QueuesManager[this.Id];
            queueManager.AcquireLock();
            --queueManager.RefCount;
            if (queueManager.RefCount == 0)
            {
                QueuesManager.Remove(this.Id);
            }

            queueManager.ReleaseLock();
        }
    }
}