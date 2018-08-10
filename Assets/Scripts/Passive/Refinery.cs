using UnityEngine;

//
// Refinery.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Controls functionality of the refineries
//

public class Refinery : MonoBehaviour {

    void OnTriggerEnter(Collider other) {
        // Character Snap
        if (other.GetComponent<CharacterSnap>() != null) {
            other.GetComponent<CharacterSnap>().SetSnapUp(transform.up);
        }
    }

    void OnTriggerExit(Collider other) {
        // Character Snap
        if (other.GetComponent<CharacterSnap>() != null) {
            other.GetComponent<CharacterSnap>().RemoveSnapUp();
        }
    }
}