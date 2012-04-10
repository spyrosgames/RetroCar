private var globals : Globals;
private var enemyCarsArray : GameObject[];
private var secondEnemyCarArray : GameObject[];

function Start()
{
	globals = Globals.GetInstance();
} 


function Update () {
	if(globals.lifesCount < 4 && globals.lifesCount > 0 && globals.returningFromCrash)
	{
		
		if(globals.scoreCount > 1000)
		{
			enemyCarsArray = GameObject.FindGameObjectsWithTag("EnemyCar");
			secondEnemyCarArray = GameObject.FindGameObjectsWithTag("SecondEnemyCar");
			//Destroy(enemyCarsArray[0]);
			if(secondEnemyCarArray.length != 0)
			{
				Destroy(secondEnemyCarArray[0]);
				Debug.Log("Destroyed The First Car After Crash.");
			}

			if(enemyCarsArray.length != 0)
			{
				Destroy(enemyCarsArray[0]);
				Debug.Log("Destroyed The First Car After Crash.");
			}

		}
		globals.returningFromCrash = false;
	}
}