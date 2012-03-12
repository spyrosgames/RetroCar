private var globals : Globals;
var topColliderPrefab : GameObject;
var LifesGUITexture : GUITexture;
var fourLifesGUITexture : Texture2D;
var threeLifesGUITexture : Texture2D;
var twoLifesGUITexture : Texture2D;
var oneLifeGUITexture : Texture2D;

var frames : Texture2D[];
var AfterCrashScreenFrames : Texture2D[];
var CollisionLimit : GameObject;
private var framesPerSecond = 0.0001;
var AnimationPlane : GameObject;
var ToFollowObject : GameObject;
var RightJoystick : Joystick;
var LeftJoystick : Joystick;
var Car : GameObject;
private var startAnimation : boolean;

private var thisOther : Collision;
private var thisOtherX;
private var thisOtherY;
private var thisOtherZ;

private var retroCarHeadYDimension;
private var collisionSeparatorYDimension;

var carComponents : GameObject[];
var MainCamera : GameObject;
var CollisionCamera : GameObject;
var SecondMainCamera : GameObject;

private var timesSetOtherDimensionsCalled : int;
private var timesSetYDimensionsCalled : int;
private var timesCrashSoundPlayed : int;
private var timesOpenFeintSubmitScoreCalled : int;
private var asyncOperation : AsyncOperation;

public var retroCarHead : GameObject;
//public var collisionSeparator : GameObject;
var audioClip : AudioClip;
private var currentCameraDamp : float;
private var j : int = 0;

function Awake()
{
	
}

function Start()
{
	globals = Globals.GetInstance();

	if(globals.lifesCount == 4)
	{
		LifesGUITexture.texture = fourLifesGUITexture;
		PlayerPrefs.SetInt("lifesGUITexture", 4);
	}
	else if(globals.lifesCount == 0)
	{
		LifesGUITexture.texture = null;
	}
	else
	{
		LifesGUITexture.texture = globals.currentLifesGUITexture;
	}
	globals.lifesCount--;
}

function OnCollisionEnter(other : Collision)
{
	SetTouchesState(false);
		
	MainCamera.camera.enabled = false;
	CollisionCamera.camera.enabled = true;
	globals.crashed = true;
	
	var MoveForwardScript : MoveForward = GameObject.Find("RetroCar").GetComponent(MoveForward);
	MoveForwardScript.enabled = false;
	
	var ScoreIncreaseScript : ScoreIncrease = GameObject.Find("ToFollowObject").GetComponent(ScoreIncrease);
	ScoreIncreaseScript.enabled = false;
	
	var ToFollowObjectPushScript : ToFollowObjectPush = GameObject.Find("ToFollowObject").GetComponent(ToFollowObjectPush);	
	ToFollowObjectPushScript.enabled = false;
	
	
		
	var cameraFollowScript : CameraFollow = GameObject.Find("Collision Camera").GetComponent(CameraFollow);
	cameraFollowScript.enabled = false;
	
	/*
	if(globals.accelerateButtonPressed)
	{
		CollisionCamera.transform.position.y -= 1;
	}
	*/
	CollisionCamera.transform.position.x = 0.5351226;
	SecondMainCamera.active = false;
	SecondMainCamera.active = true;
	
	var collisionLimits = GameObject.FindGameObjectsWithTag("CollisionLimit");
	setYDimensions(retroCarHead.transform.position.y, collisionLimits[globals.i-2].transform.position.y);
	setOtherDimensions(other, other.transform.position.x, other.transform.position.y, other.transform.position.z);
	
	for(var i : int = 0; i < carComponents.length; i++)
	{
		carComponents[i].renderer.enabled = false;
	}
	var clones = GameObject.FindGameObjectsWithTag("topColliderClones");
	for(var clone in clones)
	{
		Destroy(clone);
	}
	
	PlayerPrefs.SetInt("Score", globals.scoreCount);

	
	if(other.gameObject.tag == "EnemyCar")
	{
		
		if(globals.lifesCount != 0)
		{
			if(globals.lifesCount == 3)
			{
				globals.currentLifesGUITexture = threeLifesGUITexture;
				PlayerPrefs.SetInt("lifesGUITexture", 3);
			}
			else if(globals.lifesCount == 2)
			{
				globals.currentLifesGUITexture = twoLifesGUITexture;
				PlayerPrefs.SetInt("lifesGUITexture", 2);
			}
			else if(globals.lifesCount == 1)
			{
				globals.currentLifesGUITexture = oneLifeGUITexture;
				PlayerPrefs.SetInt("lifesGUITexture", 1);
			}
			LifesGUITexture.texture = globals.currentLifesGUITexture;
			if(j == 0)
			{
				CollisionCamera.transform.position.y -= 2;
				Animate();
			}
			yield WaitForSeconds(6);    
		}
		else
		{
			PlayerPrefs.SetInt("lifesGUITexture", 0);
			LifesGUITexture.texture = null;
			if(j == 0)
			{
				Animate();
				SubmitOpenFeintScore();
			}
		}
		//Application.LoadLevel("two");
		//Destroy(gameObject);
	}
	j++;
	
}

