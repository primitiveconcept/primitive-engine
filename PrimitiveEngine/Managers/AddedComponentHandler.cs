namespace PrimitiveEngine
{
	/// <summary>
	/// Delegate AddedComponentHandler.
	/// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="component">The component.</param>
    public delegate void AddedComponentHandler(Entity entity, IEntityComponent component);
}