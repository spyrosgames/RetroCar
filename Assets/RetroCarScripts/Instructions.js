var firstInstructionTexture : Texture2D;
var secondInstructionTexture : Texture2D;
var thirdInstructionTexture : Texture2D;
var InstructionsMaterial : Material;
private var currentTime : float;

function Awake()
{
	
}
function Update () {
	
		if((Time.time) % 4 >= 0 && (Time.time) % 8 < 1)
		{
			InstructionsMaterial.mainTexture = thirdInstructionTexture;
		}
		else if((Time.time) % 2 >=0 && (Time.time) % 4 < 1)
		{
			InstructionsMaterial.mainTexture = secondInstructionTexture;
		}
		else if((Time.time) % 6 >=0 && (Time.time) % 2 < 1)
		{
			InstructionsMaterial.mainTexture = firstInstructionTexture;
		}
}