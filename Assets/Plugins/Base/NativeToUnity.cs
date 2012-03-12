using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// This class is in charge of communicating Native code ( such as Java, Objective-c ) with Unity.
/// </summary>
public class NativeToUnity : MonoBehaviour
{
	
	/// <summary>
	/// This method is invoked by native code and is the only way for native code to be able to send a 
	/// message to Unity. It will try to fetch a callback for given method name and executes it with 
	/// given arguments in case they exist.
	/// </summary>
	/// <param name='jsonMessage'>
	/// It will contain method name and optional arguments in case they are necessary. It is encoded as json.
	/// </param>
	public void HandleMessage (string jsonMessage) {
		object decodedMessage = MiniJSON.JsonDecode(jsonMessage);
		string methodName = parseMethodName(decodedMessage);
		if (methodName != null) {
			Gree.Unity.CallbackFetcherBase callbackHandler = Gree.Unity.CallbackFetcherFactory.fetchCallbackHandler(methodName);
			
			if (callbackHandler == null) {
				Debug.Log("Event for : \"" + methodName + "\" can not be handled, a module may be missing. Please check the MODULES value of properties.json file");
			} else {
				Gree.Unity.ICallback callback = callbackHandler.fetchCallbackFor(methodName);
				
				if (callback == null) {
					Debug.Log("Event for : \"" + methodName + "\" can not be handled, please check that the name in the Module is the correct one. ");
				} else {
					callback.execute(parseArguments(decodedMessage));
				}
			}
		}

	}
	
	/// <summary>
	/// It parses method name. It takes decoded message and looks for 1st position of decoded message array.
	/// </summary>
	/// <returns>
	/// The method name.
	/// </returns>
	/// <param name='decodedMessage'>
	/// Message coming from native code already decoded by <see cref="MiniJSON"/>.
	/// </param>
	private string parseMethodName(object decodedMessage) {
		
    	string methodName = null;
		if (!(decodedMessage is ArrayList)) {
    		Debug.Log("NativeToUnity.HandleMessage passed invalid json string for method name");	
    	} else {
	        ArrayList args = (ArrayList)decodedMessage;
	        var firstArg = args[0];
	        if (!(firstArg is string)) {
	    		Debug.Log("NativeToUnity.HandleMessage first argument is not string");	
	        } else {
				methodName = (string) firstArg;
			}
		}
		
		return methodName;
	}
	
	/**
	 * 
	 * */
	/// <summary>
	/// It parses method arguments. It takes decoded message and looks for an array of arguments at 2nd 
	/// position of decoded message array in case it exists.
	/// </summary>
	/// <returns>
	/// ArrayList containing method arguments. Empty if no arguments were found.
	/// </returns>
	/// <param name='decodedMessage'>
	/// Decoded message coming from Native code.
	/// </param>
	private ArrayList parseArguments(object decodedMessage) {
		
		ArrayList result = new ArrayList();
		if (!(decodedMessage is ArrayList)) {
    		Debug.Log("NativeToUnity.HandleMessage passed invalid json string for Arguments");	
    	} 
		else {
			ArrayList methodAndArguments = (ArrayList)decodedMessage;
			if (methodAndArguments.Count > 1) {	
				if (methodAndArguments[1] is ArrayList) {
					result = (ArrayList)methodAndArguments[1];
				 }
				 else {
				 	result = (ArrayList)methodAndArguments.GetRange(1, methodAndArguments.Count - 1); 
				 }
			}
		}
		return result;
	}
	
}

