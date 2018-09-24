using System.Collections.Generic;
using UnityEngine;

//
// AICluster.cs
//
// Author: Gabriel Cimolino (Dead Battery Games)
// Purpose: 
//

public class AICluster : MonoBehaviour {

	[SerializeField] GameObject xromShipPrefab;
	[SerializeField] GameObject planetMesh;
	[SerializeField] List<GameObject> xroms;
	[SerializeField] List<GameObject> asteroids;
	[SerializeField] float boundingRadius = 0.0f;
	[SerializeField] bool debug;
	[SerializeField] bool debugHeading;
	[SerializeField] bool debugCohesion;
	[SerializeField] bool debugSeparation;
	[SerializeField] bool debugAlignment;
	[SerializeField] bool debugAttraction;
	[SerializeField] bool debugBounding;
	float planetRadius = 0.0f;
	List<Vector3> attractorPositions;
	List<float> attractorScales;
	List<Vector3> separatorPositions;
	List<float> separatorScales;

	void Start(){
		xroms = new List<GameObject>();
		attractorPositions = new List<Vector3>();
		attractorScales = new List<float>();
		separatorPositions = new List<Vector3>();
		separatorScales = new List<float>();

		Bounds bounds = new Bounds(transform.position, Vector3.zero);

		Renderer renderer = planetMesh.GetComponent<Renderer>();

		bounds.Encapsulate(renderer.bounds);

		planetRadius = bounds.extents.magnitude / 2;

		foreach(Renderer r in GetComponentsInChildren<Renderer>()) {
			bounds.Encapsulate(r.bounds);
		}

		boundingRadius = bounds.extents.magnitude * 1.0f;

		Vector3 clusterPosition = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
		clusterPosition = clusterPosition * boundingRadius;

		for(int i = 0; i  < 10; i++){
			Vector3 position = new Vector3(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)) * asteroids[0].transform.localScale.x;
			position = position.normalized;
			xroms.Add(Instantiate(xromShipPrefab, transform.position + clusterPosition + position, Quaternion.identity));
		}

		//attractorPositions.Add(transform.position);
		//attractorScales.Add(boundingRadius);
		separatorPositions.Add(transform.position);
		separatorScales.Add(planetRadius * 2);

		for(int i = 0; i < asteroids.Count; i++){
			attractorPositions.Add(asteroids[i].transform.position);
			attractorScales.Add(asteroids[i].transform.localScale.x * 10);
			separatorPositions.Add(asteroids[i].transform.position);
			separatorScales.Add(asteroids[i].transform.localScale.x * 2);
		}
	}

	void FixedUpdate(){
		List<Vector3> positions = new List<Vector3>();
		List<Vector3> velocities = new List<Vector3>();

		for(int i = 0; i < xroms.Count; i++){
			positions.Add(xroms[i].transform.position);
			velocities.Add(xroms[i].transform.GetComponent<Rigidbody>().velocity);
		}

		Vector3[] headings = FlockingManager.Headings(positions.ToArray(), velocities.ToArray(), attractorPositions.ToArray(), attractorScales.ToArray(), separatorPositions.ToArray(), separatorScales.ToArray(), transform.position, boundingRadius, 50.0f, 4, debug, debugHeading, debugCohesion, debugSeparation, debugAlignment, debugAttraction, debugBounding);
	
		for(int i = 0; i < xroms.Count && i < headings.Length; i++){
			xroms[i].GetComponent<Xrom>().Move(headings[i], debug && debugHeading);
			xroms[i].GetComponent<Xrom>().Rotate(xroms, 50.0f);
		}
	}
}
