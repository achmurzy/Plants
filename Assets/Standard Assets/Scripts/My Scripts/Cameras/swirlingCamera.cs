using UnityEngine;
using System.Collections;

//A Shoulder Axis Camera with First-Person, Snapping, and Environment Collision capabilities
public class swirlingCamera : BaseCameraController 
{	
	//*****************GENERAL VARIABLES***************************************//
	private swirlingJumpController sJC;
	private Vector3 targetCenter;
	private float offSetStep;
	private float offSetSwitchSpeed;

	private bool animatingStateChange = false;
	private float slerpParameter = 0.0f;
	private Quaternion slerpStart;

	private int state;
	private int lastState;

	//****************************FOCUSING THE CAMERA***************************//
	private bool focus = false;
	private Transform focusObject;
	private float focusDistance = 25.0f;
	private float focusHeight = -3;
	private float focusGazeChangeSpeed = 1.0f;
	private Vector3 offsetAxis = Vector3.back;

	//*****************GROUNDED CAMERA VARIABLES*******************************//
	private float groundedHeight = 5.0f;
	private float groundedDistance = 10.0f;
	private float swirlingToGroundSpeed = 1.0f;
	private float selectingToGroundSpeed = 1.0f;

	//*****************SELECTION CAMERA VARIABLES******************************//
	private float selectionHeight = 1.0f;
	private float selectionDistance = 0.75f;
	private float selectionOffsetX = 0.0f;
	private float selectionOffsetY = 1.0f;
	private float selectionOffsetZ = -0.1f;
	private Vector3 selectionOffset;
	private float cameraSelectionDistanceSpeed = 1.0f;

	//*****************SWIRLING CAMERA VARIABLES*******************************//	 
	private float swirlingHeightBase = 5.0f;
	private float swirlingDistanceBase = 10.0f;
	private float swirlingHeightBonus = 35.0f;
	private float swirlingDistanceBonus = 30.0f;
	private float swirlingHeight = 0;
	private float swirlingDistance = 0;		
	private float cameraSwirlingTransitionSpeed = 2.0f;

	void  Awake ()
	{
		//Link necessary transforms and components
		if(!cameraTransform && Camera.main)
			cameraTransform = Camera.main.transform;
		if(!cameraTransform) {
			Debug.Log("Please assign a camera to the ThirdPersonCamera script.");
			enabled = false;	
		}
		minimumCameraDistance = 10.0f;
		_target = transform;
		//Determine which character controller script is in use and its camera control scheme 
		controller = GetComponent<swirlingJumpController> ();
		sJC = controller as swirlingJumpController;

		CharacterController characterController = (CharacterController)_target.GetComponent<Collider>();
		centerOffset = characterController.bounds.center - _target.position;
		headOffset = centerOffset;
		headOffset.y = characterController.bounds.max.y - _target.position.y;

		targetCenter= _target.position + centerOffset;

		height = groundedHeight;
		distance = groundedDistance;

		offSetStep = 0;
		offSetSwitchSpeed = selectingToGroundSpeed * 0.0005f;
		angularSmoothLag = 0.0095f;

		//Calculate initial Position
		Cut(_target, centerOffset);

		//Initialize state variables
		castLayer = 2299;
	}
	
