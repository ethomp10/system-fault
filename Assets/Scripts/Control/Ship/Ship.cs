using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//
// Ship.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Controls the player's ship
//

[RequireComponent(typeof(Rigidbody))]

public class Ship : MonoBehaviour, IControllable, IUsable {

    ModuleSlot[] moduleSlots;
    FuelPack fuelPack = null;
    Thrusters thrusters = null;
    Boosters boosters = null;

    LandingGear landingGear;

    [SerializeField] float yawMultiplier = 0.5f;
    [SerializeField] float astroThrottleSensitivity = 0.5f;
    [SerializeField] float cameraSensitivity = 1f;

    [SerializeField] Transform cameraRig;
    [SerializeField] float cameraResetSpeed = 1f;
    [SerializeField] Transform playerExit;

    [SerializeField] Image throttleGauge;
    [SerializeField] Image fuelGauge;
    Color throttleColour = new Color(0f, 1f, 0f, 130f/255f);
    Color fuelColour = new Color(0f, 180f/255f, 1f, 130f/255f);
    Color warningColour = new Color(1f, 0f, 0f, 130f / 255f);


    bool powered = false;
    bool busy = false;

    bool freeLook = false;

    bool limitHoverSpeed = false;

    GameTypes.AssistMode assistMode = GameTypes.AssistMode.NoAssist;
    GameTypes.AssistMode previousAssistMode = GameTypes.AssistMode.NoAssist;

    Rigidbody rb;

    void Start() {
        moduleSlots = GetComponentsInChildren<ModuleSlot>();
        ToggleModuleSlots(false);

        landingGear = GetComponentInChildren<LandingGear>();
        if (!landingGear) Debug.LogError("Ship: No landing gear set as child");

        rb = GetComponent<Rigidbody>();

        if (cameraRig == null) Debug.LogError("Ship: No camera rig set in inspector");
        if (playerExit == null) Debug.LogError("Ship: No player exit set in inspector");
    }

    public void CheckInput(ControlObject controlObject) {
        // Toggle Power
        if (controlObject.jump) {
            if (powered) TogglePower(false);
            else if (!powered && FuelAvailable() && !busy) TogglePower(true);
        }

        if (powered) {
            if (FuelAvailable()) {
                // Module Control
                foreach (ModuleSlot slot in moduleSlots) {
                    if (thrusters) {
                        if (assistMode == GameTypes.AssistMode.Astro) {
                            UpdateThrottleGauge(thrusters.AdjustAstroThrottle(controlObject.forwardBack * astroThrottleSensitivity * Time.deltaTime));
                            fuelPack.DrainFuel(thrusters.GetAstroFuel() * Time.deltaTime);
                        } else {
                            thrusters.SetThrottle(controlObject.forwardBack);
                            UpdateThrottleGauge(controlObject.forwardBack);
                            fuelPack.DrainFuel(Mathf.Abs(controlObject.forwardBack) / thrusters.efficiency * Time.deltaTime);
                        }
                    }

                    if (boosters) {
                        Vector3 torque;

                        if (!freeLook) torque = new Vector3(-controlObject.verticalLook, controlObject.horizontalLook * yawMultiplier, controlObject.roll);
                        else torque = new Vector3(0f, 0f, controlObject.roll);
                        boosters.PassInput(controlObject.rightLeft, controlObject.upDown, torque);

                        float totalBoost = Mathf.Abs(controlObject.rightLeft) + Mathf.Abs(controlObject.upDown);
                        fuelPack.DrainFuel(totalBoost / boosters.efficiency * Time.deltaTime);
                    }
                }

                UpdateFuelGauge(fuelPack.GetFuelPercentage());

                // Assist Modes
                if (controlObject.changeAssist) {
                    if (assistMode == GameTypes.AssistMode.NoAssist) ChangeAssistMode(previousAssistMode);
                    else if (assistMode == GameTypes.AssistMode.Hover && boosters && thrusters) ChangeAssistMode(GameTypes.AssistMode.Astro);
                    else if ((assistMode == GameTypes.AssistMode.Astro)) ChangeAssistMode(GameTypes.AssistMode.Hover);
                    
                }
                if (controlObject.toggleAssist) {
                    if (assistMode != GameTypes.AssistMode.NoAssist) {
                        previousAssistMode = assistMode;
                        ChangeAssistMode(GameTypes.AssistMode.NoAssist);
                    } else ChangeAssistMode(previousAssistMode);
                }
            } else TogglePower(false);
        } else if (controlObject.interact && !busy) StartCoroutine("ExitShip");

        // Camera
        if (controlObject.aim) {
            if (freeLook) freeLook = false;
            else freeLook = true;
        }
        if (controlObject.changeCamera && !busy) PlayerCamera.instance.ToggleThirdPerson();

        if (freeLook) FreeLook(controlObject.horizontalLook, controlObject.verticalLook);
        else ResetCameraRig();
    }

    void Hover() {
        rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, boosters.hoverDampaning * Time.fixedDeltaTime);
        if (limitHoverSpeed) rb.velocity = Vector3.ClampMagnitude(rb.velocity, boosters.maxHoverSpeed);

