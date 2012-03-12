using UnityEngine;
using System.Collections;
using System;

namespace Gree.Unity {
	
	/// <summary>
	/// Interface for customizing properties computation.
	/// </summary>
	/// <remarks>
	/// Any class computing properties should implement this interface.
	/// </remarks>
	public interface IPropertiesCustomization 
	{

		string PropertiesDirectory {
			get;
		}
		
		/// <summary>
		/// Modfies properties.
		/// </summary>
		/// <returns>
		/// Modified properties
		/// </returns>
		/// <param name='properties'>
		/// Hashtable of properties to modify
		/// </param>
		Hashtable CustomizeProperties(Hashtable properties);
	}
}