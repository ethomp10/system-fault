using UnityEngine;

//
// EnergyCrystal.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Crystals which shatter into collectable energy
//

public class EnergyCrystal : MonoBehaviour, IDamageable {

    [SerializeField] GameObject shatteredCrystal;
    float health = 10f;

    public void Damage(float amount) {
        health -= amount;
        if (amount >= 0) Shatter();
    }

    void Shatter() {
        GameObject pieces = Instantiate(shatteredCrystal, transform.position, transform.rotation);
        pieces.transform.localScale = transform.localScale;
        Destroy(gameObject);
    }
}
