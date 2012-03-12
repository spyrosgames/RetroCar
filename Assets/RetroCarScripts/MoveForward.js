var car : Rigidbody;
private var globals : Globals;
private var scoreIncrease : ScoreIncrease;
private var currentScoreCount : int;
private var currentCameraDamp : float;
private var currentEnemySeparationDistance : float;
private var currentCollisionLimitSeparationDistance : int;

var speedNumberGUIText : GUIText;
var audioClip : AudioClip;
var MainCamera : Camera;

function Awake()
{
	globals = Globals.GetInstance();
	audio.clip = audioClip;
	audio.playOnAwake = true;
	audio.loop = true;
	if(globals.muted == false)
	{
		audio.Play();
	}
	speedNumberGUIText.text = "0";
}

function Start()
{
	currentCameraDamp = globals.cameraDampTime;
	currentEnemySeparationDistance = globals.enemySeparationDistance;
	currentCollisionLimitSeparationDistance = globals.collisionLimitSeparationDistance;
}
function Update()
{
	if(Input.GetKey("right"))
	{
		//transform.Translate(-20, 0, 0);
		//car.MovePosition(Vector3(-15, 15, 0));
		car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
		car.position = Vector3(-12, car.position.y, -7.433446);	
	}
	if(Input.GetKey("left"))
	{
		//transform.Translate(20, 0, 0);
		car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
		car.position = Vector3(126, car.position.y, -7.433446);
	}
	MainCamera.transform.position.x=0.5351226;
}
function FixedUpdate()
{
	if(car != null)
	{
		globals.scoreCount++;
		if(globals.scoreCount <= 200)
		{
			globals.subtractedValueFromEnemyCarY = 6.1;	
			speedNumberGUIText.text = "1";
		}
		else if(globals.scoreCount > 200 && globals.scoreCount <= 400)
		{
			globals.subtractedValueFromEnemyCarY = 7.1;
			globals.collisionLimitSeparationDistance = 714;
			globals.cameraDampTime = currentCameraDamp - 0.12;
			speedNumberGUIText.text = "2";
		}
		else if(globals.scoreCount > 400 && globals.scoreCount <= 600)
		{
			globals.subtractedValueFromEnemyCarY = 8.1;
			globals.collisionLimitSeparationDistance = 715;
			globals.cameraDampTime = currentCameraDamp - 0.185;
			speedNumberGUIText.text = "3";
		}
		else if(globals.scoreCount > 600 && globals.scoreCount <= 800)
		{
			globals.subtractedValueFromEnemyCarY = 9.1;
			globals.collisionLimitSeparationDistance = 715;
			globals.cameraDampTime = currentCameraDamp - 0.28;
			speedNumberGUIText.text = "4";
			globals.enemySeparationDistance = currentEnemySeparationDistance + 88.69999;
			globals.collisionLimitSeparationDistance = 803;
		}
		else if(globals.scoreCount > 800 && globals.scoreCount <= 1000)
		{
			globals.subtractedValueFromEnemyCarY = 10.1;
			//globals.collisionLimitSeparationDistance = 1000;	
			globals.cameraDampTime = currentCameraDamp - 0.33;
			speedNumberGUIText.text = "5";
			globals.enemySeparationDistance = currentEnemySeparationDistance + 88.69999;
			globals.collisionLimitSeparationDistance = 803.05;
		}
		else if(globals.scoreCount > 1000 && globals.scoreCount <= 1200)
		{
			globals.subtractedValueFromEnemyCarY = 11.1;	
			globals.cameraDampTime = currentCameraDamp - 0.38;
			speedNumberGUIText.text = "6";
			globals.enemySeparationDistance = currentEnemySeparationDistance + 88.69999;
			globals.collisionLimitSeparationDistance = 803.05;
		}
		else if(globals.scoreCount > 1200 && globals.scoreCount <= 1400)
		{
			globals.subtractedValueFromEnemyCarY = 12.5;	
			globals.cameraDampTime = currentCameraDamp - 0.43;
			speedNumberGUIText.text = "7";
			globals.enemySeparationDistance = currentEnemySeparationDistance + 88.69999;
			globals.collisionLimitSeparationDistance = 803.05;
		}
		else if(globals.scoreCount > 1400 && globals.scoreCount <= 1600)
		{
			globals.subtractedValueFromEnemyCarY = 13.1;	
			globals.cameraDampTime = currentCameraDamp - 0.45;
			speedNumberGUIText.text = "8";
			globals.enemySeparationDistance = currentEnemySeparationDistance + 88.69999;
			globals.collisionLimitSeparationDistance = 803.05;
		}
		else if(globals.scoreCount > 1600 && globals.scoreCount <= 1800)
		{
			globals.subtractedValueFromEnemyCarY = 13.5;	
			globals.cameraDampTime = currentCameraDamp - 0.46;
			speedNumberGUIText.text = "9";
			globals.enemySeparationDistance = currentEnemySeparationDistance + 88.69999;
			globals.collisionLimitSeparationDistance = 803.05;
		}
		else
		{
			globals.subtractedValueFromEnemyCarY = 14;	
			globals.cameraDampTime = currentCameraDamp - 0.47;
			speedNumberGUIText.text = "10";
			globals.enemySeparationDistance = currentEnemySeparationDistance + 88.69999;
			globals.collisionLimitSeparationDistance = 803.05;
		}
		car.transform.position = Vector3(car.position.x, car.position.y-globals.subtractedValueFromEnemyCarY, car.position.z);
		//MainCamera.transform.Translate(Vector3(0, -((car.position.y-globals.subtractedValueFromEnemyCarY)/50), 0));
		//car.transform.position = Vector3.Lerp(car.transform.position, car.transform.position - Vector3(0, globals.subtractedValueFromEnemyCarY, 0), Time.deltaTime*100);
		PlayerPrefs.SetString("Speed", speedNumberGUIText.text);
	}
}

function Wait()
{
	yield WaitForSeconds(1*Time.deltaTime);
}