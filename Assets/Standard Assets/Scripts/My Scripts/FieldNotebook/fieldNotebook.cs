using UnityEngine;
using UnityEngine.UI;

public class fieldNotebook : MonoBehaviour 
{
	private randomTextGenerator rTG;
	private Turtle2D turtle2D;
	private tutorialScript tS;
	private swirlingJumpController sJC;

	private Object objFont;
	private Font font;

	public GameObject vectorCanvas;

	//Bio
	private GameObject BioButton;
	public GameObject BioBackground;

	private GameObject MonoButton;
	private GameObject SymButton;
	private GameObject RosetteButton;
	private GameObject SimpleButton;
	private GameObject CompoundButton;
	private GameObject PalmateButton;
	private GameObject ZygoButton;
	private GameObject ActinoButton;
	private GameObject InfloresenceButton;

	private GameObject NodeButton;

	private GameObject BioInfoPanel;			//This is the panel that appears when the player clicks a
	private string componentName;				//component button. Each of the callbacks will instantiate
	private string componentDescription;		//this information in a different way and display the same
	private GameObject[] demoModels;			//panel. The correct model will be spawned somewhere off map
	private string[] demoText;
	private int modelIndex;
	private GameObject modelCamera;
	private Object[] modelArray;
	private Object[] textArray;

	private GameObject[] BioGroup;
	private int BioMenuIndex = 0;

	//Objectives
	private GameObject ObjectivesButton;
	public GameObject ObjectivesBackground;
	private GameObject DescriptionObjectives;
	private GameObject[] Objectives;

	//Scene Select
	private GameObject SceneButton;
	public GameObject SceneSelectBackground;
	private GameObject SceneSwapButton;
	private GameObject MainMenuButton;
	private GameObject QuitGameButton;
	private GameObject SurenessPanel;
	private GameObject[] SceneGroup;
	private int SceneOutlineIndex = 0;
	private int sure = -1;
	private string surenessText;

	//Overlay
	private GameObject HideShowButton;
	private GameObject TurtleObject;
	private GameObject TurtleObjectChild;
	private Sprite BioButtonBud, BioOpenBud, ObjectivesButtonBud, ObjectivesOpenBud, SceneButtonBud, SceneOpenBud;

	private float menuBufferLength = 0.25f;
	private float menuTimer = 0;


