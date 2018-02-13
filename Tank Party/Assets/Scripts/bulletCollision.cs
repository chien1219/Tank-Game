using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletCollision : MonoBehaviour {
	//public GameObject bullet;
	private GameObject effectSet;
	private GameObject particleSystemSet;
	public GameObject bulletEffect;
	public GameObject explosionEffect;

	void Start(){
		effectSet = AudioController.instance.AudioSet;
	}

	void OnCollisionEnter(Collision col){
		//FenceCollision (col);
		HouseCollision (col);
		TreeCollision (col);

		if (this.gameObject.tag == "enemyBullet" || this.gameObject.tag == "bossBullet" || this.gameObject.tag == "bossLaser")
            enemyBulletCollision(col);

        else
            EnemyCollision(col);

    }

	void FenceCollision(Collision col){
		if (col.gameObject.tag == "Fence") {
			col.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
			col.gameObject.GetComponent<ParticleSystem> ().Play ();
			Destroy (this.gameObject);
			// Add audio Effect
			GameObject effect = Instantiate (bulletEffect, this.transform.position, this.transform.rotation) as GameObject;
			effect.transform.SetParent (effectSet.transform);
			Destroy (effect, 3);
			// Add Particle System Effect
			GameObject Explosion = Instantiate (explosionEffect, this.transform.position, this.transform.rotation) as GameObject;

		}
	}
	void HouseCollision(Collision col){
		if (col.gameObject.tag == "House") {
			Destroy (this.gameObject);
			col.gameObject.GetComponent<ParticleSystem> ().Play ();
			if (col.gameObject.GetComponent<Rigidbody> () != null) {
				//col.gameObject.GetComponent<Rigidbody> ().isKinematic = false;
			}
			this.gameObject.GetComponent<Rigidbody> ().isKinematic = true;

			// Add audio Effect
			GameObject effect = Instantiate (bulletEffect, this.transform.position, this.transform.rotation) as GameObject;
			effect.transform.SetParent (effectSet.transform);
			Destroy (effect, 3);
			// Add Particle System Effect
			GameObject Explosion = Instantiate (explosionEffect, this.transform.position, this.transform.rotation) as GameObject;

		}
	}
	void TreeCollision(Collision col){
		if (col.gameObject.tag == "Tree") {
			Destroy (this.gameObject);
			// Add audio Effect
			GameObject effect = Instantiate (bulletEffect, this.transform.position, this.transform.rotation) as GameObject;
			effect.transform.SetParent (effectSet.transform);
			Destroy (effect, 3);
			// Add Particle System Effect
			GameObject Explosion = Instantiate (explosionEffect, this.transform.position, this.transform.rotation) as GameObject;
		}
	}
	void EnemyCollision(Collision col){
		if (col.gameObject.tag == "Enemy") {
			Destroy (this.gameObject);
			// Add audio Effect
			GameObject effect = Instantiate (bulletEffect, this.transform.position, this.transform.rotation) as GameObject;
			effect.transform.SetParent (effectSet.transform);
			Destroy (effect, 3);
			// Add Particle System Effect
			GameObject Explosion = Instantiate (explosionEffect, this.transform.position, this.transform.rotation) as GameObject;

		}
	}

    void enemyBulletCollision(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
            // Add audio Effect
			if (this.gameObject.tag == "enemyBullet")
				PlayerControl.instance.Life -= remoteEnemyBehavior.instance.attack;
			else if (this.gameObject.tag == "bossBullet")
				PlayerControl.instance.Life -= bossBehavior.instance.attack;
			else if (this.gameObject.tag == "bossLaser")
				PlayerControl.instance.Life -= bossBehavior.instance.laserAttack;
			GameObject effect = Instantiate(bulletEffect, this.transform.position, this.transform.rotation) as GameObject;
            effect.transform.SetParent(effectSet.transform);
            Destroy(effect, 3);
            // Add Particle System Effect
            GameObject Explosion = Instantiate(explosionEffect, this.transform.position, this.transform.rotation) as GameObject;
			Destroy (Explosion, 1);
        }
		if (col.gameObject.tag == "Enemy") 
		{
			Destroy(this.gameObject);
		}
    }


}
