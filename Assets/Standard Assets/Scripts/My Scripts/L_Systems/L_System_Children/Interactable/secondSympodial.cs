using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class secondSympodial : Parametric_L_System {

	void Awake()
	{
		Leaf = Resources.Load ("Prefabs/L_Systems/Leaves/palmateLeaf3") as GameObject;
		leafGrowth = 0.053f;

		Flower = Resources.Load ("Prefabs/L_Systems/Flowers/flower6") as GameObject;
		flowerGrowth = 0.1f;
	}

	// Use this for initialization
	void Start () 
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();

		branchAngle = -90;
		divergenceAngle = 180;

		segmentGrowth = 0.755f;
		axlGrowth = 2.0f;
	
		widthGrowth = 0.75f;

		maturity = 6.0f;
		
		numOfProductions = 4;
		productionLengths = new int[numOfProductions];
		
		productionLengths [0] = 12;
		productionLengths [1] = 13;
		productionLengths [2] = 14;
		productionLengths [3] = 6;

		returnList.Add (new Module ('!', 0, 1, widthGrowth));
		returnList.Add (new Module ('F', 0, 1, segmentGrowth/10));
		returnList.Add (new Module ('a', 1, 1, 0));
		
		productions.Add ('a', new Module [productionLengths[0]]);
		productions ['a'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions ['a'][1] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['a'][2] = (new Module ('[', 0, -1, 0));
		productions ['a'][3] = (new Module ('N', 0, -1, 0));
		productions ['a'][4] = (new Module('d', 0, 1, 0));
		productions ['a'][5] = (new Module ('*', 0, 1, axlGrowth));
		productions ['a'][6] = (new Module ('b', 0, 1.5f, 0));
		productions ['a'][7] = (new Module (']', 0, -1, 0));
		productions ['a'][8] = (new Module ('+', 0, -1, divergenceAngle));
		productions ['a'][9] = (new Module('d', 0, 1, 0));
		productions ['a'][10] = (new Module ('*', 0, 1, axlGrowth));
		productions ['a'][11] = (new Module ('c', 0, 1.5f, 0));

		productions.Add ('b', new Module [productionLengths[1]]);

		productions ['b'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions ['b'][1] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['b'][2] = (new Module ('[', 0, -1, 0));
		productions ['b'][3] = (new Module ('N', 0, -1, 0));
		productions ['b'][4] = (new Module('d', 0, 1, 0));
		productions ['b'][5] = (new Module ('%', 0, 1, 0));
		productions ['b'][6] = (new Module ('a', 0, 1, 0));
		productions ['b'][7] = (new Module (']', 0, -1, 0));
		productions ['b'][8] = (new Module ('&', 0, 1, branchAngle));
		productions ['b'][9] = (new Module ('I', 0, 1, flowerGrowth));
		productions ['b'][10] = (new Module ('+', 0, -1, divergenceAngle));
		productions ['b'][11] = (new Module('d', 0, 1, 0));
		productions ['b'][12] = (new Module ('%', 0, 1, axlGrowth));

		productions.Add('c', new Module [productionLengths[2]]);
		productions ['c'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions ['c'][1] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['c'][2] = (new Module ('[', 0, -1, 0));
		productions ['c'][3] = (new Module ('N', 0, -1, 0));
		productions ['c'][4] = (new Module ('d', 0, 1, 0));
		productions ['c'][5] = (new Module ('^', 0, 1, axlGrowth));
		productions ['c'][6] = (new Module ('a', 0, 1, 0));
		productions ['c'][7] = (new Module (']', 0, -1, 0));
		productions ['c'][8] = (new Module ('&', 0, 1, branchAngle));
		productions ['c'][9] = (new Module ('I', 0, 1, flowerGrowth));
		productions ['c'][10] = (new Module ('+', 0, -1, divergenceAngle));
		productions ['c'][11] = (new Module ('d', 0, 1, 0));
		productions ['c'][12] = (new Module ('^', 0, 1, axlGrowth));
		productions ['c'][13] = (new Module ('b', 0, 1, 0));

		productions.Add ('d', new Module[productionLengths [3]]);
		productions ['d'][0] = (new Module ('[', 0, -1, 0));
		productions ['d'][1] = (new Module ('N', 0, -1, 0));
		productions ['d'][2] = (new Module ('&', 0, 1, branchAngle));
		productions ['d'][3] = (new Module ('^', 0, 1, axlGrowth*4));
		productions ['d'][4] = (new Module ('L', 0, 1, leafGrowth));
		productions ['d'][5] = (new Module (']', 0, -1, 0));
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
			case '&':
				return gp;
			default:
				return gp*Mathf.Exp(exponentialParameter*time);
		}
	}
}