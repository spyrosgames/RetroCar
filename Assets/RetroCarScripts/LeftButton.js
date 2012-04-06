private var globals : Globals;
var ButtonTexture : Texture2D;
var guiSkin : GUISkin;
var car : Rigidbody;
var LeftTouch : Joystick;
var CollisionCamera : Camera;
private var lastJoyPos : float = 0;
private var isInLeft;
private var isInRight;

function Start()
{
	globals = Globals.GetInstance();
}


function OnGUI() {
	GUI.skin = guiSkin;
	//if(GUI.Button(Rect(globals.LeftButtonX, globals.LeftButtonY, globals.LeftButtonSizeX, globals.LeftButtonSizeY), ButtonTexture))
	var absJoyPos = Vector2(Mathf.Abs(LeftTouch.position.x), Mathf.Abs(LeftTouch.position.y));
	/*
	if((LeftTouch.tapCount > 0))
	{
		if(car != null)
		{
			car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
			car.position = Vector3(126, car.position.y, -7.433446);
			globals.lastCarPositionY = car.position.y;
		}
	}
	*/

	if((absJoyPos.x > 0))
	//if(absJoyPos.x > absJoyPos.y && absJoyPos.x > 0)
	{
		if(car != null)
		{
			//Move to Left
			car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
			car.position = Vector3(34.4553, car.position.y, -7.433446);
			globals.lastCarPositionY = car.position.y;
		}
	}
		

	//Accelerate
	if((LeftTouch.tapCount > 0) && (absJoyPos.y > absJoyPos.x))
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
				car.position = Vector3(126, car.position.y, -7.433446);
			}
		}
	} 
}
*/