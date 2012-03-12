using UnityEngine;
using System.Collections;

public enum eNavScreen
{
	Main,
	OpenFeint,
	OpenFeintX
}

namespace Gree.Unity {
	public class OFPropertiesCustomization : IPropertiesCustomization
	{
		private static OFPropertiesCustomization _instance = new OFPropertiesCustomization();
		
		public static OFPropertiesCustomization Instance {
			get { return _instance; }
		}
		
		public string PropertiesDirectory {
			get { return "GamePreferences/"; }
		}
		
		public Hashtable CustomizeProperties(Hashtable properties) {
			return properties;
		}
		
		public static void InitializePropertiesSystem() {
			Properties.PropertiesCustomizer = Instance;
		}
		
	}
}


public class RenderLoop : MonoBehaviour {
  	
	private eNavScreen navScreen = eNavScreen.Main;
	
	/*
	private MainMenu mainMenu = new MainMenu();
	private OpenFeintSample ofSample;// = new OpenFeintSample();
	private OpenFeintXSample ofxSample;// = new OpenFeintXSample();
	*/
	void InitializeWithConfiguration(bool initSuccess) {
		//ofSample.initializeInterface();
		//ofxSample.initializeInterface();
	}

	void Awake() {
	//	Gree.Unity.CallbackFetcherProperties.OnPropertiesLoaded += new Gree.Unity.CallbackFetcherProperties.PropertiesCallback(InitializeWithConfiguration);
		Gree.Unity.OFPropertiesCustomization.InitializePropertiesSystem();
		Gree.Unity.UnityToNativeProperties.LoadAndSendPropertiesToNative((string)null);
		/*
		ofSample = new OpenFeintSample();
		ofxSample = new OpenFeintXSample();
		
		ofSample.initializeInterface();
		ofxSample.initializeInterface();
		*/
	}
	
	void OnGUI() {
        //scale and position the GUI element to draw it at the screen's top left corner  
//		GUI.matrix = Matrix4x4.TRS(positionVector, Quaternion.identity, screenScale);		
		/*
		if (navScreen == eNavScreen.Main) {
			GUI.matrix = mainMenu.scalingMatrix;
			navScreen = mainMenu.DrawGUI();
		}
		if (navScreen == eNavScreen.OpenFeint) {
			GUI.matrix = ofSample.scalingMatrix;
			if (ofSample.DrawGUI()) {
				navScreen = eNavScreen.Main;
			}
		}
		if (navScreen == eNavScreen.OpenFeintX) {
			GUI.matrix = ofxSample.scalingMatrix;			
			if (ofxSample.DrawGUI()) {
				navScreen = eNavScreen.Main;
			}
		}
		*/
	}
}
