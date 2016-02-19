using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class thirdSympodial : Parametric_L_System {

	void Awake()
	{
		//Leaves and flowers need to be changed on this one
		//Make Rounded Palmate Leaves and an interesting flower
		Leaf = Resources.Load ("Prefabs/L_Systems/Leaves/simpleLeaf2") as GameObject;
		leafGrowth = 0.11f;
		
		Flower = Resources.Load ("Prefabs/L_Systems/Flowers/flower1") as GameObject;
		flowerGrowth = 0.1f;

		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();
	}
	// Use this for initialization
	void Start () 
	{
		widthGrowth = 0.75f;
		
		maturity = 6.0f;

		segmentGrowth = 1.05f;

		axlGrowth = 4.25f;
		divergenceAngle = 180.0f;
		
		numOfProductions = 3;
		productionLengths = new int[numOfProductions];
		
		productionLengths [0] = 16;
		productionLengths [1] = 10;
		productionLengths [2] = 6;

		returnList.Add (new Module ('!', 0, 1, widthGrowth));
		returnList.Add (new Module ('F', 0, 1, segmentGrowth*0.5f));
		returnList.Add (new Module ('a', 0, 1, 0));

		productions.Add ('a', new Module [productionLengths[0]]);
		productions ['a'][0] = (new Module ('I', 0, 1, flowerGrowth));
		productions ['a'][1] = (new Module ('[', 0, -1, 0));
		productions ['a'][2] = (new Module ('b', 0, 0.5f, 0));
		productions ['a'][3] = (new Module (']', 0, 1, 0));
		productions ['a'][4] = (new Module ('+', 0, 1, divergenceAngle));
		productions ['a'][5] = (new Module ('[', 0, -1, 0));
		productions ['a'][6] = (new Module ('b', 0, 0.5f, 0));
		productions ['a'][7] = (new Module (']', 0, 1, 0));
		productions ['a'][8] = (new Module ('+', 0, 1, divergenceAngle/2));
		productions ['a'][9] = (new Module ('[', 0, -1, 0));
		productions ['a'][10] = (new Module ('b', 0, 0.5f, 0));
		productions ['a'][11] = (new Module (']', 0, -1, 0));
		productions ['a'][12] = (new Module ('+', 0, 1, divergenceAngle));
		productions ['a'][13] = (new Module ('[', 0, -1, 0));
		productions ['a'][14] = (new Module ('b', 0, 0.5f, 0));
		productions ['a'][15] = (new Module (']', 0, 1, 0));

		productions.Add('b', new Module[productionLengths[1]]);
		productions ['b'][0] = (new Module ('[', 0, -1, 0));
		productions ['b'][1] = (new Module ('&', 0, 1, axlGrowth));
		productions ['b'][2] = (new Module ('N', 0, 1, 0));
		productions ['b'][3] = (new Module ('c', 0, 1, 0));
		productions ['b'][4] = (new Module (']', 0, -1, 0));
		productions ['b'][5] = (new Module ('[', 0, -1, 0));
		productions ['b'][6] = (new Module ('*', 0, 1, axlGrowth));
		productions ['b'][7] = (new Module ('N', 0, 1, 0));
		productions ['b'][8] = (new Module ('c', 0, 1, 0));
		productions ['b'][9] = (new Module (']', 0, -1, 0));
		
		productions.Add ('c', new Module[productionLengths[2]]);
		productions ['c'][0] = (new Module ('!', 0, 1, widthGrowth));
		productions ['c'][1] = (new Module ('F', 0, 1, segmentGrowth));
		productions ['c'][2] = (new Module ('L', 0, 1, leafGrowth));
		productions ['c'][3] = (new Module ('I', 0, 1, flowerGrowth));
		productions ['c'][4] = (new Module ('+', 0, 1, divergenceAngle));
		productions ['c'][5] = (new Module ('b', 0, 0.25f, 0));
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
				return (gp)*Mathf.Exp(exponentialParameter*time);
		}
	}
}
