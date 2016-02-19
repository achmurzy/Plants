using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class monopodialArching : Parametric_L_System {

	void Awake()
	{
		Leaf = Resources.Load ("Prefabs/L_Systems/Leaves/simpleLeaf1") as GameObject;
		leafGrowth = 0.1f;
		
		Flower = Resources.Load ("Prefabs/L_Systems/Flowers/flower2") as GameObject;
		flowerGrowth = 0.1f;
	}

	// Use this for initialization
	void Start () 
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();

		widthGrowth = 0.1f;

		maturity = 6.0f;

		segmentGrowth = 0.725f;

		axlGrowth = 5.0f;
		
		numOfProductions = 3;
		productionLengths = new int[numOfProductions];
		
		productionLengths [0] = 12;
		productionLengths [1] = 6;
		productionLengths [2] = 9;

		returnList.Add (new Module ('a', 0, 0.25f, 0));

		productions.Add ('a', new Module[productionLengths [0]]);
		productions ['a'][0] = new Module('!', 0, 1, widthGrowth);
		productions ['a'][1] = new Module('F', 0, 1, segmentGrowth);
		productions ['a'][2] = new Module('[', 0, -1, 0);
		productions	['a'][3] = new Module('N', 0, -1, 0);
		productions ['a'][4] = new Module('b', 0, 1, 0);
		productions ['a'][5] = new Module(']', 0, -1, 0);
		productions ['a'][6] = new Module('[', 0, -1, 0);
		productions ['a'][7] = new Module('N', 0, -1, 0);
		productions ['a'][8] = new Module('+', 0, 1, divergenceAngle);
		productions ['a'][9] = new Module('b', 0, 1, 0);
		productions ['a'][10] = new Module(']', 0, -1, 0);
		productions ['a'][11] = new Module('a', 0, 1, 0);

		productions.Add ('b', new Module[productionLengths[1]]);
		productions ['b'][0] = new Module('%', 0, 1, axlGrowth);
		productions ['b'][1] = new Module('^', 0, 1, axlGrowth);
		productions ['b'][2] = new Module('L', 0, 1, leafGrowth);
		productions ['b'][3] = new Module('!', 0, 1, widthGrowth);
		productions ['b'][4] = new Module('F', 0, 1, segmentGrowth);
		productions ['b'][5] = new Module('c', 0, 1, 0);


		productions.Add('c', new Module[productionLengths[2]]);
		productions ['c'][0] = new Module('+', 0, 1, divergenceAngle);
		productions ['c'][1] = new Module('L', 0, 1, leafGrowth);
		productions ['c'][2] = new Module('+', 0, 1, divergenceAngle);
		productions ['c'][3] = new Module('L', 0, 1, leafGrowth);
		productions ['c'][4] = new Module('+', 0, 1, divergenceAngle);
		productions ['c'][5] = new Module('L', 0, 1, leafGrowth);
		productions ['c'][6] = new Module('+', 0, 1, divergenceAngle);
		productions ['c'][7] = new Module('L', 0, 1, leafGrowth);
		productions ['c'][8] = new Module('I', 0, 1, flowerGrowth);
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
