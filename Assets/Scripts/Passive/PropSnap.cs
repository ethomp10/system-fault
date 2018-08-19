using UnityEngine;

//
// PropSnap.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Snaps props to their parent in edit mode; useful for placing objects on planets
//

[ExecuteInEditMode]
public class PropSnap : MonoBehaviour, ISnappable {

    public void SnapToPoint(Vector3 point) {
        Vector3 up = (transform.position - point).normalized;

        transform.rotation = Quaternion.FromToRotation(transform.up, up) * transform.rotation;
    }

    void Update () {
        #if UNITY_EDITOR
        if (!Application.isPlaying && transform.parent != null) SnapToPoint(transform.parent.position);
        #endif
    }
}
