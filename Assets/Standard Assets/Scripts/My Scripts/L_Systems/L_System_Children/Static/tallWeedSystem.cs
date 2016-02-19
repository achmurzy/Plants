using Vectrosity;
using UnityEngine;
using System.Collections;

public class tallWeedSystem : L_System {

	// Use this for initialization
	void Awake () 
	{
		angle = 35.0f;
		edgeLength = 0.5f;
		drawColor = Color.green;

		productions = new ArrayList ();
		productions.Add ("NF[*F]F[%F]F");

		returnList = "F";
		lineWidth = 2.0f;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		//lookAtCamera ();
	}
}
