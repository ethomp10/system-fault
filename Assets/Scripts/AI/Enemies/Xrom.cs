using System.Collections.Generic;
using UnityEngine;

//
// Xrom.cs
//
// Author: Gabriel Cimolino (Dead Battery Games)
// Purpose: 
//

public class Xrom : MonoBehaviour, IFlocker {

	float maxVelocity = 50;
	float rotationSpeed = 5;
	[SerializeField] Rigidbody rb;
	void Start(){
		rb = transform.GetComponent<Rigidbody>();
	}

	public void Move(Vector3 heading, bool debug){
		rb.velocity += heading.normalized;
		if(rb.velocity.magnitude > maxVelocity) rb.velocity = rb.velocity.normalized * maxVelocity;
		if(debug) Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.white);

		Quaternion targetRotation = Quaternion.FromToRotation(transform.forward, rb.velocity) * transform.rotation;
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
	}

	public void Rotate(Vector3 rotation){
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rotation), Time.fixedDeltaTime);
		// transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.down), Time.fixedDeltaTime);
	}

	public void GetAttractors(){}
}
