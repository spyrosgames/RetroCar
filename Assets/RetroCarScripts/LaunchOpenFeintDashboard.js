private var globals : Globals;
var latestScoreGUIText : GUIText;

function Awake()
{
	globals = Globals.GetInstance();
}
	
function Start()
{
	globals.numberLaunchOpenFeintDashboardCalled++;
	if(globals.numberLaunchOpenFeintDashboardCalled == 1)
	{
		OpenFeint.LaunchDashboard();
	}
	if(globals.numberLaunchOpenFeintDashboardCalled > 1)
	{
		latestScoreGUIText.text = "Last:" + (PlayerPrefs.GetInt("Score") - 1);
	}
}
