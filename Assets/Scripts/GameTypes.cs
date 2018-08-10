//
// GameTypes.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Holds various game types
//

public class GameTypes {
	public enum ModuleType {
        Thrusters,
        Boosters,
        FuelPack,
        QuantumDrive,
        KineticWeapon,
        PulseWeapon
    };

    public enum AssistMode {
        NoAssist,
        Hover,
        Astro,
        Quantum
    };
}
