namespace PrimitiveEngine
{
    using System;


    /// <summary>
    /// Class IntervalTagSystem.
    /// </summary>
    public abstract class IntervalTagSystem : TagSystem
    {
        private readonly Timer timer;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalTagSystem"/> class.
        /// </summary>
        /// <param name="timeSpan">The time span.</param>
        /// <param name="tag">The tag.</param>
        protected IntervalTagSystem(TimeSpan timeSpan, string tag)
            : base(tag)
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