using UnityEngine;
using UnityEditor;

//
// TreeEditor.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Plants trees
//

[CustomEditor(typeof(TreePlanter))]
public class TreeEditor : Editor {

    TreePlanter planter;

    void OnEnable() {
        planter = (TreePlanter)target;
    }

    void OnSceneGUI() {
        Event e = Event.current;
        if (e.keyCode == KeyCode.P && e.type == EventType.KeyDown) PlantTree();
    }

    void PlantTree() {
        RaycastHit hit;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.CompareTag("PlantableSurface")) {
                if (Physics.OverlapSphere(hit.point, 50f / planter.density, LayerMask.GetMask("Foliage")).Length == 0) {
                    GameObject tree = PrefabUtility.InstantiatePrefab(planter.treePrefabs[Random.Range(0, planter.treePrefabs.Length)]) as GameObject;

                    float scale = Random.Range(planter.minimumScale, planter.maximumScale);
                    tree.transform.position = hit.point;
                    tree.transform.parent = hit.transform;
                    tree.transform.localScale = new Vector3(scale, scale, scale);
                }
            } else if (!hit.collider.CompareTag("Tree")) {
                Debug.LogWarning("TreeEditor: Not a plantable surface");
            }
        }
    }
}
