﻿using UnityEngine;

//
// Interfaces.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Defines various interfaces
//

public interface ISnappable {
    void SnapToPoint(Transform point);
}

public interface IMaterializeable {
    void Materialize();
    void Dematerialize(Material demat);
    bool IsColliding();
}

public interface IPowerable {
    void TogglePower(bool toggle);
}

public interface ICollectable {
    void Pickup();
}

public interface IDamageable {
    void Damage(float amount, Vector3 damageForce);
}

public interface IWeapon {
    void Fire();
}

public interface IControllable {
    void CheckInput(ControlObject controlObject);
    void SetCam(PlayerCamera controlCam);
}

public interface IGroundable {
    void SetGrounded(bool grounded);
}

public interface IUsable {
    void Use();
}
