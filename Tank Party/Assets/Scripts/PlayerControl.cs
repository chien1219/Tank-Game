using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {

	/* Instance Setting */
	public static PlayerControl instance;

	// Nobody can create the instance
	private PlayerControl(){
		
	}
	// Set new max speed to the tank
	public void setMaxSpeed(float newSpeed){
		maxSpeed = newSpeed;
	}


	/* End Instance Setting*/

	public Rigidbody tank;
	public Transform turret, rightTrack, leftTrack;
	public GameObject bullet, bulletSet, bulletDir;
	public Transform lookAtPosition;
	public float attack;
	public GameObject shootParti;
	public GameObject lowHealthSmoke;
	public GameObject engineEffect, singleShotEffect, multiShotEffect, explosionEffect;
	public GameObject audioSet;
	public GameObject deadEffect;

	private bool isPlay, isSet;
	private bool attackType;
	private float bulletSpeed;
	private float maxSpeed, forceParameter, turretRotate;
	private Vector3 mousePressed;

	public float Life;
	private const float LifeMax = 1000.0f;
	public Image lifeLine;

	public Image singleShotCircle;      //for bullets CD and UI CD time change
	public Image multiShotCircle;
	public Transform singleShotFrame;
	public Transform multiShotFrame;
	private float singleShotCD;
	private float multiShotCD;
	private const float singleShotCDMax = 1.2f;
	private const float multiShotCDMax = 10.7f;

	private bool isDead;

	//GameOver Canvas
	[SerializeField]
	public GameObject GameOverCanvas;

	// Use this for initialization
	void Start () {
		instance = this;
		initial ();
	}

	public void initial(){
		isSet = false;
		isDead = false;
		Life = LifeMax;
		maxSpeed = 8.0f;
		attack = 20.0f;
		attackType = true;
		forceParameter = -3500.0f;
		turretRotate = 30.0f;
		tank.maxAngularVelocity = 1;
		bulletSpeed = 100.0f;
		isPlay = false;
		singleShotFrame.localScale = new Vector3 (1.2f, 1.2f, 1.2f);
		multiShotFrame.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		singleShotCD = singleShotCDMax;
		multiShotCD = multiShotCDMax;
	}

	// Update is called once per frame
	void Update () {
		if (MenuController.instance.gameStart == false)
			return;
		isPlay = false;
		tankMove ();
		torrentRotate ();
		changeAttackType ();
		shooting ();
		CdUiUpdate ();
		LifeUpdate ();
		if (MenuController.instance.gameStart && isSet == false) {
			isSet = true;
			this.transform.position = new Vector3 (110.0f, 0.2f, 145.0f);
			this.transform.rotation = new Quaternion (0.0f, 0.0f, 0.0f, 1.0f);
		}
	}
		
	void tankMove(){
		leftTrackMove ();
		rightTrackMove ();
		if (tank.velocity.magnitude > maxSpeed) {
			//Debug.Log ("In PlayerControl: " + maxSpeed);
			tank.velocity = tank.velocity.normalized * maxSpeed;
		}
		var effect = engineEffect.GetComponent<AudioSource> ();
		if (isPlay)
			effect.Play ();
		else 
			effect.Stop ();
		
	}
	void leftTrackMove(){
		if (Input.GetKey ("q")) {
			isPlay = true;
			tank.AddForceAtPosition (leftTrack.up * forceParameter, leftTrack.position);
		} else if (Input.GetKey ("a")) {
			isPlay = true;
			tank.AddForceAtPosition (leftTrack.up * forceParameter * -1, leftTrack.position);
		} else {
			Vector3 vAtTrack = tank.GetPointVelocity (leftTrack.position);
			if (vAtTrack.magnitude <= 1.0f)
				tank.AddForceAtPosition (vAtTrack * -1000, leftTrack.position);
			else
				tank.AddForceAtPosition (vAtTrack * -500, leftTrack.position);
		}
	}
	void rightTrackMove(){
		if (Input.GetKey ("o")) {
			isPlay = true;
			tank.AddForceAtPosition (rightTrack.up * forceParameter , rightTrack.position);
		} else if (Input.GetKey ("l")) {
			isPlay = true;
			tank.AddForceAtPosition (rightTrack.up * forceParameter * -1 , rightTrack.position);
		} else {
			Vector3 vAtTrack = tank.GetPointVelocity (rightTrack.position);
			if (vAtTrack.magnitude <= 1.0f)
				tank.AddForceAtPosition (vAtTrack * -1000, rightTrack.position);
			else
				tank.AddForceAtPosition (vAtTrack * -500, rightTrack.position);
		}

	}

	void changeAttackType(){
		if (Input.GetKey (KeyCode.RightArrow)) {
			attackType = false;
			attack = 20.0f;
			singleShotFrame.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
			multiShotFrame.localScale = new Vector3 (1.2f, 1.2f, 1.2f);

		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			attackType = true;
			attack = 30.0f;
			singleShotFrame.localScale = new Vector3 (1.2f, 1.2f, 1.2f);
			multiShotFrame.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		}
	}

	void torrentRotate(){
		if (Input.GetMouseButtonDown (1)) {
			mousePressed = Input.mousePosition;
		}
		if (Input.GetMouseButton (1)) {
			if (Input.mousePosition.x > mousePressed.x) {
				turret.Rotate(Vector3.forward * Time.deltaTime * turretRotate);
			} else if (Input.mousePosition.x < mousePressed.x) {
				turret.Rotate(Vector3.back * Time.deltaTime * turretRotate);
			}
		}
	}

	void shooting(){
		if (Input.GetMouseButtonDown (0)) {
			//Debug.Log ("Shooting!");
			if (attackType) {
				if(singleShotCD == singleShotCDMax){
					GameObject effect = Instantiate (singleShotEffect, bulletDir.transform.position, new Quaternion (0, 0, 0, 1)) as GameObject;
					effect.transform.SetParent (audioSet.transform);
					var e = effect.GetComponent<AudioSource> ();
					e.Play ();
					Destroy (effect, 3);
					GameObject Explosion = Instantiate(shootParti, bulletDir.transform.position, new Quaternion (0, 0, 0, 1)) as GameObject;
					Destroy (Explosion, 1);
					bulletDir.GetComponent<ParticleSystem> ().Play ();
					GameObject b = Instantiate (bullet, bulletDir.transform.position, new Quaternion (0, 0, 0, 1)) as GameObject;
					b.tag = "playerBullet";
					b.SetActive (true);
					b.transform.SetParent (bulletSet.transform);
					Rigidbody rigidBullet = b.GetComponent<Rigidbody> ();
					rigidBullet.AddForce (bulletDir.transform.forward * 3000.0f);
					Destroy (b, 7);
					singleShotCD = 0.0f;
				}
			} 
			else {
				if(multiShotCD == multiShotCDMax){
					bulletDir.GetComponent<ParticleSystem> ().Play ();
					GameObject effect = Instantiate (multiShotEffect, bulletDir.transform.position, new Quaternion (0, 0, 0, 1)) as GameObject;
					effect.transform.SetParent (audioSet.transform);
					var e = effect.GetComponent<AudioSource> ();
					e.Play ();
					Destroy (effect, 3);
					GameObject Explosion = Instantiate(shootParti, bulletDir.transform.position, new Quaternion (0, 0, 0, 1)) as GameObject;
					Destroy (Explosion, 1);
					for(int i=0;i<5;++i){
						var shootDir = bulletDir.transform;
						shootDir.Rotate (0.0f, -20.0f + i*10.0f, 0.0f);						
						GameObject b = Instantiate (bullet, bulletDir.transform.position + shootDir.forward*1.7f, new Quaternion (0, 0, 0, 1)) as GameObject;
						b.transform.SetParent (bulletSet.transform);
						b.tag = "playerBullet";
						b.SetActive (true);
						Rigidbody rigidBullet = b.GetComponent<Rigidbody> ();
						rigidBullet.AddForce (shootDir.forward * 3000.0f);
						shootDir.Rotate (0.0f, 20.0f - i*10.0f, 0.0f);
						Destroy (b, 0.45f);
						multiShotCD = 0.0f;
					}
				}
			}

		}
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Terrain") {
			tank.constraints = RigidbodyConstraints.FreezePositionY;
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == "tool") {
			Life += LifeMax * 0.2f;
			Destroy (col.gameObject);
		}
	}

	void CdUiUpdate(){
		if (singleShotCD < singleShotCDMax)
			singleShotCD += Time.deltaTime;
		else
			singleShotCD = singleShotCDMax;
		
		if (multiShotCD < multiShotCDMax)
			multiShotCD += Time.deltaTime;
		else
			multiShotCD = multiShotCDMax;

		singleShotCircle.fillAmount = singleShotCD / singleShotCDMax;
		multiShotCircle.fillAmount = multiShotCD / multiShotCDMax;
	}

	void LifeUpdate(){
		if (Life > LifeMax)
			Life = LifeMax;
		if (Life < 0.0f)
			Life = 0.0f;
		lifeLine.fillAmount = Life / LifeMax;
		var Parti = lowHealthSmoke.GetComponent<ParticleSystem> ();
		if (Life < (LifeMax * 0.25)) {
			Parti.Play ();
		} else {
			Parti.Stop ();
		}
		if (Life == 0.0f && isDead == false) {
			isDead = true;
			DeadHandler ();
		}
	}

	void DeadHandler(){
		
		//gameover scene
		GameObject effect = Instantiate (deadEffect, this.transform.position + new Vector3(0.0f,2.0f,0.0f), new Quaternion (-90, 0, 0, 1)) as GameObject;
		GameObject effect2 = Instantiate (deadEffect, this.transform.position + new Vector3(0.0f,2.0f,0.0f), new Quaternion (0, 0, 0, 1)) as GameObject;
		Destroy (effect, 5);
		Destroy (effect2, 5);
		GameObject b = Instantiate (explosionEffect, this.transform.position, this.transform.rotation) as GameObject;
		b.transform.SetParent (audioSet.transform);
		Destroy (b, 5);
		//Destroy (this.gameObject, 0.1f);
		//this.gameObject.SetActive(false);

		//GameObject newGameOverCanvas = Instantiate (GameOverCanvas) as GameObject;
		GameOverCanvas.SetActive (true);

		StartCoroutine (Wait ());
	}

	IEnumerator Wait(){

		yield return new WaitForSeconds (3);
		GameOverCanvas.SetActive (false);

		System.Threading.Thread.Sleep (2000);

		MenuController.instance.mainMenuObject.SetActive (true);
		MenuController.instance.Reinitiate ();
		isSet = false;
	}

}