        // Start to limit hover speed once ship is slowed down enough (prevents instant slowdowns from other flight modes)
        if (rb.velocity.magnitude < boosters.maxHoverSpeed) limitHoverSpeed = true;
    }

    void Astro() {
        rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * thrusters.GetAstroSpeed(), thrusters.astroAcceleration * Time.fixedDeltaTime);
    }

    void FreeLook(float horizontal, float vertical) {
        cameraRig.Rotate(cameraRig.parent.up, horizontal * cameraSensitivity, Space.World);
        cameraRig.Rotate(cameraRig.right, -vertical * cameraSensitivity, Space.World);
    }

    void ResetCameraRig() {
        cameraRig.localRotation = Quaternion.Slerp(cameraRig.localRotation, Quaternion.identity, cameraResetSpeed * Time.deltaTime);
    }

    void FixedUpdate() {
        switch (assistMode) {
            case GameTypes.AssistMode.Hover:
                Hover();
                break;
            case GameTypes.AssistMode.Astro:
                Astro();
                break;
            case GameTypes.AssistMode.Quantum:
                break;
            default:
                break;
        }
    }

    void ChangeAssistMode(GameTypes.AssistMode mode) {
        switch (mode) {
            case GameTypes.AssistMode.NoAssist:
                rb.useGravity = true;
                Debug.Log("Ship: Assist off");
                break;
            case GameTypes.AssistMode.Hover:
                rb.useGravity = false;
                limitHoverSpeed = false;
                landingGear.Extend();
                Debug.Log("Ship: Hover Mode on");
                break;
            case GameTypes.AssistMode.Astro:
                rb.useGravity = false;
                landingGear.Retract();
                Debug.Log("Ship: Astro Flight on");
                break;
            case GameTypes.AssistMode.Quantum:
                break;
        }

        if (thrusters) thrusters.ResetThrottles();
        if (boosters) boosters.ResetThrottles();

        assistMode = mode;
    }

    public void UpdateModuleStatus(ShipModule module, GameTypes.ModuleType type, bool connected) {
        switch (type) {
            case GameTypes.ModuleType.FuelPack:
                if (connected) {
                    fuelPack = module.GetComponent<FuelPack>();
                    Debug.Log("Ship: Fuel pack connected");
                } else {
                    fuelPack = null;
                    Debug.Log("Ship: Fuel pack disconnected");
                }
                break;
            case GameTypes.ModuleType.Thrusters:
                if (connected) {
                    thrusters = module.GetComponent<Thrusters>();
                    Debug.Log("Ship: Thrusters connected");

                } else {
                    thrusters = null;
                    Debug.Log("Ship: Thrusters disconnected");

                }
                break;
            case GameTypes.ModuleType.Boosters:
                if (connected) {
                    boosters = module.GetComponent<Boosters>();
                    Debug.Log("Ship: Boosters connected");
                } else {
                    boosters = null;
                    Debug.Log("Ship: Boosters disconnected");
                }
                break;
        }
    }

    void UpdateThrottleGauge(float throttle) {
        throttleGauge.fillAmount = Mathf.Abs(throttle);

        if (throttle >= 0f) throttleGauge.color = throttleColour;
        else throttleGauge.color = warningColour;
    }

    void UpdateFuelGauge(float fuel) {
        fuelGauge.fillAmount = fuel;

        if (fuel > 0.25f) fuelGauge.color = fuelColour;
        else fuelGauge.color = warningColour;
    }

    public void Use() {
        PlayerControl.instance.TakeControl(this);

        if (!busy) StartCoroutine("EnterShip");

        SceneManager.instance.DespawnPlayer();
    }

    public void TogglePower(bool toggle) {
        if (toggle) {
            powered = true;
            Debug.Log("Ship: Powered on");
            if (boosters) ChangeAssistMode(GameTypes.AssistMode.Hover);
        } else {
            ChangeAssistMode(GameTypes.AssistMode.NoAssist);
            powered = false;
            UpdateThrottleGauge(0f);
            UpdateFuelGauge(0f);
            Debug.Log("Ship: Powered off");
        }
    }

    bool FuelAvailable() {
        if (fuelPack == null) return false;

        return fuelPack.IsEmpty();
    }

    public void SetCam(PlayerCamera controlCam) {
        controlCam.transform.SetParent(cameraRig);

        PlayerHUD.instance.ToggleUsePrompt(false);
        PlayerHUD.instance.ToggleDematPrompt(false);
        controlCam.checkForUsable = false;
        controlCam.checkForMaterializable = false;
    }

    public void ToggleModuleSlots(bool toggle) {
        foreach (ModuleSlot slot in moduleSlots) {
            if (!slot.connectedModule) slot.GetComponent<MeshRenderer>().enabled = toggle;
        }
    }

    IEnumerator EnterShip() {
        busy = true;

        yield return new WaitForSeconds(1f);

        Canopy canopy = GetComponentInChildren<Canopy>();
        if (canopy.IsOpen()) canopy.Use();

        busy = false;
    }

    IEnumerator ExitShip() {
        busy = true;

        Canopy canopy = GetComponentInChildren<Canopy>();
        if (!canopy.IsOpen()) canopy.Use();

        PlayerCamera.instance.FirstPerson();

        yield return new WaitForSeconds(1f);

        SceneManager.instance.SpawnPlayer(playerExit, rb.velocity);
        busy = false;
    }
}
