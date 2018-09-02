using UnityEngine;

//
// Tree.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Knocks over trees when they are hit at a certain speed
//

public class TreeKnocker : MonoBehaviour {

    static float speedTolerance = 50f;
    static float massTolerance = 50f;

    void OnCollisionEnter(Collision collision) {
        if (collision.rigidbody.mass > massTolerance && collision.relativeVelocity.magnitude > speedTolerance && !GetComponent<Rigidbody>()) {
            // Add Rigidbody
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 50f;
            rb.velocity = collision.relativeVelocity;
            SceneManager.instance.AddGravityBody(rb);

            // Add box collider to prevent excessive rolling
            BoxCollider bc = gameObject.AddComponent<BoxCollider>();
            bc.center = new Vector3(0f, 3f, 0f);
            bc.size = new Vector3(0.7f, 0.7f, 0.7f);

            Destroy(this);
        }
    }
}
