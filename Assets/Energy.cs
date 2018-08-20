using UnityEngine;
using System.Collections;

//
// Energy.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Pickup script for energy
//

public class Energy : MonoBehaviour, ICollectable {

    Rigidbody rb;

    void Awake() {
        // Add rigidbody to scene
        rb = GetComponent<Rigidbody>();
        rb.velocity = -transform.localPosition * 10f;
        SceneManager.instance.AddGravityBody(rb);
    }

    public void Pickup() {
        // TODO: Add energy to player's fuel pack

        StartCoroutine("Vapourize");
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Pickup();
        }
    }

    IEnumerator Vapourize() {
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<MeshCollider>());
        SceneManager.instance.RemoveGravityBody(rb);
        Destroy(rb);

        GetComponent<ParticleSystem>().Emit(50);

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
