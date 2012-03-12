using System;
using System.Collections;
using UnityEngine;

namespace Gree.Unity
{
	/// <summary>
	/// This class is accessible store for loaded properties.
	/// </summary>
	public class Properties
	{	
		public static IPropertiesCustomization PropertiesCustomizer; 
		
		public static Hashtable ConfigurationProperties {
			get;
			set;
		}
		
		private static string propertiesDirectory;
		
		public static Hashtable FetchProperties(string overridePropertiesFile) {
			if (PropertiesCustomizer != null) {
				propertiesDirectory = PropertiesCustomizer.PropertiesDirectory;
			}
			else {
				propertiesDirectory = "";
			}
			
			TextAsset jsonText = (TextAsset)Resources.Load(propertiesDirectory + "properties.json", typeof(TextAsset));
			if (jsonText == null) {
				Debug.LogError("properties.json file not found.");
			}
			Hashtable properties = MiniJSON.JsonDecode(jsonText.text) as Hashtable;
			if (properties == null) {
				Debug.Log("properties.json is invalid as json.");
				properties = new Hashtable();
			}
			
			if (overridePropertiesFile != null && overridePropertiesFile.Length > 0) {
				TextAsset overrideText = (TextAsset)Resources.Load(propertiesDirectory + overridePropertiesFile, typeof(TextAsset));
				if (overrideText == null) {
					Debug.LogError("the override properties file was not found: " + overridePropertiesFile);
				}
				Hashtable overrideProperties = MiniJSON.JsonDecode(overrideText.text) as Hashtable;
		        if (overrideProperties != null) {
					foreach (DictionaryEntry entry in overrideProperties){
						properties[entry.Key] = entry.Value;
					}
				} else {
					Debug.Log("the override properties file is invalid as json: " + overridePropertiesFile);
				}
			}
			if (PropertiesCustomizer != null) {
				return PropertiesCustomizer.CustomizeProperties(properties);
			}
			else {
				return properties;
			}
		}
		
		
	}
}
