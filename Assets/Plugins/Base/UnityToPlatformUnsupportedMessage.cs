using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Gree.Unity {
	
	/// <summary>
	/// This class represents an unsopported messenger for an unknown platform.
	/// </summary>
    public class UnityToPlatformUnsupportedMessage : IUnityToNativeMessage
    {
		
		/// <summary>
		/// It returns null.
		/// </summary>
		/// <returns>
		/// Null.
		/// </returns>
		/// <param name='methodname'>
		/// Method name.
		/// </param>
		/// <param name='args'>
		/// Arguments.
		/// </param>
		public string SendMessageToNative(string methodname, ArrayList args) {
			return null;
		}
		
		/// <summary>
		/// It returns null.
		/// </summary>
		/// <returns>
		/// Null.
		/// </returns>
		public Dictionary<string, string> GetNativeMethodMap() {
			return null;
		}
		
	}
}

