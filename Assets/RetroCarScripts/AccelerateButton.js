private var globals : Globals;
var ButtonTexture : Texture2D;
var guiSkin : GUISkin;
var car : Rigidbody;
var AccelerateButtonGUITexture : GUITexture;

private var delay = 0;

function Start()
{
	globals = Globals.GetInstance();
}
/*
function OnGUI () {
	GUI.skin = guiSkin;
	if(GUI.Button(Rect(globals.AccelerateButtonX, globals.AccelerateButtonY, globals.AccelerateButtonSizeX, globals.AccelerateButtonSizeY), ButtonTexture))
	{
		if(Input.touchCount > 0)
		{
				if(TouchPhase.Began)
				{
					car.position = Vector3(car.position.x, car.position.y-8.1, car.position.z);
				}
		}
	}
}
*/

function Update()
{ 
	//for(touch in iPhoneInput.touches)
	if(iPhoneInput.touchCount > 0)
	{
		var touch : iPhoneTouch = iPhoneInput.touches[0];
		if(touch.phase == iPhoneTouchPhase.Began && guiTexture.HitTest(touch.position))
		{
			if(car != null)
			{
				car.position = Vector3(car.position.x, car.position.y-(globals.subtractedValueFromEnemyCarY*2), car.position.z);
			}
		}
	} 
}
