using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace Gree.Unity
{
	/// <summary>
	/// This class is in charge of fetching corresponding callback fetcher.
	/// </summary>
	public class CallbackFetcherFactory
	{
		static CallbackFetcherFactory ()
		{
			_callbackHandlers = new Dictionary<string, CallbackFetcherBase> ();
			String modules = getModules();
			if(modules != null && modules.Trim().Length > 0){
				init (modules.Split(new char[]{','}));
			}
		}
		
		private static String getModules(){
			Hashtable properties = Properties.FetchProperties(null);
			String modules = (String)properties["MODULES"];
			
			if(modules == null){
				modules = "Properties";
			}
			
			//the Properties module is always required
			if(!modules.Contains("Properties")){
				modules += ",Properties";
			}
			return modules;
		}
		
        private static Dictionary<string, CallbackFetcherBase> _callbackHandlers;

		/// <summary>
		/// Fetches the correct CallBackFetcher based upon method's name.
		/// </summary>
		/// <returns>
		/// Corresponding callback fetcher.
		/// </returns>
		/// <param name='forThisMethod'>
		/// Method key for callback fetching.
		/// </param>
		public static CallbackFetcherBase fetchCallbackHandler(string forThisMethod)
		{
			CallbackFetcherBase callback = null;
			
			if (_callbackHandlers.TryGetValue(forThisMethod, out callback)) {
                return callback;
			} else {
				// The callback couldn't be found
				return null;
			}
		}
		
        /**
         * Initializes callbacks/types for every method to register.
         */
		/// <summary>
		/// Loads the callback fetchers dictionary.
		/// </summary>
		private static void init (string[] moduleNames)
		{
			bool testMode = File.Exists("BaseTests.dll");
			foreach (String name in moduleNames) {
				string className = "Gree.Unity.CallbackFetcher" + name.Trim();
				
				try{				
					CallbackFetcherBase cb = null;
					if (testMode) {					
						try {
							Assembly asm = Assembly.LoadFrom("BaseTests.dll");
							cb = (CallbackFetcherBase) asm.CreateInstance(className);						
						} catch(Exception ex) {
							// could not load from this assembly
						}
					} else {
						cb = (CallbackFetcherBase)Activator.CreateInstance(Type.GetType(className));		
					}

					// Reflection… do we have the .cs? if Yes, instanciate it.
					Hashtable handlers = cb.callBackHandlersList();
					foreach (string key in handlers.Keys) {
	  					_callbackHandlers.Add(key, (CallbackFetcherBase)handlers[key]);
					}
				} catch (Exception e){
					Debug.Log("Error while loading callbacks from fetcher: "+ className +". Is this module available? Error: " + e.ToString());
				}
				
			}
		}
		
	}
}
