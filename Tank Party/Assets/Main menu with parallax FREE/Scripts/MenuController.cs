using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class MenuController : MonoBehaviour {

    public static MenuController instance;

	//Difficulty 1 = easy 2 = normal 3 = hard
	[HideInInspector]
	public int difficulty;
	public bool gameStart;

    //Active option and bool to check if main menu is active
    private int option = 0;
    private bool mainMenu = true;

    //Check to move the menu using the keys or only the arrows
    [SerializeField, Tooltip("Check to move the menu using the keys or only the arrows")]
    public bool useKeys = true;

    //Check if using parallax or not
    public bool useParallax = true;

    //Scenes animation
    private bool isAnimating = false;
    private int activeScene = 1;
    [SerializeField, Tooltip("Animation speed in seconds")]
    public float animSpeed;

    //Option quantity
    [SerializeField, Tooltip ("Introduce all the options in your menu")]
    public string[] options;

    //Backgrounds
    [SerializeField, Tooltip("Introduce all the backgrounds for the scenes in your menu")]
    public GameObject[] backgrounds;
    [SerializeField, Tooltip("Introduce all the backgrounds for the scenes in your menu")]
    public GameObject[] backgroundsParallax;
    [SerializeField, Tooltip("Introduce the main bck for your menu")]
    public GameObject mainBackgroundParallax;
    [SerializeField, Tooltip("Introduce the main bck for your menu")]
    public GameObject mainBackground;
    [SerializeField, HideInInspector]
    public Text menuText;
    [SerializeField]
    public GameObject[] activeBackground;

    //Arrow Animators
	[SerializeField, HideInInspector]
    public Animator ArrowR;
	[SerializeField, HideInInspector]
    public Animator ArrowL;

    //Menu bar gameobject
	[SerializeField, HideInInspector]
    public GameObject menuBar;

    //Backgrounds Controller
	[SerializeField, HideInInspector]
    public GameObject backgroundsController;

    //Sounds
    [Header("Sounds")]
    [Space(10)]
    public AudioClip Select;
    public AudioClip SceneSelect;
    private AudioSource Audio;

    //Events
    [SerializeField]
    public UnityEvent[] Events;

    //Exit Menu
    [SerializeField]
    public GameObject exitMenu;

    //Options menu
    [SerializeField]
    public GameObject OptionsMenu;

	//Difficulty menu
	[SerializeField]
	public GameObject DifficultyMenu;

    //Wave menu
    [SerializeField]
    public GameObject WaveMenu;

    //Game Scene
    [SerializeField]
	public GameObject GameScene1;
	public GameObject GameScene2;
	public GameObject GameScene3;

	//Game UI
	[SerializeField]
	public GameObject GameUI;


	//Main Menu
	[SerializeField]
	public GameObject mainMenuObject;

    void Start()
    {
		difficulty = 1;
		Audio = gameObject.GetComponent<AudioSource>();
		//Audio.Play ();
        instance = this;
		gameStart = false;
        //Set the activeBackground array length
        if (useParallax) { activeBackground = new GameObject[backgroundsParallax.Length]; } else { activeBackground = new GameObject[backgrounds.Length]; }
        initiate();      
    }

	void Update () {

        if (mainMenu) { 
        //Changes the text corresponding option
        menuText.text = options[option];

        //Deactivate arrows
        //If the option is less than 1 left arrow deactivated
        if(option < 1)
        {
            ArrowL.SetBool("Deactivate", true);
        }else
        {
            ArrowL.SetBool("Deactivate", false);
        }

        //If the option is the last option deactivate right arrow
        if (option == options.Length-1)
        {
            ArrowR.SetBool("Deactivate", true);
        }
        else
        {
            ArrowR.SetBool("Deactivate", false);
        }

            //If use keys is active move with the keys pressed
            if (useKeys)
            {
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    moveRight();
                }

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    moveLeft();
                }

                //If enter is pressed reproduce the corresponding event
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    pressEnter();
                }
            }
        }

        //Check is the scenes are animating and puts the variable in true or false
        var anim = backgroundsController.GetComponent<Animation>();
        if (anim.isPlaying)
        {
            isAnimating = true;
        }else
        {
            isAnimating = false;
        }


    }

    //Initiate
    void initiate()
    {
        //If use parallax is active then instantiate the parallax main bck
        //Else instantiate the normal background

		Audio.volume = 0.3f;
        mainMenu = true;
        menuBar.SetActive(true);

		gameStart = false;
		GameUI.SetActive (false);

        if (useParallax)
        {
            //Instantiate the background an set the parent to this gameobject
            //Then reset the scale and position
            //Set the sibling to first so the background is visible
            //Then adjust the rect values
            //And lastly set the active background array position 0 to this background
            var Bck = Instantiate(mainBackgroundParallax) as GameObject;
            Bck.transform.SetParent(this.gameObject.transform);
            Bck.transform.localScale = new Vector3(1, 1, 1);
            Bck.transform.localPosition = new Vector3(0, 0, 0);
            Bck.transform.SetSiblingIndex(0);
            var rect = Bck.GetComponent<RectTransform>();
            rect.offsetMax = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            activeBackground[0] = Bck;
        }
        else
        {
            //Instantiate the background an set the parent to this gameobject
            //Then reset the scale and position
            //Set the sibling to first so the background is visible
            //Then adjust the rect values
            //And lastly set the active background array position 0 to this background
            var Bck = Instantiate(mainBackground) as GameObject;
            Bck.transform.SetParent(this.gameObject.transform);
            var rect = Bck.GetComponent<RectTransform>();
            Bck.transform.SetSiblingIndex(0);
            rect.transform.localScale = new Vector3(1, 1, 1);
            rect.transform.localPosition = new Vector3(0, 0, 0);
            rect.offsetMax = new Vector2(0, 0);
            rect.offsetMin = new Vector2(0, 0);
            activeBackground[0] = Bck;
        }
    }

	public void Reinitiate()
	{
		//If use parallax is active then instantiate the parallax main bck
		//Else instantiate the normal background

		Audio.volume = 0.3f;
		mainMenu = true;
		menuBar.SetActive(true);
		ActiveScene (difficulty, false);

		gameStart = false;
		GameUI.SetActive (false);
	}

    //Press enter or click on option 
    public void pressEnter()
    {
        Events[option].Invoke();
    }

    //Function to go foward in the menu
    public void moveRight()
    {
        if(option < options.Length-1)
        {
            option = option + 1;
            ArrowR.SetBool("Click", true);
            Audio.clip = Select;
            Audio.Play();
        }
    }

    //Function to go back in the menu
    public void moveLeft()
    {
        if (option > 0)
        {
            option = option - 1;
            ArrowL.SetBool("Click", true);
            Audio.clip = Select;
            Audio.Play();
        }
    }
    
    //New Game event
    public void newGame()
    {
        //Loads the first scene, change the number to your desired scene
		//SceneManager.LoadScene(1);

		closeWave ();

		StartCoroutine(GameStart());
    }

	IEnumerator GameStart(){
	
		gameStart = true;
		ActiveScene (difficulty, true);
		PlayerControl.instance.initial ();
		yield return new WaitForSeconds (1.7f);

		GameUI.SetActive (true);
		mainMenuObject.SetActive (false);
		mainMenu = false;

	}

	void ActiveScene(int difficulty, bool active){
		switch (difficulty) {
		case 1:
			GameScene1.SetActive (active);
			break;
		case 2:
			GameScene2.SetActive (active);
			break;
		case 3:
			GameScene3.SetActive (active);
			break;
		default:
			break;
		}
	}

    //Continue
    public void continueGame()
    {
        //In this part you need to include your save game script to implement the continue function
    }

    //Select Difficulty Event
    public void selectDifficulty()
    {
		DifficultyMenu.gameObject.GetComponent<Animation>().Play("Fade In");
		mainMenu = false;
		DifficultyMenu.transform.SetAsLastSibling();
    }

   

    //Opens the exit menu
    public void exitMenuOpen()
    {
        var animEx = exitMenu.GetComponent<Animation>();
        exitMenu.transform.SetAsLastSibling();
        animEx.Play("Fade In");
        mainMenu = false;
    }

    //Closes the exit menu
    public void exitMenuClose()
    {
        var animEx = exitMenu.GetComponent<Animation>();
        animEx.Play("Fade out");
        mainMenu = true;
    }

    //Exit Game
    public void exitGame()
    {
        Application.Quit();
    }

    //Open Options
    public void openOptions()
    {
        OptionsMenu.gameObject.GetComponent<Animation>().Play("Fade In");
        mainMenu = false;
        OptionsMenu.transform.SetAsLastSibling();
    }

    //Close Options
    public void closeOptions()
    {
        OptionsMenu.gameObject.GetComponent<Animation>().Play("Fade out");
        mainMenu = true;
    }

    //Open Wave Selection
    public void openWave()
    {
        WaveMenu.gameObject.GetComponent<Animation>().Play("Fade In");
        mainMenu = false;
        WaveMenu.transform.SetAsLastSibling();
    }

    //Close Wave Selection
    public void closeWave()
    {
        WaveMenu.gameObject.GetComponent<Animation>().Play("Fade out");
        mainMenu = true;
    }

    public void setEasy(){
		difficulty = 1;
		//GameScene = GameScene1;
		DifficultyMenu.gameObject.GetComponent<Animation>().Play("Fade out");
		mainMenu = true;
	}

	public void setNormal(){
		difficulty = 2;
		//GameScene = GameScene2;
		DifficultyMenu.gameObject.GetComponent<Animation>().Play("Fade out");
		mainMenu = true;
	}

	public void setHard(){
		difficulty = 3;
		//GameScene = GameScene3;
		DifficultyMenu.gameObject.GetComponent<Animation>().Play("Fade out");
		mainMenu = true;
	}
}

