var audioClip : AudioClip;
private var globals : Globals;

function Awake()
{
	globals = Globals.GetInstance();
	
	audio.clip = audioClip;
	if(globals.muted == false)
	{
		audio.PlayOneShot(audioClip);
	}
}