using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class tutorialScript : MonoBehaviour {

	private swirlingJumpController sJC;		//Its all here baby
	private swirlingCamera sC;
	private fieldNotebook fNB;

	private bool informationReadAndMovement = false;
	private float movement = 0;
	private float movementCap = 5.0f;
	private float rotationCap = 2.0f;

	private bool movementControls = false;
	public GameObject movementGoal;
	public GameObject placementGoal;
	private GameObject placementSystem;

	private bool selectionLesson = false;
	private bool tabulated = false;
	private bool nodeLesson = false;
	private bool nodeSelected = false;
	private bool formLesson = false;
	private bool leafLesson = false;
	private bool flowerLesson = false;
	private bool plantGot = false;
	private bool plantSet = false;
	private bool done = false;

	private bool monoGrown = false;
	private bool monoSelected = false;
	private bool symGrown = false;
	private bool symSelected = false;
	private bool rosetteGrown = false;
	private bool rosetteSelected = false;
	private bool systemsGrown = false;

	private bool funRockTriggered = false;	//They know how to swirl

	private bool systemsReached = false;

	private bool waterFull = false;

	private bool selecting = false;

	private bool guiShow = false;

	private bool objectivesChecked;
	private bool scenesChecked;
	private bool biologyChecked;
	
	public GameObject System1, System2, System3, waterPoolTrigger, funRockTrigger, systemsTrigger;
	private Parametric_L_System monopodialSystem, sympodialSystem, rosetteSystem;
	public GameObject tutorialPanel;
	private GameObject tutorialTip;
	private GameObject tutorialContinue;
	private GameObject nextButton, prevButton;

	private Object[] tutorialText;
	private string[] tips;
	private int tipIndex = 0;
	private int tutorialProgressIndex = 1;

	void Awake()
	{
		sJC = this.GetComponent<swirlingJumpController> ();
		sC = this.GetComponent<swirlingCamera> ();
		fNB = this.GetComponent<fieldNotebook> ();
		sJC.gameObject.GetComponentInChildren<selectionLine> ().InTutorial ();
	}

	void Start () 
	{
		objectivesInventory.SetDropOffZone(placementGoal.GetComponent<BoxCollider>());

		monopodialSystem = System1.GetComponent<Parametric_L_System> ();
		sympodialSystem = System2.GetComponent<Parametric_L_System> ();
		rosetteSystem = System3.GetComponent<Parametric_L_System> ();

		tutorialPanel = Instantiate(Resources.Load ("Prefabs/UI/Tutorial/TutorialPanel")) as GameObject;
		tutorialPanel.transform.SetParent (questionGenerator.questionPanel.transform, false);

		tutorialText = Resources.LoadAll ("Prefabs/UI/Tutorial/TutorialText");

		tips = new string[tutorialText.Length];
		for(int i = 0; i < tips.Length; i++)
		{
			TextAsset ta = Instantiate(tutorialText[i]) as TextAsset;
			tips[i] = ta.text;
		}

		nextButton = tutorialPanel.transform.GetChild (0).gameObject;
		nextButton.GetComponent<Button> ().onClick.AddListener (nextTutorialTip);

		tutorialTip = tutorialPanel.transform.GetChild(1).gameObject;
		tutorialTip.GetComponent<Text>().text = tips[tipIndex];

		tutorialContinue = nextButton.transform.GetChild(0).gameObject;
		tutorialContinue.SetActive (false);



		prevButton = tutorialPanel.transform.GetChild (2).gameObject;
		prevButton.GetComponent<Button> ().onClick.AddListener (prevTutorialTip);

		sJC.killSwirl (true);
		sJC.killSelect (true);

		fNB.OverlayDisable ();

		waterPoolTrigger.SetActive (false);
		funRockTrigger.SetActive (false);

		tutorialProgress ();
		prevTutorialTip ();

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Triangle"))
			nextTutorialTip();
		if(Input.GetButtonDown ("Circle") && !done)
			prevTutorialTip();

		Cursor.visible = true;
		if(!movementControls)
			sJC.killSwirl(true);

		if(!informationReadAndMovement)
		{
			checkOutset();
		}

		//Check movement controls
		else if(!movementControls)
		{
			sC.updateFocus(movementGoal.transform, 10, 5);
		}

		if(!systemsReached)
			sJC.waterLevel = 100;

		if(sJC.waterLevel > 90 && systemsReached && !waterFull)
			plantTime();

		//Check plant growth
		if(!systemsGrown)
		{
			checkGrowthUpdate();
		}

		if(checkGUIKnowledge() && systemsGrown)
		{
			done = true;
		}
	}

	void checkOutset()
	{
		if(sJC.isMoving)
			movement+=Time.deltaTime;
		if(tipIndex != 0 && movement > movementCap)
		{
			informationReadAndMovement = true;
			tutorialProgressIndex++;
			tutorialProgress();
		}
	}

	void checkMovementUpdate()
	{
		movementControls = true;
		tutorialProgressIndex++;
		tutorialProgress();
		sJC.killSwirl (false);
		movementGoal.GetComponent<BoxCollider> ().enabled = false;
		waterPoolTrigger.SetActive (true);
		funRockTrigger.SetActive (true);
	}
	
	void checkSwirlingUpdate()
	{
		waterPoolTrigger.SetActive (false);
		sC.updateFocus(funRockTrigger.transform, 10, 5);
		funRockTriggered = true;
		tutorialProgressIndex++;
		tutorialProgress();
	}

	void depriveWater()
	{
		sC.cancelFocus ();
		if(!systemsReached)
		{
			sJC.waterLevel = 0;
			tutorialProgressIndex++;
			tutorialProgress();
			systemsReached = true;
		}
	}

	void plantTime()
	{
		waterFull = true;
		sC.updateFocus(monopodialSystem.transform, 10, 5);
		sympodialSystem.gameObject.SetActive (false);
		rosetteSystem.gameObject.SetActive (false);
		tutorialProgressIndex++;
		tutorialProgress();
	}

	void smallSelectionTutorial(Parametric_L_System sys)
	{
		if(!tabulated && sJC.IsSelecting())
		{
			tabulated = true;
			tutorialTip.GetComponent<Text>().text = tips[tipIndex];
			tutorialProgressIndex = 9;
			tutorialProgress();
			tipIndex = 8;
			tutorialTip.GetComponent<Text>().text = tips[tipIndex];
			tutorialContinue.SetActive (true);
			nextButton.SetActive (true);
		}
		if(!nodeLesson && nodeSelected)
		{
			nodeLesson = true;
		}
		else if(!formLesson && sys.formSelected && nodeSelected)
		{
			formLesson = true;
			sJC.gameObject.GetComponentInChildren<selectionLine> ().cycleMode ();
			tutorialProgressIndex++;
			tutorialProgress();
		}
		if(!leafLesson && sys.leavesSelected)
		{
			leafLesson = true;
			sJC.gameObject.GetComponentInChildren<selectionLine> ().cycleMode ();
			tutorialProgressIndex++;
			tutorialProgress();
		}
		if(!flowerLesson && sys.flowersSelected)
		{
			flowerLesson = true;
			objectivesInventory.makeDescribing();
			sJC.gameObject.GetComponentInChildren<selectionLine> ().cycleMode ();
			tutorialProgressIndex++;
			tutorialProgress();
			//sJC.gameObject.GetComponentInChildren<selectionLine> ().plsLast = null;
		}
		if(!plantSet && objectivesInventory.IsDescribing())
		{
			if(sJC.gameObject.GetComponentInChildren<selectionLine> ().descriptionObjective != null)
			{
				if(sJC.gameObject.GetComponentInChildren<selectionLine> ()
				   	.descriptionObjective.GetComponent<Parametric_L_System>() == sys)
				{
					placementSystem = sJC.gameObject.GetComponentInChildren<selectionLine> ().descriptionObjective; 
				}
			}
			else if(placementSystem != null)
				if(objectivesInventory.inDropOffArea(placementSystem))
				{
					plantGot = true;
				}
			if(plantGot)
			{
					plantSet = true;
			}
		}
		if(!selectionLesson && formLesson && leafLesson && flowerLesson && plantSet)
		{
			selectionLesson = true;
			objectivesInventory.makeDescribing();
			sJC.gameObject.GetComponentInChildren<selectionLine> ().cycleMode ();
			//displayLesson(sys.gameObject.tag);
			tabulated = false;
			nodeLesson = false;
			nodeSelected = false;
			formLesson = false;
			leafLesson = false;
			flowerLesson = false;
			plantGot = false;
			plantSet = false;
			placementSystem = null;
			sJC.killSelect(false);
		}
	}

	void checkGrowthUpdate()
	{
		if(monopodialSystem.stopAnimating())
		{
			if(!monoGrown)
			{
				tutorialProgressIndex++;
				tutorialProgress();
				monoGrown = true;
				selecting = true;
				sJC.killSelect (false);
				selectionLesson = false;
			}
			if(!selectionLesson && !monoSelected)
			{
				if(sJC.IsSelecting())
					sJC.killSelect(true);
				smallSelectionTutorial(monopodialSystem);
			}
			if(checkBotanicalKnowledge(monopodialSystem))
			{
				if(!monoSelected && selectionLesson)
				{
					monoSelected = true;
					tutorialProgressIndex++;
					tutorialProgress();
					sympodialSystem.gameObject.SetActive(true);
				}
				if(sympodialSystem.stopAnimating())
				{
					if(!symGrown)
					{
						symGrown = true;
						selecting = true;
						sJC.killSelect (false);
						selectionLesson = false;
					}
					if(!selectionLesson && !symSelected)
					{
						if(sJC.IsSelecting())
							sJC.killSelect(true);
						smallSelectionTutorial(sympodialSystem);
					}
					if(checkBotanicalKnowledge(sympodialSystem))
					{
						if(!symSelected && selectionLesson)
						{
							symSelected = true;
							tutorialProgressIndex++;
							tutorialProgress();
							rosetteSystem.gameObject.SetActive(true);
						}

						if(rosetteSystem.stopAnimating())
						{
							if(!rosetteGrown)
							{
								rosetteGrown = true;
								selecting = true;
								sJC.killSelect (false);
								selectionLesson = false;
							}
							if(!selectionLesson && !systemsGrown)
							{
								if(sJC.IsSelecting())
									sJC.killSelect(true);
								smallSelectionTutorial(rosetteSystem);
							}
							if(checkBotanicalKnowledge(rosetteSystem))
							{
								if(!systemsGrown && selectionLesson)
								{
									systemsGrown = true;
									tutorialProgressIndex++;
									tutorialProgressIndex++;
									tutorialProgress();
									fNB.OverlayDisable();
								}
							}
						}
					}
				}
			}
		}
	}

	bool checkBotanicalKnowledge(Parametric_L_System pLS)
	{
		if(pLS.leavesSelected == true && pLS.flowersSelected == true && pLS.formSelected == true)
		{
			return true;
		}
		else 
			return false;	
	}

	bool checkGUIKnowledge()
	{
		if(fNB.BioBackground.activeSelf)
			biologyChecked = true;
		if(fNB.ObjectivesBackground.activeSelf)
			objectivesChecked = true;
		if(fNB.SceneSelectBackground.activeSelf)
			scenesChecked = true;

		if(biologyChecked && objectivesChecked && scenesChecked)
			return true;
		else 
			return false;
	}

	void tutorialProgress()
	{
			nextButton.SetActive(false);
			tutorialContinue.SetActive(false);
		tipIndex = tutorialProgressIndex;
		tutorialTip.GetComponent<Text>().text = tips[tipIndex];
	}

	void nextTutorialTip()
	{
		tipIndex++;
		if(tipIndex < tutorialProgressIndex)
		{
			tutorialContinue.SetActive (true);
			nextButton.SetActive (true);
		}
		else if(tipIndex == tutorialProgressIndex)
		{
			nextButton.SetActive(false);
			tutorialContinue.SetActive(false);
		}
		else
		{
			tipIndex--;
			nextButton.SetActive(false);
			tutorialContinue.SetActive(false);
		}
		tutorialTip.GetComponent<Text>().text = tips[tipIndex];
		prevButton.SetActive (true);
	}

	void prevTutorialTip()
	{
		tipIndex--;
		if(tipIndex > -1)
		{
			tutorialContinue.SetActive (true);
			nextButton.SetActive (true);
			tutorialTip.GetComponent<Text>().text = tips[tipIndex];
			if(tipIndex == 0)
				prevButton.SetActive(false);
		}
		else
			tipIndex++;
	}

	void displayLesson(string plsTag)
	{
		if(plsTag == "Monopodial")
		{
			fNB.monopodialPlantDescription();
		}
		else if(plsTag == "Sympodial")
		{
			fNB.sympodialPlantDescription();
		}
		else if(plsTag == "Rosette")
		{
			fNB.rosettePlantDescription();
		}
		else if(plsTag == "Actinomorphic")
		{
			fNB.actinomorphicPlantDescription();
		}
		else if(plsTag == "Zygomorphic")
		{
			fNB.zygomorphicPlantDescription();
		}
		else if(plsTag == "Inflorescence")
		{
			fNB.infloresencePlantDescription();
		}
		else if(plsTag == "Simple")
		{
			fNB.simplePlantDescription();
		}
		else if(plsTag == "Compound")
		{
			fNB.compoundPlantDescription();
		}
		else if(plsTag == "Palmate")
		{
			fNB.palmatePlantDescription();
		}
		else if(plsTag == "Node")
		{
			fNB.nodeDescription();
		}
	}

	void getNode()
	{
		nodeSelected = true;
	}
}
