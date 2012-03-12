var car : Rigidbody;
private var globals : Globals;

function Awake()
{
	globals = Globals.GetInstance();
}
function Update () {
	if(car != null)
	{
		transform.position = Vector3(0, car.position.y, 0);
	}
}