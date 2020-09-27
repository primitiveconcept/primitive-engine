namespace PrimitiveEngine
{
	using System;
	using System.Runtime.Serialization;


	/// <summary>
	/// Exception that is thrown when trying to use a EntityTemplate which does not exist.
	/// </summary>
	[Serializable]
	public class MissingEntityTemplateException : Exception
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MissingEntityTemplateException" /> class.
		/// </summary>
		/// <param name="entityTemplateTag">The entity template tag.</param>
		/// <param name="inner">The inner.</param>
		public MissingEntityTemplateException(
			string entityTemplateTag, 
			Exception inner = null)
			: base(
				message: "EntityTemplate for the tag " + entityTemplateTag + " was not registered.", 
				innerException: inner) {}


		/// <summary>
		/// Initializes a new instance of the <see cref="MissingEntityTemplateException" /> class.
		/// </summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The serialization context.</param>
		protected MissingEntityTemplateException(
			SerializationInfo info, 
			StreamingContext context)
			: base(info: info, context: context) {}
		#endregion
	}
}