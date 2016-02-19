using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zygomorhpicRosette : Parametric_L_System {

	void Awake()
	{
		Leaf = Resources.Load ("Prefabs/L_Systems/Leaves/simpleLeaf2") as GameObject;
		leafGrowth = 0.0175f;
		Flower = Resources.Load ("Prefabs/L_Systems/Flowers/flower5") as GameObject;
		flowerGrowth = 0.015f;
	}
	// Use this for initialization
	void Start () 
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();
		
		divergenceAngle = 180.0f;
		segmentGrowth = 0.6f;
		widthGrowth = 0.15f;
		
		axlGrowth = 2.5f;
		
		maturity = 6.0f;
		
		numOfProductions = 3;
		productionLengths = new int[numOfProductions];
		
		productionLengths [0] = 9;
		productionLengths [1] = 6;
		productionLengths [2] = 6;
		
		returnList.Add (new Module ('N', 0, -1, 0));
		returnList.Add (new Module ('a', 0.75f, 1, 0));
		
		
		productions.Add ('a', new Module[productionLengths[0]]);	//Basal Growth
		productions['a'][0] = (new Module('+', 0, 1, divergenceAngle/4));
		productions['a'][1] = (new Module('[', 0, -1, 0));
		productions['a'][2] = (new Module('b', 0, 1, 0));
		productions['a'][3] = (new Module(']', 0, 1, 0));
		productions['a'][4] = (new Module('+', 0, 1, divergenceAngle));
		productions['a'][5] = (new Module('[', 0, -1, 0));
		productions['a'][6] = (new Module('b', 0, 1, 0));
		productions['a'][7] = (new Module(']', 0, 1, 0));
		productions['a'][8] = (new Module('a', 0, 0.5f, 0));
		
		productions.Add ('b', new Module[productionLengths[1]]);	//Leaf stalk
		productions['b'][0] = (new Module('&', 0, 1, axlGrowth));
		productions['b'][1] = (new Module('!', 0, 1, widthGrowth));
		productions['b'][2] = (new Module('F', 0, 1, segmentGrowth));
		productions['b'][3] = (new Module('L', 0, 1, leafGrowth));
		productions['b'][4] = (new Module('N', 0, -1, 0));
		productions['b'][5] = (new Module('c', 0, 1, 0));
		
		productions.Add ('c', new Module[productionLengths[2]]);	//Floral stalk
		productions['c'][0] = (new Module('^', 0, 1, axlGrowth*2));
		productions['c'][1] = (new Module('^', 0, 1, axlGrowth*3));
		productions['c'][2] = (new Module('!', 0, 1, widthGrowth));
		productions['c'][3] = (new Module('F', 0, 1, segmentGrowth));
		productions['c'][4] = (new Module('I', 0, 1, flowerGrowth));
		productions['c'][5] = (new Module('N', 0, -1, 0));
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
