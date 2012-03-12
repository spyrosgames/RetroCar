using UnityEngine;
using System;
namespace Gree.Unity {
	
	/// <summary>
	/// This class is in charge of instantiating the correct <see cref="Gree.Unity.IUnityToNativeMessage"/> 
	/// depending on current platform.
	/// </summary>
    public class UnityToNativeMessageManager {
		
		/// <summary>
		/// Unity to native messenger.
		/// </summary>
		protected static IUnityToNativeMessage messenger;
		
		/// <summary>
		/// Gets the messenger. Depending on whether is an <b>iOS</b> or <b>Android</b> 
		/// app messenger gets instantiated.
		/// </summary>
		/// <returns>
		/// Corresponding unity to native messenger.
		/// </returns>
		public static IUnityToNativeMessage GetMessenger() {
			if (messenger == null) {
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
				    messenger = new UnityToObjCMessage();
				}
				else if (Application.platform == RuntimePlatform.Android) {
					messenger = new UnityToJavaMessage();
				}
				else {
					messenger = new UnityToPlatformUnsupportedMessage();
				}
			}
			return messenger;
		}
	}
}
