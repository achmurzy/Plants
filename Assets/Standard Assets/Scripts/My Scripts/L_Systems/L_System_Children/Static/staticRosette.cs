using UnityEngine;
using System.Collections;

public class staticRosette : L_System {

	// Use this for initialization
	void Awake () 
	{
		returnList = "N[1]+[1]+[1]+[1]+[1]+[1]+[1]+[1]+[1]+[1]+[1]+[1]";
			drawColor = Color.green;
		lineWidth = 2.0f;
		edgeLength = 5.0f;
		angle = 30.0f;
		productions = new ArrayList ();
		productions.Add ("F");
		productions.Add ("1[&F]");
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
