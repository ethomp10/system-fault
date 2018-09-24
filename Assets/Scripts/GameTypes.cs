//
// GameTypes.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Holds various game types
//

public class GameTypes {
	public enum ModuleType {
        FuelPack,
        Thrusters,
        Boosters,
        QuantumDrive,
        PulseWeapon,
        KineticWeapon
    };

    public enum AssistMode {
        NoAssist,
        Hover,
        Astro,
        Quantum
    };
}
