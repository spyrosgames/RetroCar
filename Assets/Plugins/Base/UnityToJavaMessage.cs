using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections;

namespace Gree.Unity {
	
	/// <summary>
	/// This class is in charge of invoking a java method with given arguments.
	/// </summary>
    public class UnityToJavaMessage : IUnityToNativeMessage
    {
		#if UNITY_ANDROID		
		private static AndroidJavaObject javaFacadeInstance;

		private const string JAVA_FACADE_CLASS_NAME = "com.gree.giraffe.UnityJavaFacade";
		
		private const string JAVA_FACADE_METHOD_NAME = "invokeJava";
		#endif
		/// <summary>
		/// Invokes a method on java code.
		/// </summary>
		/// <returns>
		/// Return message from Java. This is optional.
		/// </returns>
		/// <param name='methodname'>
		/// Method name.
		/// </param>
		/// <param name='arguments'>
		/// Arguments. This is optional.
		/// </param>
		public string SendMessageToNative(string methodname, ArrayList arguments) {
			string json_result = "";
		#if UNITY_ANDROID			
			if (javaFacadeInstance == null)
				javaFacadeInstance = new AndroidJavaObject(JAVA_FACADE_CLASS_NAME);			
			
			object[] args = null;
			if (arguments == null || arguments.Count<=0)
				args = new object[] {methodname};
			else{
				AndroidJavaObject list = new AndroidJavaObject("java.util.ArrayList");
				foreach (object i in arguments)
				{
				  Boolean b = list.Call<Boolean>("add", getJavaObject(i));
				}
				args = new object[] {methodname, list};
			}
			json_result = javaFacadeInstance.Call<string>(JAVA_FACADE_METHOD_NAME, args);
		#endif
			return json_result;
		}
		
		/// <summary>
		/// It converts given object value to a corresponding java value.
		/// </summary>
		/// <returns>
		/// The java object.
		/// </returns>
		/// <param name='value'>
		/// c# object value.
		/// </param>
		protected object getJavaObject(object value) {
		#if UNITY_ANDROID			
			if (value is string) {
				return value;
			}
			if (value is Char) {
				return new AndroidJavaObject("java.lang.Character", value);
			}
			if ((value is Boolean)) {
				return new AndroidJavaObject("java.lang.Boolean", value);
			} 
			if (value is int) {
				return new AndroidJavaObject("java.lang.Integer", value);
			}
			if (value is long) {
				return new AndroidJavaObject("java.lang.Long", value);
			}
			if (value is float) {
				return new AndroidJavaObject("java.lang.Float", value);
			}
			if (value is double) {
               return new AndroidJavaObject("java.lang.Float", value);
           }
			if (value == null) {
				return value;
			} 
		#endif			
			return MiniJSON.JsonEncode(value);
		}
	}
}

