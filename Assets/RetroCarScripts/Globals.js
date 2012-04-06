class Globals
{
	public var i : int = 1;
	public var topColliderY : float;
	public var topColliderX : float;
	public var topColliderZ : float;
	public var gridX : float;
	public var gridY : float;
	public var gridZ : float;
	public var gridSeparationDistance : float;
	public var enemySeparationDistance : float;
	public var collisionLimitSeparationDistance : float;
	public var enemyCarX : float;
	public var enemyCarY : float;
	public var enemyCarZ : float;
	
	public var secondEnemyCarX : float;
	public var secondEnemyCarY : float;
	public var secondEnemyCarZ : float;
	
	public var rightPavementX : float;
	public var rightPavementY : float;
	public var rightPavementZ : float;
	public var leftPavementX : float;
	public var leftPavementY : float;
	public var leftPavementZ : float; 
	public var spawned : boolean = false;
	public var lifesCount : int;
	public var scoreCount : int;
	public var currentLifesGUITexture : Texture2D;
	public var gridClonesArray = new Array();
	public var enemyCarClonesArray = new Array();
	public var topColliderClonesArray = new Array();	
	public var rightPavementClonesArray = new Array();
	public var leftPavementClonesArray = new Array();
	
	public var RightButtonX : float;
	public var RightButtonY : float;
	public var RightButtonSizeX : float;
	public var RightButtonSizeY : float;
	
	public var LeftButtonX : float;
	public var LeftButtonY : float;
	public var LeftButtonSizeX : float;
	public var LeftButtonSizeY : float;
	
	public var AccelerateButtonX : float;
	public var AccelerateButtonY : float;
	public var AccelerateButtonSizeX : float;
	public var AccelerateButtonSizeY : float;
	
	public var PlayButtonX : float;
	public var PlayButtonY : float;
	public var PlayButtonSizeX : float;
	public var PlayButtonSizeY : float;
	
	public var RetroCarX : float;
	public var RetroCarY : float;
	public var RetroCarZ : float;
	
	public var AfterCrashScreenX : float;
	public var AfterCrashScreenY : float;
	public var AfterCrashScreenZ : float;
	
	public var PauseButtonX : float;
	public var PauseButtonY : float;
	public var PauseButtonSizeX : float;
	public var PauseButtonSizeY : float;
	
	public var ScoreButtonX : float;
	public var ScoreButtonY : float;
	public var ScoreButtonSizeX : float;
	public var ScoreButtonSizeY : float;
	
	public var collisionLimitX : float;
	public var collisionLimitY : float;
	public var collisionLimitZ : float;
	
	public var collisionLimitSizeX : float;
	public var collisionLimitSizeY : float;
	
	public var cameraDampTime : float;
	
	public var subtractedValueFromEnemyCarY;
	
	public var currentCollisionsLimitObject : GameObject;
	
	public var randomEnemyCarPosition : int;
	
	public var numberLaunchOpenFeintDashboardCalled : int;
	
	public var OpenFeintLeaderBoardID : int;
	
	public var crashed: boolean = false;
	
	public var muted : boolean = false;
	
	public var MuteText : String = "Mute";
	
	public var accelerateButtonPressed : boolean = false;
	
	public var lastCarPositionY : float;
	
	public var carAtLeft : boolean = false;
	public var carAtRight : boolean = false;
	public var carAtCenter : boolean = true;

	public var currentCarXPosition : float = 81.01407;

	private static var Instance : Globals;
	
	protected function Globals()
	{
		topColliderX = 95.31404;
		topColliderY = 45.65338;
		topColliderZ = -7.433446;
		
		gridX = 94.13937;
		gridY = -0.06588745;
		gridZ = 0;
		
		gridSeparationDistance = 803;
		enemySeparationDistance = 714.29999;
		collisionLimitSeparationDistance = 712;
		
		enemyCarX = 126.0737;
		//enemyCarX = 218.6932;
		enemyCarY = 136.0381;
		enemyCarZ = -7.433446;
		
		secondEnemyCarX = -12.32131;
		secondEnemyCarY = -88.68069;
		secondEnemyCarZ = -7.433446;
		
		rightPavementX = -116.3831;
		rightPavementY = -152.6413;
		rightPavementZ = -29.89357;
		
		leftPavementX = 299.1728;
		leftPavementY = rightPavementY;
		leftPavementZ = rightPavementZ;
		
		lifesCount = 4;
		
		RightButtonX = 125;
		RightButtonY = 400;
		RightButtonSizeX = 90;
		RightButtonSizeY = 90;
		
		AccelerateButtonX = 190;
		AccelerateButtonY = 400;
		AccelerateButtonSizeX = 60;
		AccelerateButtonSizeY = 90;
		
		LeftButtonX = 20;
		LeftButtonY = 400;
		LeftButtonSizeX = 90;
		LeftButtonSizeY = 90;
		
		PlayButtonX = 15;
		PlayButtonY = 305;
		PlayButtonSizeX = 200;
		PlayButtonSizeY = 90;
		
		/*
		PauseButtonX = 254;
		PauseButtonY = 416;
		PauseButtonSizeX = 50;
		PauseButtonSizeY = 55;
		*/
		
		PauseButtonX = 219;
		PauseButtonY = 285;
		PauseButtonSizeX = 120;
		PauseButtonSizeY = 122;
		
		ScoreButtonX = 0;
		ScoreButtonY = -17;
		ScoreButtonSizeX = 200;
		ScoreButtonSizeY = 90;
		
		collisionLimitX = 94.83465;
		collisionLimitY = 2.868954;
		collisionLimitZ = -7.433482;
		
		cameraDampTime = 0.97;   //0.73 //0.99
		
		subtractedValueFromEnemyCarY = 6.1;
		
		OpenFeintLeaderBoardID = 1107467;
		
	}
	
	public static function GetInstance() : Globals
	{
		if(Instance == null)
		{
			Instance = new Globals();
		}
		return Instance;
	}
	
	public static function Wait()
	{
		yield WaitForSeconds(Time.deltaTime*10);
	}
}