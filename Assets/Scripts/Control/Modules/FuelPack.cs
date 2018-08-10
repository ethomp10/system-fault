using UnityEngine;

//
// FuelPack.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Handles fuel and player shields
//

public class FuelPack : ShipModule {

    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private int maxCells = 4;

    const float SHIELDS_PER_CELL = 100f;
    const float CELL_CHARGE_COST = 50f;

    private int currentCells = 0;
    private float shields = 0f;
    private float fuel = 0f;

    protected override void Awake() {
        base.Awake();
        moduleType = GameTypes.ModuleType.FuelPack;
        AddFuel(maxFuel); // TODO: Remove
    }

    public void DrainFuel(float amount) {
        fuel -= amount;
        if (fuel < 0f) fuel = 0f; ;
    }

    public void AddFuel(float amount) {
        fuel += amount;
        if (fuel > maxFuel) fuel = maxFuel;
    }

    public void AddShields(float amount) {
        shields += amount;
        if (shields > currentCells * SHIELDS_PER_CELL) {
            shields = currentCells * SHIELDS_PER_CELL;
        }
    }

    public void ChargeFuelCell() {
        if (fuel >= CELL_CHARGE_COST && shields < SHIELDS_PER_CELL * currentCells) {
            DrainFuel(CELL_CHARGE_COST);
            AddShields(SHIELDS_PER_CELL);
        }
    }

    public float AbsorbDamage(float amount) {
        if (shields - amount > 0f) {
            shields -= amount;
            Debug.LogWarning("FuelPack: Shields: " + shields);
            return 0f;
        } else {
            amount -= shields;
            shields = 0f;
            Debug.LogWarning("FuelPack: Shields: " + shields);
            return amount;
        }
    }

    public void AttachAllPossibleCells() {
        if (maxCells - currentCells > 0) {
            int cells = PlayerData.instance.DropShieldCells(maxCells - currentCells);
            if (cells > 0) {
                currentCells += cells;
                Debug.Log("FuelPack: Attached cells: " + currentCells);
            }
        }
    }

    public float GetFuelPercentage() {
        return fuel / maxFuel;
    }

    public bool IsEmpty() {
        return fuel > 0f;
    }
}
