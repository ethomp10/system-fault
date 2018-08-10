using UnityEngine;

//
// Canopy.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Opens and closes the ship canopy
//

public class Canopy : MonoBehaviour, IUsable {

    [SerializeField] float speed = 3f;

    bool open = false;

    Vector3 currentPosition;
    Vector3 closedPosition;
    Vector3 openPosition;

    void Awake() {
        closedPosition = transform.localPosition;
        openPosition = transform.localPosition - Vector3.forward * 3f;
        currentPosition = closedPosition;
    }

    public void Use() {
        if (open) {
            currentPosition = closedPosition;
            open = false;
        } else {
            currentPosition = openPosition;
            open = true;
        }
    }

    void FixedUpdate() {
        if (transform.localPosition != currentPosition) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentPosition, speed * Time.fixedDeltaTime);
        }
    }

    public bool IsOpen() {
        return open;
    }
}
