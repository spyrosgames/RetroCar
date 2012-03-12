private var globals : Globals;
var guiSkin : GUISkin;

function Start()
{
	globals = Globals.GetInstance();
}

function OnGUI () {
	GUI.skin = guiSkin;
	if(GUI.Button(Rect(globals.ScoreButtonX, globals.ScoreButtonY, globals.ScoreButtonSizeX, globals.ScoreButtonSizeY), "High Scores"))
	{
		//Application.LoadLevel("highscores");
		OpenFeint.LaunchDashboardWithLeaderboardID(globals.OpenFeintLeaderBoardID);
	}
}
