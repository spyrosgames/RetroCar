var car : Rigidbody;

function Update () {
	if(Input.touchCount == 1)
	{
		var touch : Touch = Input.touches[0];
		
		if(touch.phase == TouchPhase.Began && guiTexture.HitTest(touch.position))
		{
			if(this.gameObject.tag == "RightButton")
			{
				car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
				car.position = Vector3(-9, car.position.y, -7.433446);
			
			}
			if(this.gameObject.tag == "LeftButton")
			{
				car.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
				car.position = Vector3(126, car.position.y, -7.433446);
			}
		}
	}	
}