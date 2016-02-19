using UnityEngine;
using System.Collections;

public class BoundaryGenerator : MonoBehaviour {

	private Object[] BoundaryFlats;
	private GameObject north, south, east, west;

	private float flatWidth = 2.0f;
	private float flatHeight = 13.0f;
	private float flatLength = 15.0f;

	private float depthOffsetHeight = 100.0f;

	private int heightNum = 10;
	private int widthNum = 17;

	// Use this for initialization
	void Start () 
	{
		BoundaryFlats = Resources.LoadAll ("Prefabs/Terrain/Flats");

		west = new GameObject ();
		west.name = "West";
		west.transform.position = new Vector3 (0, 150, 250);
		GenerateBoundary (west);

		south = new GameObject ();
		south.name = "South";
		south.transform.position = new Vector3 (250, 150, 0);
		GenerateBoundary (south);
		south.transform.Rotate (Vector3.up, -90);

		east = new GameObject ();
		east.name = "East";
		east.transform.position = new Vector3 (500, 150, 80);
		GenerateBoundary (east);
		east.transform.Rotate (Vector3.up, 180);

		north = new GameObject ();
		north.name = "North";
		north.transform.position = new Vector3 (93, 150, 500);
		GenerateBoundary (north);
		north.transform.Rotate (Vector3.up, 90);


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GenerateBoundary(GameObject reference)
	{
		GameObject flat;

		for(int j = -heightNum; j < heightNum; j++)
		{
			for(int i = -widthNum; i < widthNum; i++)
			{
				flat = Instantiate(BoundaryFlats[Random.Range(0, BoundaryFlats.Length)]) as GameObject;
				flat.transform.parent = reference.transform;
				if((j+heightNum) * flatHeight > depthOffsetHeight)
				{
					float leftDepth = reference.transform.GetChild
						(((heightNum+j)*widthNum*2)+((i+widthNum))-1).position.x;
					float bottomDepth = reference.transform.GetChild
						(((heightNum+j)*widthNum*2)+((i+widthNum))-(widthNum*2)).position.x;
					float otherDepth = (leftDepth+bottomDepth)/2;
	
					float newDepth = otherDepth+Random.Range(flatWidth/4, flatWidth);

					flat.transform.position = new Vector3(newDepth, j*flatHeight, i*flatLength);
					flat.transform.position += new Vector3(0, reference.transform.position.y, 
					                                       		reference.transform.position.z);
				}
				else
				{
					flat.transform.position = new Vector3(Random.Range(-flatWidth/2, flatWidth/2), j*flatHeight, i*flatLength);
					flat.transform.position += reference.transform.position;
				}
			}
		}
	}
}
