using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlocker {

	void Move(Vector3 heading, bool debug);
	void Rotate(Vector3 rotation);
	void GetAttractors();
}
