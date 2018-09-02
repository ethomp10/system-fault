using UnityEngine;

//
// PlayerData.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Holds information that should persist if the player is destroyed
//

public class PlayerData : MonoBehaviour {

    public static PlayerData instance = null;
    private int shieldCells = 0;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PickupShieldCell() {
        shieldCells++;
        Debug.Log("PlayerData: Shield cells: " + shieldCells);
    }

    public bool DropShieldCell() {
        if (shieldCells > 0) {
            shieldCells--;
            return true;
        } else return false;
    }
}