	// Use this for initialization
	void Awake () 
	{
		tS = this.GetComponent<tutorialScript> ();

		modelArray = Resources.LoadAll ("Prefabs/UI/Biology/DemoModels");
		textArray = Resources.LoadAll ("Prefabs/UI/Biology/DemoText");
		modelCamera = Resources.Load ("Prefabs/UI/Biology/BioModelCamera") as GameObject;
		modelCamera = Instantiate (modelCamera) as GameObject;
		modelCamera.GetComponent<Camera>().enabled = false;
		demoModels = new GameObject[10];
		demoText = new string[10];
		for(int i = 0; i < 10; i++)
		{
			demoModels[i] = Instantiate(modelArray[i]) as GameObject;
			demoModels[i].SetActive(false);
			TextAsset ta = textArray[i] as TextAsset;
			demoText[i] = ta.text;
		}

		if(tS == null)
		{
			GameObject go = GameObject.Find("Goal");
			objectivesInventory.SetDropOffZone(go.GetComponent<BoxCollider>());		
		}

		objectivesInventory.Start ();
		objectivesInventory.notebook = this.GetComponent<fieldNotebook> ();
		sJC = this.GetComponent<swirlingJumpController> ();

		questionGenerator.Awake ();

		objFont = Resources.Load ("NI7seg");
		font = (Font)Object.Instantiate (objFont);



		vectorCanvas = Vectrosity.VectorLine.canvas.gameObject;
		vectorCanvas.AddComponent<GraphicRaycaster> ();
		vectorCanvas.AddComponent<CanvasScaler> ();
		vectorCanvas.GetComponent<CanvasScaler> ().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		vectorCanvas.GetComponent<CanvasScaler> ().referenceResolution = new Vector2 (1537, 910);
		vectorCanvas.GetComponent<CanvasScaler> ().matchWidthOrHeight = 0.5f;

		Button button;
		Image image;

		Object[] o = Resources.LoadAll ("Textures/BudClosed");
		ObjectivesButtonBud = o [1] as Sprite;
		BioButtonBud = o [3] as Sprite;
		SceneButtonBud = o [5] as Sprite;
		Object[] oi = Resources.LoadAll ("Textures/BudOpen");
		BioOpenBud = oi [1] as Sprite;
		ObjectivesOpenBud = oi [3] as Sprite;
		SceneOpenBud = oi [5] as Sprite;
		//**********************************************OVERLAY**********************************************//
		HideShowButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Overlay/HideShowButton"));
		HideShowButton.transform.SetParent (vectorCanvas.transform, false);
		button = HideShowButton.GetComponent<Button> ();
		button.onClick.AddListener (ShowHideGUI);
		HideShowButton.GetComponent<Button> ().enabled = false;
		HideShowButton.GetComponent<Image> ().enabled = false;

		BioButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Overlay/BiologyInfo"));
		BioButton.transform.SetParent (vectorCanvas.transform, false);
		button = BioButton.GetComponent<Button> ();
		button.onClick.AddListener (ShowBiologyInfo);
		image = BioButton.GetComponent<Image> ();
		image.sprite = BioButtonBud;

		ObjectivesButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Overlay/Objectives"));
		ObjectivesButton.transform.SetParent (vectorCanvas.transform, false);
		button = ObjectivesButton.GetComponent<Button> ();
		button.onClick.AddListener (ShowObjectives);
		image = ObjectivesButton.GetComponent<Image> ();
		image.sprite = ObjectivesButtonBud;
		ObjectivesButton.transform.Rotate (Vector3.forward, 25.7f);

		SceneButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Overlay/SceneSelect"));
		SceneButton.transform.SetParent (vectorCanvas.transform, false);
		button = SceneButton.GetComponent<Button> ();
		button.onClick.AddListener (ShowSceneSelection);
		image = SceneButton.GetComponent<Image> ();
		image.sprite = SceneButtonBud;
		SceneButton.transform.Rotate (Vector3.forward, -25.7f);

		TurtleObject = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Overlay/TurtleObject"));
		TurtleObject.transform.SetParent (HideShowButton.transform, false);

		turtle2D = TurtleObject.AddComponent <Turtle2D> ();
		turtle2D.offset = HideShowButton.GetComponent<RectTransform> ().anchoredPosition;

		//***************************************************************************************************//

		//*******************************************BIOLOGY REFERENCE***************************************//
		BioBackground = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/BiologyPanel"));
		BioBackground.transform.SetParent (vectorCanvas.transform, false);
		BioBackground.SetActive (false);

		BioInfoPanel = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/InfoPanel"));
		BioInfoPanel.transform.SetParent (vectorCanvas.transform, false);
		BioInfoPanel.SetActive (false);
		BioInfoPanel.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(showPlantDescription);

		ActinoButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/ActinoButton"));
		ActinoButton.transform.SetParent (BioBackground.transform.GetChild(0).transform, false);
		button = ActinoButton.GetComponent<Button> ();
		ActinoButton.AddComponent<Outline> ().effectColor = Color.yellow;
		button.onClick.AddListener (actinomorphicPlantDescription);
		
		ZygoButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/ZygoButton"));
		ZygoButton.transform.SetParent (BioBackground.transform.GetChild(0).transform, false);
		button = ZygoButton.GetComponent<Button> ();
		ZygoButton.AddComponent<Outline> ().effectColor = Color.yellow;
		ZygoButton.GetComponent<Outline> ().enabled = false;
		button.onClick.AddListener (zygomorphicPlantDescription);
		
		InfloresenceButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/InfloresenceButton"));
		InfloresenceButton.transform.SetParent (BioBackground.transform.GetChild(0).transform, false);
		button = InfloresenceButton.GetComponent<Button> ();
		InfloresenceButton.AddComponent<Outline> ().effectColor = Color.yellow;
		InfloresenceButton.GetComponent<Outline> ().enabled = false;
		button.onClick.AddListener (infloresencePlantDescription);

		SimpleButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/SimpleButton"));
		SimpleButton.transform.SetParent (BioBackground.transform.GetChild(1).transform, false);
		button = SimpleButton.GetComponent<Button> ();
		SimpleButton.AddComponent<Outline> ().effectColor = Color.yellow;
		SimpleButton.GetComponent<Outline> ().enabled = false;
		button.onClick.AddListener (simplePlantDescription);
		
		CompoundButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/CompoundButton"));
		CompoundButton.transform.SetParent (BioBackground.transform.GetChild(1).transform, false);
		button = CompoundButton.GetComponent<Button> ();
		CompoundButton.AddComponent<Outline> ().effectColor = Color.yellow;
		CompoundButton.GetComponent<Outline> ().enabled = false;
		button.onClick.AddListener (compoundPlantDescription);
		
		PalmateButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/PalmateButton"));
		PalmateButton.transform.SetParent (BioBackground.transform.GetChild(1).transform, false);
		button = PalmateButton.GetComponent<Button> ();
		PalmateButton.AddComponent<Outline> ().effectColor = Color.yellow;
		PalmateButton.GetComponent<Outline> ().enabled = false;
		button.onClick.AddListener (palmatePlantDescription);

		MonoButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/MonopodialButton"));
		MonoButton.transform.SetParent (BioBackground.transform.GetChild(2).transform, false);
		button = MonoButton.GetComponent<Button> ();
		MonoButton.AddComponent<Outline> ().effectColor = Color.yellow;
		MonoButton.GetComponent<Outline> ().enabled = false;
		button.onClick.AddListener (monopodialPlantDescription);

		SymButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/SympodialButton"));
		SymButton.transform.SetParent (BioBackground.transform.GetChild(2).transform, false);
		button = SymButton.GetComponent<Button> ();
		SymButton.AddComponent<Outline> ().effectColor = Color.yellow;
		SymButton.GetComponent<Outline> ().enabled = false;
		button.onClick.AddListener (sympodialPlantDescription);

		RosetteButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/RosetteButton"));
		RosetteButton.transform.SetParent (BioBackground.transform.GetChild(2).transform, false);
		button = RosetteButton.GetComponent<Button> ();
		RosetteButton.AddComponent<Outline> ().effectColor = Color.yellow;
		RosetteButton.GetComponent<Outline> ().enabled = false;
		button.onClick.AddListener (rosettePlantDescription);

		NodeButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Biology/NodeButton"));
		NodeButton.transform.SetParent (vectorCanvas.transform, false);
		button = NodeButton.GetComponent<Button> ();
		NodeButton.AddComponent<Outline> ().effectColor = Color.yellow;
		NodeButton.GetComponent<Outline> ().enabled = false;
		button.onClick.AddListener (nodeDescription);
		NodeButton.SetActive (false);

		BioGroup = new GameObject[10];
		BioGroup [0] = ActinoButton; BioGroup [1] = ZygoButton; BioGroup [2] = InfloresenceButton;
		BioGroup [3] = SimpleButton; BioGroup [4] = CompoundButton; BioGroup [5] = PalmateButton;
		BioGroup [7] = MonoButton; BioGroup [6] = SymButton; BioGroup [8] = RosetteButton;	BioGroup [9] = NodeButton;
		//***************************************************************************************************//

		//************************************OBJECTIVES COMPONENT******************************************//
		ObjectivesBackground = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/Objectives/ObjectivesPanel"));
		ObjectivesBackground.transform.SetParent (vectorCanvas.transform, false);
		ObjectivesBackground.SetActive (false);

		Objectives = new GameObject[10];

		RectTransform rt = ObjectivesBackground.transform as RectTransform;
		for(int i = 0; i < 10; i++)
		{
			GameObject objectivesText = new GameObject();
			objectivesText.transform.SetParent(ObjectivesBackground.transform, false);

			Text objText = objectivesText.AddComponent<Text>();
			objText.text = objectivesInventory.objectivesList[i].writeObjective();

			Rect objectivesAnchor = new Rect(0, (rt.rect.height/2) - (i*rt.rect.height/10), 
			                                 	4*rt.rect.width/5, rt.rect.height/10);

			objText.rectTransform.anchoredPosition = new Vector2(objectivesAnchor.x, 
			                                                     objectivesAnchor.y-objectivesAnchor.height/2);
			objText.rectTransform.sizeDelta = new Vector2(objectivesAnchor.width, objectivesAnchor.height);
			objText.font = font;
			objText.fontSize = 20;
			objText.color = Color.green;
			Objectives[i] = objectivesText;
		}

		DescriptionObjectives = 
					(GameObject)Instantiate (Resources.Load ("Prefabs/UI/Objectives/DescriptionObjectives"));
		DescriptionObjectives.transform.SetParent(ObjectivesBackground.transform, false);
		DescriptionObjectives.GetComponent<Text> ().font = font;
		DescriptionObjectives.SetActive (false);
		//**************************************************************************************************//

		//************************************SCENE SELECTION COMPONENT**************************************//
		SceneSelectBackground = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/SceneSelect/ScenePanel"));
		SceneSelectBackground.transform.SetParent (vectorCanvas.transform, false);
		SceneSelectBackground.SetActive (false);

		SceneSwapButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/SceneSelect/SceneSwap"));
		SceneSwapButton.transform.SetParent (SceneSelectBackground.transform, false);
		button = SceneSwapButton.GetComponent<Button>();	button.onClick.AddListener (SceneSwap);
		SceneSwapButton.AddComponent<Outline> ().effectColor = Color.cyan;

		MainMenuButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/SceneSelect/MainMenu"));
		MainMenuButton.transform.SetParent (SceneSelectBackground.transform, false);
		button = MainMenuButton.GetComponent<Button>();	button.onClick.AddListener (MainMenu);
		MainMenuButton.AddComponent<Outline> ().effectColor = Color.cyan;
		MainMenuButton.GetComponent<Outline> ().enabled = false;

		QuitGameButton = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/SceneSelect/Quit Game"));
		QuitGameButton.transform.SetParent (SceneSelectBackground.transform, false);
		button = QuitGameButton.GetComponent<Button>();	button.onClick.AddListener (QuitGame);
		QuitGameButton.AddComponent<Outline> ().effectColor = Color.cyan;
		QuitGameButton.GetComponent<Outline> ().enabled = false;

		SceneGroup = new GameObject[3];
		SceneGroup [0] = SceneSwapButton;
		SceneGroup [1] = MainMenuButton;
		SceneGroup [2] = QuitGameButton;

		SurenessPanel = (GameObject)Instantiate (Resources.Load ("Prefabs/UI/SceneSelect/SurenessCheck"));
		SurenessPanel.transform.SetParent (SceneSelectBackground.transform, false);
		button = SurenessPanel.transform.GetChild (2).GetComponent<Button> (); button.onClick.AddListener (unsure);
		button.gameObject.AddComponent<Outline> ().effectColor = Color.red;
		button = SurenessPanel.transform.GetChild (1).GetComponent<Button> (); button.onClick.AddListener (_sure);
		button.gameObject.AddComponent<Outline> ().effectColor = Color.green;
		button.gameObject.GetComponent<Outline> ().enabled = false;
		SurenessPanel.SetActive (false);
		//***************************************************************************************************//
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(overlayActive() || questionGenerator.questioning())
		{
			if(Input.GetButtonDown("StateToggle"))
			{
				if(questionGenerator.questioning())
				{
					questionGenerator.submitAnswer();
				}
				else if(SurenessPanel.activeSelf)
				{
					if(SurenessPanel.transform.GetChild(2).gameObject.GetComponent<Outline>().enabled)
					{
						UnityEngine.EventSystems.PointerEventData pe = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current); 
						UnityEngine.EventSystems.ExecuteEvents.Execute(SurenessPanel.transform.GetChild(2).gameObject, pe, UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
					}
					else
					{
						UnityEngine.EventSystems.PointerEventData pe = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current); 
						UnityEngine.EventSystems.ExecuteEvents.Execute(SurenessPanel.transform.GetChild(1).gameObject, pe, UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
					}
				}
				else if(BioBackground.activeSelf)
				{
					UnityEngine.EventSystems.PointerEventData pe = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current); 
					UnityEngine.EventSystems.ExecuteEvents.Execute(BioGroup[BioMenuIndex], pe, UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
				}
				else if(SceneSelectBackground.activeSelf)
				{
					UnityEngine.EventSystems.PointerEventData pe = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current); 
					UnityEngine.EventSystems.ExecuteEvents.Execute(SceneGroup[SceneOutlineIndex], pe, UnityEngine.EventSystems.ExecuteEvents.pointerClickHandler);
				}
				else
					ShowObjectives();
			}
			if(Input.GetButtonDown ("Square"))
				ShowBiologyInfo();
			if(Input.GetButtonDown("Circle"))
				ShowSceneSelection();
			if(tS == null || questionGenerator.questioning())
				sJC.killSelect(true);
		}
		else if(tS==null && !questionGenerator.questioning())
			sJC.killSelect(false);
		
