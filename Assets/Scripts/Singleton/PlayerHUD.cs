using UnityEngine;
using UnityEngine.UI;

//
// PlayerHUD.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Toggles various HUD elements
//

public class PlayerHUD : MonoBehaviour {

    public static PlayerHUD instance = null;

    [SerializeField] Image crosshair;
    [SerializeField] Text usePrompt;
    [SerializeField] Text dematPrompt;

    void Awake() {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start () {
		if (!crosshair) {
            Debug.LogError("PlayerHUD: Crosshair not set");
        }
        if (!usePrompt) {
            Debug.LogError("PlayerHUD: Use prompt not set");
        }
    }
	
    public void ToggleCrosshair(bool show) {
        if (show) {
            crosshair.enabled = true;
        } else {
            crosshair.enabled = false;
        }
    }

    public void ToggleUsePrompt(bool show) {
        if (show) {
            usePrompt.enabled = true;
        } else {
            usePrompt.enabled = false;
        }
    }

    public void ToggleDematPrompt(bool show) {
        if (show) {
            dematPrompt.enabled = true;
        } else {
            dematPrompt.enabled = false;
        }
    }
}
