using UnityEngine;

//
// Xrom.cs
//
// Author: Gabriel Cimolino (Dead Battery Games)
// Purpose: 
//

public class Xrom : MonoBehaviour {

	float maxVelocity = 100;
	[SerializeField] Rigidbody rb;
	void Start(){
		rb = transform.GetComponent<Rigidbody>();
	}

	public void Move(Vector3 heading){
		//Debug.Log("Xrom::Move ~ Moving xrom by heading of " + FlockingManager.Vector3ToString(heading.normalized));
		//rb.AddForce(heading.normalized, ForceMode.VelocityChange);\
		rb.velocity += heading.normalized;
		if(rb.velocity.magnitude > maxVelocity) rb.velocity = rb.velocity.normalized * maxVelocity;
		Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.white);

		Quaternion targetRotation = Quaternion.FromToRotation(transform.forward, rb.velocity) * transform.rotation;
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime);
		//transform.LookAt(transform.position + rb.velocity);
	}
}
