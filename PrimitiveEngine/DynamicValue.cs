namespace PrimitiveEngine
{
	using System.Runtime.InteropServices;


	[StructLayout(LayoutKind.Explicit)]
	public struct DynamicValue
	{
		[FieldOffset(0)] public readonly ValueType Type;
		[FieldOffset(4)] private bool booleanValue;
		[FieldOffset(4)] private int integerValue;
		[FieldOffset(4)] private double doubleValue;
		[FieldOffset(16)] private char[] stringValue;


		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicValue"/> struct.
		/// </summary>
		/// <param name="value">The value.</param>
		public DynamicValue(int value)
		{
			this.Type = ValueType.Integer;
			this.booleanValue = false;
			this.doubleValue = default(double);
			this.stringValue = null;
			this.integerValue = value;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicValue"/> struct.
		/// </summary>
		/// <param name="value">The value.</param>
		public DynamicValue(double value)
		{
			this.Type = ValueType.Double;
			this.booleanValue = false;
			this.integerValue = default(int);
			this.stringValue = null;
			this.doubleValue = value;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicValue"/> struct.
		/// </summary>
		/// <param name="value">The value.</param>
		public DynamicValue(string value)
		{
			this.Type = ValueType.String;
			this.booleanValue = false;
			this.integerValue = default(int);
			this.doubleValue = default(double);
			this.stringValue = value.ToCharArray();
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicValue"/> struct.
		/// </summary>
		/// <param name="value">The value.</param>
		public DynamicValue(bool value)
		{
			this.Type = ValueType.Boolean;
			this.integerValue = default(int);
			this.doubleValue = default(double);
			this.stringValue = null;
			this.booleanValue = value;
		}
		#endregion


		
		/// <summary>
		/// The type of value contained by the DynamicValue instance.
		/// </summary>
		public enum ValueType
		{
			Boolean,
			Integer,
			Double,
			String
		}
		


		
		/// <summary>
		/// Performs an implicit conversion from <see cref="System.String"/> to <see cref="DynamicValue"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator DynamicValue(string value)
		{
			return new DynamicValue(value);
		}


		/// <summary>
		/// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="DynamicValue"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator DynamicValue(int value)
		{
			return new DynamicValue(value);
		}


		/// <summary>
		/// Performs an implicit conversion from <see cref="System.Boolean"/> to <see cref="DynamicValue"/>.
		/// </summary>
		/// <param name="value">if set to <c>true</c> [value].</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator DynamicValue(bool value)
		{
			return new DynamicValue(value);
		}


		/// <summary>
		/// Performs an implicit conversion from <see cref="System.Double"/> to <see cref="DynamicValue"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator DynamicValue(double value)
		{
			return new DynamicValue(value);
		}


		/// <summary>
		/// Performs an implicit conversion from <see cref="DynamicValue"/> to <see cref="System.String"/>.
		/// </summary>
		/// <param name="dynamicValue">The dynamic value.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator string(DynamicValue dynamicValue)
		{
			return dynamicValue.ToString();
		}


		/// <summary>
		/// Performs an implicit conversion from <see cref="DynamicValue"/> to <see cref="System.Int32"/>.
		/// </summary>
		/// <param name="dynamicValue">The dynamic value.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator int(DynamicValue dynamicValue)
		{
			return dynamicValue.ToInteger();
		}


		/// <summary>
		/// Performs an implicit conversion from <see cref="DynamicValue"/> to <see cref="System.Double"/>.
		/// </summary>
		/// <param name="dynamicValue">The dynamic value.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator double(DynamicValue dynamicValue)
		{
			return dynamicValue.ToDouble();
		}


		/// <summary>
		/// Performs an implicit conversion from <see cref="DynamicValue"/> to <see cref="System.Boolean"/>.
		/// </summary>
		/// <param name="dynamicValue">The dynamic value.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator bool(DynamicValue dynamicValue)
		{
			return dynamicValue.ToBoolean();
		}
		


		/// <summary>
		/// Get value as a boolean.
		/// </summary>
		/// <returns>Value as a boolean.</returns>
		public bool ToBoolean()
		{
			if (this.Type == ValueType.Boolean)
				return this.booleanValue;
			else if (this.Type == ValueType.Integer)
				return this.integerValue > 0;
			else if (this.Type == ValueType.Double)
				return this.doubleValue > 0;
			else
				return this.stringValue != null;
		}


		/// <summary>
		/// Get value as a double.
		/// </summary>
		/// <returns>value as a double.</returns>
		public double ToDouble()
		{
			if (this.Type == ValueType.Double)
				return this.doubleValue;
			else if (this.Type == ValueType.Boolean)
				return this.booleanValue ? 1 : 0;
			else if (this.Type == ValueType.Integer)
				return this.integerValue;
			else
			{
				double result = 0;
				double.TryParse(this.stringValue.ToString(), out result);
				return result;
			}
		}


		/// <summary>
		/// Get value as an integer.
		/// </summary>
		/// <returns>value as an integer.</returns>
		public int ToInteger()
		{
			if (this.Type == ValueType.Integer)
				return this.integerValue;
			else if (this.Type == ValueType.Boolean)
				return this.booleanValue ? 1 : 0;
			else if (this.Type == ValueType.Double)
				return (int)this.doubleValue;
			else
			{
				int result = 0;
				int.TryParse(this.stringValue.ToString(), out result);
				return result;
			}
		}


		/// <summary>
		/// Get value as a string.
		/// </summary>
		/// <returns>value as a string.</returns>
		public override string ToString()
		{
			if (this.Type == ValueType.String)
				return this.stringValue.ToString();
			else if (this.Type == ValueType.Boolean)
				return this.booleanValue ? "true" : "false";
			if (this.Type == ValueType.Integer)
				return this.integerValue.ToString();
			else
				return this.doubleValue.ToString();
		}
	}
}