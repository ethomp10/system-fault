﻿using UnityEngine;

//
// Interfaces.cs
//
// Author: Eric Thompson (Dead Battery Games)
// Purpose: Defines various interfaces
//

public interface ISnappable {
    void SnapToPoint(Vector3 point);
}

public interface IMaterializeable {
    void Materialize();
    void Dematerialize(Material demat);
}

public interface ICollectable {
    void Pickup();
}

public interface IDamageable {
    void Damage(float amount);
}

public interface IWeapon {
    void Fire(Transform firePoint = null);
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
