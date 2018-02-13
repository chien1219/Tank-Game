using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossBehavior : MonoBehaviour {

	/* Instance Setting */
	public static bossBehavior instance;

	private bossBehavior(){

	}

	public void beAttacked(float damage){
		Life -= damage;
	}

	/* End Instance Setting*/

	public float attack, laserAttack;
	public GameObject explosionEffect;
	public GameObject body, wrench;
	public GameObject enemyBullet, enemyLazer, enemyBulletSet, bossDir;
	public GameObject chargeEffect, laserEffect;
	public GameObject audioSet, rocketSound;
	public GameObject deadEffect;

	private const float singleShotCDMax = 2.4f;
	private const float razerCDMax = 10.0f;
	private float shootingTimer;
	private int shootContinue;
	private float razerTimer;
	private bool isPlay;
	private bool laserWorking;
	private bool laserResetCounting;
	private float laserReset;

	private Transform enemyTransform;
	private float Life;
	private int difficulty;

	// Use this for initialization
	void Start () {
		instance = this;
		shootingTimer = 2.0f;
		razerTimer = 0.0f;
		enemyTransform = GetComponent<Transform> ();
		difficulty = MenuController.instance.difficulty;
		isPlay = false;
		enemySetup ();
		shootContinue = 2;
		laserWorking = false;
		laserResetCounting = false;
		laserReset = 0.0f;
	}

	// Update is called once per frame
	void Update () {
		if (MenuController.instance.gameStart == false) {
			Destroy (this.gameObject);
		} 
		action ();
		isDead ();
		if(laserResetCounting == true)
			laserReset += Time.deltaTime;
	}

	void enemySetup(){
		// According to the difficulty, the enemy's life and attack will different.
		if (difficulty == 1) 
		{
			Life = 500.0f;
			attack = 15.0f;
			laserAttack = 50.0f;
		} 
		else if (difficulty == 2) 
		{
			Life = 600.0f;
			attack = 30.0f;
			laserAttack = 70.0f;
		} 
		else 
		{
			Life = 700.0f;
			attack = 50.0f;
			laserAttack = 100.0f;
		}
	}

	// The main behavior of the enemy.
	/* **************************** */
	void action(){
		Vector3 tankPos = PlayerControl.instance.gameObject.GetComponent<Transform> ().position;
		Vector3 dir = tankPos - enemyTransform.position;
		if (dir.magnitude > 100.0f) {    //make the enemy to move faster when it is outside the screen
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = tankPos;
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().speed = 65.0f;
		} else if (dir.magnitude > 80.0f) {

			//  Vector3 move = new Vector3 (dir.normalized.x, 0.0f, dir.normalized.z);
			//	enemyTransform.LookAt (new Vector3 (tankPos.x, 0.0f, tankPos.z));
			//	enemyTransform.Translate (move / 5.0f);
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = tankPos;
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().speed = 5.0f;
			enemyTransform.LookAt (tankPos);
			enemyTransform.transform.Rotate (0, -90, 0);
		} else if (dir.magnitude > 45.0f) {
			Ray ray = new Ray ();
			ray.origin = enemyTransform.position;
			ray.direction = dir;
			RaycastHit hit;
			Physics.Raycast (ray, out hit);
			if (hit.transform.tag == "Player") {
				enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = tankPos;
				enemyTransform.LookAt (tankPos);
				enemyTransform.transform.Rotate (0, -90, 0);

				//add shooting behavior
				remoteShooting (dir);
				shootingTimer += Time.deltaTime;
				razerTimer += Time.deltaTime;

			} else if (hit.transform.tag == "Enemy") {
				enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = tankPos;
				enemyTransform.LookAt (tankPos);
				enemyTransform.transform.Rotate (0, -90, 0);
			} else {
				enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = tankPos;
				enemyTransform.LookAt (tankPos);
				enemyTransform.transform.Rotate (0, -90, 0);
			}
		} else {
			Ray ray = new Ray();
			ray.origin = enemyTransform.position;
			ray.direction = dir;
			RaycastHit hit;
			Physics.Raycast (ray, out hit);
			if (hit.transform.tag == "Player") {
				enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = enemyTransform.position;
				enemyTransform.LookAt (tankPos);
				enemyTransform.transform.Rotate (0, -90, 0);

				//add shooting behavior
				remoteShooting(dir);
				shootingTimer += Time.deltaTime;
				razerTimer += Time.deltaTime;

			} else if(hit.transform.tag == "Enemy"){
				enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = enemyTransform.position;
				enemyTransform.LookAt (tankPos);
				enemyTransform.transform.Rotate (0, -90, 0);
			}
			else{
				enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = tankPos;
				enemyTransform.LookAt (tankPos);
				enemyTransform.transform.Rotate (0, -90, 0);
			}
		}
		if (laserReset > 2.1f) {   //this is so complex, do not try to understand it
			razerTimer = 0.0f;
			laserWorking = false;
			laserReset = 0.0f;
			laserResetCounting = false;
			shootingTimer = 0.0f;
			isPlay = false;
			var Parti = body.GetComponent<ParticleSystem> ();
			Parti.Stop ();
		}
	}

	void remoteShooting(Vector3 dir)
	{
		if (shootingTimer >= singleShotCDMax && laserWorking == false)
		{
			enemyBulletSet.GetComponent<ParticleSystem>().Play();
			//bossDir.GetComponent<ParticleSystem> ().Stop ();
			GameObject b = Instantiate(enemyBullet, bossDir.transform.position, new Quaternion(0, 0, 0, 1)) as GameObject;      //vector(0,4,0) to let bullet appear upon the floor
			b.tag = "bossBullet";
			b.SetActive(true);
			b.transform.SetParent(enemyBulletSet.transform);
			var Parti = b.GetComponent<ParticleSystem> ();
			Parti.Play ();
			GameObject effect = Instantiate(rocketSound, bossDir.transform.position, new Quaternion(0, 0, 0, 1)) as GameObject;
			effect.transform.SetParent(audioSet.transform);
			Destroy(effect, 3);
			Rigidbody rigidBullet = b.GetComponent<Rigidbody>();
			Vector3 dirNormal = dir.normalized * 50.0f;
			rigidBullet.AddForce(new Vector3(dirNormal.x, 0, dirNormal.z) * 10.0f);
			rigidBullet.useGravity = false;
			Destroy(b, 7);
			if (shootContinue == 0) {
				shootContinue = 2;
				shootingTimer = 0.0f;
			} else {
				--shootContinue;
				shootingTimer = 2.0f;
			}
		}

		if (razerTimer >= razerCDMax-2.0f && isPlay == false) {
			laserReset = 0.0f;
			laserResetCounting = true;
			laserWorking = true;
			var partiSys = body.GetComponent<ParticleSystem> ();
			partiSys.Play ();
			isPlay = true;
			GameObject effect = Instantiate (chargeEffect, enemyTransform.position, new Quaternion (0, 0, 0, 1)) as GameObject;
			effect.transform.SetParent (audioSet.transform);
			var e = effect.GetComponent<AudioSource> ();
			e.Play ();
			Destroy (effect, 2);
		}


		if (razerTimer >= razerCDMax) {
			Vector3 tankPos = PlayerControl.instance.gameObject.GetComponent<Transform> ().position;
			razerTimer = 0;
			GameObject b = Instantiate(enemyLazer, bossDir.transform.position, new Quaternion(0, 0, 0, 1)) as GameObject;      //vector(0,4,0) to let bullet appear upon the floor
			b.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
			b.transform.LookAt (tankPos);
			//b.transform.Rotate (0, 90, 0);
			b.tag = "bossLaser";
			b.SetActive(true);
			b.transform.SetParent(enemyBulletSet.transform);

			GameObject effect = Instantiate (laserEffect, enemyTransform.position, new Quaternion (0, 0, 0, 1)) as GameObject;
			effect.transform.SetParent (audioSet.transform);
			var e = effect.GetComponent<AudioSource> ();
			e.Play ();
			Destroy (effect, 2);

			Rigidbody rigidBullet = b.GetComponent<Rigidbody>();
			Vector3 dirNormal = dir.normalized * 50.0f;
			rigidBullet.AddForce(new Vector3(dirNormal.x, 0, dirNormal.z) * 110.0f);
			rigidBullet.useGravity = false;
			Destroy(b, 7);

			var partiSys = body.GetComponent<ParticleSystem> ();
			partiSys.Stop();
			isPlay = false;

			laserWorking = false;
			shootingTimer = 0.0f;
		}
	}

	void isDead(){
		if (Life <= 0.0f) {
			Destroy (this.gameObject);
			GameObject b = Instantiate (explosionEffect, this.transform.position, this.transform.rotation);
			Destroy (b, 1);
			dropTool ();
			GameObject effect = Instantiate (deadEffect, enemyTransform.position, new Quaternion (0, 0, 0, 1)) as GameObject;
			effect.transform.SetParent (audioSet.transform);
			var e = effect.GetComponent<AudioSource> ();
			e.Play ();
			Destroy (effect, 3);
		}
	}

	void dropTool(){
		GameObject tool = Instantiate (wrench, this.transform.position, this.transform.rotation) as GameObject;
		tool.transform.Rotate (40, 0, 40);
		tool.SetActive (true);
	}

	// Bullet Collision
	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "playerBullet") {
			beAttacked (PlayerControl.instance.attack);
			Debug.Log ("Enemy Life:" + Life);
		}
	}
}
