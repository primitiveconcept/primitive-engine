namespace PrimitiveEngine
{
	using System;


	public class WorldMismatchException : Exception
	{
		#region Constructors
		public WorldMismatchException(string message)
			: base(message) {}
		#endregion
	}
}