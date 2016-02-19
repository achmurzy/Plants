using UnityEngine;
using System.Collections;

public class YWeedAnimated : L_System_Animated {

	// Use this for initialization
	void Start () 
	{
		//Initialize axiom
		returnList = "1";
		
		//Add productions
		productions = new ArrayList ();
		productions.Add ("FF");
		productions.Add ("F%[[1]*1]*F[*F1]%1");
		
		angleAnimateSpeed = 0.1f;
		edgeAnimateSpeed = 0.001f;
		
		initializeAnimation ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		base.Animate ();
	}
}
