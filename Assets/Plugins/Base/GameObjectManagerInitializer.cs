using UnityEngine;
using System.Collections;

/// <summary>
/// It is in charge of initializing an empty game object dynamically and attaching 
/// <b>NativeToUnity</b> script to <b>NativeUnityManager</b> game object.
/// </summary>
/// <remarks>
/// Please note that this script must be attached to an existing unity game object 
/// in order to get executed; for instance, main camera.
/// </remarks>
public class GameObjectManagerInitializer : MonoBehaviour {

	/// <summary>
	/// Execution of this script. It attaches <b>NativeToUnity</b> script to 
	/// <b>NativeUnityManager</b> game object.
	/// </summary>
	void Start () {
		GameObject manager = new GameObject("NativeUnityManager");
		manager.AddComponent("NativeToUnity");
		Destroy(this);
		
	}
	
}
