using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remoteEnemyBehavior : MonoBehaviour {

	/* Instance Setting */
	public static remoteEnemyBehavior instance;

	private remoteEnemyBehavior(){

	}

	public void beAttacked(float damage){
		Life -= damage;
	}

	/* End Instance Setting*/

	public float attack;
	public GameObject explosionEffect;
	public GameObject wrench;
    public GameObject enemyBullet, enemyBulletSet, enemyBulletDir;
	public GameObject audioSet, rocketSound;
	public GameObject deadEffect;

    private const float singleShotCDMax = 3.0f;
    private float shootingTimer;

    private Animator anim;
	private Transform enemyTransform;
	private float Life;
	private int difficulty;

	// Use this for initialization
	void Start () {
		instance = this;
        shootingTimer = 0.0f;
        enemyTransform = GetComponent<Transform> ();
		difficulty = MenuController.instance.difficulty;
		anim = this.GetComponent<Animator> ();
		enemySetup ();
	}

	// Update is called once per frame
	void Update () {
		if (MenuController.instance.gameStart == false) {
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
			attack = 15.0f;
		} 
		else if (difficulty == 2) 
		{
			Life = 40.0f;
			attack = 30.0f;
		} 
		else 
		{
			Life = 60.0f;
			attack = 50.0f;
		}
	}

	// The main behavior of the enemy.
	/* **************************** */
	void action(){
		Vector3 myPos = enemyTransform.position;
		enemyTransform.position = new Vector3 (myPos.x, 4.0f, myPos.z);
		Vector3 tankPos = PlayerControl.instance.gameObject.GetComponent<Transform> ().position;
		Vector3 dir = tankPos - enemyTransform.position;
		if (dir.magnitude > 100.0f) {    //make the enemy to move faster when it is outside the screen
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = tankPos;
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().speed = 65.0f;
		}
		else if (dir.magnitude > 50.0f) {

			//  Vector3 move = new Vector3 (dir.normalized.x, 0.0f, dir.normalized.z);
			//	enemyTransform.LookAt (new Vector3 (tankPos.x, 0.0f, tankPos.z));
			//	enemyTransform.Translate (move / 5.0f);
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = tankPos;
			enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().speed = 5.0f;
		} else {
			Ray ray = new Ray();
			ray.origin = enemyTransform.position;
			ray.direction = dir;
			RaycastHit hit;
			Physics.Raycast (ray, out hit);
			if (hit.transform.tag == "Player") {
				enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = enemyTransform.position;
				enemyTransform.LookAt (tankPos);

                //add shooting behavior
                remoteShooting(dir);
                shootingTimer += Time.deltaTime;


            } else if(hit.transform.tag == "Enemy"){
				enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = enemyTransform.position;
				enemyTransform.LookAt (tankPos);
			}
			else{
				enemyTransform.GetComponent<UnityEngine.AI.NavMeshAgent> ().destination = tankPos;
			}
		}

	}

    void remoteShooting(Vector3 dir)
    {
        if (shootingTimer >= singleShotCDMax)
        {
            shootingTimer = 0;
            enemyBulletSet.GetComponent<ParticleSystem>().Play();
			GameObject b = Instantiate(enemyBullet, enemyBulletDir.transform.position + enemyBulletDir.transform.forward*3.0f, new Quaternion(0, 0, 0, 1)) as GameObject;      //vector(0,4,0) to let bullet appear upon the floor
            b.tag = "enemyBullet";
            b.SetActive(true);
            b.transform.SetParent(enemyBulletSet.transform);
			var Parti = b.GetComponent<ParticleSystem> ();
			Parti.Play ();
			GameObject effect = Instantiate(rocketSound, enemyBulletDir.transform.position, new Quaternion(0, 0, 0, 1)) as GameObject;
			effect.transform.SetParent(audioSet.transform);
			Destroy(effect, 3);
			Rigidbody rigidBullet = b.GetComponent<Rigidbody>();
			Vector3 dirNormal = dir.normalized * 50.0f;
            rigidBullet.AddForce(new Vector3(dirNormal.x, 0, dirNormal.z) * 10.0f);
            rigidBullet.useGravity = false;
            Destroy(b, 7);
        }
    }

	void isDead(){
		if (Life <= 0.0f) {
			anim.SetBool ("isDead", true);
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
