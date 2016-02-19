using UnityEngine;
using System.Collections;

public class infloresence : L_System {

	// Use this for initialization
	void Awake () 
	{
		angle = 45.0f;
		edgeLength = 0.5f;
		lineWidth = 2.0f;
		drawColor = Color.green;

		returnList = "NF";

		productions = new ArrayList ();
		productions.Add ("F1F");
		productions.Add ("[&I]+");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
