using UnityEngine;

//
// MatterManipulator.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Picks up materializable objects, and connects ship modules
//

public class MatterManipulator : MonoBehaviour {

    [SerializeField] float dematRange = 5f;
    [SerializeField] float snapSpeed = 5f;
    [SerializeField] Material dematMaterial;

    [SerializeField] Transform snapPoint;
    GameObject heldObject;

    ModuleSlot assignedSlot = null;

    public bool equipped = true;

	void Start () {
        if (snapPoint == null) Debug.LogError("MatterManipulator: No snap point set");

		if (dematMaterial == null) Debug.LogError("MatterManipulator: No demat material set");
	}

    public void Fire() {
        if (heldObject == null) {
            RaycastHit? hit = FindObjectOfType<PlayerCamera>().GetMaterializableTarget(dematRange);
            if (hit != null) GrabObject(hit.Value.collider.gameObject);
        } else {
            if (assignedSlot) ConnectModule(assignedSlot);
            else DropObject();
        }
    }

    public void AssignModuleSlot(ModuleSlot slot) {
        assignedSlot = slot;
    }

    public void EquipFuelPack(ModuleSlot playerSlot) {
        if (heldObject != null) {
            if (heldObject.GetComponent<FuelPack>()) {
                if (playerSlot.connectedModule == null) ConnectModule(playerSlot);
                else Debug.LogWarning("MatterManipulator: A different fuel pack is already equipped");
            } else Debug.LogWarning("MatterManipulator: Not holding fuel pack");
        } else {
            if (playerSlot.connectedModule != null) {
                GrabObject(playerSlot.connectedModule.gameObject);
                assignedSlot = null;
            }
        }
    }

    void ConnectModule(ModuleSlot slot) {
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        SceneManager.instance.RemoveGravityBody(rb);
        Destroy(rb);

        heldObject.transform.SetParent(slot.transform);

        ShipModule currentModule = heldObject.GetComponent<ShipModule>();
        slot.connectedModule = currentModule;
        currentModule.connected = true;

        slot.GetComponent<MeshRenderer>().enabled = false;
        assignedSlot = null;

        Ship ship = slot.GetComponentInParent<Ship>();
        if (ship) {
            ship.ToggleModuleSlots(false);
            ship.UpdateModuleStatus(currentModule, currentModule.moduleType, true);
            currentModule.shipRB = ship.GetComponent<Rigidbody>();
        }

        heldObject = null;

        PlayerHUD.instance.ToggleCrosshair(true);
        PlayerCamera.instance.checkForUsable = true;
        PlayerCamera.instance.checkForMaterializable = true;
    }

    void DisconnectModule(ModuleSlot slot) {
        Rigidbody rb = heldObject.AddComponent<Rigidbody>();
        rb.mass = heldObject.GetComponent<ShipModule>().mass;
        SceneManager.instance.AddGravityBody(rb);

        slot.connectedModule = null;
        heldObject.GetComponent<ShipModule>().connected = false;
        assignedSlot = slot;

        slot.GetComponent<MeshRenderer>().enabled = true;
    }

    void DropObject() {
        FindObjectOfType<Ship>().ToggleModuleSlots(false);

        heldObject.transform.parent = null;
        heldObject.GetComponent<IMaterializeable>().Materialize();
        heldObject.GetComponent<Rigidbody>().velocity = GetComponentInParent<Rigidbody>().velocity;
        heldObject = null;

        PlayerHUD.instance.ToggleCrosshair(true);
        PlayerCamera.instance.checkForUsable = true;
        PlayerCamera.instance.checkForMaterializable = true;
    }

    void GrabObject(GameObject gameObject) {
        heldObject = gameObject;

        ShipModule currentModule = gameObject.GetComponent<ShipModule>();
        if (currentModule) {
            Ship ship = FindObjectOfType<Ship>();

            if (currentModule.connected) {
                DisconnectModule(gameObject.GetComponentInParent<ModuleSlot>());
                currentModule.shipRB = null;
                ship.UpdateModuleStatus(currentModule, currentModule.moduleType, false);
            }

            ship.ToggleModuleSlots(true);
        }

        gameObject.transform.parent = transform;

        gameObject.GetComponent<IMaterializeable>().Dematerialize(dematMaterial);

        PlayerHUD.instance.ToggleCrosshair(false);
        PlayerHUD.instance.ToggleUsePrompt(false);
        PlayerHUD.instance.ToggleDematPrompt(false);
        PlayerCamera.instance.checkForUsable = false;
        PlayerCamera.instance.checkForMaterializable = false;
    }

    void FixedUpdate() {
        if (heldObject) SnapObject();
    }

    void SnapObject() {
        if (Vector3.Distance(heldObject.transform.localPosition, snapPoint.localPosition) > 0.02f) {
            heldObject.transform.localPosition = Vector3.Lerp(heldObject.transform.localPosition, snapPoint.localPosition, snapSpeed * Time.fixedDeltaTime);
            heldObject.transform.localRotation = Quaternion.Slerp(heldObject.transform.localRotation, snapPoint.localRotation, snapSpeed * Time.fixedDeltaTime);
        }
    }

    public bool IsHoldingObject() {
        if (heldObject) return true;
        else return false;
    }
}
