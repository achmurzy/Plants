using UnityEngine;
using System.Collections;

public class LightPoleGenerator : MonoBehaviour {

	private GameObject poleOne, poleTwo, poleThree, poleFour;
	private BoxCollider bc;
	// Use this for initialization
	void Awake () 
	{
		poleOne = Instantiate(Resources.Load ("Prefabs/Terrain/PowerLine")) as GameObject;
		poleTwo = Instantiate(Resources.Load ("Prefabs/Terrain/PowerLine")) as GameObject;
		poleThree = Instantiate(Resources.Load ("Prefabs/Terrain/PowerLine")) as GameObject;
		poleFour = Instantiate(Resources.Load ("Prefabs/Terrain/PowerLine")) as GameObject;

		bc = this.GetComponent<BoxCollider> ();
		if(bc == null)
			Debug.Log ("Missing collider");
		else
		{
			poleOne.GetComponent<powerLineEngine>().poleTwo = poleTwo;
			poleOne.GetComponent<powerLineEngine> ().setLineCurvature (bc.size.y/2);
			poleTwo.GetComponent<powerLineEngine>().poleTwo = poleThree;
			poleTwo.GetComponent<powerLineEngine> ().setLineCurvature (bc.size.y/2);
			poleThree.GetComponent<powerLineEngine>().poleTwo = poleFour;
			poleThree.GetComponent<powerLineEngine> ().setLineCurvature (bc.size.y/2);
			poleFour.GetComponent<powerLineEngine>().poleTwo = poleOne;
			poleFour.GetComponent<powerLineEngine> ().setLineCurvature (bc.size.y/2);

			poleOne.transform.parent = this.transform;
			poleOne.transform.localPosition = (bc.center-(Vector3.up*bc.size.y))+(bc.size/2);
			poleOne.transform.localScale = new Vector3(poleOne.transform.localScale.x, 
			                                           bc.size.y*2, poleOne.transform.localScale.z);
			
			poleTwo.transform.parent = this.transform;
			poleTwo.transform.localPosition = (bc.center-(Vector3.up*bc.size.y)) + 
				(new Vector3(-bc.size.x, bc.size.y, bc.size.z)/2);
			poleTwo.transform.localScale = new Vector3(poleTwo.transform.localScale.x, 
			                                           bc.size.y*2, poleTwo.transform.localScale.z);
			
			poleThree.transform.parent = this.transform;
			poleThree.transform.localPosition = (bc.center-(Vector3.up*bc.size.y)) + 
				(new Vector3(bc.size.x, bc.size.y, -bc.size.z)/2);
			poleThree.transform.localScale = new Vector3(poleThree.transform.localScale.x, 
			                                             bc.size.y*2, poleThree.transform.localScale.z);
			
			poleFour.transform.parent = this.transform;
			poleFour.transform.localPosition = (bc.center-(Vector3.up*bc.size.y)) + 
				(new Vector3(-bc.size.x, bc.size.y, -bc.size.z)/2);
			poleFour.transform.localScale = new Vector3(poleFour.transform.localScale.x, 
			                                            bc.size.y*2, poleFour.transform.localScale.z);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