	void  Apply ( Transform dummyTarget ,   Vector3 dummyCenter  ){
		// Early out if we don't have a target
		if (!controller)
			return;
			lastState = state;
			state = sJC._characterState;
			
			offsetAxis = Vector3.back;

			if(focus)
			{
				offsetAxis = new Vector3(sJC.transform.position.x - focusObject.position.x, 0,
				                         sJC.transform.position.z - focusObject.position.z);
				offsetAxis.Normalize();
				if(!sJC.IsSelecting())
				{
					targetCenter = _target.position;
					distance = Mathf.SmoothDamp
						(distance, focusDistance, ref distanceVelocity, focusGazeChangeSpeed);
					height = Mathf.SmoothDamp
						(height, focusHeight, ref heightVelocity, focusGazeChangeSpeed);
					
					cameraTransform.position = targetCenter;								
					cameraTransform.position += offsetAxis * distance;
				}
				else
					sJC.selectionOff = true;
			} 
			else if(sJC.IsSwirling())
			{
				float originalTargetAngle= _target.eulerAngles.y;		
				float heightRatio = (sJC.transform.position.y/sJC.maxHeight);
				currentAngle=cameraTransform.eulerAngles.y;		//This is how we make the camera fixed -- 
				//have the target rotation angle be our target's rotation
				currentAngle = Mathf.SmoothDampAngle
					(currentAngle, originalTargetAngle, ref angleVelocity, snapSmoothLag, snapMaxSpeed);
				Quaternion currentRotation= Quaternion.Euler (0, currentAngle, 0);	//Convert the angle into a rotation, 
				//by which we then reposition the camera

				swirlingHeight = swirlingHeightBase + (swirlingHeightBonus * heightRatio);
				swirlingDistance = swirlingDistanceBase + (swirlingDistanceBonus * heightRatio);

				targetCenter = _target.position + centerOffset;
				distance = Mathf.SmoothDamp
							(distance, swirlingDistance, ref distanceVelocity, cameraSwirlingTransitionSpeed);
				height = Mathf.SmoothDamp
							(height, swirlingHeight, ref heightVelocity, cameraSwirlingTransitionSpeed);

				cameraTransform.position = targetCenter;								
				cameraTransform.position += currentRotation * offsetAxis * distance;
			}
			else if(sJC.IsSelecting())
			{
				if(state != lastState)
				{
					animatingStateChange = true;
					slerpParameter = 0.0f;
					slerpStart = cameraTransform.rotation;
				}
				updateOffset();
				targetCenter = _target.position + selectionOffset;
			
				Quaternion currentRotation= Quaternion.Euler (0, _target.eulerAngles.y, 0);

				distance = 
					Mathf.SmoothDamp(distance, selectionDistance, ref distanceVelocity, cameraSelectionDistanceSpeed);
				height = 
					Mathf.SmoothDamp(height, selectionHeight, ref heightVelocity, cameraSelectionDistanceSpeed);

				cameraTransform.position = targetCenter;								
				cameraTransform.position += currentRotation * offsetAxis * distance;
			}
			else
			{
				if(lastState == 7)
				{
					centerOffset = selectionOffset;
					offSetStep = 0;
					animatingStateChange = true;
					slerpParameter = 0.0f;
					slerpStart = cameraTransform.rotation;
				}

				float originalTargetAngle= _target.eulerAngles.y;		
				currentAngle=cameraTransform.eulerAngles.y;		
				//This is how we make the camera fixed -- 
				//have the target rotation angle be our target's rotation
				currentAngle = Mathf.SmoothDampAngle
					(currentAngle, originalTargetAngle, ref angleVelocity, snapSmoothLag, snapMaxSpeed);
				Quaternion currentRotation= Quaternion.Euler (0, currentAngle, 0);	
				//Convert the angle into a rotation, 
				//by which we then reposition the camera

				centerOffset = Vector3.Lerp(centerOffset, Vector3.zero, offSetStep);
				
				offSetStep+=Time.deltaTime*offSetSwitchSpeed;
				targetCenter = _target.position + centerOffset;

				distance = 
					Mathf.SmoothDamp(distance, groundedDistance, ref distanceVelocity, swirlingToGroundSpeed);
				height = 
					Mathf.SmoothDamp(height, groundedHeight, ref heightVelocity, swirlingToGroundSpeed);

				cameraTransform.position = targetCenter;								
				cameraTransform.position += transform.rotation * offsetAxis * distance;
			}
			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target, set the height to height above targetCenter
		
			cameraTransform.position = new Vector3(cameraTransform.position.x, 
			                                       		targetCenter.y + height, 
			                                       			cameraTransform.position.z);
		
		//Perform a ray cast and set the camera distance based on this 
		//This is to prevent objects from appearing between the camera and the player
		if(Physics.Raycast (targetCenter, cameraTransform.position - targetCenter, out info, distance, castLayer)
		   	&& !focus)
		{
			rayTime = Time.time;
			if(reached)
			{
				saveDistance = distance;
				saveHeight = height;
				reached = false;
				savePosition = cameraTransform.position;
			}
			if(info.collider.gameObject.tag == "Terrain" && info.distance < minimumCameraDistance)
			{}
			else
			{
				distance = info.distance;
				height = info.point.y - targetCenter.y;
				cameraTransform.position = targetCenter;								
				cameraTransform.position += transform.rotation * offsetAxis * (distance - 0.1f);
				cameraTransform.position = new Vector3(cameraTransform.position.x, height+targetCenter.y, cameraTransform.position.z);
				cameraTransform.position+=info.normal;
				startPosition = cameraTransform.position;
			}

		}
		else if((!reached && Time.time - rayTime > 0.5f))	 
		{												//This portion is meant to begin attempting to move the
														//camera out to its prior position after a collision
			distance += lerpIteration * (saveDistance/saveHeight); 	//teleports it closer to maintain vision
 			height += lerpIteration;
			if(saveDistance <= distance && saveHeight <= height)
			{
				reached = true;
			}
		}

		if(focus)
			targetCenter = focusObject.transform.position;
		SetUpRotation(targetCenter, targetCenter);
	}
	
	void  LateUpdate (){
		Apply (transform, Vector3.zero);
	}
	
	void  Cut ( Transform dummyTarget ,   Vector3 dummyCenter  ){
		float oldHeightSmooth= heightSmoothLag;
		float oldSnapMaxSpeed= snapMaxSpeed;
		float oldSnapSmooth= snapSmoothLag;
		
		snapMaxSpeed = 10000;
		snapSmoothLag = 0.001f;
		heightSmoothLag = 0.001f;
		
		snap = true;
		Apply (transform, Vector3.zero);
		
		heightSmoothLag = oldHeightSmooth;
		snapMaxSpeed = oldSnapMaxSpeed;
		snapSmoothLag = oldSnapSmooth;
	}
	
