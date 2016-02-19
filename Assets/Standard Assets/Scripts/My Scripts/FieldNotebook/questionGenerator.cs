using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public static class questionGenerator {

	public static GameObject questionPanel;

	private static Object[] selectionImages;
	private static Sprite[] selectionSprites;
	public static GameObject selectionImage;
	
	private static GameObject questionWindow;
	private static GameObject answerToggle1, answerToggle2, answerToggle3, answerToggle4;
	private static GameObject submitButton, questionTitle;

	private static GameObject answerGroup;

	private static string question = "How would you classify this part?";
	private static string answer;
	private static string[] answers;

	private static bool correctlyAnswered = false;
	private static bool answered = false;

	private static string[] possibleAnswers = {"Node", "Monopodial", "Sympodial", "Rosette",
													"Simple", "Compound", "Palmate", 
														"Actinomorphic", "Zygomorphic", "Inflorescence"};
	private static int numPossibleAnswers = 10;

	public static GameObject[] Toggles;
	public static int ToggleIndex = 0;
	// Use this for initialization
	public static void Awake () 
	{
		questionPanel = (GameObject)UnityEngine.Object.Instantiate(Resources.Load ("Prefabs/UI/UIPanel"));
		questionPanel.AddComponent<GraphicRaycaster> ();
		questionPanel.AddComponent<CanvasScaler> ();
		questionPanel.GetComponent<Image> ().enabled = false;
		questionPanel.GetComponent<CanvasScaler> ().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		questionPanel.GetComponent<CanvasScaler> ().referenceResolution = new Vector2 (1537, 910);
		questionPanel.GetComponent<CanvasScaler> ().matchWidthOrHeight = 0.5f;

		selectionSprites = new Sprite[4];
		selectionImages = Resources.LoadAll ("Textures/selectionImages");

		selectionSprites [0] = selectionImages [5] as Sprite;
		selectionSprites [1] = selectionImages [7] as Sprite;
		selectionSprites [2] = selectionImages [3] as Sprite;
		selectionSprites [3] = selectionImages [1] as Sprite;

		selectionImage = UnityEngine.Object.Instantiate(Resources.Load ("Prefabs/UI/SelectionImage")) as GameObject;
		selectionImage.transform.SetParent (questionPanel.transform, false);

		questionWindow = UnityEngine.Object.Instantiate(Resources.Load ("Prefabs/UI/QuestionPanel/Panel")) as GameObject;
		questionWindow.transform.SetParent (questionPanel.transform, false);
		answerToggle1 = questionWindow.transform.GetChild (0).gameObject;
		answerToggle2 = questionWindow.transform.GetChild (1).gameObject;
		answerToggle3 = questionWindow.transform.GetChild (2).gameObject;
		answerToggle4 = questionWindow.transform.GetChild (3).gameObject;

		answerGroup = new GameObject ();
		answerGroup.AddComponent<ToggleGroup> ();
		ToggleGroup group = answerGroup.GetComponent<ToggleGroup> ();
		group.allowSwitchOff = true;

		answerToggle1.GetComponent<Toggle> ().group = group;
		answerToggle2.GetComponent<Toggle> ().group = group;
		answerToggle3.GetComponent<Toggle> ().group = group;
		answerToggle4.GetComponent<Toggle> ().group = group;

		Toggles = new GameObject[4];
		Toggles [0] = answerToggle1; Toggles [1] = answerToggle2;
		Toggles [2] = answerToggle3; Toggles [3] = answerToggle4;

		answers = new string[4];

		submitButton = questionWindow.transform.GetChild (4).gameObject;
		submitButton.GetComponentInChildren<Text> ().text = "Identify";
		submitButton.GetComponent<Button>().onClick.AddListener(submitAnswer);
		submitButton.AddComponent<Outline> ().effectColor = Color.green;
		submitButton.GetComponent<Outline> ().effectDistance = Vector2.one * 3;

		questionTitle = questionWindow.transform.GetChild (5).gameObject;
		questionTitle.GetComponent<Text>().text = question;

		questionWindow.SetActive (false);
		toggleImage();
	}
	
	private static void generateAndUpdateAnswers()
	{
		for(int i = 0; i < 4; i++)	
		{ 								
			answers[i] = answer;    		
			while(answers[i] == answer)
			{
				answers[i] = possibleAnswers[Random.Range(0, numPossibleAnswers)];	//Choose a random answer
				int j = i-1;
				while(j > -1)
				{
					if(answers[i] == answers[j])	//Ensure no duplicates among answers
						answers[i] = answer;
					j--;
				}
			}								
		}										
		answers [Random.Range (0, 4)] = answer;

		answerToggle1.transform.GetChild (1).GetComponent<Text> ().text = answers [0];
		answerToggle1.GetComponent<Toggle> ().isOn = false;

		answerToggle2.transform.GetChild (1).GetComponent<Text> ().text = answers [1];
		answerToggle2.GetComponent<Toggle> ().isOn = false;

		answerToggle3.transform.GetChild (1).GetComponent<Text> ().text = answers [2];
		answerToggle3.GetComponent<Toggle> ().isOn = false;

		answerToggle4.transform.GetChild (1).GetComponent<Text> ().text = answers [3];
		answerToggle4.GetComponent<Toggle> ().isOn = false;
	}

	public static void createQuestion(string ans)
	{
		questionWindow.SetActive (true);
		answer = ans;
		correctlyAnswered = false;
		generateAndUpdateAnswers ();
		Cursor.visible = true;
	}

	public static void submitAnswer()
	{
		if(answerGroup.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault() != null)
		{
			if(answerGroup.GetComponent<ToggleGroup>().ActiveToggles().			//An unlisted property of ToggleGroup, ActiveToggles().
			   	FirstOrDefault().GetComponentInChildren<Text>().text == answer)	//We use FirstOrDefault to return the first or default
			{
				questionWindow.GetComponent<Image>().color = Color.green;
				correctlyAnswered = true;										//member of a given collection. By the nature of Toggle
			}
			else 
			{
				questionWindow.GetComponent<Image>().color = Color.red;			//Groups, this will be the active toggle
			}
			Cursor.visible = false;
			answered = true;
		}
	}

	public static bool answerResults()
	{
		return correctlyAnswered;
	}

	public static bool questioning()
	{
		return questionWindow.activeSelf;
	}

	public static bool replied()
	{
		return answered;
	}

	public static void clearQuestion()
	{
		correctlyAnswered = false;
		answered = false;
		questionWindow.GetComponent<Image> ().color = Color.white;
		questionWindow.SetActive (false);
	}

	public static void cycleImage(int i)
	{
		selectionImage.GetComponent<Image>().sprite = selectionSprites[i];
	}

	public static void toggleImage()
	{
		selectionImage.SetActive (!selectionImage.activeSelf);
	}
}
