class ScoreIncrease extends MonoBehaviour
{
	var car : Rigidbody;
	private var globals : Globals;
	var scoreCountGUIText : GUIText;
	var scoreCountLabelGUIText : GUIText;
	
	function ScoreIncrease()
	{
	
	}
	
	function Start()
	{
		globals = Globals.GetInstance();
	}
	
	function Update () {
		incrementScore();
	}
	
	
	function incrementScore()
	{
		if(car != null)
		{
			//globals.scoreCount++;
			scoreCountGUIText.text = globals.scoreCount+"";
		}
	}
}