	void  SetUpRotation ( Vector3 centerPos ,   Vector3 headPos  )
	{
		// Now it's getting hairy. The devil is in the details here, the big issue is jumping of course.
		// * When jumping up and down we don't want to center the guy in screen space.
		//  This is important to give a feel for how high you jump and avoiding large camera movements.
		//   
		// * At the same time we dont want him to ever go out of screen and we want all rotations to be totally smooth.
		//
		// So here is what we will do:
		//
		// 1. We first find the rotation around the y axis. Thus he is always centered on the y-axis
		// 2. When grounded we make him be centered
		// 3. When jumping we keep the camera rotation but rotate the camera to get him back into view if his head is above some threshold
		// 4. When landing we smoothly interpolate towards centering him on screen
		Quaternion slerpGoal;
		if(!sJC.IsSelecting())
		{
			Vector3 cameraPos= cameraTransform.position;
			Vector3 offsetToCenter= centerPos - cameraPos;
			// Generate base rotation only around y-axis
			Quaternion yRotation= Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));
			
			Vector3 relativeOffset= Vector3.forward * distance + Vector3.down * height;	//We assume the camera is always above, 
			cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);
			slerpGoal = yRotation * Quaternion.LookRotation(relativeOffset);	//so we use Vector3.down. Must this be so?
			
			// Calculate the projected center position and top position in world space
			Ray centerRay= cameraTransform.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
			Ray topRay= cameraTransform.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, clampHeadPositionScreenSpace, 1f));
			
			Vector3 centerRayPos= centerRay.GetPoint(distance);
			Vector3 topRayPos= topRay.GetPoint(distance);
			
			float centerToTopAngle= Vector3.Angle(centerRay.direction, topRay.direction);
			
			float heightToAngle= centerToTopAngle / (centerRayPos.y - topRayPos.y);
			
			float extraLookAngle= heightToAngle * (centerRayPos.y - centerPos.y);
			if (extraLookAngle > centerToTopAngle)
			{
				extraLookAngle = extraLookAngle - centerToTopAngle;
				if(!focus)
					slerpGoal = cameraTransform.rotation * Quaternion.Euler(-extraLookAngle, 0, 0);
			}
		}
		else
		{
			slerpGoal = sJC.ML.gameObject.transform.rotation;
		}

		if(animatingStateChange)
		{
			sJC.togglePlayerControllable(false);
			slerpParameter += angularSmoothLag;
			if(slerpParameter >= 1)
			{
				animatingStateChange = false;
				sJC.togglePlayerControllable(true);
			}
			cameraTransform.rotation = Quaternion.Slerp
				(slerpStart, slerpGoal, slerpParameter);
		}
		else
		{
			cameraTransform.rotation = slerpGoal;
		}
	}

	void OnTriggerEnter(Collider cityStreet)
	{
		if(cityStreet.gameObject.name == "Avenue")
		{
			updateFocus(cityStreet.gameObject.GetComponent<AvenueTrigger> ().emblem.transform, 20.0f, -3);
		}
		else if(cityStreet.gameObject.name == "WaterPoolTrigger")
		{
			updateFocus(cityStreet.gameObject.transform, 20.0f, 5.0f);
		}
		/*else if(cityStreet.gameObject.name == "FunRockTrigger")
		{
			focusObject = cityStreet.gameObject.transform;
			focus = true;
			focusDistance = 2.0f;
			focusHeight = 2.0f;
		}
		else if(cityStreet.gameObject.name == "CliffTrigger")
		{
			focusObject = cityStreet.gameObject.transform;
			focus = true;
		}*/
	}

	void OnTriggerExit(Collider cityStreet)
	{
		if(cityStreet.gameObject.name == "Avenue" || cityStreet.gameObject.name == "WaterPoolTrigger")
		{
			cancelFocus();
		}
	}

	public void updateFocus(Transform focusTransform, float fdistance, float fheight)
	{
		focus = true;
		focusObject = focusTransform;
		focusDistance = fdistance;
		focusHeight = fheight;
	}

	public void cancelFocus()
	{
		focus = false;
	}

	Vector3  GetCenterOffset ()
	{
		return centerOffset;
	}

	void updateOffset()
	{
			selectionOffset = (selectionOffsetX * this.transform.right) +
								(selectionOffsetY * this.transform.up) +
									(selectionOffsetZ * this.transform.forward);
	}
	
	float  AngleDistance ( float a ,   float b  )
	{
		a = Mathf.Repeat(a, 360);
		b = Mathf.Repeat(b, 360);
		return Mathf.Abs(b - a);
	}
}