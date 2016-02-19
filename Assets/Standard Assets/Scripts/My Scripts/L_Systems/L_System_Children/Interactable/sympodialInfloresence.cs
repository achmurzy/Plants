using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class sympodialInfloresence : Parametric_L_System {

	void Awake()
	{
		Leaf = Resources.Load ("Prefabs/L_Systems/Leaves/compoundLeaf2") as GameObject;
		leafGrowth = 0.04f;
		
		Flower = Resources.Load ("Prefabs/L_Systems/Flowers/infloresence2") as GameObject;
		flowerGrowth = 0.045f;
		}
	// Use this for initialization
	void Start () 
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();

		axlGrowth = 5.0f;

		widthGrowth = 0.85f;

		segmentGrowth = 0.45f;
		
		divergenceAngle = 180.0f;

		maturity = 6.0f;
		
		numOfProductions = 2;
		productionLengths = new int[numOfProductions];
		
		productionLengths [0] = 19;
		productionLengths [1] = 18;
		
		returnList.Add (new Module ('!', 0, 1, widthGrowth));
		returnList.Add (new Module ('F', 0, 1, segmentGrowth));
		returnList.Add (new Module ('a', 0, 0.5f, 0));
		
		productions.Add ('a', new Module [productionLengths[0]]);	
		productions['a'][0] = (new Module ('+', 0, 1, divergenceAngle/4));
		productions['a'][1] = (new Module ('!', 0, 1, widthGrowth));
		productions['a'][2] = (new Module ('F', 0, 1, segmentGrowth));
		productions['a'][3] = (new Module ('[', 0, -1, 0));
		productions['a'][4] = (new Module ('N', 0, -1, 0));
		productions['a'][5] = (new Module ('&', 0, 1, axlGrowth));
		productions['a'][6] = (new Module ('L', 0, 1, leafGrowth));
		productions['a'][7] = (new Module ('a', 0, 1.0f, 0));
		productions['a'][8] = (new Module (']', 0, 1, 0));
		productions['a'][9] = (new Module ('+', 0, 1, divergenceAngle));
		productions['a'][10] = (new Module ('[', 0, -1, 0));
		productions['a'][11] = (new Module ('%', 0, 1, axlGrowth));
		productions['a'][12] = (new Module ('c', 0, 1, 0));
		productions['a'][13] = (new Module (']', 0, -1, 0));
		productions['a'][14] = (new Module ('[', 0, -1, 0));
		productions['a'][15] = (new Module ('&', 0, 1, axlGrowth));
		productions['a'][16] = (new Module ('L', 0, 1, leafGrowth));
		productions['a'][17] = (new Module ('a', 0, 1.0f, 0));
		productions['a'][18] = (new Module (']', 0, 1, 0));

		
		productions.Add ('c', new Module[productionLengths[1]]);	
		productions['c'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions['c'][1] = (new Module ('F', 0, 1, segmentGrowth));
		productions['c'][2] = (new Module ('+', 0, 1, 120.0f));
		productions['c'][3] = (new Module ('[', 0, -1, 0));
		productions['c'][4] = (new Module ('^', 0, 1, 45));
		productions['c'][5] = (new Module ('I', 0, 1, flowerGrowth));
		productions['c'][6] = (new Module (']', 0, 1, 0));
		productions['c'][7] = (new Module ('+', 0, 1, 120.0f));
		productions['c'][8] = (new Module ('[', 0, -1, 0));
		productions['c'][9] = (new Module ('^', 0, 1, 45));
		productions['c'][10] = (new Module ('I', 0, 1, flowerGrowth));
		productions['c'][11] = (new Module (']', 0, 1, 0));
		productions['c'][12] = (new Module ('+', 0, 1, 120.0f));
		productions['c'][13] = (new Module ('[', 0, -1, 0));
		productions['c'][14] = (new Module ('^', 0, 1, 45));
		productions['c'][15] = (new Module ('I', 0, 1, flowerGrowth));
		productions['c'][16] = (new Module (']', 0, 1, 0));
		productions['c'][17] = (new Module ('c', 0, 0.5f, 0));
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
			default:
				return (gp) * Mathf.Exp(exponentialParameter*time);
		}
	}
}
