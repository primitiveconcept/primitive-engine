namespace PrimitiveEngine
{
    /// <summary>
    /// Delegate RemovedComponentHandler.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="component">The component.</param>
    public delegate void RemovedComponentHandler(Entity entity, IEntityComponent component);
}