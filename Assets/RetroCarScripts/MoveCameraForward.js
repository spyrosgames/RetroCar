var car : GameObject;
private var globals : Globals;

function Awake()
{
	globals = Globals.GetInstance();
}
function Update () {
		transform.Translate(Vector3(0, 4, 0));
}