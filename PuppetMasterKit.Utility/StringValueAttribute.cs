using System;
using System.Reflection;

namespace PuppetMasterKit.Utility
{
	public class StringValueAttribute : Attribute
	{
		private string value;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:PuppetMasterKit.Utility.StringValueAttribute"/> class.
		/// </summary>
		/// <param name="value">Value.</param>
		public StringValueAttribute(string value)
		{
			this.value = value;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public string Value {
			get { return value; }
		}
	}

	public static class StringValueAttributeExt{
		/// <summary>
		/// Gets the string value.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		public static string GetStringValue(this Enum value)
	{
		Type type = value.GetType();	
		FieldInfo fi = type.GetRuntimeField(value.ToString());
		StringValueAttribute[] attrs =
			fi.GetCustomAttributes(typeof(StringValueAttribute), false)
			 as StringValueAttribute[];

		if (attrs.Length > 0) {
			return attrs[0].Value;
		}
		return null;
	}
	}
}
