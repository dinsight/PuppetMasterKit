using System;
using System.ComponentModel;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.AI.Components
{
	/// <summary>
	/// Tag component. Use the generic class to specifiy a state enum
	/// </summary>
	public abstract class StateComponent : Component 
  {
    public bool IsSelected { get; set; }
	}

	/// <summary>
	/// State component.
	/// </summary>
	public class StateComponent<T> : StateComponent where T: struct, IComparable, IFormattable
	{
		public T CurrentState { get; set;}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:PuppetMasterKit.AI.Components.StateComponent`1"/> class.
		/// </summary>
		/// <param name="initialState">Initial state.</param>
		public StateComponent(T initialState)
		{
			CurrentState = initialState;
      IsSelected = false;
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:PuppetMasterKit.AI.Components.StateComponent`1"/>.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:PuppetMasterKit.AI.Components.StateComponent`1"/>.</returns>
		public override string ToString()
		{
			Enum val = CurrentState as Enum;
			return val.GetStringValue();
		}
	}
}
