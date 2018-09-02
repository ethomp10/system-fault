using UnityEngine;

//
// CharacterSnap.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Orients objects to a specified point in game time; useful the player and AI
//

[RequireComponent(typeof(Rigidbody))]

public class CharacterSnap : MonoBehaviour, ISnappable {

    [SerializeField] float snapSpeed = 3f;

    Vector3? snapPoint = null;  // For planet snapping
    Vector3? snapUp = null;     // For room snapping

    Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void SnapToPoint(Vector3 point) {
        Vector3 up = (transform.position - point).normalized;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, up) * transform.rotation;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, snapSpeed * Time.fixedDeltaTime));
    }

    public void SnapToVector(Vector3 up) {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, up) * transform.rotation;
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, snapSpeed * Time.fixedDeltaTime));
    }

    void FixedUpdate() {
        if ((snapPoint != null && snapUp != null) || (snapPoint == null && snapUp != null)) {
            SnapToVector(snapUp.Value);
        } else if (snapPoint != null && snapUp == null) {
            SnapToPoint(snapPoint.Value);
        }
    }

    public void SetSnapPoint(Vector3 point) {
        snapPoint = point;
        CheckPlayerSnapStatus();
    }

    public void RemoveSnapPoint() {
        snapPoint = null;
        CheckPlayerSnapStatus();
    }

    public void SetSnapUp(Vector3 up) {
        snapUp = up;
        CheckPlayerSnapStatus();
    }

    public void RemoveSnapUp() {
        snapUp = null;
    }

    void CheckPlayerSnapStatus() {
        Player player = GetComponent<Player>();
        if (player) {
            if (snapPoint == null && snapUp == null) {
                player.SetSnapping(false);
            } else {
                player.SetSnapping(true);
            }
        }
    }
}
