namespace PrimitiveEngine
{
	/// <summary>
	/// Interface that components can use for data reset.
	/// </summary>
	public interface IResettableComponent
	{
		void Reset (params object[] args);
	}
}