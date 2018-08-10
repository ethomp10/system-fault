using UnityEngine;

//
// PlayerControl.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Handles input for all types of controls
//

public struct ControlObject {
    // Movement
    public float forwardBack;
    public float rightLeft;
    public float upDown;
    public float roll;
    public bool jump;
    public bool run;
    public bool changeAssist;
    public bool toggleAssist;

    // Aiming
    public float horizontalLook;
    public float verticalLook;
    public bool changeCamera;

    // Player action
    public bool fire;
    public bool aim;
    public bool interact;
    public bool attachShieldCell;
    public bool chargeShieldCell;

    // Weapon types
    public bool matterManipilator;
    public bool weapon0;
    public bool weapon1;
    public bool weapon2;
    public bool weapon3;
}

public class PlayerControl : MonoBehaviour {

    public static PlayerControl instance = null;

    IControllable controlActor = null;

    void Awake() {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    ControlObject currentInput;

    void Start() {
        currentInput = new ControlObject();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        if (controlActor != null) {
            currentInput.forwardBack = Input.GetAxis("Move Forward/Back");
            currentInput.rightLeft = Input.GetAxis("Move Right/Left");
            currentInput.upDown = Input.GetAxis("Move Up/Down");
            currentInput.roll = Input.GetAxis("Roll");
            currentInput.jump = Input.GetButtonDown("Jump");
            currentInput.run = Input.GetButton("Run/Change Assist Mode");
            currentInput.changeAssist = Input.GetButtonDown("Run/Change Assist Mode");
            currentInput.toggleAssist = Input.GetButtonDown("Toggle Flight Assist");

            currentInput.horizontalLook = Input.GetAxis("Horizontal Look");
            currentInput.verticalLook = Input.GetAxis("Vertical Look");
            currentInput.changeCamera = Input.GetButtonDown("Change Camera");

            currentInput.fire = Input.GetButtonDown("Fire");
            currentInput.aim = Input.GetButtonDown("Aim/Equip Fuel Pack");
            currentInput.interact = Input.GetButtonDown("Interact");
            currentInput.attachShieldCell = Input.GetButtonDown("Attach Shield Cell");
            currentInput.chargeShieldCell = Input.GetButtonDown("Charge Shield Cell");

            currentInput.matterManipilator = Input.GetButtonDown("Matter Manipulator");
            currentInput.weapon0 = Input.GetButtonDown("Pulse Cannon/Energy Weapon");
            currentInput.weapon1 = Input.GetButtonDown("Foam Cannon/Kinetic Weapon");
            currentInput.weapon2 = Input.GetButtonDown("Flame Cannon");
            currentInput.weapon3 = Input.GetButtonDown("Ice Cannon");

            controlActor.CheckInput(currentInput);
        }
    }

    public void TakeControl(IControllable actor) {
        controlActor = actor;
        actor.SetCam(PlayerCamera.instance);
    }

    public void RemoveControl() {
        controlActor = null;
    }
}
