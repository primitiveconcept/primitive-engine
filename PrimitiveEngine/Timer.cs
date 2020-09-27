namespace PrimitiveEngine
{
    using System;


    /// <summary>
    /// The class Timer.
    /// </summary>
    public class Timer
    {
        /// <summary>
        /// The delay ticks.
        /// </summary>
        private readonly long delayTicks;


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Timer" /> class.
        /// </summary>
        /// <param name="delay">The delay.</param>
        public Timer(TimeSpan delay)
        {
            this.delayTicks = delay.Ticks;
            Reset();
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets the accumulated ticks.
        /// </summary>
        /// <value>The accumulated ticks.</value>
        public long AccumulatedTicks { get; private set; }
        #endregion


        /// <summary>
        /// Determines whether the specified delta is reached.
        /// </summary>
        /// <param name="deltaTicks">The delta in ticks.</param>
        /// <returns><see langword="true" /> if the specified delta is reached; otherwise, <see langword="false" />.</returns>
        public bool IsReached(long deltaTicks)
        {
            this.AccumulatedTicks += deltaTicks;
            if (this.AccumulatedTicks >= this.delayTicks)
            {
                this.AccumulatedTicks -= this.delayTicks;
                return true;
            }

            return false;
        }


        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            this.AccumulatedTicks = 0;
        }
    }
}
