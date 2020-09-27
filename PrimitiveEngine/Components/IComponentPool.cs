namespace PrimitiveEngine
{
	/// <summary>
	/// Interface IComponentPool.
	/// </summary>
	/// <typeparam name="T">The <see langword="Type"/> T.</typeparam>
	public interface IComponentPool<T>
		where T : ComponentPoolable
	{
		/// <summary>
		/// Cleans up the pool by checking each valid object
		/// to ensure it is still actually valid.
		/// Must be called regularly to free returned Objects.
		/// </summary>
		void CleanUp();


		/// <summary>
		/// Gets a new object from the Pool.
		/// </summary>
		/// <returns>The next object in the pool if available, null if all instances are valid.</returns>
		T New();


		/// <summary>
		/// Returns the specified component.
		/// </summary>
		/// <param name="component">The component.</param>
		void ReturnObject(T component);
	}
}