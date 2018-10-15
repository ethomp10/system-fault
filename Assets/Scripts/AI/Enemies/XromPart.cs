using UnityEngine;

//
// XromPart.cs
//
// Author: Eric Thompson & Gabriel Cimolino (Dead Battery Games)
// Purpose: Detaches parts of the Xrom model
//

public class XromPart : MonoBehaviour, IDamageable {

    [SerializeField] bool vital;
    [SerializeField] float partHealth = 20f;
    [SerializeField] float partMass = 1f;

    float currentHealth;

    void Start() {
        currentHealth = partHealth;
    }

    public void Damage(float amount, Vector3 damageForce) {
        currentHealth -= amount;
        if (currentHealth <= 0f) BreakXrom(damageForce);
    }

    void BreakXrom(Vector3 detachForce) {
        XromWalker xromWalker = GetComponentInParent<XromWalker>();
        if (vital && xromWalker) {
            if (transform.parent.parent.GetComponent<XromWalker>()) xromWalker.SeparateXrom(detachForce);
            else {
                DetachPart(detachForce);
                xromWalker.KillXrom();
            }
        } else DetachPart(detachForce);
    }

    public void DetachPart(Vector3 detachForce) {
        transform.parent = null;

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = partMass;
        rb.velocity = detachForce;
        SceneManager.instance.AddGravityBody(rb);

        Destroy(this);
    }
}
