using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    [SerializeField] float damageDelay = 0.5f;
    [SerializeField] float damageDrainSpeed = 1f;

    [Header("Fuel Pack")]
    [SerializeField] GameObject fuelPackHUD;
    [SerializeField] Image shields;
    [SerializeField] Image damage;
    [SerializeField] Image fuel;
    [SerializeField] Image[] availableCells;
    [SerializeField] Image[] chargedCells;

    [Header("Radar")]
    [SerializeField] GameObject radar;
    [SerializeField] Image shipLeft;
    [SerializeField] Image shipRight;

    bool animateDamage = false;

    void Awake() {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start () {
		if (!crosshair) Debug.LogError("PlayerHUD: Crosshair not set");
        if (!usePrompt) Debug.LogError("PlayerHUD: Use prompt not set");
        if (!fuelPackHUD) Debug.LogError("PlayerHUD: Fuel pack HUD not set");

        DisableFuelPackHUD();
    }

    void Update() {
        if (animateDamage) {
            damage.fillAmount = Mathf.MoveTowards(damage.fillAmount, shields.fillAmount, damageDrainSpeed * Time.deltaTime);
            if (damage.fillAmount == shields.fillAmount) animateDamage = false;
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
        if (show) usePrompt.enabled = true;
        else usePrompt.enabled = false;
    }

    public void ToggleDematPrompt(bool show) {
        if (show) dematPrompt.enabled = true;
        else dematPrompt.enabled = false;
    }

    public void ToggleShipRadar(bool show) {
        if (show) radar.SetActive(true);
        else radar.SetActive(false);
    }

    public void UpdateShipRadar(int direction) {
        switch (direction) {
            case 1:
                shipLeft.enabled = false;
                shipRight.enabled = true;
                break;
            case -1:
                shipLeft.enabled = true;
                shipRight.enabled = false;
                break;
            case 0:
                shipLeft.enabled = true;
                shipRight.enabled = true;
                break;
            default:
                Debug.LogError("PlayerHUD: Invalid argument passed to UpdateRadar");
                break;
        }
    }

    public void EnableFuelPackHUD(FuelPack fuelPack) {
        fuel.fillAmount = fuelPack.GetFuelPercentage();
        shields.fillAmount = fuelPack.GetShieldPercentage();
        damage.fillAmount = shields.fillAmount;
        for (int i = 0; i < fuelPack.GetChargedCells(); i++) chargedCells[i].enabled = true;
        for (int i = 0; i < fuelPack.GetAvailableCells(); i++) availableCells[i].enabled = true;

        fuelPackHUD.SetActive(true);
    }

    public void DisableFuelPackHUD() {
        foreach (Image cell in chargedCells) cell.enabled = false;
        foreach (Image cell in availableCells) cell.enabled = false;

        fuelPackHUD.SetActive(false);
    }

    public void AddShields(float shieldPercentage) {
        shields.fillAmount = shieldPercentage;
        if (!animateDamage && damage.fillAmount < shieldPercentage) damage.fillAmount = shieldPercentage;
    }

    public void DamageShields(float shiedPercentage) {
        shields.fillAmount = shiedPercentage;
        if (shields.fillAmount > damage.fillAmount) damage.fillAmount = 1f;

        StopCoroutine("ShowDamage");
        StartCoroutine("ShowDamage");
    }

    public void ChargeShieldCell(int cells) {
        damage.fillAmount = 0f;
        for (int i = 0; i < cells; i++) chargedCells[i].enabled = true;
    }

    public void RemoveShieldCell(int cells) {
        for (int i = cells; i < 3; i++) chargedCells[i].enabled = false;
    }

    public void UpdateFuelGauge(float fuelPercentage) {
        fuel.fillAmount = fuelPercentage;
    }

    IEnumerator ShowDamage() {
        animateDamage = false;
        yield return new WaitForSeconds(damageDelay);
        animateDamage = true;
    }
}
