using System.Collections.Generic;
using UnityEngine;

//
// FlockingManager.cs
//
// Author: Gabriel Cimolino (Dead Battery Games)
// Purpose: 
//

public static class FlockingManager {

	public static string Vector3ToString(Vector3 vector){
		return "[" + vector.x.ToString() + ", " + vector.y.ToString() + ", " + vector.z.ToString() + "]";
	}

	public static Vector3[] Headings(Vector3[] positions, Vector3[] headings, Vector3[] attractorPositions, float[] attractorScales, Vector3[] separatorPositions, float[] separatorScales, Vector3 boundingOrigin, float boundingScale, float perceptiveDistance, int flockSize, bool debug, bool debugHeading, bool debugCohesion, bool debugSeparation, bool debugAlignment, bool debugAttraction, bool debugBounding){
		List<Vector3> newHeadings = new List<Vector3>();

		Vector3[] cohesionVectors = Cohesion(positions, perceptiveDistance, flockSize, debug && debugCohesion);
		Vector3[] separationVectors = Separation(positions, separatorPositions, separatorScales, perceptiveDistance, debug && debugSeparation);
		Vector3[] alignmentVectors = Alignment(positions, headings, perceptiveDistance, flockSize, debug && debugAlignment);
		Vector3[] attractionVectors = Attraction(positions, attractorPositions, attractorScales, debug && debugAttraction);
		Vector3[] boundingVectors = Bounding(positions, boundingOrigin, boundingScale, debug && debugBounding);

		for(int i = 0; i < cohesionVectors.Length && i < separationVectors.Length && i < alignmentVectors.Length && i < attractionVectors.Length && i < boundingVectors.Length; i++){
			//Debug.Log("FlockingManager::Headings ~ Creating heading from cohesion vector " + Vector3ToString(cohesionVectors[i]));
			newHeadings.Add(cohesionVectors[i] + separationVectors[i] + alignmentVectors[i] + attractionVectors[i] + boundingVectors[i]);
		}

		return newHeadings.ToArray();
	}

	public static Vector3 Rotation(Vector3[] rotations){
		Vector3 avg = Vector3.zero;

		foreach(Vector3 rotation in rotations){
			avg += rotation;
		}

		avg = avg / rotations.Length;

		return avg;
	}

	private static Vector3[] Cohesion(Vector3[] positions, float perceptiveDistance, int flockSize, bool debug){

		List<Vector3> cohesionVectors = new List<Vector3>();

		for(int i = 0; i < positions.Length; i++){
			Vector3 vector = Vector3.zero;
			int neighbours = 0;

			//Debug.Log("FlockingManager::Cohesion ~ Calculating cohesion vector of object at position " + Vector3ToString(positions[i]));

			for(int j = 0; j < positions.Length; j++){
				if(i != j){
					Vector3 differenceVector = positions[j] - positions[i];

					if(Mathf.Abs(differenceVector.magnitude) < perceptiveDistance){
						neighbours++;

						//vector += gorbons[j].transform.position;
						vector += positions[j];//differenceVector;
					}
				}
			}

			//vector = (vector / neighbours) - gorbons[i].transform.position;
			//cohesionVectors.Add(vector.normalized);

			if(neighbours > 0) {
				vector = (vector / neighbours) - positions[i];
				vector = vector * Happiness(neighbours, flockSize);
				if(debug) Debug.DrawLine(positions[i], positions[i] + vector, Color.green);
			}

			cohesionVectors.Add(vector);
			//Debug.Log("FlockingManager::Cohesion ~ Calculated cohesion vector of " + Vector3ToString(cohesionVectors[i]));
		}

		return cohesionVectors.ToArray();
	}

