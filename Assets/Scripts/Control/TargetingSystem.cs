using UnityEngine;

//
// TargetingSystem.cs
//
// Author: Eric Thompson & Gabriel Cimolino (Dead Battery Games)
// Purpose: Casts rays from the attached camera in order for the player to target various objects
//

[RequireComponent(typeof(Camera))]

public class TargetingSystem : MonoBehaviour {

    public IUsable usableTarget = null;
    private Camera cam;
    
    public void Start() {
        cam = GetComponent<Camera>();
    }

    public RaycastHit? Target(float range, LayerMask mask) {
        Vector3 aimPoint = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

        Debug.DrawLine(aimPoint, transform.position + cam.transform.forward * range);

        RaycastHit hit;
        // Cast a ray from the crosshair and set gun's fire direction based on ray
        if (Physics.Raycast(aimPoint, cam.transform.forward, out hit, range, mask)) {
            return hit;
        } else return null;
    }
}
