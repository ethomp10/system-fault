﻿using UnityEngine;
using System.Collections;

//
// PrintDrivePort.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Allows player to unlock blueprints & control part printers
//

public class PrintDrivePort : MonoBehaviour, IUsable {

    [SerializeField] PartPrinter printer;
    [SerializeField] Transform playerReset;
    [SerializeField] Material disabledMaterial;

    PrintDrive printDrive = null;
    static float insertionSpeed = 1.5f;

    Blueprint blueprint = null;
    static float blueprintDisplayTime = 3f;

    Vector3 desiredLocation;
    bool driveConnected = false;

    void Awake() {
        blueprint = GetComponent<Blueprint>();
    }

    public void Use() {
        if (!printDrive) {
            printDrive = Instantiate(PartPrinterData.instance.printDrivePrefab, transform.position + transform.up / 2f, transform.rotation, transform).GetComponent<PrintDrive>();
            printDrive.AssignPort(this);

            if (!blueprint) FindObjectOfType<WeaponSlot>().UnequipAll();
            desiredLocation = Vector3.zero;
        }
    }

    void Update() {
        if (printDrive) {
            if (printDrive.transform.localPosition != desiredLocation) {
                printDrive.transform.localPosition = Vector3.MoveTowards(printDrive.transform.localPosition, desiredLocation, insertionSpeed * Time.deltaTime);
            } else if (!driveConnected) ConnectDrive();

            if (desiredLocation != Vector3.zero && printDrive.transform.localPosition == desiredLocation) {
                Destroy(printDrive.gameObject);
                printDrive = null;
                if (blueprint) {
                    gameObject.layer = LayerMask.GetMask("Default");
                    Destroy(blueprint);
                    Destroy(this);
                } else FindObjectOfType<WeaponSlot>().SwitchWeapons();
            }
        }
    }

    void ConnectDrive() {
        printDrive.PowerScreen(true);

        if (blueprint) {
            blueprint.Unlock(printDrive);
            StartCoroutine("BlueprintTimer");
        } else {
            PlayerHUD.instance.ToggleShipRadar(false);
            PlayerHUD.instance.ToggleUsePrompt(false);

            PlayerControl.instance.TakeControl(printDrive.GetComponent<IControllable>());

            Player currentPlayer = FindObjectOfType<Player>();
            if (currentPlayer.GetComponentInChildren<ModuleSlot>().connectedModule) {
                currentPlayer.GetComponentInChildren<FuelPack>().ShowPack(false);
            }
            currentPlayer.ResetPlayer(playerReset);

        }

        driveConnected = true;
    }

    public void RemovePrintDrive() {
        driveConnected = false;
        desiredLocation = Vector3.up / 2f;

        Player currentPlayer = FindObjectOfType<Player>();
        if (currentPlayer.GetComponentInChildren<ModuleSlot>().connectedModule) currentPlayer.GetComponentInChildren<FuelPack>().ShowPack(true);

        printDrive.PowerScreen(false);

        PlayerControl.instance.TakeControl(currentPlayer);
        PlayerCamera.instance.checkForUsable = true;

        PlayerHUD.instance.ToggleShipRadar(true);
    }

    public void SendPrintSignal(Vector2Int index) {
        printer.ProcessPrintSignal(index);
    }

    IEnumerator BlueprintTimer() {
        yield return new WaitForSeconds(blueprintDisplayTime);

        // Remove drive
        RemovePrintDrive();

        // Disable print drive port
        MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
        Material[] mats = meshRenderer.materials;
        mats[0] = disabledMaterial;
        mats[1] = disabledMaterial;
        meshRenderer.materials = mats;
    }
}
