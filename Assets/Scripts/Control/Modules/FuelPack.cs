using UnityEngine;
using UnityEngine.UI;

//
// FuelPack.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Handles fuel and player shields
//

public class FuelPack : ShipModule {

    [SerializeField] float maxFuel = 100f;
    [SerializeField] float shieldChargeRate = 1f;
    [SerializeField] float shieldChargeCost = 1f;
    [SerializeField][Range(0, 10)] int shieldCellSlots = 4;
    [SerializeField] Image fuelGuage;

    const float SHIELDS_PER_CELL = 100f;

    float currentShields = 0f;
    int availableCells = 0;
    int chargedCells = 0;

    float fuel = 0f;

    protected override void Awake() {
        base.Awake();
        moduleType = GameTypes.ModuleType.FuelPack;
    }

    public void AddFuel(float amount) {
        fuel += amount;
        if (fuel > maxFuel) fuel = maxFuel;

        UpdateFuelGauge();
        PlayerHUD.instance.UpdateFuelGauge(GetFuelPercentage());
    }

    public void DrainFuel(float amount) {
        fuel -= amount;
        if (fuel < 0f) fuel = 0f;

        UpdateFuelGauge();
        PlayerHUD.instance.UpdateFuelGauge(GetFuelPercentage());
    }

    public void AddShields(float amount) {
        currentShields += amount;
        if (currentShields > SHIELDS_PER_CELL) {
            if (chargedCells < availableCells) {
                chargedCells++;
                currentShields -= SHIELDS_PER_CELL;
                PlayerHUD.instance.ChargeShieldCell(chargedCells);
            } else currentShields = SHIELDS_PER_CELL;
        }

        PlayerHUD.instance.AddShields(GetShieldPercentage());
    }

    public void ChargeShields() {
        if (fuel >= shieldChargeCost * shieldChargeRate * Time.deltaTime) {
            if (chargedCells < availableCells || currentShields < SHIELDS_PER_CELL) {
                DrainFuel(shieldChargeCost * shieldChargeRate * Time.deltaTime);
                PlayerHUD.instance.UpdateFuelGauge(GetFuelPercentage());

                AddShields(shieldChargeRate * Time.deltaTime);
            }
        } else Debug.LogWarning("FuelPack: Not enough fuel to charge cell");
    }

    public float AbsorbDamage(float amount) {
        currentShields -= amount;
        float leftOverDamage = 0f;

        if (currentShields <= 0) {
            if (chargedCells > 0) {
                chargedCells--;
                currentShields += SHIELDS_PER_CELL;
                PlayerHUD.instance.RemoveShieldCell(chargedCells);
            } else {
                leftOverDamage = Mathf.Abs(currentShields);
                currentShields = 0f;
            }
        }

        PlayerHUD.instance.DamageShields(GetShieldPercentage());
        return leftOverDamage;
    }

    public void AttachShieldCell() {
        if (availableCells < shieldCellSlots) {
            if (PlayerData.instance.DropShieldCell()) {
                availableCells++;
                PlayerHUD.instance.AttachShieldCell(availableCells);
            }
        } else Debug.LogWarning("FuelPack: No available shield cell slot");
    }

    public void UpdateFuelGauge() {
        fuelGuage.fillAmount = fuel / maxFuel;
    }

    public float GetFuelPercentage() {
        return fuel / maxFuel;
    }

    public float GetShieldPercentage() {
        return currentShields / SHIELDS_PER_CELL;
    }

    public int GetChargedCells() {
        return chargedCells;
    }

    public int GetAvailableCells() {
        return availableCells;
    }

    public bool IsEmpty() {
        return fuel == 0f;
    }

    public bool IsFull() {
        return fuel == maxFuel;
    }
}
