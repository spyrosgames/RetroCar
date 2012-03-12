using System;
using System.Collections.Generic;
using System.Collections;

namespace Gree.Unity
{
	/// <summary>
	/// Base class for all callbacks fetcher. All fetchers should extend from this class.
	/// </summary>
	public abstract class CallbackFetcherBase
	{
		/// <summary>
		/// Registered callbacks.
		/// </summary>
		protected Dictionary<string, ICallback> registeredCallbacks;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="Gree.Unity.CallbackFetcherBase"/> class.
		/// </summary>
		public CallbackFetcherBase() {
            registeredCallbacks = new Dictionary<string, ICallback>();
		}
		
		/// <summary>
		/// It Fetchs the callback for given key.
		/// </summary>
		/// <returns>
		/// <see cref="Gree.Unity.ICallback"/> for given key.
		/// </returns>
		/// <param name='key'>
		/// Key to look the callback at.
		/// </param>
		public ICallback fetchCallbackFor(string key) {
			return registeredCallbacks != null ? registeredCallbacks[key] : null;
		}
		
		public abstract Hashtable callBackHandlersList();
	}
}

