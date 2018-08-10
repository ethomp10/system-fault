using UnityEngine;

//
// Thrusters.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Powers forward/back movement of the ship
//

public class Thrusters : ShipModule {

    [SerializeField] float acceleration = 50f;
    public float maxAstroSpeed = 50f;
    public float astroAcceleration = 1f;
    public float efficiency = 1f;

    float throttle = 0f;
    float astroThrottle = 0f;

    TrailRenderer[] trails;
    bool trailsEmitting = false;

    protected override void Awake() {
        base.Awake();
        moduleType = GameTypes.ModuleType.Thrusters;
        trails = GetComponentsInChildren<TrailRenderer>();
    }

    void FixedUpdate() {
        if (connected) {
            Thrust();

            if (throttle > 0f || astroThrottle > 0f && !trailsEmitting) ToggleTrails(true);
            else if (throttle <= 0f && astroThrottle <= 0f && trailsEmitting) ToggleTrails(false);
        }
    }

    public void SetThrottle(float amount) {
        throttle = amount;
    }

    public float AdjustAstroThrottle(float amount) {
        astroThrottle += amount;
        astroThrottle = Mathf.Clamp(astroThrottle, -0.5f, 1f);
        return astroThrottle;
    }

    public void ResetThrottles() {
        throttle = 0f;
        astroThrottle = 0f;
        ToggleTrails(false);
    }

    public float GetAstroSpeed() {
        return astroThrottle * maxAstroSpeed;
    }

    public float GetAstroFuel() {
        return Mathf.Abs(astroThrottle) / efficiency;
    }

    void Thrust() {
        shipRB.AddForce(shipRB.transform.forward * throttle * acceleration, ForceMode.Acceleration);
    }

    public void ToggleTrails(bool toggle) {
        if (toggle) {
            foreach (TrailRenderer trail in trails) trail.emitting = true;
            trailsEmitting = true;
        } else {
            foreach (TrailRenderer trail in trails) trail.emitting = false;
            trailsEmitting = false;
        }
    }
}
