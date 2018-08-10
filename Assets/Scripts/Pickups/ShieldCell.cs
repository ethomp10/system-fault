using UnityEngine;

//
// ShieldCell.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Pickup which allows the player to have additional shields
//

public class ShieldCell : MonoBehaviour, ICollectable {
    public void Pickup() {
        PlayerData.instance.PickupShieldCell();
        SceneManager.instance.RemoveGravityBody(GetComponent<Rigidbody>());
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.GetComponent<Player>()) {
            Pickup();
        }
    }
}
