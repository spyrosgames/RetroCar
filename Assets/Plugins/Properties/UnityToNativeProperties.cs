using UnityEngine;
using System.Collections;
using System.Text;
using System;

namespace Gree.Unity {
	
	/// <summary>
	/// Unity to native utility class.
	/// </summary>
    public class UnityToNativeProperties : UnityToNative
    {

		public static void LoadAndSendPropertiesToNative(string overridePropertiesFile) {
			Hashtable properties;
			properties = Properties.FetchProperties(overridePropertiesFile);
			Properties.ConfigurationProperties = properties;
		    ArrayList array = new ArrayList();
		    array.Add(properties);
		    UnityToNative.SendMessage("loadedConfiguration", array);
		}
	}
}