	private static Vector3[] Separation(Vector3[] positions, Vector3[] separatorPositions, float[] separatorScales, float perceptiveDistance, bool debug){
		List<Vector3> separationVectors = new List<Vector3>();

		for(int i = 0; i < positions.Length; i++){
			Vector3 vector = Vector3.zero;

			for(int j = 0; j < positions.Length; j++){
				if(i != j){
					Vector3 differenceVector = positions[j] - positions[i];

					if(Mathf.Abs(differenceVector.magnitude) < perceptiveDistance / 2){

						vector -= differenceVector;
					}
				}
			}

			for(int j = 0; j < separatorPositions.Length && j < separatorScales.Length; j++){
				Vector3 differenceVector = separatorPositions[j] - positions[i];

				if(debug) Debug.DrawLine(separatorPositions[j], separatorPositions[j] + Vector3.down * separatorScales[j], Color.red);

				if(Mathf.Abs(differenceVector.magnitude) < separatorScales[j]){

					//Debug.Log("FlockingManager::Separation ~ A flock member has entered the influence of a separator");

					Vector3 avoidanceVector = differenceVector.normalized * Mathf.Pow(1 + (separatorScales[j] - Mathf.Abs(differenceVector.magnitude) / separatorScales[j]), 2);
					if(debug) Debug.DrawLine(positions[i], positions[i] - avoidanceVector, Color.magenta);
					vector -= avoidanceVector;
				}
			}

			if(debug) Debug.DrawLine(positions[i], positions[i] + vector, Color.red);
			separationVectors.Add(vector);
		}

		return separationVectors.ToArray();
	}

	private static Vector3[] Alignment(Vector3[] positions, Vector3[] headings, float perceptiveDistance, int flockSize, bool debug){
		List<Vector3> alignmentVectors = new List<Vector3>();

		for(int i = 0; i < positions.Length; i++){
			Vector3 vector = Vector3.zero;
			int neighbours = 0;

			for(int j = 0; j < positions.Length && j < headings.Length; j++){
				if(i != j){
					Vector3 differenceVector = positions[j] - positions[i];

					if(Mathf.Abs(differenceVector.magnitude) < perceptiveDistance){
						neighbours++;

						vector += headings[j];
					}
				}
			}

			//if(neighbours > 0) vector = vector / neighbours;
			if(debug) Debug.DrawLine(positions[i], positions[i] + vector.normalized * perceptiveDistance, Color.blue);
			//Debug.Log("FlockingManager::Alignment ~ Adding aligment vector " + Vector3ToString(vector));
			alignmentVectors.Add(vector);
		}

		return alignmentVectors.ToArray();
	}

	private static Vector3[] Attraction(Vector3[] positions, Vector3[] attractorPositions, float[] attractorScales, bool debug){
		List<Vector3> attractionVectors = new List<Vector3>();

		for(int i = 0; i < positions.Length; i++){
			Vector3 vector = Vector3.zero;

			for(int j = 0; j < attractorPositions.Length && j < attractorScales.Length; j++){
				Vector3 differenceVector = attractorPositions[j] - positions[i];

				if(debug) Debug.DrawLine(attractorPositions[j], attractorPositions[j] + Vector3.back * attractorScales[j], Color.green);

				if(Mathf.Abs(differenceVector.magnitude) < attractorScales[j]){

					Vector3 attractionVector = differenceVector * (attractorScales[j] - Mathf.Abs(differenceVector.magnitude)) / attractorScales[j];
					if(debug) Debug.DrawLine(positions[i], positions[i] + attractionVector, Color.green);
					vector += attractionVector;
				}
			}

			attractionVectors.Add(vector);
		}

		return attractionVectors.ToArray();
	}

	private static Vector3[] Bounding(Vector3[] positions, Vector3 boundingOrigin, float boundingScale, bool debug){
		List<Vector3> boundingVectors = new List<Vector3>();

		if(debug) Debug.DrawLine(boundingOrigin, boundingOrigin + Vector3.forward * boundingScale, Color.yellow);

		for(int i = 0; i < positions.Length; i++){
			Vector3 differenceVector = boundingOrigin - positions[i];
			Vector3 vector = (Mathf.Abs(differenceVector.magnitude) > boundingScale) ? differenceVector.normalized * (Mathf.Abs(differenceVector.magnitude) - boundingScale/ boundingScale) : Vector3.zero;
			if(debug) Debug.DrawLine(positions[i], positions[i] + vector, Color.yellow);
			boundingVectors.Add(vector);
		}

		return boundingVectors.ToArray();
	}

	private static float Happiness(int neighbours, int flockSize){
		float happiness = - Mathf.Pow(neighbours / flockSize, 2) + 2;
		//Debug.Log("FlockingManager::Happiness ~ Happiness given neighbourhood and ideal flock size is " + happiness.ToString());
		return happiness;
	}
}
