using UnityEngine;
using System.Collections;

public class bushSystem : L_System {

	// Use this for initialization
	void Start () 
	{
		angle = 22.5f;
		edgeLength = 0.5f;
		
		productions = new ArrayList ();
		productions.Add("2-----F");
		productions.Add("[&F3!1]-----'[&F3!1]-------'[&F3!1]");
		productions.Add("F3");
		productions.Add("['''^^{L}]");
		
		returnList = "1";
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
