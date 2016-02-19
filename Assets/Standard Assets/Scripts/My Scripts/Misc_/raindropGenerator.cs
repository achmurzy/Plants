using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class raindropGenerator : MonoBehaviour {

	public List<VectorLine> lineList;

	//maximum line count and radius to spawn around player
	public int fallingLines = 25;
	private float columnRadius = 1.0f;

	//transformation parameters
	//private float lengthChange = 0.1f;
	private float dropSpeed = 0.25f;
	private float dropSpeedBase = 0.5f; //We want to drop lines faster when we are higher up cause they live longer

	//The starting height of the lines as a proportion of the distance between the ship and the terrain
	private float lineHeightProportion = 0.95f;
	private float minLineHeight = 1.0f;
	//The last raycasted distance from a surface
	private float lastHeight;
	//Time between line update coroutines
	private float dropRefresh = 0.1f;
	//are we drawing lines
	private bool raining = false;

	private L_System_Field raycastBitch;
	private float heightRatio;

	private int poolSpawnCount = 0;
	private int spawnPoolAt = 2;
	private Object waterPool;
	// Use this for initialization
	public void Start () 
	{
		waterPool = Resources.Load ("Prefabs/Terrain/Daylight Simple Water");
		lineList = new List<VectorLine> ();
		raycastBitch = new L_System_Field ();
	}
	
	// Update is called once per frame
	public void Update () 
	{
		ModifyLines ();
	}

	private void addLine()
	{	//Determine line position
		Vector2 offset = (Random.insideUnitCircle * columnRadius); 
		Vector2 xz = new Vector2(transform.position.x, transform.position.z) + offset;
		float height;

		raycastBitch.rayCastingDetermination (xz, out height);
		float startHeight = Random.Range
			((transform.position.y*lineHeightProportion), transform.position.y);
		float endHeight = Random.Range(minLineHeight, startHeight - height);

		VectorLine line = VectorLine.SetLine3D(Color.blue, 
		                    	new Vector3(offset.x, startHeight - transform.position.y, offset.y),
		                                       new Vector3(offset.x, endHeight - transform.position.y, offset.y)); 

		GameObject lineObject = new GameObject ();

		lineObject.transform.position = transform.position;

		line.drawTransform = lineObject.transform;
		line.Draw3DAuto ();
		lineList.Add (line);
	}

	private void removeLine(VectorLine line)
	{
		VectorLine deadLine = line;
		lineList.Remove(deadLine);
		VectorLine.Destroy(ref deadLine, deadLine.drawTransform.gameObject);
	}

	private IEnumerator UpdateLines()
	{
		while(true)
		{
			yield return new WaitForSeconds(dropRefresh);

			if(lineList.Count >= fallingLines)
			{
				dropSpeed = dropSpeedBase;// + (dropSpeedBase*heightRatio); 
				removeLine(lineList[0]);
			}
			else
				addLine();
		}
	}

	public void DrawLines(bool on)
	{
		if(on)
		{
			raining = true;
			StartCoroutine("UpdateLines");
		}
		else
		{
			raining = false;
			StopCoroutine("UpdateLines");
		}
	}

	/*private IEnumerator ModifyLines()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.25f);
			for(int i = 0; i < lineList.Count; i++)
				if(i % 2 == 0)
					changeLineLength(lineList[i]);
				else
					transformLine(lineList[i]);
		}
	}*/

	private void ModifyLines()
	{
		for(int i = 0; i < lineList.Count; i++)
		{
				changeLineLength(lineList[i]);
				transformLine(lineList[i]);
		}
	}

	public void straightVerticalReferenceLine(Vector3 start, Vector3 finish)
	{
		lineList.Add(VectorLine.SetLine3D(Color.blue, start, finish));
	}

	void changeLineLength(VectorLine line)
	{
		if(line.GetLength() < lastHeight/2)
		{
			//line.
		}	//Increase Length
		else
		{}	//reduce length
	}

	void transformLine(VectorLine line)
	{
		GameObject lineObj = line.drawTransform.gameObject;
		Vector2 xz = new Vector2(lineObj.transform.position.x, lineObj.transform.position.z);
		float height;
		raycastBitch.rayCastingDetermination (xz, out height);

		{															//Destroy the bastard below terrain
			lineObj.transform.position -= Vector3.up * dropSpeed;
			if((lineObj.transform.position.y - (Vector3.Magnitude(lineObj.transform.position - line.GetPoint3D(0)))) 
			   						- height < 0)
			{
				removeLine(line);
				poolSpawnCount++;
			}
		}

		{		//But after destruction make a water pool
			if(poolSpawnCount == spawnPoolAt)
			{
				poolSpawnCount = 0;
				//Debug.Log (height);
				//Debug.Log (lineObj.transform.position.y);
				Vector3 poolPos = new Vector3(lineObj.transform.position.x, height, lineObj.transform.position.z);
				xz = Random.insideUnitCircle;
				poolPos += new Vector3(xz.x, 0.1f, xz.y);
				GameObject pool = (GameObject)Instantiate(waterPool, poolPos, Quaternion.identity);
				pool.GetComponent<WaterSimple>().raindropPool();
			}
		}
	}

	public void setLastHeight(float lh)
	{
		lastHeight = lh;
	}

	public void clearLines()
	{
		VectorLine line;
		foreach(VectorLine l in lineList)
		{
			line = l;
			VectorLine.Destroy(ref line, line.drawTransform.gameObject);
		}

		lineList.Clear ();
	}

	public bool IsRaining()
	{
		return raining;
	}

	public void exposeHeightRatio(float h)
	{
		heightRatio = h;
	}
}
