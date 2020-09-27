namespace PrimitiveEngine
{
    using System;


    /// <summary>
    /// Class IntervalEntitySystem.
    /// </summary>
    public abstract class IntervalEntitySystem : EntitySystem
    {
        private readonly Timer timer;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalEntitySystem"/> class.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="aspect">The aspect.</param>
        protected IntervalEntitySystem(TimeSpan timeSpan, Aspect aspect)
            : base(aspect)
        {
            this.timer = new Timer(timeSpan);
        }
        #endregion


        /// <summary>
        /// Checks the processing.
        /// </summary>
        /// <returns><see langword="true" /> if this instance is enabled, <see langword="false" /> otherwise</returns>
        protected override bool CheckProcessing()
        {
            return this.timer.IsReached(this.EntityWorld.Delta) 
                   && this.IsEnabled;
        }
    }
}