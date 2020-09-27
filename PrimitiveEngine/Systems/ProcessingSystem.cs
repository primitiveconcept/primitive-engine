namespace PrimitiveEngine
{
    /// <summary>
    /// The processing system class.
    /// Special type of System that has NO entity associated (called once each frame).
    /// Extend it and override the ProcessSystem function.
    /// </summary>
    public abstract class ProcessingSystem : EntitySystem
    {
        /// <summary>
        /// Processes the System. Users must extend this method. Called once per frame.
        /// </summary>
        public abstract void ProcessSystem();


        /// <summary>
        /// Called when [change].
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void OnEntityChanged(Entity entity)
        {
            // By overriding base.OnChange we disable notification of Entities' changes
        }


        /// <summary>
        /// Processes this instance. [Internal]
        /// </summary>
        public override void Process()
        {
            if (CheckProcessing())
            {
                Begin();
                ProcessSystem();
                End();
            }
        }
    }
}