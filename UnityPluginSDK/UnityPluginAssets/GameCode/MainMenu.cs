using UnityEngine;
using System.Collections;

public class MainMenu : BaseUI {
  
	// Use this for initialization
		
	public eNavScreen DrawGUI() {
		DrawTitleBar("Unity Bridge Test App", null, null);
		#if UNITY_ANDROID
			GUI.skin.verticalScrollbar.fixedWidth = 45;
			GUI.skin.verticalScrollbarThumb.fixedWidth = 45;
		#endif
		
		Vector2 startPoint = new Vector2(0.0f, titleBarHeight + heightSeparator);
		
		if (GUI.Button(new Rect (modelScreenWidth / 2 - buttonWidth / 2, startPoint.y, buttonWidth, buttonHeight), "Launch Dashboard")) {
			OpenFeint.LaunchDashboard();
		}
		
		startPoint.y += buttonHeight + heightSeparator;
		
		if (GUI.Button(new Rect (modelScreenWidth / 2 - buttonWidth / 2, startPoint.y, buttonWidth, buttonHeight), "OpenFeint")) {
			return eNavScreen.OpenFeint;
		}
		
		startPoint.y += buttonHeight + heightSeparator;
		
		if (GUI.Button(new Rect (modelScreenWidth / 2 - buttonWidth / 2, startPoint.y, buttonWidth, buttonHeight), "OpenFeintX")) {
			return eNavScreen.OpenFeintX;
		}
		return eNavScreen.Main;
	}
}
