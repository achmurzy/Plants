using UnityEngine;
using UnityEngine.UI;
using Vectrosity;
using System.Collections;

public class mainMenuUI : MonoBehaviour {

	public Canvas menuCanvas;
	private GameObject StartButton, TutorialButton, QuitButton;
	private GameObject[] ButtonGroup;
	private int ButtonIndex = 0;
	private float menuTimer = 0;
	private float menuBuffer = 0.5f;
	// Use this for initialization
	void Awake()
	{
		Object fuckyou = Resources.Load ("Prefabs/UI/MainMenu/Start");  
		StartButton = Instantiate(fuckyou) as GameObject;
		fuckyou = Resources.Load ("Prefabs/UI/MainMenu/Tutorial");
		TutorialButton = Instantiate(fuckyou) as GameObject;
		fuckyou = Resources.Load ("Prefabs/UI/MainMenu/Quit");
		QuitButton = Instantiate(fuckyou) as GameObject;
	}

	void Start () 
	{
		StartButton.transform.SetParent (menuCanvas.transform, false);
		StartButton.GetComponent<Button> ().onClick.AddListener (start);
		StartButton.AddComponent<Outline> ().effectColor = Color.green;

		TutorialButton.transform.SetParent (menuCanvas.transform, false);
		TutorialButton.GetComponent<Button> ().onClick.AddListener (learn);
		TutorialButton.AddComponent<Outline> ().effectColor = Color.green;
		TutorialButton.GetComponent<Outline> ().enabled = false;

		QuitButton.transform.SetParent (menuCanvas.transform, false);
		QuitButton.GetComponent<Button> ().onClick.AddListener (quit);
		QuitButton.AddComponent<Outline> ().effectColor = Color.green;
		QuitButton.GetComponent<Outline> ().enabled = false;

		ButtonGroup = new GameObject[3];
		ButtonGroup [0] = StartButton;
		ButtonGroup [1] = TutorialButton;
		ButtonGroup [2] = QuitButton;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Cursor.visible = true;
		int input = (int)(Input.GetAxis("L2_Axis"));
		if(input != 0)
			JoystickButtonLogic(ButtonGroup, ref ButtonIndex, input);
		else
			input = (int)Input.GetAxis("R2_Axis")*-1;
		if(input != 0)
			JoystickButtonLogic(ButtonGroup, ref ButtonIndex, input);
		menuTimer -= Time.deltaTime;

		if(Input.GetButtonDown("StateToggle"))
		{
			UnityEngine.EventSystems.PointerEventData pe = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current); 
			UnityEngine.EventSystems.ExecuteEvents.Execute(ButtonGroup[ButtonIndex], pe, UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
		}
	}

	void start()
	{
		Application.LoadLevel ("Playground");
	}

	void learn()
	{
		Application.LoadLevel ("Tutorial");
	}

	void quit()
	{
		Application.Quit ();
	}

	private void JoystickButtonLogic(GameObject[] buttons, ref int index, int SMACK)
	{
		if(menuTimer < 0)
		{
			int first = index;
			index += SMACK;
			if(index < 0)
				index+=buttons.Length;
			index %= buttons.Length; 
			menuTimer = menuBuffer;
			updateButtonOutline (buttons, ref first, index);
		}
	}
	
	private void updateButtonOutline(GameObject[] buttons, ref int index, int newIndex)
	{
		buttons[index].GetComponent<Outline> ().enabled = false;
		index = newIndex;
		buttons[index].GetComponent<Outline> ().enabled = true;
	}
}
