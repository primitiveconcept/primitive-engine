namespace PrimitiveEngine
{
    using System;
	using System.Collections.Generic;


    /// <summary>Class DelayedEntitySystem.</summary>
    public abstract class DelayedEntitySystem : EntitySystem
    {
        private Timer timer;
        private bool isRunning;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedEntitySystem" /> class.
        /// </summary>
        /// <param name="aspect">The aspect.</param>
        protected DelayedEntitySystem(Aspect aspect)
            : base(aspect)
        {
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets the initial time delay.
        /// </summary>
        /// <value>The initial time delay.</value>
        public TimeSpan InitialTimeDelay { get; private set; }
        #endregion


        /// <summary>
        /// Processes the entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <param name="accumulatedDelta">The accumulated delta.</param>
        public abstract void ProcessEntities(IDictionary<int, Entity> entities, long accumulatedDelta);


        /// <summary>
        /// Gets the remaining time until processing.
        /// </summary>
        /// <returns>The remaining time in ticks.</returns>
        public TimeSpan GetRemainingTimeUntilProcessing()
        {
            if (this.isRunning)
            {
                return TimeSpan.FromTicks(this.InitialTimeDelay.Ticks - this.timer.AccumulatedTicks);
            }

            return TimeSpan.Zero;
        }


        /// <summary>
        /// Determines whether this instance is running.
        /// </summary>
        /// <returns><see langword="true" /> if this instance is running; otherwise, <see langword="false" />.</returns>
        public bool IsRunning()
        {
            return this.isRunning;
        }


        /// <summary>
        /// Starts the delayed run.
        /// </summary>
        /// <param name="delay">The time span.</param>
        public void StartDelayedRun(TimeSpan delay)
        {
            this.InitialTimeDelay = delay;
            this.timer = new Timer(delay);
            this.isRunning = true;
        }


        /// <summary>
        /// Stops this instance.
        /// Aborts running the system in the future and stops it.
        /// Call delayedRun() to start it again.
        /// </summary>
        public void Stop()
        {
            if (this.timer == null)
            {
                throw new NullReferenceException("Call StartDelayRun before Stop.");
            }

            this.isRunning = false;
            this.timer.Reset();
        }


        /// <summary>
        /// Checks the processing.
        /// </summary>
        /// <returns><see langword="true" /> if this instance is enabled, <see langword="false" /> otherwise</returns>
        protected override bool CheckProcessing()
        {
            if (this.isRunning)
            {
                if (this.timer.IsReached(this.EntityWorld.Delta))
                {
                    return this.IsEnabled;
                }
            }

            return false;
        }


        /// <summary>
        /// Processes the entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        protected override void ProcessEntities(IDictionary<int, Entity> entities)
        {
            ProcessEntities(entities, this.timer.AccumulatedTicks);
            Stop();
        }
    }
}