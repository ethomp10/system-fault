using System.Collections.Generic;
using UnityEngine;

//
// Xrom.cs
//
// Author: Gabriel Cimolino (Dead Battery Games)
// Purpose: 
//

public class Xrom : MonoBehaviour {

	float maxVelocity = 50;
	float rotationSpeed = 5;
	[SerializeField] Rigidbody rb;
	void Start(){
		rb = transform.GetComponent<Rigidbody>();
	}

	public void Move(Vector3 heading, bool debug){
		//Debug.Log("Xrom::Move ~ Moving xrom by heading of " + FlockingManager.Vector3ToString(heading.normalized));
		//rb.AddForce(heading.normalized, ForceMode.VelocityChange);\
		rb.velocity += heading.normalized;
		if(rb.velocity.magnitude > maxVelocity) rb.velocity = rb.velocity.normalized * maxVelocity;
		if(debug) Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.white);

		Quaternion targetRotation = Quaternion.FromToRotation(transform.forward, rb.velocity) * transform.rotation;
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
		//transform.LookAt(transform.position + rb.velocity);
		//transform.rotation = Quaternion.Euler(rb.velocity);
	}

	public void Rotate(List<GameObject> xroms, float perceptiveDistance){
		Vector3 rotation = Vector3.zero;
		foreach(GameObject xrom in xroms){
			if(Vector3.Magnitude(xrom.transform.position - transform.position) < perceptiveDistance){
				rotation += xrom.transform.rotation.eulerAngles - transform.rotation.eulerAngles;
			}
		}

		// transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles + rotation), Time.fixedDeltaTime);
		// transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.down), Time.fixedDeltaTime);
	}
}
