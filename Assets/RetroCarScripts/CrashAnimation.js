private var AnimationPlane : GameObject;
private var RetroCar : GameObject;
private var firstFrame : GameObject;
private var secondFrame : GameObject;
private var globals : Globals;

class CrashAnimation
{
	public function CrashAnimation()
	{
		AnimationPlane = GameObject.Find("AnimationPlane");
		RetroCar = GameObject.Find("RetroCar");
		firstFrame = GameObject.Find("frame1");
		secondFrame = GameObject.Find("frame2");
		globals = Globals.GetInstance();
	}
	
	public function Animate()
	{
		AnimationPlane.renderer.enabled = true;
		AnimationPlane.transform.position.x = globals.RetroCarX;
		AnimationPlane.transform.position.y = globals.RetroCarY;
		AnimationPlane.transform.position.z = globals.RetroCarZ;
		AnimationPlane.renderer.material.mainTexture = firstFrame.renderer.material.mainTexture;
		
		yield WaitForSeconds(3);
				
		AnimationPlane.renderer.material.mainTexture = secondFrame.renderer.material.mainTexture;
				
		yield WaitForSeconds(3);
				
		AnimationPlane.renderer.material.mainTexture = firstFrame.renderer.material.mainTexture;
				
		yield WaitForSeconds(3);
				
		AnimationPlane.renderer.material.mainTexture = secondFrame.renderer.material.mainTexture;
	}
}