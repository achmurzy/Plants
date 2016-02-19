using UnityEngine;
using System.Collections;

public static class objectivesInventory
{
	public static fieldNotebook notebook;

	private static int startingCount = 0;
	private static int completedCount = 1;

	private static Objective node;

	private static Objective monopodial;
	private static Objective sympodial;
	private static Objective rosette;
	
	private static Objective simpleLeaf;
	private static Objective compoundLeaf;
	private static Objective palmateLeaf;

	private static Objective actinomorphicFlower;
	private static Objective zygomorphicFlower;
	private static Objective infloresence;

	private static Objective descriptionExercise;

	public static Objective[] objectivesList;

	private static Object[] possibleLsystems;
	private static GameObject descriptionLSystem;

	private static BoxCollider dropOffZone = new BoxCollider();

	private static bool formsDone, leavesDone, flowersDone, describing;	//We can use the 'describing' boolean
																	//to activate input semantics in the
	// Use this for initialization										selection script for L_System storage
	public static void Start () 
	{
		objectivesList = new Objective[11];

		node = new Objective (0, 0, 5, "Nodes Identified: ");
		objectivesList [0] = node;

		monopodial = new Objective (1, startingCount, completedCount, "Monopodial Growths Identified: ");
		objectivesList [1] = monopodial;

		sympodial = new Objective (2, startingCount, completedCount, "Sympodial Growths Identified: ");
		objectivesList [2] = sympodial;

		rosette = new Objective (3, startingCount, completedCount, "Rosette Growths Identified: ");
		objectivesList [3] = rosette;
		
		simpleLeaf = new Objective (4, startingCount, completedCount, "Simple Leaves Identified: ");
		objectivesList [4] = simpleLeaf;

		compoundLeaf = new Objective (5, startingCount, completedCount, "Compound Leaves Identified: ");
		objectivesList [5] = compoundLeaf;

		palmateLeaf = new Objective (6, startingCount, completedCount, "Palmate Leaves Identified: ");
		objectivesList [6] = palmateLeaf;

		actinomorphicFlower = new Objective (7, startingCount, completedCount, "Actinomorphic Flowers Identified: ");
		objectivesList [7] = actinomorphicFlower;

		zygomorphicFlower = new Objective (8, startingCount, completedCount, "Zygomorphic Flowers Identified: ");
		objectivesList [8] = zygomorphicFlower;

		infloresence = new Objective (9, startingCount, completedCount, "Infloresences Identified: ");
		objectivesList [9] = infloresence;

		descriptionExercise = new Objective (10, 0, 1, "");
		objectivesList [10] = descriptionExercise;

		possibleLsystems = Resources.LoadAll ("Prefabs/L_Systems/Interactable");

		formsDone = false;
		leavesDone = false;
		flowersDone = false;
		describing = false;
	}

	public static void addGrowthForm(string kind)
	{
		if(!formsDone)
		{
			if(kind == "Monopodial")
			{
				if(!monopodial.checkCompleted())
					monopodial.AddItem();
			}
			else if(kind == "Sympodial")
			{
				if(!sympodial.checkCompleted())
					sympodial.AddItem();
			}
			else if(kind == "Rosette")
			{
				if(!rosette.checkCompleted())
					rosette.AddItem();
			}
			else if(kind == "Node")
			{
				if(!node.checkCompleted())
					node.AddItem();
			}
			if(monopodial.checkCompleted() && sympodial.checkCompleted() &&
			   	rosette.checkCompleted() && node.checkCompleted())
				formsDone = true;

			finishScavengerHunt();
		}
	}

	public static void addLeaf(string kind)
	{
		if(!leavesDone)
		{
			if(kind == "Simple")
			{
				if(!simpleLeaf.checkCompleted())
					simpleLeaf.AddItem();
			}
			else if(kind == "Palmate")
			{
				if(!palmateLeaf.checkCompleted())
					palmateLeaf.AddItem();
			}
			else if(kind == "Compound")
			{
				if(!compoundLeaf.checkCompleted())
					compoundLeaf.AddItem();
			}
			if(simpleLeaf.checkCompleted() && palmateLeaf.checkCompleted() && compoundLeaf.checkCompleted())
				leavesDone = true;
			finishScavengerHunt();
		}
	}

	public static void addFlower(string kind)
	{
		if(!flowersDone)
		{
			if(kind == "Actinomorphic")
			{
				if(!actinomorphicFlower.checkCompleted())
					actinomorphicFlower.AddItem();
			}
			else if(kind == "Zygomorphic")
			{
				if(!zygomorphicFlower.checkCompleted())
					zygomorphicFlower.AddItem();
			}
			else if(kind == "Inflorescence")
			{
				if(!infloresence.checkCompleted())
					infloresence.AddItem();
			}
			if(actinomorphicFlower.checkCompleted() &&
			   		zygomorphicFlower.checkCompleted() &&
			   			infloresence.checkCompleted())
				flowersDone = true;
			finishScavengerHunt();
		}
	}

