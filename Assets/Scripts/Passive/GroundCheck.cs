using UnityEngine;

//
// GroundCheck.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Checks whether the parent object is grounded
//

public class GroundCheck : MonoBehaviour {

    IGroundable groundable;
    int colliders;

	void Start () {
        groundable = GetComponentInParent<IGroundable>();
        if (groundable == null) {
            Debug.LogError("Ground Check: No IGroundable set as parent");
        }
	}
	
    void OnTriggerEnter(Collider other) {
        if (!other.isTrigger || other.GetComponent<IMaterializeable>() != null) {
            colliders++;
            if (colliders == 1) {
                groundable.SetGrounded(true);
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (!other.isTrigger || other.GetComponent<IMaterializeable>() != null) {
            colliders--;
            if (colliders == 0) {
                groundable.SetGrounded(false);
            }
        }
    }
}
