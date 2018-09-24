using UnityEngine;

//
// Mainframe.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Holds a blueprint index to unlock in the Print Drive
//

public class Blueprint : MonoBehaviour, IUsable {

    [SerializeField] GameTypes.ModuleType moduleType;
    [SerializeField] [Range(1, 3)] int moduleTier;
    [SerializeField] GameObject printDrivePrefab;
    [SerializeField] Transform printDrivePort;

    bool used = false;

    public void Use() {
        if (!used) {
            PartPrinterData.instance.UnlockModule(moduleType, moduleTier);

            //Instantiate(printDrivePrefab, printDrivePort.position, printDrivePort.rotation).GetComponent<PrintDrive>().ShowBlueprint(moduleType, moduleTier);

            used = true;
        }
    }
}
