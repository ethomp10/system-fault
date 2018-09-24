using UnityEngine;
using System.Collections;

//
// PartPrinter.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Prints ship modules for the player to use
//

public class PartPrinter : MonoBehaviour {

    [SerializeField] float printTime = 2f;
    [SerializeField] Transform printPoint;
    [SerializeField] Material printMaterial;

    MeshRenderer meshRenderer;

    ShipModule newPart = null;
    bool printSurfaceClear = true;
    float opacity = 0f;

    private void Start() {
        if (!printPoint) Debug.LogError("PartPrinter: No print point set");
        if (!printMaterial) Debug.LogError("PartPrinter: No print material set");
    }

    void Update () {
        // TODO: This should probably be in a shader
        if (newPart) {
            opacity += Time.deltaTime / printTime;
            foreach (Material mat in meshRenderer.materials) {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, opacity);
            }
        }
    }

    public void ProcessPrintSignal(Vector2Int index) {
        int prefabIndex;

        if (index.x < PartPrinterData.MODULE_TYPES) prefabIndex = index.x * 3 + index.y;
        else prefabIndex = 9;

        Debug.Log("PartPrinter: Print request for index " + index + " / " + prefabIndex);
        StartCoroutine(PrintPart(PartPrinterData.instance.modulePrefabs[prefabIndex]));
    }

    void OnTriggerStay(Collider other) {
        printSurfaceClear = false;
    }

    void OnTriggerExit(Collider other) {
        printSurfaceClear = true;
    }

    IEnumerator PrintPart(GameObject shipModule) {
        if (printSurfaceClear) {
            newPart = Instantiate(shipModule, printPoint.position, printPoint.rotation).GetComponent<ShipModule>();
            SceneManager.instance.AddGravityBody(newPart.GetComponent<Rigidbody>());
            newPart.Dematerialize(printMaterial);
            meshRenderer = newPart.GetComponent<MeshRenderer>();

            yield return new WaitForSeconds(printTime);

            newPart.Materialize();
            newPart = null;
            opacity = 0f;
        } else {
            Debug.LogWarning("PartPrinter: Please clear print surface and try again");
        }
    }
}
