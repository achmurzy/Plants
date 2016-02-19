using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	public Transform playerTransform;
	public Transform sunTransform;
	public Transform realSunTransform;
	public Transform lightTransform;
	private float orbitSpeed = -0.5f;
	private float rotateSpeed = -5.0f;
	private float orbitMinimum = 70.0f;
	private float orbitMaximum = 110.0f;
	private float orbitDistance = 2000.0f;
	private Vector3 orthogonalToRight;

	private Vector3 scaleStep = Vector3.one*0.0001f;
	private float scaleParameter = 0.0f;
	private float scalePerturbSpeed = 1.0f;

	// Use this for initialization
	void Start () 
	{
		orthogonalToRight = transform.forward;
		transform.Rotate (0, -45, 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate (transform.right * Time.deltaTime * orbitSpeed, Space.World);

		sunTransform.position = transform.position + transform.up * orbitDistance;
		sunTransform.LookAt (playerTransform);

		lightTransform.transform.LookAt (transform.position);
		realSunTransform.Rotate (sunTransform.forward * Time.deltaTime * rotateSpeed, Space.World);
		getAngle ();
		perturbRayScales ();
	}

	void getAngle()
	{
		float orbitAngle = Vector3.Angle(this.transform.position - sunTransform.position,
		                           		orthogonalToRight);

		if(orbitAngle < orbitMinimum || orbitAngle > orbitMaximum)
		{
			orbitSpeed = -orbitSpeed;
			rotateSpeed = -rotateSpeed;
		}
	}

	void perturbRayScales()
	{
		realSunTransform.localScale += scaleStep*scalePerturbSpeed;
		scaleParameter += Time.deltaTime*scalePerturbSpeed;
		if(scaleParameter >= 1)
		{
			scaleParameter = 1;
			scalePerturbSpeed = -scalePerturbSpeed;
		}
		else if(scaleParameter <= 0)
		{
			scaleParameter = 0;
			scalePerturbSpeed = -scalePerturbSpeed;
		}
	}
}
