using UnityEngine;
using System.Collections;
using System.Text;
using System;

namespace Gree.Unity {
	
	/// <summary>
	/// This class is in charge of sending a message to native code.
	/// </summary>
    public class UnityToNative
    {
		/// <summary>
		/// Messenger instantiation of <see cref="Gree.Unity.IUnityToNativeMessage"/> that will 
		/// be in charge of connecting unity to native.
		/// </summary>
		public static IUnityToNativeMessage messenger = UnityToNativeMessageManager.GetMessenger();

		/// <summary>
		/// It sends a message to native code for given method name and arguments.
		/// </summary>
		/// <returns>
		/// The message. This is optional
		/// </returns>
		/// <param name='methodName'>
		/// Method name of native.
		/// </param>
		/// <param name='arguments'>
		/// Arguments. This is optional.
		/// </param>
		public static object SendMessage(string methodName, ArrayList arguments) {
           	string ret = messenger.SendMessageToNative(methodName, arguments);
           	return MiniJSON.JsonDecode(ret);
		}

		/// <summary>
		/// Helper method to throw exceptions on unsupported method for the Android SDK
		/// </summary>
		/// <param name="className">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="methodName">
		/// A <see cref="System.String"/>
		/// </param>
		static public void throwAndroidUnsupported(string className, string methodName){
			throwUnsupportedLog(className, methodName, "is not supported for the Android SDK.");
		}
		
		/// <summary>
		/// Helper method to throw exceptions on unsupported method for the iOS SDK
		/// </summary>
		/// <param name="className">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="methodName">
		/// A <see cref="System.String"/>
		/// </param>
		static public void throwIOSUnsupported(string className, string methodName){
			throwUnsupportedLog(className, methodName, "is not supported for the iOS SDK.");
		}
		
		/// Author: Chris Chua
		/// <summary>
		/// throws unsupported platform class method
		/// </summary>
		/// <param name="className">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="methodName">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="platformMessage">
		/// A <see cref="System.String"/>
		/// </param>
		private static void throwUnsupportedLog(string className, string methodName, string platformMessage){
			StringBuilder error = new StringBuilder(className)
				.Append(" '")
				.Append(methodName)
				.Append("' ")
				.Append(platformMessage);
			throw new System.NotSupportedException(error.ToString());
		}
	}
}
