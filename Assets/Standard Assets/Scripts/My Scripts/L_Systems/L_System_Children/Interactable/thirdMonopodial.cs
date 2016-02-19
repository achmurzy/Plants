using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class thirdMonopodial : Parametric_L_System 
{
	void Awake()
	{
		Leaf = Resources.Load ("Prefabs/L_Systems/Leaves/compoundLeaf1") as GameObject;
		leafGrowth = 0.15f;
		
		Flower = Resources.Load ("Prefabs/L_Systems/Flowers/flower5") as GameObject;
		flowerGrowth = 0.15f;
	}

	// Use this for initialization
	void Start () 
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();

		maturity = 6.0f;

		segmentGrowth = 0.75f;
		widthGrowth = 0.175f;

		axlGrowth = 10.5f;
		
		numOfProductions = 2;
		productionLengths = new int[numOfProductions];
		
		productionLengths [0] = 8;
		productionLengths [1] = 9;
		

		returnList.Add (new Module ('a', 0.5f, 1, 0));
		
		productions.Add ('a', new Module [productionLengths[0]]);
		productions ['a'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions ['a'][1] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['a'][2] = (new Module ('-', 0, 1, divergenceAngle));
		productions ['a'][3] = (new Module ('[', 0, -1, 0));
		productions ['a'][4] = (new Module ('N', 0, -1, 0));
		productions ['a'][5] = (new Module ('b', 0, 1.0f, 0));
		productions ['a'][6] = (new Module (']', 0, -1, 0));
		productions ['a'][7] = (new Module ('a', 0, 1.0f, 0));

		productions.Add ('b', new Module[productionLengths[1]]);
		productions ['b'][0] = (new Module ('[', 0, -1, 0));
		productions ['b'][1] = (new Module ('N', 0, -1, 0));
		productions ['b'][2] = (new Module ('&', 0, 1, axlGrowth));
		productions ['b'][3] = (new Module ('L', 0, 1, leafGrowth));
		productions ['b'][4] = (new Module (']', 0, -1, 0));
		productions ['b'][5] = (new Module ('&', 0, 1, -axlGrowth));
		productions ['b'][6] = (new Module ('!', 0, 1, widthGrowth));
		productions ['b'][7] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['b'][8] = (new Module ('I', 0, 1, flowerGrowth));
	}
	
	// Update is called once per frame
	void Update () 
	{
		base.Update ();
	}
	
	public override float growthFunction(char sym, float gp, float time)
	{
		//if(time > 1)
		//	time = 1;
		switch(sym)
		{
			case '-':
				return gp;
			default:
				return gp*Mathf.Exp(exponentialParameter*time);
		}
	}
}