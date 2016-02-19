using UnityEngine;
using System.Collections;

public class wheatWeedSystem : L_System {
	
	// Use this for initialization
	void Start () 
	{	
		//Initialize axiom
		returnList = "F";
		drawColor = Color.green;
		//Add productions
		productions = new ArrayList ();
		productions.Add ("1[*F][%F]F1");
		productions.Add ("FF");
		lineWidth = 1.5f;
		angle = 25.7f;
		edgeLength = 0.1f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		lookAtCamera ();
	}
}
