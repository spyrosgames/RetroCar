var dampTime : float = 0; //offset from the viewport center to fix damping
private var velocity = Vector3.zero;
var target : Transform;
private var globals : Globals;

function Start()
{
	globals = Globals.GetInstance();
}

function Update() {
	dampTime = globals.cameraDampTime;
    if(target) {
        var point : Vector3 = camera.WorldToViewportPoint(target.position);
        var delta : Vector3 = target.position - camera.ViewportToWorldPoint(Vector3(0.5, -0.18, point.z));
        var destination : Vector3 = transform.position + delta;
        transform.position = Vector3.SmoothDamp(transform.position, destination, velocity, dampTime);
    }
}