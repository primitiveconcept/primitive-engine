namespace PrimitiveEngine
{
	using System;


	public class ValueRange<T>
		where T : struct,
			IComparable,	
			IComparable<T>,
			IConvertible,
			IEquatable<T>,
			IFormattable
	{
		private T value;
		private T maximum;
		private T minimum;


		#region Properties
		public T Maximum
		{
			get { return this.maximum; }
			set { this.maximum = value; }
		}


		public T Minimum
		{
			get { return this.minimum; }
			set { this.minimum = value; }
		}


		public T Value
		{
			get { return this.value; }
			set
			{
				if (value.CompareTo(this.maximum) > 0)
					this.value = this.maximum;
				else if (value.CompareTo(this.minimum) < 0)
					this.value = this.minimum;
				this.value = value;
			}
		}
		#endregion


		#region Operators
		public static implicit operator T(ValueRange<T> valueRange)
		{
			return valueRange.value;
		}
		#endregion
	}
}