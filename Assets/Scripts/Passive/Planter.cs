using UnityEngine;

//
// TreePlanter.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Planter object for PlanterEditor script
//

public class Planter : MonoBehaviour {
    public GameObject[] objectPrefabs;
    public float minimumScale = 0.6f;
    public float maximumScale = 1.2f;
    [Range(1, 10)] public float density = 5f;
    public LayerMask densityLayer;
}
