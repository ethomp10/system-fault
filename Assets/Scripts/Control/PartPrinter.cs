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
    [SerializeField] GameObject[] partsAvailable; // TODO: This should be in a list, added by blueprints
    [SerializeField] Material printMaterial;

    MeshRenderer meshRenderer;

    ShipModule newPart = null;
    bool printSurfaceClear = true;
    float opacity = 0f;

    private void Start() {
        if (!printPoint) {
            Debug.LogError("PartPrinter: No print point set");
        }

        if (!printMaterial) {
            Debug.LogError("PartPrinter: No print material set");
        }
    }

    void Update () {
        // TODO: This is just for testing
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
            StartCoroutine(PrintPart(partsAvailable[0]));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            StartCoroutine(PrintPart(partsAvailable[1]));
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            StartCoroutine(PrintPart(partsAvailable[2]));
        }

        // TODO: This should probably be in a shader
        if (newPart) {
            opacity += Time.deltaTime / printTime;
            foreach (Material mat in meshRenderer.materials) {
                mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, opacity);
            }
        }
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
