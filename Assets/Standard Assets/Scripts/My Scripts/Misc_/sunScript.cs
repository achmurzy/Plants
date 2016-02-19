using UnityEngine;
using System.Collections;

public class sunScript : L_System {
	private bool follow;
	// Use this for initialization
	void Awake()
	{
		/*generations = 4;
		edgeLength = 10.0f;
		angle = 25.0f;
		lineWidth = 3.0f;*/
	}

	void Start () 
	{
		returnList = "1";

		productions = new ArrayList ();
		productions.Add ("FF");
		productions.Add ("F[%1][*1]F1");

		drawColor = Color.yellow;

		if(gameObject.transform.parent.name == "SolarAxis")
		{}
		else
			follow = true;

	}
	
	// Update is called once per frame
	void Update () 
	{
		if(follow)
			gameObject.transform.parent.LookAt(GameObject.Find("playerShip").transform);
		//Every tenth frame the sun in yellow
		/*if(sunTimer == 20)
		{
			renderer.material = materials[2];
			sunTimer = 0;
		}
		else if(sunTimer % 2 == 0)
			renderer.material = materials[1];
		else
			renderer.material = materials[0];

		sunTimer++;*/
		//lookAtCamera ();
	}
}
