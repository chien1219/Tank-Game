using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyGenerate : MonoBehaviour {

	public static enemyGenerate instance;

	public void destroyAllEnemy(){
		while (enemySet.transform.childCount > 0) {
			Destroy (enemySet.transform.GetChild (0));
		}

	}

	public GameObject meleeEnemy;
	public GameObject remoteEnemy;
	public GameObject boss;
	public GameObject enemySet;
	public GameObject explosionEffect;
	public Image waveNumberTen;    //for showing and changing the wave number on the canvas (this object and all the below)
	public Image waveNumberOne;
	public Sprite zero;
	public Sprite one;
	public Sprite two;
	public Sprite three;
	public Sprite four;
	public Sprite five;
	public Sprite six;
	public Sprite seven;
	public Sprite eight;
	public Sprite nine;

	private int totalEnemy;
	private int currentSpawnEnemy;
	private int wave;
	private float timeStep;
	private Vector3[] initPos;
	private int[] fivePos;
	private static float timeStepMax = 15.0f;

	// Use this for initialization
	void Start () {
		instance = this;
		totalEnemy = 2;
		currentSpawnEnemy = 0;
		wave = 1;
		timeStep = 14.0f;
		initPos = new Vector3[12];
		settingInitPos ();
		fivePos = new int[5];
	}

	// Update is called once per frame
	void Update () {
		//When game starts, the scene manager will begin to generate the enemies.
		//The enemies' settings are based on the difficulty and the num of the waves.
		if(!MenuController.instance.gameStart){
			wave = WaveSlider.instance.waveValue;
			currentSpawnEnemy = 0;
			timeStep = 14.0f;
		}
		if (MenuController.instance.gameStart) {
			StartCoroutine (wait ());
		}
	}

	IEnumerator wait(){
        waveNumberUpdate();
        yield return new WaitForSeconds (5);
		generateEnemy();
		checkingWaveEnd ();
	}

	void generateEnemy (){
			totalEnemy = wave + 1;
			if (wave <= 10) {
				if (timeStep >= timeStepMax) {
					fiveDiffPosNum ();
					for (int i = 0; i < 5; ++i) {
						if (currentSpawnEnemy == totalEnemy)
							break;
						generateMelee (initPos [fivePos [i]]);          
						++currentSpawnEnemy;
					}
					timeStep = 0.0f;
				}
			} else if (wave <= 19) {
				int maxRemoteEnemy = ((wave - 10) / 5) + 1;
				totalEnemy -= 4;
				if (timeStep >= timeStepMax) {
					fiveDiffPosNum ();
					for (int i = 0; i < 5; ++i) {
						if (currentSpawnEnemy == totalEnemy)
							break;
						if (maxRemoteEnemy != 0) {
							generateRemote (initPos [fivePos [i]]);
							++currentSpawnEnemy;
							--maxRemoteEnemy;
						} else {
							generateMelee (initPos [fivePos [i]]);
							++currentSpawnEnemy;
						}
					}
					timeStep = 0.0f;
				}

			} else if (wave == 20) {
				totalEnemy = 1;
				if (timeStep >= timeStepMax) {
					if (currentSpawnEnemy == totalEnemy)
						return;
					fiveDiffPosNum ();
					generateBoss (initPos [fivePos [0]]);
					++currentSpawnEnemy;
					timeStep = 0.0f;
				}
			} else if (wave % 5 == 0) {
				totalEnemy -= 10;
				int maxRemoteEnemy = (Random.Range (0, 200))%(totalEnemy/3);
				int bossNum = 1;
				if (timeStep >= timeStepMax) {
					fiveDiffPosNum ();
					for (int i = 0; i < 5; ++i) {
						if (currentSpawnEnemy == totalEnemy)
							break;
						if (bossNum == 1) {
							generateBoss (initPos [fivePos [0]]);
							++currentSpawnEnemy;
							bossNum = 0;
						} else if (maxRemoteEnemy == 0) {
							generateMelee (initPos [fivePos [i]]);
							++currentSpawnEnemy;
						} else {
							int random = Random.Range (0, 100);
							if (random > 40) {
								generateRemote (initPos [fivePos [i]]);
								++currentSpawnEnemy;
								--maxRemoteEnemy;
							} else {
								generateMelee (initPos [fivePos [i]]);
								++currentSpawnEnemy;
							}
						}
					}
					timeStep = 0.0f;
				}
			} else {
				totalEnemy -= 7;
				int maxRemoteEnemy = (Random.Range (0, 200))%(totalEnemy/3);
				if (timeStep >= timeStepMax) {
					fiveDiffPosNum ();
					for (int i = 0; i < 5; ++i) {
						if (currentSpawnEnemy == totalEnemy)
							break;
						if (maxRemoteEnemy == 0) {
							generateMelee (initPos [fivePos [i]]);
							++currentSpawnEnemy;
						} else {
							int random = Random.Range (0, 100);
							if (random > 40) {
								generateRemote (initPos [fivePos [i]]);
								++currentSpawnEnemy;
								--maxRemoteEnemy;
							} else {
								generateMelee (initPos [fivePos [i]]);
								++currentSpawnEnemy;
							}
						}
					}
					timeStep = 0.0f;
				}
			}
		timeStep += Time.deltaTime;
	}

	void checkingWaveEnd(){
		if (currentSpawnEnemy == totalEnemy && enemySet.transform.childCount == 0) {
			wave += 1;
			currentSpawnEnemy = 0;
			timeStep = 13.0f;
		}
	}

	void generateBoss(Vector3 pos){
		GameObject newEnemy;
		newEnemy = Instantiate (boss, pos, new Quaternion (0, 0, 0, 1)).gameObject;
		newEnemy.SetActive (true);
		newEnemy.tag = "Enemy";
		//newEnemy.transform.localScale = new Vector3 (100.0f, 100.0f, 100.0f);
		newEnemy.AddComponent<Rigidbody> ();
		var rigidbody = newEnemy.GetComponent<Rigidbody> ();
		rigidbody.mass = 100.0f;
		rigidbody.useGravity = true;
		rigidbody.isKinematic = true;
		newEnemy.AddComponent<BoxCollider> ();
		var collider = newEnemy.GetComponent<BoxCollider> ();
		collider.center = new Vector3 (0.0f, 0.025f, 0.0f);
		collider.size = new Vector3 (0.15f, 0.05f, 0.15f);
		newEnemy.transform.SetParent (enemySet.transform);

	}

	void generateMelee(Vector3 pos){
		// Generate the melee enemy.
		GameObject newEnemy;
		newEnemy = Instantiate (meleeEnemy, pos, new Quaternion (0, 0, 0, 1)).gameObject;
		newEnemy.SetActive (true);
		newEnemy.tag = "Enemy";
		newEnemy.transform.localScale = new Vector3 (8.0f, 8.0f, 8.0f);
		newEnemy.AddComponent<Rigidbody> ();
		var rigidbody = newEnemy.GetComponent<Rigidbody> ();
		rigidbody.mass = 100.0f;
		rigidbody.useGravity = true;
		rigidbody.isKinematic = true;
		newEnemy.AddComponent<BoxCollider> ();
		var collider = newEnemy.GetComponent<BoxCollider> ();
		collider.center = new Vector3 (0.0f, 0.5f, -0.15f);
		newEnemy.transform.SetParent (enemySet.transform);
	}

	void generateRemote(Vector3 pos){
		// Generate the remote enemy.
		GameObject newEnemy;
		newEnemy = Instantiate (remoteEnemy, pos, new Quaternion (0, 0, 0, 1)).gameObject;
		newEnemy.tag = "Enemy";
		newEnemy.SetActive (true);
		newEnemy.transform.localScale = new Vector3 (9.0f, 9.0f, 9.0f);
		newEnemy.transform.Translate (0, 10, 0);
		newEnemy.AddComponent<Rigidbody> ();
		var rigidbody = newEnemy.GetComponent<Rigidbody> ();
		rigidbody.mass = 100.0f;
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
		//newEnemy.AddComponent<BoxCollider> ();
		//var collider = newEnemy.GetComponent<BoxCollider> ();
		//collider.size = new Vector3 (0.75f, 0.75f, 1.0f);
		//newEnemy.transform.rotation =new Quaternion (0, 0, 0, 1);
		newEnemy.transform.SetParent (enemySet.transform);
	}

	void settingInitPos(){
		for (int i = 0; i <= 3; ++i) {
			initPos [i] = new Vector3 (-150, 6, -150 + (200 * i));
		}
		for (int i = 4; i <= 6; ++i) {
			initPos [i] = new Vector3 (-150 + (200*(i-3)), 6, -150);
		}
		for (int i = 7; i <= 9; ++i) {
			initPos [i] = new Vector3 (-150+(200*(i-6)), 6, 450);
		}
		initPos [10] = new Vector3 (450, 6, 50);
		initPos [11] = new Vector3 (450, 6, 250);

	}

	void fiveDiffPosNum(){
		for(int i=0;i<5;++i){
			fivePos [i] = -1;
			int random = Random.Range (0, 100);
			random = random % 12;
			fivePos [i] = random;
			for (int j = 0; j < i; ++j) {
				if (fivePos [i] == fivePos [j]) {
					--i;
					break;
				}
			}
		}
	}

	void waveNumberUpdate (){     //to update the wave number on the canvas
		int tenTimes = wave / 10;
		int oneTimes = wave % 10;

		switch (tenTimes) {
		case 0:
			waveNumberTen.sprite = zero;
			break;
		case 1:
			waveNumberTen.sprite = one;
			break;
		case 2:
			waveNumberTen.sprite = two;
			break;
		case 3:
			waveNumberTen.sprite = three;
			break;
		case 4:
			waveNumberTen.sprite = four;
			break;
		case 5:
			waveNumberTen.sprite = five;
			break;
		case 6:
			waveNumberTen.sprite = six;
			break;
		case 7:
			waveNumberTen.sprite = seven;
			break;
		case 8:
			waveNumberTen.sprite = eight;	
			break;
		case 9:
			waveNumberTen.sprite = nine;
			break;
		default:
			waveNumberTen.sprite = zero;
			break;
		}

		switch (oneTimes) {
		case 0:
			waveNumberOne.sprite = zero;
			break;
		case 1:
			waveNumberOne.sprite = one;
			break;
		case 2:
			waveNumberOne.sprite = two;
			break;
		case 3:
			waveNumberOne.sprite = three;
			break;
		case 4:
			waveNumberOne.sprite = four;
			break;
		case 5:
			waveNumberOne.sprite = five;
			break;
		case 6:
			waveNumberOne.sprite = six;
			break;
		case 7:
			waveNumberOne.sprite = seven;
			break;
		case 8:
			waveNumberOne.sprite = eight;	
			break;
		case 9:
			waveNumberOne.sprite = nine;
			break;
		default:
			waveNumberOne.sprite = zero;
			break;
		}
	}
}
