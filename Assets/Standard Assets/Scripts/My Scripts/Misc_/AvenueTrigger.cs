using UnityEngine;
using System.Collections;

public class AvenueTrigger : MonoBehaviour {

	public GameObject emblem;
	private BoxCollider avenue;
	// Use this for initialization
	void Start () {
		avenue  = GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () 
	{}
}
