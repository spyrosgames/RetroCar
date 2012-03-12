private var globals : Globals;
var LifesGUITexture : GUITexture;
var defaultLifesGUITexture : Texture2D;

function Start()
{
	globals = Globals.GetInstance();
}
function Update () {
	/*
	if(globals.lifesCount == 4)
	{
		LifesGUITexture.texture = defaultLifesGUITexture;
	}
	else
	{
		LifesGUITexture.texture = globals.currentLifesGUITexture;
	}
	*/
	LifesGUITexture.texture = globals.currentLifesGUITexture;
}