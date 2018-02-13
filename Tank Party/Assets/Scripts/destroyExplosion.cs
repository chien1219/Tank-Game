using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyExplosion : MonoBehaviour {

	private int delayTime;

	// Use this for initialization
	void Start () {
		delayTime = 1;
		Destroy (this.gameObject, delayTime);
	}

}
