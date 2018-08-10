using UnityEngine;

//
// Player.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Controls the player
//

[RequireComponent(typeof(CharacterSnap))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]

public class Player : MonoBehaviour, IControllable, IGroundable, IDamageable {

    [SerializeField] float maxHealth = 10f;
    float health;

    [Header("Movement")]
    [SerializeField] float jumpForce = 20f;
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float walkForce = 1f;
    [SerializeField] float airForce = 0.2f;
    [SerializeField] float airTorque = 0.2f;

    [Header("Mouse")]
    [SerializeField] float lookSensitivity = 1f;
    [SerializeField] float minYAngle = -60f;
    [SerializeField] float maxYAngle = 60f;

    [Space]
    [SerializeField] float useRange = 5f;

    ModuleSlot fuelSlot;

    bool grounded = false;
    bool snapping = false;
    float yAngle;

    Rigidbody rb;
    Vector3 moveDirection;
    Vector3 lookRotation;

    [SerializeField] Transform cameraRig;

    void Start() {
        health = maxHealth; // TODO: Remove!

        rb = GetComponent<Rigidbody>();
        fuelSlot = GetComponentInChildren<ModuleSlot>();
        if (fuelSlot == null || fuelSlot.slotType != GameTypes.ModuleType.FuelPack) {
            Debug.LogError("Player: No fuel slot set as child");
        }

        if (cameraRig == null) {
            Debug.LogError("Player: No camera rig set in inspector");
        }
    }

    void FixedUpdate() {
        Move();
        if (!snapping) Rotate();
        if (snapping) Look();
    }

    public void CheckInput(ControlObject controlObject) {
        if (snapping) moveDirection = (controlObject.rightLeft * transform.right + controlObject.forwardBack * transform.forward).normalized;
        else moveDirection = (controlObject.rightLeft * transform.right + controlObject.forwardBack * transform.forward + controlObject.upDown * transform.up).normalized;

        lookRotation += new Vector3(-controlObject.verticalLook, controlObject.horizontalLook, controlObject.roll);

        if (controlObject.jump && grounded) Jump();

        if (controlObject.interact) {
            RaycastHit? hit = GetComponentInChildren<PlayerCamera>().GetUsableTarget(useRange);
            if (hit != null && PlayerCamera.instance.checkForUsable) hit.Value.collider.GetComponent<IUsable>().Use();
        }

        if (controlObject.chargeShieldCell) {
            if (fuelSlot.connectedModule) {
                fuelSlot.connectedModule.GetComponent<FuelPack>().ChargeFuelCell();
            }
        }

        if (controlObject.fire) {
            MatterManipulator matterManipulator = GetComponentInChildren<MatterManipulator>();
            if (matterManipulator.equipped) {
                matterManipulator.Fire();
            }
        }

        if (controlObject.aim) { 
            GetComponentInChildren<MatterManipulator>().EquipFuelPack(fuelSlot);
        }
    }

    void Move() {
        if (grounded) rb.AddForce(Vector3.ClampMagnitude(moveDirection * walkSpeed - rb.velocity, walkForce), ForceMode.VelocityChange);
        else rb.AddForce(moveDirection * airForce, ForceMode.Acceleration);
    }

    void Look() {
        // Horizontal
        transform.Rotate(0f, lookRotation.y * lookSensitivity, 0f, Space.Self);

        // Vertical
        yAngle = Mathf.Clamp(yAngle + -lookRotation.x * lookSensitivity, minYAngle, maxYAngle);
        cameraRig.transform.localEulerAngles = new Vector3(-yAngle, 0f, 0f);

        lookRotation = Vector3.zero;
    }

    // TODO: This should probably call a function in fuel pack
    void Rotate() {
        rb.AddRelativeTorque(lookRotation * airTorque, ForceMode.Acceleration);

        lookRotation = Vector3.zero;
    }

    void Jump() {
        rb.AddRelativeForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
    }

    public void SetGrounded(bool grounded) {
        this.grounded = grounded;
    }

    public void SetSnapping(bool snapping) {
        if (snapping) {
            rb.drag = 0f;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        } else {
            rb.drag = 1f;
            rb.constraints = RigidbodyConstraints.None;
        }
        this.snapping = snapping;
    }

    public void AttemptCellTransfer() {
        if (fuelSlot.connectedModule != null) {
            fuelSlot.connectedModule.GetComponent<FuelPack>().AttachAllPossibleCells();
        }
    }

    public void SetCam(PlayerCamera controlCam) {
        controlCam.transform.SetParent(cameraRig);

        controlCam.checkForMaterializable = true;
        controlCam.checkForUsable = true;
    }

    public void Damage(float amount) {
        // Check if player has shields
        if (fuelSlot.connectedModule != null) {
            amount = fuelSlot.connectedModule.GetComponent<FuelPack>().AbsorbDamage(amount);
        }

        health -= amount;
        Debug.LogWarning("Player: Health: " + health);
        if (health <= 0) {
            PlayerControl.instance.RemoveControl();
            SceneManager.instance.DespawnPlayer();
        }
    }
}
