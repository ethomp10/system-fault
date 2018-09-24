using UnityEngine;

//
// PlayerData.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Holds information that should persist if the player is destroyed
//

public class PlayerData : MonoBehaviour {

    public static PlayerData instance = null;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
