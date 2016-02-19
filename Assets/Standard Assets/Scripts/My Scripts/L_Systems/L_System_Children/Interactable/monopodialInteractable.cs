using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class monopodialInteractable : Parametric_L_System {

	void Awake()
	{
		Leaf = (GameObject)Resources.Load ("Prefabs/L_Systems/Leaves/simpleLeaf3") as GameObject;
		leafGrowth = 0.1f;
		
		Flower = (GameObject)Resources.Load ("Prefabs/L_Systems/Flowers/flower4") as GameObject;
		flowerGrowth = 0.1f;
	}
	// Use this for initialization
	void Start () 
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();

		segmentGrowth = 0.45f;

		axlGrowth = 6.0f;

		widthGrowth = 0.15f;

		divergenceAngle = 60.0f;

		maturity = 5.5f;

		numOfProductions = 3;
		productionLengths = new int[numOfProductions];

		productionLengths [0] = 18;
		productionLengths [1] = 11;
		productionLengths [2] = 10;

		returnList.Add (new Module ('!', 0, 1, widthGrowth));
		returnList.Add (new Module ('F', 0, 1, segmentGrowth));
		returnList.Add (new Module ('a', 0, 1, 0));

		productions.Add ('a', new Module [productionLengths[0]]);
		productions ['a'][0] = (new Module ('[', 0, -1, 0));
		productions ['a'][1] = (new Module ('N', 0, -1, 0));
		productions ['a'][2] = (new Module ('&', 0, 1, axlGrowth));
		productions ['a'][3] = (new Module ('!', 0, 1, widthGrowth));
		productions ['a'][4] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['a'][5] = (new Module ('b', 0, 1, 0));
		productions ['a'][6] = (new Module (']', 0, -1, 0));
		productions ['a'][7] = (new Module ('+', 0, 1, divergenceAngle));
		productions ['a'][8] = (new Module ('L', 0, 1, leafGrowth));
		productions ['a'][9] = (new Module ('[', 0, -1, 0));
		productions ['a'][10] = (new Module ('N', 0, -1, 0));
		productions ['a'][11] = (new Module ('^', 0, 1, axlGrowth));
		productions ['a'][12] = (new Module ('!', 0, 1, widthGrowth));
		productions ['a'][13] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['a'][14] = (new Module ('c', 0, 1, 0));
		productions ['a'][15] = (new Module (']', 0, -1, 0));
		productions ['a'][16] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['a'][17] = (new Module ('a', 0, 1, 0));

		productions.Add ('b', new Module[productionLengths[1]]);
		productions ['b'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions ['b'][1] = (new Module ('[', 0, -1, 0));
		productions ['b'][2] = (new Module ('N', 0, -1, 0));
		productions ['b'][3] = (new Module ('^', 0, 1, axlGrowth));
		productions ['b'][4] = (new Module ('L', 0, 1, leafGrowth));
		productions ['b'][5] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['b'][6] = (new Module ('c', 0, 1, 0));
		productions ['b'][7] = (new Module (']', 0, -1, 0));
		productions ['b'][8] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['b'][9] = (new Module ('L', 0, 1, leafGrowth));
		productions ['b'][10] = (new Module ('c', 0, 1, 0));

		productions.Add ('c', new Module[productionLengths[2]]);
		productions ['c'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions ['c'][1] = (new Module ('[', 0, -1, 0));
		productions ['c'][2] = (new Module ('N', 0, -1, 0));
		productions ['c'][3] = (new Module ('*', 0, 1, axlGrowth));
		productions ['c'][4] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['c'][5] = (new Module ('b', 0, 1, 0));
		productions ['c'][6] = (new Module (']', 0, -1, 0));
		productions ['c'][7] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['c'][8] = (new Module ('b', 0, 1, 0));
		productions ['c'][9] = (new Module ('I', 0, 1, flowerGrowth));
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
