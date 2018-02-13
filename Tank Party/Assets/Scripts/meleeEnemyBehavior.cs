using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class meleeEnemyBehavior : MonoBehaviour {

	/* Instance Setting */
	public static meleeEnemyBehavior instance;

	private meleeEnemyBehavior(){
		
	}

	public void beAttacked(float damage){
		Life -= damage;
	}

	/* End Instance Setting*/

	public float attack;
	public GameObject explosionEffect;
	public GameObject audioSet;
	public GameObject deadEffect;
	public GameObject wrench;

	public Transform enemyTransform;
	private Animator anim;
	private float Life;
	private int difficulty;

	// Use this for initialization
	void Start () {
		instance = this;
		enemyTransform = this.GetComponent<Transform> ();
		anim = this.GetComponent<Animator> ();
		difficulty = MenuController.instance.difficulty;
		enemySetup ();
	}
	
	// Update is called once per frame
	void Update () {
		if (MenuController.instance.gameStart == false) {
			//Debug.Log (MenuController.instance.gameStart);
			Destroy (this.gameObject);
		} 
		action ();
		isDead ();
	}

	void enemySetup(){
		// According to the difficulty, the enemy's life and attack will different.
		if (difficulty == 1) 
		{
			Life = 20.0f;
			attack = 5.0f;
		} 
		else if (difficulty == 2) 
		{
			Life = 40.0f;
			attack = 10.0f;
		} 
		else 
		{
			Life = 60.0f;
			attack = 20.0f;
		}
	}

	// The main behavior of the enemy.
	/* **************************** */
	void action(){
		Vector3 tankPos = PlayerControl.instance.gameObject.GetComponent<Transform> ().position;
		Vector3 dir = tankPos - enemyTransform.position;
		if (dir.magnitude > 100.0f) {     //make the enemy to move faster when it is outside the screen
			anim.SetBool ("isAttack", false);
			anim.SetBool ("isMove", true);
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = tankPos;
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().speed = 65.0f;
		}
		else if (dir.magnitude > 12.5f) {
			// Move toward the player
			anim.SetBool ("isAttack", false);
			anim.SetBool ("isMove", true);

			//Vector3 move = new Vector3 (dir.normalized.x, 0.0f, dir.normalized.z);
			//enemyTransform.LookAt (tankPos);
			//enemyTransform.Translate (move / 5.0f);
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = tankPos;
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().speed = 6.5f;
		} else {
			anim.SetBool ("isMove", false);
			anim.SetBool ("isAttack", true);
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = enemyTransform.position;
			enemyTransform.LookAt (tankPos);

		}
	}

	void isDead(){
		if (Life <= 0.0f) {
			Debug.Log ("Enemy Dead");
			anim.SetBool ("isAttack", false);
			anim.SetBool ("isMove", false);
			anim.SetBool ("isDead", true);
			//var par = GetComponent<ParticleSystem> ();
			//par.Play ();
			Destroy (this.gameObject);
			GameObject effect = Instantiate (deadEffect, enemyTransform.position, new Quaternion (0, 0, 0, 1)) as GameObject;
			effect.transform.SetParent (audioSet.transform);
			var e = effect.GetComponent<AudioSource> ();
			e.Play ();
			Destroy (effect, 3);
			Debug.Log ("Effect");
			GameObject b = Instantiate (explosionEffect, this.transform.position, this.transform.rotation) as GameObject;
			Destroy (b, 1);
			dropTool ();
		}
	}

	void dropTool(){
		int i = Random.Range(0, 100);
		if (i % 5 == 0) {
			GameObject tool = Instantiate (wrench, this.transform.position, this.transform.rotation) as GameObject;
			tool.transform.Rotate (40, 0, 40);
			tool.SetActive (true);
		}
	}

	// Bullet Collision
	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "playerBullet") {
			beAttacked (PlayerControl.instance.attack);
			Debug.Log ("Enemy Life:" + Life);
		}
	}
}