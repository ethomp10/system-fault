using UnityEngine;
using UnityEngine.UI;

//
// ShipComputer.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Controls the ship's digital display
//

public class ShipComputer : MonoBehaviour, IPowerable {

    [Header("Screen")]
    [SerializeField] MeshRenderer shipBodyMesh;
    [SerializeField] Material screenOnMaterial;
    [SerializeField] Material screenOffMaterial;

    [Header("Guages")]
    [SerializeField] Image throttleGauge;
    [SerializeField] Image fuelGauge;

    [Header("Indicators")]
    [SerializeField] Text hoverIndicator;
    [SerializeField] Text astroIndicator;
    [SerializeField] Text quantumIndicator;
    [SerializeField] Text speedometer;

    Color activeColour = new Color(0f, 130f/255f, 1f, 1f);
    Color inactiveColour = new Color(120f/255f, 120f/255f, 120f/255f, 1f);
    Color throttleColour = new Color(0f, 1f, 0f, 1f);
    Color warningColour = new Color(1f, 0f, 0f, 1f);


    public void UpdateThrottleGauge(float throttle) {
        throttleGauge.fillAmount = Mathf.Abs(throttle);

        if (throttle >= 0f) throttleGauge.color = throttleColour;
        else throttleGauge.color = warningColour;
    }

    public void UpdateFuelGauge(float fuel) {
        fuelGauge.fillAmount = fuel;

        if (fuel > 0.25f) fuelGauge.color = activeColour;
        else fuelGauge.color = warningColour;
    }

    public void UpdateSpeedometer(float speed, bool reverse) {
        speedometer.text = speed.ToString("F0");
        if (reverse) speedometer.color = warningColour;
        else speedometer.color = activeColour;
    }

    public void TogglePower(bool toggle) {
        if (toggle) {
            gameObject.SetActive(true);
            Material[] mats = shipBodyMesh.materials;
            mats[4] = screenOnMaterial;
            shipBodyMesh.materials = mats;
        } else {
            gameObject.SetActive(false);
            Material[] mats = shipBodyMesh.materials;
            mats[4] = screenOffMaterial;
            shipBodyMesh.materials = mats;
        }
    }

    public void ChangeAssistMode(GameTypes.AssistMode mode) {
        switch(mode) {
            case GameTypes.AssistMode.NoAssist:
                hoverIndicator.color = inactiveColour;
                astroIndicator.color = inactiveColour;
                quantumIndicator.color = inactiveColour;
                break;
            case GameTypes.AssistMode.Hover:
                hoverIndicator.color = activeColour;
                astroIndicator.color = inactiveColour;
                quantumIndicator.color = inactiveColour;
                break;
            case GameTypes.AssistMode.Astro:
                hoverIndicator.color = inactiveColour;
                astroIndicator.color = activeColour;
                quantumIndicator.color = inactiveColour;
                break;
            case GameTypes.AssistMode.Quantum:
                hoverIndicator.color = inactiveColour;
                astroIndicator.color = inactiveColour;
                quantumIndicator.color = activeColour;
                break;
        }
    }
}
