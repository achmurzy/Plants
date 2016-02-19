using UnityEngine;
using System.Collections;

public class sunWeedSystem : L_System {

	// Use this for initialization
	void Start () 
	{
		//Initialize axiom
		returnList = "F";
		
		//Add productions
		productions = new ArrayList ();
		productions.Add ("[%F]F*F[F]");

		angle = 5.0f;
		edgeLength = 1.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
}
