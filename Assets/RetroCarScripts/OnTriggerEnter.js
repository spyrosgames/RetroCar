var topCollider : GameObject;
var grid : GameObject;
var enemyCar : GameObject;
private var globals : Globals;
var topColliderPrefab : GameObject;
var gridPrefab : GameObject;
var enemyCarPrefab : GameObject;
var rightPavementPrefab : GameObject;
var leftPavementPrefab : GameObject;
var collisionLimitPrefab : GameObject;

private var enemyCarRightPosition : float;
private var enemyCarLeftPosition : float;
private var enemyCarCenterPosition : float;
private var enemyCarPositions : float[];
private var secondEnemyCarPositions : float[];
private var enemyCarPositions_After5000 : float[];

private var randomEnemyCarPosition : float;
private var gridClone;
private var enemyCarClone : GameObject;
private var secondEnemyCarClone : GameObject;
private var topColliderClone;
private var rightPavementClone;
private var leftPavementClone;
private var collisionLimitClone : GameObject;
private var collisionLimitCloneArray : GameObject[];
private var timesOnTriggerExitCalled : int;
private var probabilityOfSecondCarHappening : int;
/*
function Awake()
{
	DontDestroyOnLoad(topCollider);
}
*/

function Start()
{
	globals = Globals.GetInstance();
	
	enemyCarRightPosition = -(globals.enemyCarX - 114);
	enemyCarLeftPosition = globals.enemyCarX;
	enemyCarCenterPosition = 81.01407;

	enemyCarPositions = [enemyCarRightPosition, enemyCarLeftPosition, enemyCarLeftPosition];
	secondEnemyCarPositions = [enemyCarLeftPosition, enemyCarRightPosition, enemyCarRightPosition];

	enemyCarPositions_After5000 = [-( (globals.enemyCarX + 48) - 114), globals.enemyCarX + 48, globals.enemyCarX + 48];

	Debug.Log("Start called.");
	globals.spawned = true;
}

