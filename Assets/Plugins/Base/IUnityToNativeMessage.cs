using UnityEngine;
using System.Collections;
using System;

namespace Gree.Unity {
	
	/// <summary>
	/// Interface for sending a message to native code.
	/// </summary>
	/// <remarks>
	/// Any class pretending to send a message to native code should implement this interface.
	/// </remarks>
	public interface IUnityToNativeMessage 
	{
		/// <summary>
		/// Sends a message to native code.
		/// </summary>
		/// <returns>
		/// Message coming back from native code. This is optional.
		/// </returns>
		/// <param name='methodname'>
		/// Native method name to invoke.
		/// </param>
		/// <param name='args'>
		/// Arguments. This is optional.
		/// </param>
		string SendMessageToNative(string methodname, ArrayList args);
	}
}