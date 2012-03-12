using UnityEngine;
using System;
using System.Text;
using System.Collections;
namespace Gree.Unity
{
    /// <summary>
    /// Callback fetcher for Open Feint events/methods.
    /// </summary>
	public class CallbackFetcherProperties : CallbackFetcherBase
    {
        #region Initialization
        public CallbackFetcherProperties()
            : base()
        {
            initializeCallbacks();
        }

        /// <summary>
        /// Initializes the callbacks for every method to register.
        /// </summary>
		private void initializeCallbacks()
        {
            registeredCallbacks.Add("LoadProperties", new LoadPropertiesCallback());
            registeredCallbacks.Add("PropertiesLoaded", new PropertiesLoadedCallback());
			registeredCallbacks.Add("NotSupportedAndroidMethod", new NotSupportedAndroidMethodCallback());
        }	
		
        #endregion
		
		public override Hashtable callBackHandlersList() {
			
			Hashtable _callbackHandlers = new Hashtable();
			_callbackHandlers.Add ("LoadProperties", this);
			_callbackHandlers.Add ("PropertiesLoaded", this);
			_callbackHandlers.Add ("NotSupportedAndroidMethod", this);
			return _callbackHandlers;
		}
		
		//
        // Callback delegates.
        public delegate void PropertiesCallback(bool result);


		/// <summary>
        /// Sent when properties are loaded and the game can continue.
		/// </summary>
        static public event PropertiesCallback OnPropertiesLoaded;


        private class LoadPropertiesCallback : ICallback
        {

            public void execute( ArrayList args )
            {
             
				Hashtable properties;
				string fetchPropertyArgs = null;
				if (args != null && args.Count > 0) {
					fetchPropertyArgs = (string)args[0];
				}
				properties = Properties.FetchProperties(fetchPropertyArgs);
                ArrayList array = new ArrayList();
                array.Add(properties);
                UnityToNative.SendMessage("initializeWithOptions", array);
            }
        }

        private class PropertiesLoadedCallback : ICallback
        {

            public void execute( ArrayList args )
            {
				if ( OnPropertiesLoaded != null ) {
					bool result = System.Convert.ToBoolean(args[0]);					
					OnPropertiesLoaded(result);
                }
            }
        }
		
		/// Author: Christopher Chua
        /// <summary>
        /// Logs an error message when we encounter a method
        /// that is not implemented for the Android Unity bridge
        /// </summary>
        /// <param name="args">ArrayList {String methodName, String... method's parameters}</param>
		private class NotSupportedAndroidMethodCallback : ICallback
		{
			public void execute( ArrayList args )
			{
				StringBuilder error = new StringBuilder("NotSupportedAndroidMethodCallback: Failed to handle method '")
					.Append(args[0].ToString())
					.Append("'");
				
				// if more than one args, that means parameters were sent
				if ( args.Count > 1 ) {
					error.Append(" with the following parameters (");
					for (int i = 1; i < args.Count; i++) {
						if (i > 1)
							error.Append(", ");
						error.Append("'");
						error.Append(args[i].ToString());
						error.Append("'");

					}
					error.Append(") ");
				}
				
	    		error.Append("because it is not implemented in the Android Unity bridge.");
				
				Debug.LogError(error.ToString());
			}
		}
    }
}
