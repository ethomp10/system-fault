using UnityEngine;

//
// Boosters.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Powers ship rotation, and vertical/horizontal movement
//

public class Boosters : ShipModule {

    [SerializeField] float acceleration = 30f;
    [SerializeField] float torqueAcceleration = 3f;
    public float maxHoverSpeed = 10f;
    public float hoverDampaning = 1f;
    public float efficiency = 1f;

    float throttleHorizontal, throttleVertical;
    Vector3 throttleTorque;

    TrailRenderer[] trails;
    bool trailsEmitting = false;

    protected override void Awake() {
        base.Awake();
        moduleType = GameTypes.ModuleType.Boosters;
        trails = GetComponentsInChildren<TrailRenderer>();
    }

    void FixedUpdate() {
        if (connected) {
            Boost();

            if ((throttleVertical != 0f || throttleTorque.z != 0f) && !trailsEmitting) ToggleTrails(true);
            else if (throttleVertical == 0f && throttleTorque.z == 0f && trailsEmitting) ToggleTrails(false);
        }
    }

    public void PassInput(float horizontal, float vertical, Vector3 torque) {
        throttleHorizontal = horizontal;
        throttleVertical = vertical;
        throttleTorque = Vector3.ClampMagnitude(torque, 1f);
    }

    public void ResetThrottles() {
        throttleHorizontal = 0f;
        throttleVertical = 0f;
        throttleTorque = Vector3.zero;
        ToggleTrails(false);
    }

    void Boost() {
        shipRB.AddForce(shipRB.transform.right * throttleHorizontal * acceleration + shipRB.transform.up * throttleVertical * acceleration, ForceMode.Acceleration);
        shipRB.AddRelativeTorque(throttleTorque * torqueAcceleration, ForceMode.Acceleration);
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
