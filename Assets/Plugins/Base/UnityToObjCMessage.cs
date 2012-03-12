using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections;

namespace Gree.Unity {
	
	/// <summary>
	/// This class is in charge of invoking an Objective-c method with given arguments.
	/// </summary>	
    public class UnityToObjCMessage : IUnityToNativeMessage
    {
        [DllImport("__Internal")]
        private static extern string _UnityBridge_sendMessage(string methodname, string json);
		public string SendMessageToNative(string methodname, ArrayList arguments) {
			string json = MiniJSON.JsonEncode(arguments);
			methodname += ':';
			string ret = _UnityBridge_sendMessage(methodname, json);
			return ret;
		}
	}
}

