var firstInstructionTexture : Texture2D;
var secondInstructionTexture : Texture2D;
var InstructionsMaterial : Material;
private var currentTime : float;

function Awake()
{
	
}
function Update () {
	
		if((Time.time) % 4 >= 0 && (Time.time) % 4 < 1)
		{
			InstructionsMaterial.mainTexture = secondInstructionTexture;
		}
		else if((Time.time) % 2 >=0 && (Time.time) % 2 < 1)
		{
			InstructionsMaterial.mainTexture = firstInstructionTexture;
		}
}