using UnityEngine;  
  
class GarbageCollectManager : MonoBehaviour {  
    public int frameFreq = 30;  
    void Update()   {  
        if (Time.frameCount % frameFreq == 0)  
            System.GC.Collect();  
    }  
} 