function OnTriggerExit(other : Collider)
{
	randomEnemyCarPosition = Random.Range(0, 3);
	probabilityOfSecondCarHappening = Random.Range(0, 4);

	gridClone = Instantiate(Resources.Load("GridPrefab"), Vector3(globals.gridX, globals.gridY - (globals.gridSeparationDistance*globals.i), globals.gridZ), Quaternion.Euler(Vector3(0, 0, 0)));

	if(globals.scoreCount < 1000)
	{
		enemyCarClone = Instantiate(Resources.Load("EnemyCarPrefab"), Vector3(enemyCarPositions[randomEnemyCarPosition], globals.enemyCarY - (globals.enemySeparationDistance*globals.i), globals.enemyCarZ), Quaternion.Euler(Vector3(270, 0, 0)));
		enemyCarClone.rigidbody.isKinematic = true;
		enemyCarClone.tag = "EnemyCar";

		topColliderClone = Instantiate(topCollider, Vector3(globals.topColliderX, globals.topColliderY - (globals.gridSeparationDistance*globals.i - 300), globals.topColliderZ), Quaternion.Euler(Vector3(270, 0, 0)));
		topColliderClone.tag = "topColliderClones";


		collisionLimitClone = Instantiate(Resources.Load("CollisionLimitPrefab"), Vector3(globals.collisionLimitX, globals.collisionLimitY- (globals.collisionLimitSeparationDistance*globals.i), globals.collisionLimitZ), Quaternion.Euler(Vector3(270, 0, 0)));
		collisionLimitClone.tag = "CollisionLimit";

	}
	if(globals.scoreCount >= 1000 && globals.scoreCount <= 5000)
	{
		enemyCarClone = Instantiate(Resources.Load("EnemyCarPrefab"), Vector3(enemyCarPositions[randomEnemyCarPosition], globals.enemyCarY - (globals.enemySeparationDistance*globals.i), globals.enemyCarZ), Quaternion.Euler(Vector3(270, 0, 0)));
		enemyCarClone.rigidbody.isKinematic = true;
		enemyCarClone.tag = "EnemyCar";
	
		topColliderClone = Instantiate(topCollider, Vector3(globals.topColliderX, globals.topColliderY - (globals.gridSeparationDistance*globals.i - 300), globals.topColliderZ), Quaternion.Euler(Vector3(270, 0, 0)));
		topColliderClone.tag = "topColliderClones";


		collisionLimitClone = Instantiate(Resources.Load("CollisionLimitPrefab"), Vector3(globals.collisionLimitX, globals.collisionLimitY- (globals.collisionLimitSeparationDistance*globals.i), globals.collisionLimitZ), Quaternion.Euler(Vector3(270, 0, 0)));
		collisionLimitClone.tag = "CollisionLimit";
	}
	if(globals.scoreCount >5000 && globals.scoreCount <= 10000)
	{
		enemyCarClone = Instantiate(Resources.Load("EnemyCarPrefab"), Vector3(enemyCarPositions_After5000[randomEnemyCarPosition], globals.enemyCarY-(globals.enemySeparationDistance*globals.i), globals.enemyCarZ), Quaternion.Euler(Vector3(270, 0, 0)));
		enemyCarClone.rigidbody.isKinematic = true;
		enemyCarClone.tag = "EnemyCar";
	
		topColliderClone = Instantiate(topCollider, Vector3(globals.topColliderX, globals.topColliderY - (globals.gridSeparationDistance*globals.i - 300), globals.topColliderZ), Quaternion.Euler(Vector3(270, 0, 0)));
		topColliderClone.tag = "topColliderClones";


		collisionLimitClone = Instantiate(Resources.Load("CollisionLimitPrefab"), Vector3(globals.collisionLimitX, globals.collisionLimitY- (globals.collisionLimitSeparationDistance*globals.i), globals.collisionLimitZ), Quaternion.Euler(Vector3(270, 0, 0)));
		collisionLimitClone.tag = "CollisionLimit";
	}
	
	if(globals.scoreCount >= 1000 && globals.scoreCount <= 2000)
	{
		if(probabilityOfSecondCarHappening == 1)
		{
			secondEnemyCarClone = Instantiate(Resources.Load("EnemyCarPrefab"), Vector3(secondEnemyCarPositions[randomEnemyCarPosition], globals.enemyCarY-(globals.enemySeparationDistance*globals.i) + (globals.enemySeparationDistance*0.5), globals.enemyCarZ), Quaternion.Euler(Vector3(270, 0, 0)));

			secondEnemyCarClone.rigidbody.isKinematic = true;
			secondEnemyCarClone.tag = "SecondEnemyCar";

			collisionLimitClone = Instantiate(Resources.Load("CollisionLimitPrefab"), Vector3(globals.collisionLimitX, secondEnemyCarClone.transform.position.y - 129, globals.collisionLimitZ), Quaternion.Euler(Vector3(270, 0, 0)));
			collisionLimitClone.tag = "SecondCollisionLimit";
		}
	}
	
	if(globals.scoreCount > 2000 && globals.scoreCount <= 3000)
	{
		if(probabilityOfSecondCarHappening == 1 || probabilityOfSecondCarHappening == 2)
		{
			secondEnemyCarClone = Instantiate(Resources.Load("EnemyCarPrefab"), Vector3(secondEnemyCarPositions[randomEnemyCarPosition], globals.enemyCarY-(globals.enemySeparationDistance*globals.i) + (globals.enemySeparationDistance*0.5), globals.enemyCarZ), Quaternion.Euler(Vector3(270, 0, 0)));

			secondEnemyCarClone.rigidbody.isKinematic = true;
			secondEnemyCarClone.tag = "SecondEnemyCar";
		}
	}
	if(globals.scoreCount > 3000 && globals.scoreCount <= 4000)
	{
		if(probabilityOfSecondCarHappening == 0 || probabilityOfSecondCarHappening == 1 || probabilityOfSecondCarHappening == 2)
		{
			secondEnemyCarClone = Instantiate(Resources.Load("EnemyCarPrefab"), Vector3(secondEnemyCarPositions[randomEnemyCarPosition], globals.enemyCarY-(globals.enemySeparationDistance*globals.i) + (globals.enemySeparationDistance*0.5), globals.enemyCarZ), Quaternion.Euler(Vector3(270, 0, 0)));

			secondEnemyCarClone.rigidbody.isKinematic = true;
			secondEnemyCarClone.tag = "SecondEnemyCar";
		}
	}
	if(globals.scoreCount > 4000 && globals.scoreCount <= 5000)
	{
			secondEnemyCarClone = Instantiate(Resources.Load("EnemyCarPrefab"), Vector3(secondEnemyCarPositions[randomEnemyCarPosition], globals.enemyCarY-(globals.enemySeparationDistance*globals.i) + (globals.enemySeparationDistance*0.5), globals.enemyCarZ), Quaternion.Euler(Vector3(270, 0, 0)));

			secondEnemyCarClone.rigidbody.isKinematic = true;
			secondEnemyCarClone.tag = "SecondEnemyCar";
	}
	if(globals.scoreCount > 5000 && globals.scoreCount <= 10000)
	{
			secondEnemyCarClone = Instantiate(Resources.Load("EnemyCarPrefab"), Vector3(secondEnemyCarPositions[randomEnemyCarPosition], globals.enemyCarY-(globals.enemySeparationDistance*globals.i) + (globals.enemySeparationDistance*0.5), globals.enemyCarZ), Quaternion.Euler(Vector3(270, 0, 0)));

			secondEnemyCarClone.rigidbody.isKinematic = true;
			secondEnemyCarClone.tag = "SecondEnemyCar";
	}
	/*
	if(randomEnemyCarPosition == 0)
	{
		enemyCarClone.rigidbody.transform.position.x = 126.0737;
	}
	else if(randomEnemyCarPosition == 1)
	{
		enemyCarClone.rigidbody.transform.position.x = -12.11494;
	}
	*/
	

	//secondEnemyCarClone = Instantiate(secondEnemyCarPrefab, Vector3(enemyCarPositions[randomEnemyCarPosition], globals.secondEnemyCarY-(globals.enemySeparationDistance*globals.i), globals.secondEnemyCarZ), Quaternion.Euler(Vector3(270, 0, 0)));
	//secondEnemyCarClone.rigidbody.isKinematic = true;
	//secondEnemyCarClone.tag = "EnemyCar";
	

	
	rightPavementClone = Instantiate(Resources.Load("RightPavementPrefab"), Vector3(globals.rightPavementX, globals.rightPavementY - (globals.gridSeparationDistance*globals.i), globals.rightPavementZ), Quaternion.Euler(Vector3(0, 0, 0)));
	leftPavementClone = Instantiate(Resources.Load("LeftPavementPrefab"), Vector3(globals.leftPavementX, globals.leftPavementY - (globals.gridSeparationDistance*globals.i), globals.leftPavementZ), Quaternion.Euler(Vector3(0, 0, 0)));

	/*
	collisionLimitCloneArray[globals.i - 1] = collisionLimitClone;
	
	if(collisionLimitCloneArray.length > 1)
	{
		globals.currentCollisionsLimitObject = collisionLimitCloneArray[globals.i-2];
	}
	else
	{
		globals.currentCollisionsLimitObject = collisionLimitCloneArray[globals.i-1];
	}
	*/
	Destroy(topCollider);
	//topCollider.active = false;
	/*
	globals.gridClonesArray[globals.i] = gridClone;
	globals.enemyCarClonesArray[globals.i] = enemyCarClone;
	globals.topColliderClonesArray[globals.i] = topColliderClone;
	globals.rightPavementClonesArray[globals.i] = rightPavementClone;
	globals.leftPavementClonesArray[globals.i] = leftPavementClone;
	*/
	
	globals.i++;
	
	Destroy(gridClone, 20);
	Destroy(enemyCarClone, 20);
	Destroy(topColliderClone, 20);
	Destroy(rightPavementClone, 20);
	Destroy(leftPavementClone, 20);
	//Destroy(collisionLimitClone, 20);
}