using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handleCollision : MonoBehaviour {

	//public GameObject laserParti;

	void OnCollisionEnter(Collision col){
		FenceCollision (col);
		HouseCollision (col);
		mudCollision (col, true);
		//laserCollision (col);
	}
	void OnCollisionExit(Collision col){
		mudCollision (col, false);
	}

	void FenceCollision(Collision col){
		if (col.gameObject.tag == "Fence") {
			col.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		}
	}
	void HouseCollision(Collision col){
		if (col.gameObject.tag == "House") {
			//col.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
		}
	}
	void mudCollision(Collision col, bool inMud){

		if (col.gameObject.tag == "Mud") {
			if (inMud) {
				PlayerControl.instance.setMaxSpeed (4.0f);
			} else {
				PlayerControl.instance.setMaxSpeed (8.0f);
			}
		}
	}

}