function Animate()
{

	PlayCrashSound();
	AnimationPlane.renderer.enabled = true;
	if( (retroCarHeadYDimension > collisionSeparatorYDimension) || (thisOtherY == -3435.462) || (thisOtherY == -4149.762)) //because of negative values, > and < are inverted.
	{
		AnimationPlane.transform.position.x = thisOtherX + 37;
		AnimationPlane.transform.position.y = thisOtherY;
		AnimationPlane.transform.position.z = thisOtherZ;
	}
	else if(retroCarHeadYDimension < collisionSeparatorYDimension)
	{
		if(thisOther.transform.position.x == 126.0737)
		{
			//AnimationPlane.transform.position.x = thisOtherX-100;
			AnimationPlane.transform.position.x = thisOtherX-100;
			AnimationPlane.transform.position.y = thisOtherY-180;
			AnimationPlane.transform.position.z = thisOtherZ;
		}
		else
		{
			AnimationPlane.transform.position.x = thisOtherX+180;
			AnimationPlane.transform.position.y = thisOtherY-180;
			AnimationPlane.transform.position.z = thisOtherZ;
		}
	}
	AnimationPlane.renderer.material.mainTexture = frames[0];
	yield WaitForSeconds(0.8);
	AnimationPlane.renderer.material.mainTexture = frames[1];
	yield WaitForSeconds(0.8);
	AnimationPlane.renderer.material.mainTexture = frames[0];
	yield WaitForSeconds(0.8);
	AnimationPlane.renderer.material.mainTexture = frames[1];
	yield WaitForSeconds(0.8);
	AnimationPlane.renderer.material.mainTexture = frames[0];
	yield WaitForSeconds(0.8);
	AnimationPlane.renderer.material.mainTexture = frames[1];
	yield WaitForSeconds(0.8);
	SetTouchesState(true);	
	asyncOperation = Application.LoadLevelAsync("two");

	yield WaitForSeconds(0.6);
}

function SubmitOpenFeintScore()
{
	if(OpenFeint.isOnline) {
		OpenFeint.InGameNotification("New HighScore : " + (globals.scoreCount-1) + " Submitted!");
		OpenFeint.SubmitHighScore( (globals.scoreCount-1), globals.OpenFeintLeaderBoardID);
		
	} else {
		OpenFeint.InGameNotification("The user is offline!");
	}
}
function Update()
{
}

function setOtherDimensions(other : Collision, otherX : float, otherY : float, otherZ : float)
{
	timesSetOtherDimensionsCalled++;
	if(timesSetOtherDimensionsCalled == 1)
	{
		thisOther = other;
		thisOtherX = otherX;
		thisOtherY = otherY;
		thisOtherZ = otherZ;
	}
}

function setYDimensions(retroCarHeadY : float, collisionSeparatorY : float)
{
	timesSetYDimensionsCalled++;
	if(timesSetYDimensionsCalled == 1)
	{
		retroCarHeadYDimension = retroCarHeadY;
		collisionSeparatorYDimension = collisionSeparatorY;
	}
}

function PlayCrashSound()
{
	timesCrashSoundPlayed++;
	if(timesCrashSoundPlayed == 1)
	{
		audio.clip = audioClip;
		if(!globals.muted)
		{
			audio.PlayOneShot(audioClip);
		}
	}
}

function SetTouchesState(state : boolean)
{
	RightJoystick.active = state;
	LeftJoystick.active = state;
}
/*
function AfterCrashScreenAnimation()
{
	AfterCrashScreenPlane.renderer.enabled = true;
	for(var i : int = 0; i < AfterCrashScreenFrames.length - 1; i++)
	{
		AfterCrashScreenPlane.renderer.material.mainTexture = AfterCrashScreenFrames[i];
		yield WaitForSeconds(0.1);
	}
	
	for(var j : int = AfterCrashScreenFrames.length - 1; j >= 0; j--)
	{
		AfterCrashScreenPlane.renderer.material.mainTexture = AfterCrashScreenFrames[j];
		yield WaitForSeconds(0.1);
	}
	globals.i = 1;
	globals.cameraDampTime = 0.93;
}
*/