	public static void finishScavengerHunt()
	{
		if(formsDone && flowersDone && leavesDone)
		{
			describing = true;
			notebook.finishScavengerObjectives();
			startNewDescriptionExercise();
		}
	}

	private static void startNewDescriptionExercise()
	{
		int systemIndex;
		systemIndex = Random.Range(0, possibleLsystems.Length);
		descriptionLSystem = UnityEngine.Object.Instantiate(possibleLsystems[systemIndex]) as GameObject;
		/*if(descriptionExercise.getCount() == 0)
		{													//We need to control where these systems spawn
			while(descriptionLSystem.tag != "Monopodial")	//AREA I
			{
				GameObject.Destroy(descriptionLSystem);
				systemIndex = Random.Range(0, possibleLsystems.Length);
				descriptionLSystem = UnityEngine.Object.Instantiate(possibleLsystems[systemIndex]) as GameObject;
			}
		}
		else if(descriptionExercise.getCount() == 1)
		{
			while(descriptionLSystem.tag != "Sympodial")	//AREA II
			{
				GameObject.Destroy(descriptionLSystem);
				systemIndex = Random.Range(0, possibleLsystems.Length-1);
				descriptionLSystem = UnityEngine.Object.Instantiate(possibleLsystems[systemIndex]) as GameObject;
			}
		}
		else
		{
			while(descriptionLSystem.tag != "Rosette")		//AREA III
			{
				GameObject.Destroy(descriptionLSystem);
				systemIndex = Random.Range(0, possibleLsystems.Length);
				descriptionLSystem = UnityEngine.Object.Instantiate(possibleLsystems[systemIndex]) as GameObject;
			}
		}*/
		changeDescriptiveObjective (descriptionLSystem.tag, 
		                           descriptionLSystem.GetComponent<Parametric_L_System> ().sendLeaf ().tag,
		                           descriptionLSystem.GetComponent<Parametric_L_System> ().sendFlower ().tag);
		notebook.updateObjectiveText (10);
	}

	public static void changeDescriptiveObjective(string form, string leaf, string flower)
	{
		descriptionExercise.changeDescription("Find a plant with a " + form + " growth form and " + leaf +
		                                      " leaves, as well as flowers of type " + flower + "\nUsing the" +
		                                      "<color=cyan>CYAN</color> selection beam take it to the pylons" +
		                                      "marked by the yellow emblem near the city's entrance.");
	}

	//Check if the argument's position is within the statically defined drop-off location
	//Simply, the drop off area is a rectangle
	public static bool inDropOffArea(GameObject systemPos)
	{
		if(dropOffZone.bounds.Contains(systemPos.transform.position))
			return true;
		return false;
	}
	//This method takes whatever L_system is in the player's inventory and check its against the descriptive
	//objective. This method should be called under input at the specified "drop-off" point.
	public static bool checkDescriptiveObjective(GameObject systemObj)
	{	
		Parametric_L_System plsys = systemObj.GetComponent<Parametric_L_System> ();
		if(plsys.tag == descriptionLSystem.tag &&
		   plsys.sendLeaf().tag == descriptionLSystem.GetComponent<Parametric_L_System> ().sendLeaf ().tag &&
		   plsys.sendFlower().tag == descriptionLSystem.GetComponent<Parametric_L_System> ().sendFlower ().tag)
		{
			descriptionExercise.AddItem();
			if(!descriptionExercise.checkCompleted())
			{
				startNewDescriptionExercise();
				return true;
			}
			else
			{
				Application.LoadLevel("Title");	//End the game here.
				return true;
			}
		}
		return false;
	}

	public static bool IsDescribing()
	{
		return describing;
	}

	public class Objective
	{
		private int index;
		private float count, max;
		private bool completed;
		private string description;

		public Objective(int i, float c, float m, string d)
		{
			index = i;
			count = c;
			max = m;
			description = d;
			completed = false;
		}

		public float getCount()
		{return count;}
		
		public void AddItem()
		{
			count++; 
			notebook.updateObjectiveText (index); 
			return;
		}

		public string writeObjective()
		{return description + count + " / " + max;}

		public void changeDescription(string s)
		{
			description = s;
		}

		public bool checkCompleted()
		{	
			if(completed)
				return true;
			else if(count >= max)
				completed = true;
			return completed;
		}
	}

	public static void makeDescribing()
	{
		describing = !describing;
	}

	public static void SetDropOffZone(BoxCollider b)
	{
		dropOffZone = b;
	}
}
