using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class actinomorphicRosette : Parametric_L_System {
	void Awake()
	{
		Leaf = Resources.Load ("Prefabs/L_Systems/Leaves/simpleLeaf1") as GameObject;

		Flower = Resources.Load ("Prefabs/L_Systems/Flowers/flower4") as GameObject;

	}
	// Use this for initialization
	void Start () 
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();
		
		divergenceAngle = 180.0f;
		segmentGrowth = 0.75f;
		widthGrowth = 0.35f;

		leafGrowth = 0.0335f;

		flowerGrowth = 0.055f;
		
		axlGrowth = 1.5f;
		
		maturity = 6.0f;
		
		numOfProductions = 3;
		productionLengths = new int[numOfProductions];

		productionLengths [0] = 11;
		productionLengths [1] = 10;
		productionLengths [2] = 10;

		returnList.Add (new Module ('N', 0, -1, 0));
		returnList.Add (new Module ('a', 0.9f, 1, 0));

		
		productions.Add ('a', new Module[productionLengths[0]]);	//Basal Growth
		productions['a'][0] = (new Module('+', 0, -1, divergenceAngle/5));
		productions['a'][1] = (new Module('[', 0, -1, 0));
		productions['a'][2] = (new Module('^', 0, 1, axlGrowth));
		productions['a'][3] = (new Module('b', 0, 1, 0));
		productions['a'][4] = (new Module(']', 0, 1, 0));
		productions['a'][5] = (new Module('+', 0, -1, divergenceAngle));
		productions['a'][6] = (new Module('[', 0, -1, 0));
		productions['a'][7] = (new Module('&', 0, 1, axlGrowth));
		productions['a'][8] = (new Module('c', 0, 1.5f, 0));
		productions['a'][9] = (new Module(']', 0, 1, 0));
		productions['a'][10] = (new Module('a', 0, 1, 0));

		productions.Add ('b', new Module[productionLengths[1]]);	//Leaf stalk
		productions['b'][0] = (new Module('&', 0, 1, axlGrowth));
		productions['b'][1] = (new Module('!', 0, 1, widthGrowth));
		productions['b'][2] = (new Module('F', 0, 1, segmentGrowth));
		productions['b'][3] = (new Module('L', 0, 1, leafGrowth));
		productions['b'][4] = (new Module('+', 0, 1, divergenceAngle/3));
		productions['b'][5] = (new Module('L', 0, 1, leafGrowth));
		productions['b'][6] = (new Module('+', 0, 1, divergenceAngle/3));
		productions['b'][7] = (new Module('L', 0, 1, leafGrowth));
		productions['b'][8] = (new Module('N', 0, -1, 0));
		productions['b'][9] = (new Module('b', 0, 1, 0));

		productions.Add ('c', new Module[productionLengths[2]]);	//Floral stalk
		productions['c'][0] = (new Module('^', 0, 1, axlGrowth));
		productions['c'][1] = (new Module('!', 0, 1, widthGrowth));
		productions['c'][2] = (new Module('F', 0, 1, segmentGrowth));
		productions['c'][3] = (new Module('I', 0, 1, flowerGrowth));
		productions['c'][4] = (new Module('+', 0, 1, divergenceAngle/3));
		productions['c'][5] = (new Module('I', 0, 1, flowerGrowth));
		productions['c'][6] = (new Module('+', 0, 1, divergenceAngle/3));
		productions['c'][7] = (new Module('I', 0, 1, flowerGrowth));
		productions['c'][8] = (new Module('N', 0, -1, 0));
		productions['c'][9] = (new Module('c', 0, 1, 0));
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
