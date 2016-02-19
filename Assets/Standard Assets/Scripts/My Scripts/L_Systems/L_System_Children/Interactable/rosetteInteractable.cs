using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class rosetteInteractable : Parametric_L_System {

	void Awake()
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();
		Flower = Resources.Load ("Prefabs/L_Systems/Flowers/flower1") as GameObject;
		Leaf = Resources.Load ("Prefabs/L_Systems/Leaves/compoundLeaf3") as GameObject;
	}
	// Use this for initialization
	void Start () 
	{
		divergenceAngle = 60.0f;
		segmentGrowth = 0.75f;
		widthGrowth = 0.15f;

		leafGrowth = 0.025f;

		flowerGrowth = 0.1f;

		axlGrowth = 5.0f;

		maturity = 6.0f;

		numOfProductions = 1;
		productionLengths = new int[numOfProductions];

		productionLengths [0] = 6;

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, 45));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('&', 0, 1, axlGrowth));
		returnList.Add (new Module ('L', 0, 1, leafGrowth));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, 15));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, divergenceAngle));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, 15));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('&', 0, 1, axlGrowth));
		returnList.Add (new Module ('L', 0, 1, leafGrowth));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, 45));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, 45));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('&', 0, 1, axlGrowth));
		returnList.Add (new Module ('L', 0, 1, leafGrowth));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, 15));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, divergenceAngle));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, 15));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('&', 0, 1, axlGrowth));
		returnList.Add (new Module ('L', 0, 1, leafGrowth));
		returnList.Add (new Module (']', 0, -1, 0));


		returnList.Add (new Module ('F', 0, -1, segmentGrowth));
		returnList.Add (new Module ('N', 0, -1, 0));
		returnList.Add (new Module ('I', 0, -1, flowerGrowth));


		productions.Add ('a', new Module[productionLengths[0]]);
		productions ['a'][0] = (new Module ('a', 0, 1, 0));
		productions ['a'][1] = (new Module ('!', 0, 1, widthGrowth));
		productions ['a'][2] = (new Module ('[', 0, -1, 0));
		productions ['a'][3] = (new Module ('&', 0, 1, axlGrowth));
		productions ['a'][4] = (new Module ('L', 0, 1, leafGrowth));
		productions ['a'][5] = (new Module (']', 0, -1, 0));
	}
	
	// Update is called once per frame
	void Update () 
	{
		base.Update ();
	}

	public override float growthFunction(char sym, float gp, float time)
	{
		switch(sym)
		{
			case '+':
				return gp;
			default:
				return gp*Mathf.Exp(exponentialParameter*time);
		}
	}
}
