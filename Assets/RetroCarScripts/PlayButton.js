private var globals : Globals;
var guiSkin : GUISkin;
private var guiStyle : GUIStyle;
private var asyncOperation : AsyncOperation;
var loadingGUIText : GUIText;
private var moveToTheNextLevel : boolean = false;

function Start()
{
	guiStyle = new GUIStyle();
	globals = Globals.GetInstance();
}
function Update()
{
	
}
function OnGUI () {
	GUI.skin = guiSkin;
	if(GUI.Button(Rect(globals.PlayButtonX, globals.PlayButtonY, globals.PlayButtonSizeX, globals.PlayButtonSizeY), "Play"))
	{
		/*if(globals.lifesCount != 0)
		{
			globals.lifesCount = 0;
		}
		if(globals.scoreCount != 0)
		{
			globals.scoreCount = 0;
		}
		if(globals.i != 1)
		{
			globals.i = 1;
		}
		*/
		moveToTheNextLevel = true;
		asyncOperation = Application.LoadLevelAsync("one");
		if(!asyncOperation.isDone)
		{
			loadingGUIText.text = "Loading...";
		}
		//Application.LoadLevel("one");
	}
}

