using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// XromWalker.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Moves and destroys Xroms
//

public class XromWalker : MonoBehaviour {

    [SerializeField] XromPart torso;
    [SerializeField] XromPart legs;

    public void SeparateXrom(Vector3 separationForce) {
        KillXrom();

        Rigidbody rb = GetComponent<Rigidbody>();
        SceneManager.instance.RemoveGravityBody(rb);
        Destroy(rb);
        Destroy(GetComponent<Collider>());

        if (torso) torso.DetachPart(separationForce);
        if (legs) legs.DetachPart(separationForce);

        Destroy(this);
    }

    public void KillXrom() {
        GetComponent<Rigidbody>().freezeRotation = false;
        Destroy(GetComponent<CharacterSnap>());
    }
}
