public var AnimationPlane : GameObject;
private var globals : Globals;
public var frames : Texture2D[];
public var scoreCountGUIText : GUIText;
public var LifesGUITexture : GUITexture;
public var speedNumberGUIText : GUIText;

private var currentLifesGUITexture : int;

var fourLifesGUITexture : Texture2D;
var threeLifesGUITexture : Texture2D;
var twoLifesGUITexture : Texture2D;
var oneLifeGUITexture : Texture2D;

function Awake()
{
	scoreCountGUIText.text = (PlayerPrefs.GetInt("Score") - 1)+"";
	currentLifesGUITexture = PlayerPrefs.GetInt("lifesGUITexture");
	speedNumberGUIText.text = PlayerPrefs.GetString("Speed");
	
	/*
	var RightButtonScript : RightButton = GameObject.Find("RightButton").GetComponent(RightButton);
	RightButtonScript.enabled = false;
	
	var LeftButtonScript : LeftButton = GameObject.Find("LeftButton").GetComponent(LeftButton);
	LeftButtonScript.enabled = false;
	
	var AccelerateButtonScript : AccelerateButton = GameObject.Find("AccelerateButton").GetComponent(AccelerateButton);
	AccelerateButtonScript.enabled = false;
	*/
	
	if(currentLifesGUITexture == 4)
	{
		LifesGUITexture.texture = fourLifesGUITexture;
	}
	else if(currentLifesGUITexture == 3)
	{
		LifesGUITexture.texture = threeLifesGUITexture;
	}
	else if(currentLifesGUITexture == 2)
	{
		LifesGUITexture.texture = twoLifesGUITexture;
	}
	else if(currentLifesGUITexture == 1)
	{
		LifesGUITexture.texture = oneLifeGUITexture;
	}
	else
	{
		LifesGUITexture.texture = null;
	}
}

function Start()
{
	globals = Globals.GetInstance();
	AnimationPlane.renderer.enabled = true;
	//AnimationPlane.transform.position.x = globals.AfterCrashScreenX;
	//AnimationPlane.transform.position.y = globals.AfterCrashScreenY;
	//AnimationPlane.transform.position.z = globals.AfterCrashScreenZ;
	
	for(var i : int = 0; i < frames.length - 1; i++)
	{
		AnimationPlane.renderer.material.mainTexture = frames[i];
		yield WaitForSeconds(0.07);
	}
	
	for(var j : int = frames.length - 1; j >= 0; j--)
	{
		AnimationPlane.renderer.material.mainTexture = frames[j];
		yield WaitForSeconds(0.07);
	}
	globals.i = 1;
	globals.cameraDampTime = 0.97;
	globals.crashed = false;
	if(globals.lifesCount != 0)
	{
		globals.enemySeparationDistance = 714.29999;
		Application.LoadLevel("one");
	}
	else if(globals.lifesCount == 0)
	{
		globals.lifesCount = 4;
		globals.scoreCount = 0;
		globals.enemySeparationDistance = 714.29999;
		Application.LoadLevel("mainmenu");
	}
}

function Update()
{

}
