function Start()
{
		var clones = GameObject.FindGameObjectsWithTag("topColliderClones");
		for(var clone in clones)
		{
			Destroy(clone);
		}
}