using UnityEngine;
using System.Collections;

public class sunWeedSky : L_System_Animated {

	private float trueEdge = 10.0f;
	private bool goingDown = false;
	
	// Use this for initialization
	void Start () 
	{
		//Initialize axiom
		returnList = "F";
		
		//Add productions
		productions = new ArrayList ();
		productions.Add ("[%F]F*F[F]");
		
		angleAnimateSpeed = 15.0f;
		
		initializeAnimation ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Animate ();
		lookAtCamera ();
	}
	
	new private void Animate()
	{
		edgeLength = trueEdge * Mathf.Sin (Time.deltaTime);
		
		if(angle > 160)
			goingDown = true;
		else if(angle < 40)
			goingDown = false;
		
		if(goingDown)
			angle -= Time.deltaTime*angleAnimateSpeed;
		else
			angle += Time.deltaTime*angleAnimateSpeed;
	}
}
