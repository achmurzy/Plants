using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class leavedRosette : Parametric_L_System {

	private float infloresenceLifetime = 0.5f;

	void Awake()
	{
		Leaf = Resources.Load ("Prefabs/L_Systems/Leaves/simpleLeaf2") as GameObject;
		leafGrowth = 0.025f;
		
		Flower = Resources.Load ("Prefabs/L_Systems/Flowers/infloresence1") as GameObject;
		flowerGrowth = 0.05f;
	}
	// Use this for initialization
	void Start () 
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();
		
		divergenceAngle = 45.0f;

		segmentGrowth = 0.55f;
		widthGrowth = 0.15f;

		axlGrowth = 3.0f;
		
		maturity = 6.0f;
		
		numOfProductions = 5;
		productionLengths = new int[numOfProductions];
		
		productionLengths [0] = 8;	//Rosetted Leaves
		productionLengths [1] = 8;	//Basal Stalk
		productionLengths [2] = 8;	//Basal Stalk
		productionLengths [3] = 8;	//Infloresence
		productionLengths [4] = 8;	//Infloresence
		
		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module ('^', 0, 1, divergenceAngle/2));
		returnList.Add (new Module ('b', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, divergenceAngle));
		
		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('L', 0, 1, leafGrowth));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, divergenceAngle));
		
		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module ('^', 0, 1, -divergenceAngle/2));
		returnList.Add (new Module ('b', 0, 0.75f, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, divergenceAngle));
		
		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('L', 0, 1, leafGrowth));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, divergenceAngle));
		
		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module ('*', 0, 1, divergenceAngle / 2));
		returnList.Add (new Module ('b', 0, 0.5f, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, divergenceAngle));
		
		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('L', 0, 1, leafGrowth));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, divergenceAngle));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module ('*', 0, 1, divergenceAngle / 2));
		returnList.Add (new Module ('b', 0, 0.25f, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		returnList.Add (new Module ('+', 0, -1, divergenceAngle));

		returnList.Add (new Module ('[', 0, -1, 0));
		returnList.Add (new Module ('L', 0, 1, leafGrowth));
		returnList.Add (new Module ('a', 0, 1, 0));
		returnList.Add (new Module (']', 0, -1, 0));
		
		productions.Add ('a', new Module[productionLengths[0]]);
		//productions ['a'][0] = (new Module ('a', 0, 1, 0));
		productions ['a'][0] = (new Module('-', 0, 1, axlGrowth*2));
		productions ['a'][1] = (new Module ('!', 0, 1, widthGrowth));
		productions ['a'][2] = (new Module ('[', 0, -1, 0));
		productions ['a'][3] = (new Module ('N', 0, 1, 0));
		productions ['a'][4] = (new Module ('&', 0, 1, axlGrowth));
		productions ['a'][5] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['a'][6] = (new Module ('L', 0, 1, leafGrowth));
		productions ['a'][7] = (new Module (']', 0, -1, 0));

		productions.Add ('b', new Module[productionLengths [1]]);
		productions['b'][0] = (new Module ('!', 0, 1, widthGrowth*0.5f));
		productions['b'][1] = (new Module ('F', 0, 1, segmentGrowth));
		productions['b'][2] = (new Module ('[', 0, -1, 0));
		productions['b'][3] = (new Module ('N', 0, -1, 0));
		productions['b'][4] = (new Module ('d', 0, 1, 0));
		productions['b'][5] = (new Module (']', 0, -1, 0));
		productions['b'][6] = (new Module ('+', 0, 1, divergenceAngle*2));
		productions['b'][7] = (new Module ('c', 0, 1, 0));

		productions.Add ('c', new Module[productionLengths [2]]);
		productions['c'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions['c'][1] = (new Module ('F', 0, 1, segmentGrowth));
		productions['c'][2] = (new Module ('[', 0, -1, 0));
		productions['c'][3] = (new Module ('N', 0, -1, 0));
		productions['c'][4] = (new Module ('e', 0, 1, 0));
		productions['c'][5] = (new Module (']', 0, -1, 0));
		productions['c'][6] = (new Module ('+', 0, 1, divergenceAngle*2));
		productions['c'][7] = (new Module ('b', 0, 1, 0));

		productions.Add ('d', new Module[productionLengths [3]]);
		productions['d'][0] = (new Module ('&', 0, 1, axlGrowth));
		productions['d'][1] = (new Module ('!', 0, 1, widthGrowth));
		productions['d'][2] = (new Module ('F', 0, 1, segmentGrowth));
		productions['d'][3] = (new Module ('[', 0, -1, 0));
		productions['d'][4] = (new Module ('&', 0, 1, axlGrowth*2));
		productions['d'][5] = (new Module ('I', 0, 1, flowerGrowth));
		productions['d'][6] = (new Module (']', 0, -1, 0));
		productions['d'][7] = (new Module ('e', 0, 1, 0));

		productions.Add ('e', new Module[productionLengths [4]]);
		productions['e'][0] = (new Module ('%', 0, 1, axlGrowth));
		productions['e'][1] = (new Module ('!', 0, 1, widthGrowth));
		productions['e'][2] = (new Module ('F', 0, 1, segmentGrowth));
		productions['e'][3] = (new Module ('[', 0, -1, 0));
		productions['e'][4] = (new Module ('%', 0, 1, axlGrowth*2));
		productions['e'][5] = (new Module ('I', 0, 1, flowerGrowth));
		productions['e'][6] = (new Module (']', 0, -1, 0));
		productions['e'][7] = (new Module ('d', 0, 1, 0));
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
			case '^':
				return gp;
			case '*':
				return gp;
			default:
				return gp*Mathf.Exp(exponentialParameter*time);
		}
	}
}
