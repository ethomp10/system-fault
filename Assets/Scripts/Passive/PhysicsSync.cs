using UnityEngine;

//
// PhysicsSync.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Syncs Unity's fixed update with the refresh rate of the current screen
//

public class PhysicsSync : MonoBehaviour {

	void Start () {
        Time.fixedDeltaTime = 1f / Screen.currentResolution.refreshRate;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        Debug.Log("PhysicsSync: Fixed Update set to 1/" + Screen.currentResolution.refreshRate);
	}
}
