using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

	public static AudioController instance;
	public GameObject AudioSet;

	public AudioClip OpengingClip;
	public AudioClip GamingClip;
	private bool audioChanged;
	private AudioSource Audio;

	// Use this for initialization
	void Start () {
		audioChanged = false;
		Audio = gameObject.GetComponent<AudioSource>();
		Audio.volume = 0.1f;
		instance = this;

		PlayOpening ();
	}
	
	// Update is called once per frame
	void Update () {
		if (MenuController.instance.gameStart && !audioChanged) {
			PlayGaming ();
			audioChanged = true;
		} else if (!MenuController.instance.gameStart && audioChanged) {
			PlayOpening ();
			audioChanged = false;
		}
	}
		
	void PlayOpening(){
		Audio.clip = OpengingClip;
		Audio.Play();
		Audio.loop = true;
	}

	void PlayGaming(){
		Audio.clip = GamingClip;
		Audio.Play();
		Audio.loop = true;
	}

}
