using UnityEngine;
using System.Collections;

public class YWeedSystem : L_System {

	// Use this for initialization
	void Awake () 
	{
		//Initialize axiom
		returnList = "N1";
		drawColor = Color.green;
		lineWidth = 1.5f;
		//Add productions
		productions = new ArrayList ();
		productions.Add ("FF");
		productions.Add ("NF%[[1]*1]*F[*F1]%1");
		
		angle = 22.5f;
		edgeLength = 0.75f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
