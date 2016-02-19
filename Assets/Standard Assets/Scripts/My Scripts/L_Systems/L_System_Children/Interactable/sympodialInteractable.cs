using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class sympodialInteractable : Parametric_L_System {

	void Awake()
	{
		Leaf = Resources.Load ("Prefabs/L_Systems/Leaves/simpleLeaf3") as GameObject;
		leafGrowth = 0.075f;
		
		Flower = Resources.Load ("Prefabs/L_Systems/Flowers/flower2") as GameObject;
		flowerGrowth = 0.05f;
		}
	// Use this for initialization
	void Start () 
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();

		axlGrowth = 2.75f;
		divergenceAngle = 180;


		segmentGrowth = 0.95f;

		widthGrowth = 0.45f;

		maturity = 6.0f;

		numOfProductions = 2;
		productionLengths = new int[numOfProductions];
		
		productionLengths [0] = 17;
		productionLengths [1] = 15;

		returnList.Add (new Module ('!', 0, 1, widthGrowth));
		//returnList.Add (new Module ('F', 0, 1, initialSegmentLength));
		returnList.Add (new Module ('a', 0, 1, 0));

		productions.Add ('a', new Module [productionLengths[0]]);
		productions ['a'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions ['a'][1] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['a'][2] = (new Module ('[', 0, -1, 0));
		productions ['a'][3] = (new Module ('%', 0, 1, axlGrowth));
		productions ['a'][4] = (new Module ('^', 0, 1, axlGrowth));
		productions ['a'][5] = (new Module ('a', 0, 1, 0));
		productions ['a'][6] = (new Module (']', 0, -1, 0));
		productions ['a'][7] = (new Module ('[', 0, -1, 0));
		productions ['a'][8] = (new Module ('N', 0, -1, 0));
		productions ['a'][9] = (new Module ('&', 0, 1, axlGrowth));
		productions ['a'][10] = (new Module ('+', 0, 1, divergenceAngle));
		productions ['a'][11] = (new Module ('L', 0, 1, leafGrowth));
		productions ['a'][12] = (new Module ('+', 0, 1, -divergenceAngle));
		productions ['a'][13] = (new Module ('b', 0, 1, 0));
		productions ['a'][14] = (new Module (']', 0, -1, 0));
		productions ['a'][15] = (new Module ('*', 0, 1, axlGrowth));
		productions ['a'][16] = (new Module ('b', 0, 1, 0));

		productions.Add ('b', new Module [productionLengths[1]]);
		productions ['b'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions ['b'][1] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['b'][2] = (new Module ('[', 0, -1, 0));
		productions ['b'][3] = (new Module ('N', 0, -1, 0));
		productions ['b'][4] = (new Module ('&', 0, 1, axlGrowth));
		productions ['b'][5] = (new Module ('L', 0, 1, leafGrowth));
		productions ['b'][6] = (new Module ('b', 0, 1, 0));
		productions ['b'][7] = (new Module (']', 0, -1, 0));
		productions ['b'][8] = (new Module ('I', 0, 1, flowerGrowth));
		productions ['b'][9] = (new Module ('%', 0, 1, axlGrowth));
		productions ['b'][10] = (new Module ('[', 0, -1, 0));
		productions ['b'][11] = (new Module ('N', 0, -1, 0));
		productions ['b'][12] = (new Module ('^', 0, 1, axlGrowth));
		productions ['b'][13] = (new Module ('b', 0, 1, 0));
		productions ['b'][14] = (new Module (']', 0, -1, 0));
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
