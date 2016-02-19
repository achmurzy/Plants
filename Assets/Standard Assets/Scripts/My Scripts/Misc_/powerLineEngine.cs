using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class powerLineEngine : MonoBehaviour {
	
	public GameObject poleTwo;

	private Vector3 start;
	private Vector3 end;

	private Vector3 startCont;
	private Vector3 endCont;
	
	private Vector3[] controlPoints;
	private Vector3[] linePoints;

	private int segmentResolution;
	
	private VectorLine currentLine;
	private VectorLine controlLines;
	private Material lineMaterialBlack, lineMaterialYellow;
	private float lineWidth = 5.0f;
	private float controlPointOffset = 15;
	
	private int surgeIndex = 0;
	private float surgeTimer = 0;
	private float surgeLimit = 0.01f;
	private VectorLine surgeSegment;

	// Use this for initialization
	void Start () 
	{
		lineMaterialBlack = Resources.Load ("Material/PowerLineBlack") as Material;
		lineMaterialYellow = Resources.Load("Material/PowerLine") as Material;

		segmentResolution = 50;
		controlPoints = new Vector3[4];

		start = transform.position + transform.up * (transform.localScale.y);
		startCont = start - Vector3.up * controlPointOffset;

		end = poleTwo.transform.position + poleTwo.transform.up * (poleTwo.transform.localScale.y);
		endCont = end - Vector3.up * controlPointOffset;

		controlPoints [0] = start;
		controlPoints [1] = startCont;
		controlPoints [2] = end;
		controlPoints [3] = endCont;

		controlLines = new VectorLine ("Controls", controlPoints, lineMaterialYellow, 1.0f);

		currentLine = new VectorLine ("powerLine", new Vector3[segmentResolution+1], 
		                              	lineMaterialBlack, lineWidth, LineType.Continuous, Joins.Weld); 

		currentLine.MakeCurve (controlPoints[0], 
		                       controlPoints[1], 
		                       controlPoints[2], 
		                       controlPoints[3], 
		                       segmentResolution);
		currentLine.Draw3DAuto ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		powerSurge ();
	}

	private void powerSurge()
	{
		surgeTimer += Time.deltaTime;
		if(surgeTimer >= surgeLimit)
		{
			surgeTimer = 0;
			surgeIndex++;
			surgeIndex = surgeIndex % segmentResolution;
			VectorLine.Destroy(ref surgeSegment);
			surgeSegment = VectorLine.SetLine3D(Color.yellow, 
			               	currentLine.points3[surgeIndex], currentLine.points3[surgeIndex+1]);
			surgeSegment.SetWidth(lineWidth+3);
			surgeSegment.Draw3DAuto();
		}
	}

	public void setLineCurvature(float curve)
	{
		controlPointOffset = curve;
	}
}
