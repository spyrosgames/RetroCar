private var globals : Globals;
var ButtonTexture : Texture2D;
var guiSkin : GUISkin;
var car : Rigidbody;
var RightTouch : Joystick;
var CollisionCamera : Camera;
function Start()
{
	globals = Globals.GetInstance();
}

function OnGUI () {
	GUI.skin = guiSkin;
	//if(GUI.Button(Rect(globals.RightButtonX, globals.RightButtonY, globals.RightButtonSizeX, globals.RightButtonSizeY), ButtonTexture))
	var absJoyPos = Vector2(Mathf.Abs(RightTouch.position.x), Mathf.Abs(RightTouch.position.y));
	if((RightTouch.tapCount > 0))
	//if(absJoyPos.x > absJoyPos.y && absJoyPos.x > 0)
	{
		if(car != null)
		{
			car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
			car.position = Vector3(-12, car.position.y, -7.433446);
			globals.lastCarPositionY = car.position.y;
		}
	}

	if((RightTouch.tapCount > 0) && (absJoyPos.y > absJoyPos.x))
	{
		globals.accelerateButtonPressed = true;
		car.position = Vector3(car.position.x, car.position.y-(globals.subtractedValueFromEnemyCarY/2), car.position.z);
		//CollisionCamera.transform.position.y -= 2;
	}
}
function Wait()
{
	yield WaitForSeconds(1);
}
/*
function Update()
{
	for(touch in iPhoneInput.touches)
	{
		if(touch.phase == iPhoneTouchPhase.Moved || touch.phase == iPhoneTouchPhase.Began)
		{
			if(car != null)
			{
				car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
				car.position = Vector3(-9, car.position.y, -7.433446);
			}
		}
	} 
}
*/
