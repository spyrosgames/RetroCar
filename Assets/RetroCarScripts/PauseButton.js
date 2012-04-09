private var globals : Globals;
public var pauseTexture : Texture2D;
public var resumeTexture : Texture2D;


public var pauseCupTexture : Texture2D;
public var pauseCupTransparentTexture : Texture2D;
private  var currentButtonTexture : Texture2D;
private var paused : boolean;
private var muted : boolean;
var guiSkin : GUISkin;
var audioClip : AudioClip;
var retroCar : GameObject;
private var windowRect;
var PauseFontMaterial : Material;
var pauseMenuBackground : Texture2D;
function Awake()
{
	globals = Globals.GetInstance();
	paused = false;
	currentButtonTexture = pauseTexture;
	audio.clip = audioClip;
}

function OnGUI()
{
	GUI.skin = guiSkin;
	if(GUI.Button(Rect(globals.PauseButtonX, globals.PauseButtonY, globals.PauseButtonSizeX, globals.PauseButtonSizeY), currentButtonTexture))
	{
		if(!globals.muted)
		{
			audio.PlayOneShot(audioClip);
		}
		if(!paused)
		{
			if(retroCar != null)
			{
				retroCar.audio.Pause();
			}
			currentButtonTexture = resumeTexture;
			paused = true;
			Time.timeScale = 0;
		}
		else
		{
			if(retroCar != null)
			{
				if(globals.crashed == false && globals.muted == false)
				{
					retroCar.audio.Play();
				}
			}
			currentButtonTexture = pauseTexture;
			paused = false;
			Time.timeScale = 1;
		}
	}
	if(paused)
	{
		GUI.color = Color.white;
		windowRect = GUI.Window(0, Rect(17.2, 102, 190, 230), DoMyWindow, "");
	}	
}

function DoMyWindow (windowID : int) {
	if (GUI.Button (Rect (30,0,130,60), "Resume"))
	{		
			audio.PlayOneShot(audioClip);
			if(retroCar != null)
			{
				if(globals.crashed == false && globals.muted == false)
				{
					retroCar.audio.Play();
				}
			}
			currentButtonTexture = pauseTexture;
			paused = false;
			Time.timeScale = 1;
	}
	if (GUI.Button (Rect (20,120,145,60), "Main Menu"))
	{
			paused = false;
			Time.timeScale = 1;
			globals.i = 1;
			globals.cameraDampTime = 0.97;
			globals.lifesCount = 4;
			globals.scoreCount = 0;
			globals.enemySeparationDistance = 714.29999;
			Application.LoadLevel("mainmenu");
	}
	if(GUI.Button(Rect(20, 60, 155, 60), "Restart Level"))
	{
			paused = false;
			Time.timeScale = 1;
			globals.i = 1;
			globals.cameraDampTime = 0.97;
			globals.lifesCount = 4;
			globals.scoreCount = 0;
			globals.enemySeparationDistance = 714.29999;
			Application.LoadLevel("one");
	}
	if (GUI.Button (Rect (19,170,155,60), globals.MuteText))
	{		
			if(!globals.muted)
			{
				//audio.PlayOneShot(audioClip);
				if(retroCar != null)
				{
					retroCar.audio.Stop();
				}
				currentButtonTexture = pauseTexture;
				globals.MuteText = "Unmute";
				//muted = true;
				globals.muted = true;
				paused = false;
				Time.timeScale = 1;
			}
			else if(globals.muted)
			{
				//audio.PlayOneShot(audioClip);
				if(retroCar != null)
				{
					if(globals.crashed == false)
					{
						retroCar.audio.Play();
					}
				}
				currentButtonTexture = pauseTexture;
				globals.MuteText = "Mute";
				//muted = false;
				globals.muted = false;
				paused = false;
				Time.timeScale = 1;
			}
	}
}