using UnityEngine;
using System.Collections;

public class bushAnimated : L_System_Animated {

	// Use this for initialization
	void Start () 
	{
		angleAnimateSpeed = 0.05f;
		edgeAnimateSpeed = 0.0025f;

		drawColor = Color.green;
		
		productions = new ArrayList ();
		productions.Add("2-----F");
		productions.Add("[&F3!1]-----'[&F3!1]-------'[&F3!1]");
		productions.Add("F3");
		productions.Add("['''^^{L}]");
		
		returnList = "1";

		initializeAnimation ();
		followCamera = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		base.Animate ();
	}
}
