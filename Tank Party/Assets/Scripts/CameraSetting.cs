using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetting : MonoBehaviour {

	private Transform camera;
	public Transform lookAtPosition;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Transform> ();
		//camera.position = new Vector3(lookAtPosition.position.x+115, lookAtPosition.position.y+200, lookAtPosition.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		camera.position = new Vector3(lookAtPosition.position.x+45, lookAtPosition.position.y+77, lookAtPosition.position.z);
	}
}