		if(questionGenerator.questioning())
		{
			vectorCanvas.SetActive(false);
		}
		else
			vectorCanvas.SetActive(true);
		
		sJC.killSwirl (BioBackground.activeSelf || BioInfoPanel.activeSelf);

		if(Input.GetButtonDown("Cancel"))
		{
			if(BioInfoPanel.activeSelf)
			{
				showPlantDescription();
			}
			else
				OverlayDisable();
		}

		if(questionGenerator.questioning())
		{
			if(menuTimer < 0)
			{
				int input = (int)(Input.GetAxis("L2_Axis"));
				if(input != 0)
				{
					questionGenerator.ToggleIndex++;
					if(questionGenerator.ToggleIndex >3)
						questionGenerator.ToggleIndex = 0;
					questionGenerator.Toggles[questionGenerator.ToggleIndex].GetComponent<Toggle>().isOn = true;
					menuTimer = menuBufferLength;
				}
				else
				{
					input = (int)Input.GetAxis("R2_Axis");
					if(input != 0)
					{
						questionGenerator.ToggleIndex--;
						if(questionGenerator.ToggleIndex < 0)
							questionGenerator.ToggleIndex = 3;
						questionGenerator.Toggles[questionGenerator.ToggleIndex].GetComponent<Toggle>().isOn = true;
						menuTimer = menuBufferLength;
					}
				}
			}
		}
		else if(SurenessPanel.activeSelf)
		{
			int input = (int)(Input.GetAxis("L2_Axis"));
			if(input != 0)
			{
				SurenessPanel.transform.GetChild(2).gameObject.GetComponent<Outline>().enabled = true;
				SurenessPanel.transform.GetChild(1).gameObject.GetComponent<Outline>().enabled = false;
			}
			else
			{
				input = (int)Input.GetAxis("R2_Axis");
				if(input != 0)
				{
					SurenessPanel.transform.GetChild(2).gameObject.GetComponent<Outline>().enabled = false;
					SurenessPanel.transform.GetChild(1).gameObject.GetComponent<Outline>().enabled = true;
				}
			}
		}
		else if(SceneSelectBackground.activeSelf)
		{
			int input = (int)(Input.GetAxis("L2_Axis"));
			if(input != 0)
				JoystickButtonLogic (SceneGroup, ref SceneOutlineIndex, input);
			else
			{
				input = (int)Input.GetAxis("R2_Axis")*-1;
				if(input != 0)
					JoystickButtonLogic (SceneGroup, ref SceneOutlineIndex, input);
			}
		}
		else if(BioBackground.activeSelf)
		{
			int input = (int)(Input.GetAxis("L2_Axis"));
			if(input != 0)
				JoystickButtonLogic (BioGroup, ref BioMenuIndex, input);
			else
			{
				input = (int)Input.GetAxis("R2_Axis")*-1;
				if(input != 0)
					JoystickButtonLogic (BioGroup, ref BioMenuIndex, input);
			}
		}
		else
		{}
		menuTimer -= Time.deltaTime;
	}

	public void ShowHideGUI()
	{
		vectorCanvas.SetActive (false);
	}

	public void ShowSceneSelection()
	{
		if(BioBackground.activeSelf)
			ShowBiologyInfo();
		if(ObjectivesBackground.activeSelf)
			ShowObjectives();
		SceneSelectBackground.SetActive (!SceneSelectBackground.activeSelf);
		SurenessPanel.SetActive (false);
		imageSwap (SceneButton.GetComponent<Image> ());
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
			menuTimer = menuBufferLength;
			updateButtonOutline (buttons, ref first, index);
		}
	}

	private void updateButtonOutline(GameObject[] buttons, ref int index, int newIndex)
	{
		buttons[index].GetComponent<Outline> ().enabled = false;
		index = newIndex;
		buttons[index].GetComponent<Outline> ().enabled = true;
	}

	public void SceneSwap()
	{
		if(Application.loadedLevelName == "Tutorial")
		{
			surenessText = "go to the main zone?";
			AskSure ();
			sure = 0;
			updateButtonOutline(SceneGroup, ref SceneOutlineIndex, 0);
		}
		else if(Application.loadedLevelName == "Playground")
		{
			surenessText = "go back to the tutorial zone?";
			AskSure ();
			sure = 1;
			updateButtonOutline(SceneGroup, ref SceneOutlineIndex, 0);
		}
	}

	public void MainMenu()
	{
		surenessText = "go to the main menu?";
		AskSure ();
		sure = 2;
		updateButtonOutline(SceneGroup, ref SceneOutlineIndex, 1);
	}

	public void QuitGame()
	{
		surenessText = "quit the game?";
		AskSure ();
		sure = 3;
		updateButtonOutline(SceneGroup, ref SceneOutlineIndex, 2);
	}

	private void _sure()
	{
		sureness(sure);
		sure = -1;
	}

	private void unsure()
	{
		AskSure();
	}

	private void sureness(int i)
	{
		OverlayDisable();
		GameObject.Destroy(vectorCanvas.gameObject);
		GameObject.Destroy(GameObject.Find("VectorCanvas3D"));
		switch(i)
		{
			case 0:
			{
				Application.LoadLevel("Playground");
			}
				break;
			case 1:
			{
				Application.LoadLevel("Tutorial");
			}
				break;
			case 2:
			{
				Application.LoadLevel("Title");
			}
				break;
			case 3:
				Application.Quit();
				break;
			break;
		}
	}

	private void AskSure()
	{
		SurenessPanel.SetActive (!SurenessPanel.activeSelf);
		SurenessPanel.transform.GetChild (0).GetComponent<Text> ().text = surenessText;
		sure = -1;
	}

	public void ShowObjectives()
	{
		ObjectivesBackground.SetActive (!ObjectivesBackground.activeSelf);
		imageSwap (ObjectivesButton.GetComponent<Image> ());
	}

	public void updateObjectiveText(int index)
	{
		if (objectivesInventory.IsDescribing ()) 
						DescriptionObjectives.GetComponent<Text> ().text = 
				objectivesInventory.objectivesList [10].writeObjective ();
		else
			Objectives [index].GetComponent<Text> ().text = 
				objectivesInventory.objectivesList[index].writeObjective ();
	}

	public void finishScavengerObjectives()
	{
		foreach(GameObject go in Objectives)
			go.SetActive(false);
		ObjectivesBackground.GetComponent<VerticalLayoutGroup> ().enabled = false;
		DescriptionObjectives.SetActive (true);
	}

	public void ShowBiologyInfo()
	{
		if(SceneSelectBackground.activeSelf)
			ShowSceneSelection();
		if(ObjectivesBackground.activeSelf)
			ShowObjectives();
		BioBackground.SetActive (!BioBackground.activeSelf);
		NodeButton.SetActive (!NodeButton.activeSelf);
		imageSwap (BioButton.GetComponent<Image> ());
	}

	public void monopodialPlantDescription()
	{
		modelIndex = 3;
		componentName = "Monopodial Growths";
		showPlantDescription ();
	}

	public void sympodialPlantDescription()
	{
		modelIndex = 8;
		componentName = "Sympodial Growths";
		showPlantDescription ();
	}

	public void rosettePlantDescription()
	{
		modelIndex = 6;
		componentName = "Rosette Growths";
		showPlantDescription ();
	}

	public void simplePlantDescription()
	{
		modelIndex = 7;
		componentName = "Simple Leaves";
		showPlantDescription ();
	}

	public void compoundPlantDescription()
	{
		modelIndex = 1;
		componentName = "Compound Leaves";
		showPlantDescription ();
	}

	public void palmatePlantDescription()
	{
		modelIndex = 5;
		componentName = "Palmate Leaves";
		showPlantDescription ();
	}

	public void actinomorphicPlantDescription()
	{
		modelIndex = 0;
		componentName = "Actinomorphic Flowers";
		showPlantDescription ();
	}

	public void zygomorphicPlantDescription()
	{
		modelIndex = 9;
		componentName = "Zygomorphic Flowers";
		showPlantDescription ();
	}

	public void infloresencePlantDescription()
	{
		modelIndex = 2;
		componentName = "Infloresences";
		showPlantDescription ();
	}

	public void nodeDescription()
	{
		modelIndex = 4;
		componentName = "Node";
		showPlantDescription ();
	}

	public void showPlantDescription()
	{ 
		if(tS != null)
		{
			if(!tS.tutorialPanel.activeSelf)
				tS.tutorialPanel.SetActive(true);
			else
				tS.tutorialPanel.SetActive(false);
		}
		else
			OverlayDisable ();

		if(BioBackground.activeSelf)
		{
			BioBackground.SetActive (!BioBackground.activeSelf);
			NodeButton.SetActive (!NodeButton.activeSelf);
			imageSwap(BioButton.GetComponent<Image>());
		}
		else
			Cursor.visible = true;

		BioInfoPanel.SetActive(!BioInfoPanel.activeSelf);

		modelCamera.GetComponent<Camera>().enabled = !modelCamera.GetComponent<Camera>().enabled;
		if(ObjectivesBackground.activeSelf)
			ShowObjectives();
		if(SceneSelectBackground.activeSelf)
			ShowSceneSelection();

		float anchorX = BioInfoPanel.GetComponent<RectTransform>().anchorMax.x - 
			BioInfoPanel.GetComponent<RectTransform>().anchorMin.x;
		float anchorY = BioInfoPanel.GetComponent<RectTransform>().anchorMax.y - 
			BioInfoPanel.GetComponent<RectTransform>().anchorMin.y;

		/*modelCamera.camera.rect = new Rect(2*BioInfoPanel.transform.position.x/Screen.width,
		               (BioInfoPanel.transform.position.y - (anchorY*Screen.height/2))/Screen.height, 
		                                   anchorX*Screen.width/Screen.width, 
		                                   anchorY*Screen.height/Screen.height);*/

		modelCamera.GetComponent<Camera>().rect = new Rect(2*BioInfoPanel.transform.position.x/Screen.width, 0, 
		                                   Screen.width - 2*BioInfoPanel.transform.position.x/Screen.width, 
		                                   Screen.height);

		//Well this is a problem
		if(demoModels[modelIndex].GetComponent<L_System>() != null)
		{
			demoModels[modelIndex].SetActive(true);
			if(demoModels[modelIndex].transform.GetChild (0).GetComponent<Turtle> ().destroyLines())
				demoModels[modelIndex].transform.GetChild (0).GetComponent<Turtle> ().cameraControl(Camera.main);
			else
				demoModels[modelIndex].transform.GetChild (0).GetComponent<Turtle> ().cameraControl(modelCamera.GetComponent<Camera>());
		}
		else
			demoModels[modelIndex].SetActive(!demoModels[modelIndex].activeSelf);

		changePanelInfo (componentName, demoText[modelIndex]);
	}

	private void changePanelInfo(string t, string d)
	{
		BioInfoPanel.transform.GetChild (0).GetComponent<Text> ().text = t;
		BioInfoPanel.transform.GetChild (1).GetComponent<Text> ().text = d;
	}

	private void imageSwap(Image i)
	{
		if(i.sprite.Equals(BioOpenBud))
			i.sprite = BioButtonBud;
		else if(i.sprite.Equals(BioButtonBud))
			i.sprite = BioOpenBud;

		if(i.sprite.Equals(ObjectivesOpenBud))
			i.sprite = ObjectivesButtonBud;
		else if(i.sprite.Equals(ObjectivesButtonBud))
			i.sprite = ObjectivesOpenBud;

		if(i.sprite.Equals(SceneOpenBud))
			i.sprite = SceneButtonBud;
		else if(i.sprite.Equals(SceneButtonBud))
			i.sprite = SceneOpenBud;
	}

	public void OverlayDisable()
	{
		turtle2D.destroyLines();
		BioButton.SetActive(!BioButton.activeSelf);
		ObjectivesButton.SetActive(!ObjectivesButton.activeSelf);
		SceneButton.SetActive(!SceneButton.activeSelf);
		if(ObjectivesBackground.activeSelf)
		{
			ObjectivesBackground.SetActive(false);
			imageSwap(ObjectivesButton.GetComponent<Image>());
		}
		if(SceneSelectBackground.activeSelf)
		{
			SceneSelectBackground.SetActive(false);
			imageSwap(SceneButton.GetComponent<Image>());
		}
		if(BioBackground.activeSelf)
		{
			imageSwap(BioButton.GetComponent<Image>());
			BioBackground.SetActive(false);
			NodeButton.SetActive(false);
		}
		//else
		Cursor.visible = !Cursor.visible;
	}

	public bool overlayActive()
	{
		return BioButton.activeSelf;
	}
}
