private var globals : Globals;
var ButtonTexture : Texture2D;
var guiSkin : GUISkin;
var car : Rigidbody;
var LeftTouch : Joystick;
var RightTouch : Joystick;

var CollisionCamera : Camera;

private var carAtCenter = true;
private var carAtRight = false;
private var carAtLeft = false;

function Start()
{
	globals = Globals.GetInstance();
}

function Update () {
	//GUI.skin = guiSkin;
	//if(GUI.Button(Rect(globals.RightButtonX, globals.RightButtonY, globals.RightButtonSizeX, globals.RightButtonSizeY), ButtonTexture))
	var absJoyPos = Vector2(Mathf.Abs(RightTouch.position.x), Mathf.Abs(RightTouch.position.y));

	//Move to Left
	if((RightTouch.position.x < -0.03) || Input.GetKey(KeyCode.LeftArrow))
	//if(absJoyPos.x > absJoyPos.y && absJoyPos.x > 0)
	{
		if(car != null)
		{
			if(globals.accelerateButtonPressed ==false)
			{
			//Move to Left
			car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
			car.position = Vector3(126, car.position.y, -7.433446);
				//car.position = Vector3(81.01407, car.position.y, -7.433446);
			globals.lastCarPositionY = car.position.y;
			}
		}

	}

	//Move to right
	if((RightTouch.position.x > 0.025) || Input.GetKey(KeyCode.RightArrow))
	{
		if(car != null)
		{
			if(globals.accelerateButtonPressed == false)
			{
			//Move to Right
			car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
			car.position = Vector3(-12, car.position.y, -7.433446); //Move one step to right
			globals.lastCarPositionY = car.position.y;
			}
		}
	}

	//if((RightTouch.tapCount > 0) && (absJoyPos.y > absJoyPos.x))
	if(absJoyPos.y > absJoyPos.x)
	{
		if(globals.accelerateButtonPressed)
		{
			car.position = Vector3(car.position.x, car.position.y-(globals.subtractedValueFromEnemyCarY/2), car.position.z);
		}
		//CollisionCamera.transform.position.y -= 2;
	}

	if(absJoyPos.x > absJoyPos.y || absJoyPos.x == absJoyPos.y)
	{
		globals.accelerateButtonPressed = false;
	}
	else
	{
		globals.accelerateButtonPressed = true;
	}
	/*
	if( (RightTouch.position.x > 0.001 && RightTouch.position.y > 0) )
	{
		globals.accelerateButtonPressed = true;
		car.position = Vector3(car.position.x, car.position.y-(globals.subtractedValueFromEnemyCarY/2), car.position.z);
		//CollisionCamera.transform.position.y -= 2;
	}
	*/
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
