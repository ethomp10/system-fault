using UnityEngine;
using System.Collections;

//
// PartPrinter.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Prints ship modules for the player to use
//

public class PartPrinter : MonoBehaviour {

    [SerializeField] Transform printPoint;

    MeshRenderer meshRenderer;
    ParticleSystem[] printParticles;

    ShipModule newPart = null;
    bool printSurfaceClear = true;
    float opacity = 0f;

    private void Start() {
        if (!printPoint) Debug.LogError("PartPrinter: No print point set");
        printParticles = GetComponentsInChildren<ParticleSystem>();
    }

    void Update () {
        // TODO: This should probably be in a shader
        if (newPart) {
            opacity += Time.deltaTime / PartPrinterData.instance.printTime;
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
            foreach (ParticleSystem particleSystem in printParticles) particleSystem.Play();

            yield return new WaitForSeconds(0.75f);
            newPart = Instantiate(shipModule, printPoint.position, printPoint.rotation).GetComponent<ShipModule>();
            SceneManager.instance.AddGravityBody(newPart.GetComponent<Rigidbody>());
            newPart.Dematerialize(PartPrinterData.instance.printMaterial);
            meshRenderer = newPart.GetComponent<MeshRenderer>();
            foreach (Material mat in meshRenderer.materials) {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, opacity);
            }

            yield return new WaitForSeconds(PartPrinterData.instance.printTime);
            foreach (ParticleSystem particleSystem in printParticles) particleSystem.Stop();

            yield return new WaitForSeconds(1f);
            newPart.Materialize();
            newPart = null;
            opacity = 0f;
        } else {
            Debug.LogWarning("PartPrinter: Please clear print surface and try again");
        }
    }
}
