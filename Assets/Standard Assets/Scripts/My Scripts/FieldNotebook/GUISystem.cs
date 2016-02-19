using UnityEngine;
using System.Collections;

public class GUISystem : L_System {

	// Use this for initialization
	void Start () 
	{	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setParameters()
	{
		//Initialize axiom
		returnList = "F";
		
		//Add productions
		productions = new ArrayList ();
		productions.Add ("1[*F][%F]F1");
		productions.Add ("FF");
		
		angle = 25.7f;
		edgeLength = 12.5f;

		generations = 5;
	}
}
