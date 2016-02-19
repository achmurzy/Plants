using UnityEngine;
using System.Collections;

public class firstFlowerSystem : L_System {

	// Use this for initialization
	void Start () 
	{
		angle = 40.0f;
		edgeLength = 0.25f;
		
		productions = new ArrayList ();
		productions.Add ("FF");
		productions.Add ("F[&F]+++[&F2]++[&F]1");
		productions.Add ("I");
		
		returnList = "1";
	}
	
	// Update is called once per frame
	void Update () 
	{}
}
