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
        Debug.LogWarning("PlayerData: Current cells: " + shieldCells);
        FindObjectOfType<Player>().AttemptCellTransfer();
    }

    public int DropShieldCells(int cellsWanted) {
        int cellsDropped = Mathf.Min(cellsWanted, shieldCells);
        shieldCells -= cellsDropped;
        Debug.LogWarning("PlayerData: Current cells: " + shieldCells);
        return cellsDropped;
    }
}
