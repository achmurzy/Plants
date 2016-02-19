using UnityEngine;
using System.Collections;

public class stickyStickCamera : BaseCameraController 
{	
	//STICKY CAMERA
	public bool horizontalStuck = false;
	public bool verticalStuck = false;
	
	public bool stickyAngularPositive;
	public bool stickyDistancePositive;
	
	public bool HnotOne;
	public bool VnotOne;
	
	private float cannonicalstickyAngularReductionFactor = 1f;
	public float stickyAngularReductionFactor;
	private float stickyAngularReductionFactorReductionFactor;
	private float stickyDistanceReductionFactor = 0.0001f;
	public float stickyAngularSpeed;
	public float stickyDistanceSpeed;
	/*********************************************/
	
	void  Awake (){
		if(!cameraTransform && Camera.main)
			cameraTransform = Camera.main.transform;
		if(!cameraTransform) {
			Debug.Log("Please assign a camera to the ThirdPersonCamera script.");
			enabled = false;	
		}
		
		//Determine which character controller script is in use and its camera control scheme 
		_target = transform;
		if (_target)
		{
			foreach(BaseController cc in this.GetComponents<BaseController>())
				if(cc.enabled)
			{
				controller = cc;
				break;
			}
		}
		
		if (controller)
		{
			CharacterController characterController = (CharacterController)_target.GetComponent<Collider>();
			centerOffset = characterController.bounds.center - _target.position;
			headOffset = centerOffset;
			headOffset.y = characterController.bounds.max.y - _target.position.y;
			
			bones = SearchHierarchyForBone(transform, "Bip001 Pelvis").gameObject;
			wrench = SearchHierarchyForBone(transform, "wrench").gameObject;
		}
		else
			Debug.Log("Please assign a target to the camera that has a ThirdPersonController script attached.");
		
		Cut(_target, centerOffset);
	}
	
	void  DebugDrawStuff (){
		Debug.DrawLine(_target.position, _target.position + headOffset);
		
	}
	
	float  AngleDistance ( float a ,   float b  ){
		a = Mathf.Repeat(a, 360);
		b = Mathf.Repeat(b, 360);
		
		return Mathf.Abs(b - a);
	}
	
	void  Apply ( Transform dummyTarget ,   Vector3 dummyCenter  ){
		// Early out if we don't have a target
		if (!controller)
			return;
		
		Vector3 targetCenter= _target.position + centerOffset;
		Vector3 targetHead= _target.position + headOffset;

		vee = Input.GetAxis ("Mouse Y");
		eych = Input.GetAxis ("Mouse X");
		
		/*if(Input.GetButtonDown ("1st-person") && !inFirstPerson)
		{
			inFirstPerson = true;
			//Save the properties of the camera
			saveDistance = distance;
			saveHeight = height;
			savePosition = cameraTransform.position;
		}
		else if(Input.GetButtonDown ("1st-person"))
		{
			inFirstPerson = false;
			distance = saveDistance;
			height = saveHeight;
			cameraTransform.position = savePosition;
			
			bones.GetComponent<SkinnedMeshRenderer>().enabled = true;
			wrench.GetComponent<MeshRenderer>().enabled = true;
		}
		
		if(inFirstPerson)
		{
			//Make the player invisible
			bones.GetComponent<SkinnedMeshRenderer>().enabled = false;
			wrench.GetComponent<MeshRenderer>().enabled = false;
			//Save the properties of the camera
			distance = 0;
			height = 0;
			cameraTransform.position = targetHead;
			
			rotationX = cameraTransform.localEulerAngles.y + eych * angularMaxSpeed * 0.1f;
			
			rotationY += vee * angularMaxSpeed * 0.1f;
			rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
			
			cameraTransform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}
		else*/
		{
			if(Mathf.Abs(eych) != 1)
			{
				distance += (3*-vee*distanceSmooth);
				height += (-vee*distanceSmooth);
				HnotOne = true;
			}
			else
			{
				HnotOne = false;
				horizontalStuck = true;
				stickyAngularSpeed = eych*angularMaxSpeed;
				stickyAngularReductionFactor = cannonicalstickyAngularReductionFactor;
				
				if(stickyAngularSpeed > 0)
					stickyAngularPositive = true;
				else
					stickyAngularPositive = false;
			}
			
			if(verticalStuck && VnotOne)
			{
				distance += (3*stickyDistanceSpeed);
				height += (stickyDistanceSpeed);
				
				if(stickyDistancePositive)
				{
					stickyDistanceSpeed -= stickyDistanceReductionFactor;
					if(stickyDistanceSpeed < 0)
						verticalStuck = false;
				}
				else
				{
					stickyDistanceSpeed += stickyDistanceReductionFactor;
					if(stickyDistanceSpeed > 0)
						verticalStuck = false;
				}
			}
			
			
			if (distance < minDist)
				distance = minDist;
			if (distance > maxDist)
				distance = maxDist;
			
			if (height < minHeight)
				height = minHeight;
			if (height > maxHeight)
				height = maxHeight;
			
			// Calculate the current & target rotation angles
			float originalTargetAngle= _target.eulerAngles.y;		
			currentAngle=cameraTransform.eulerAngles.y;
			
			targetAngle = originalTargetAngle;
			// Adjust real target angle when camera is locked
			
			
			if (Input.GetButtonDown("Snap"))
			{
				snap = true;
			}
			
			if (snap)
			{
				// We are close to the target, so we can stop snapping now!
				if (AngleDistance (currentAngle, originalTargetAngle) < 3.0f || 
				    (originalTargetAngle == 0 && currentAngle < 0.001f))
					snap = false;
				
				
				currentAngle = Mathf.SmoothDampAngle(currentAngle, originalTargetAngle, ref angleVelocity, snapSmoothLag, snapMaxSpeed);
			}
			// Normal camera motion
			else 
			{
				targetAngle = currentAngle;
				
				if(Mathf.Abs(vee) != 1)
				{
					//We need to set the target angle based on input more intelligently
					targetAngle -= (eych*angularMaxSpeed); 
					targetAngle = targetAngle % 360;
					VnotOne = true;
				}
				else
				{
					VnotOne = false;
					verticalStuck = true;
					stickyDistanceSpeed = -vee*distanceSmooth;
					if(stickyDistanceSpeed > 0)
						stickyDistancePositive = true;
					else
						stickyDistancePositive = false;
				}
				
				if(horizontalStuck && HnotOne)
				{
					targetAngle -= stickyAngularSpeed;
					
					stickyAngularReductionFactor = Mathf.SmoothDamp(stickyAngularReductionFactor, 0,
					                                                ref stickyAngularReductionFactorReductionFactor, 0.75f); 
					if(stickyAngularPositive)
					{
						stickyAngularSpeed -= stickyAngularReductionFactor;
						if(stickyAngularSpeed < 0)
							horizontalStuck = false;
					}
					else
					{
						stickyAngularSpeed += stickyAngularReductionFactor;
						if(stickyAngularSpeed > 0)
							horizontalStuck = false;
					}
				}
				
				if (controller.GetLockCameraTimer () < lockCameraTimeout)
				{
					targetAngle = currentAngle;
				}
				
				// Lock the camera when moving backwards!
				// * It is really confusing to do 180 degree spins when turning around.
				if (AngleDistance (currentAngle, targetAngle) > 160 && controller.IsMovingBackwards ())
					targetAngle += 180;
				
				currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angleVelocity, angularSmoothLag, angularMaxSpeed);
			}
			
			
			// When jumping don't move camera upwards but only down!
			if (controller.IsJumping ())
			{
				// We'd be moving the camera upwards, do that only if it's really high
				float newTargetHeight= targetCenter.y + height;
				if (newTargetHeight < targetHeight || newTargetHeight - targetHeight > 5)
					targetHeight = targetCenter.y + height;
			}
			// When walking always update the target height
			else
			{
				targetHeight = targetCenter.y + height;
			}
			
			// Damp the height
			//float currentHeight= height;
			//currentHeight = Mathf.SmoothDamp (currentHeight, targetHeight, ref heightVelocity, heightSmoothLag);
			
			// Convert the angle into a rotation, by which we then reposition the camera
			Quaternion currentRotation= Quaternion.Euler (0, currentAngle, 0);
			
			// Set the position of the camera on the x-z plane to:
			// distance meters behind the target
			cameraTransform.position = targetCenter;								
			cameraTransform.position += currentRotation * Vector3.back * distance;
			
			// Set the height of the camera
			cameraTransform.position = new Vector3(cameraTransform.position.x, targetHeight, cameraTransform.position.z);
			
			//Perform a ray cast and set the camera distance based on this 
			//This is to prevent objects from appearing between the camera and the player
			/*if(Physics.Raycast (targetCenter, cameraTransform.position - targetCenter, out info, distance))
			{
				rayTime = Time.time;
				if(reached)
				{
					saveDistance = distance;
					saveHeight = height;
					reached = false;
					savePosition = cameraTransform.position;
				}
				
				Debug.Log ("ass");
				
				distance = info.distance;

				height = info.point.y - targetCenter.y;
				
				cameraTransform.position = targetCenter;								
				cameraTransform.position += currentRotation * Vector3.back * distance;
				cameraTransform.position = new Vector3(cameraTransform.position.x, info.point.y, cameraTransform.position.z);
				startPosition = cameraTransform.position;
			}
			else if(!reached && Time.time - rayTime > 0.5f)
			{
				Debug.Log ("oughta b movin");
				//Mathf.SmoothDamp(distance, saveDistance, ref heightVelocity, 1.0f);
				//Mathf.SmoothDamp (height, saveHeight, ref heightVelocity, 1.0f);
				//cameraTransform.position = savePosition;
				
				distance += lerpIteration * (saveDistance/saveHeight);
				height += lerpIteration;
				
				if(saveDistance <= distance && saveHeight <= height)
				{
					reached = true;
				}
			}*/
			
			// Always look at the target	
			SetUpRotation(targetCenter, targetHead);
		}
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
	
	void  SetUpRotation ( Vector3 centerPos ,   Vector3 headPos  ){
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
		Vector3 cameraPos= cameraTransform.position;
		Vector3 offsetToCenter= centerPos - cameraPos;
		
		// Generate base rotation only around y-axis
		Quaternion yRotation= Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));
		
		Vector3 relativeOffset= Vector3.forward * distance + Vector3.down * height;	//We assume the camera is always above, so we use Vector3.down. Must this be so?
		cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);	
		
		// Calculate the projected center position and top position in world space
		Ray centerRay= cameraTransform.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 1f));
		Ray topRay= cameraTransform.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, clampHeadPositionScreenSpace, 1f));
		
		Vector3 centerRayPos= centerRay.GetPoint(distance);
		Vector3 topRayPos= topRay.GetPoint(distance);
		
		float centerToTopAngle= Vector3.Angle(centerRay.direction, topRay.direction);
		
		float heightToAngle= centerToTopAngle / (centerRayPos.y - topRayPos.y);
		
		float extraLookAngle= heightToAngle * (centerRayPos.y - centerPos.y);
		if (extraLookAngle < centerToTopAngle)
		{
			extraLookAngle = 0;
		}
		else
		{
			extraLookAngle = extraLookAngle - centerToTopAngle;
			cameraTransform.rotation *= Quaternion.Euler(-extraLookAngle, 0, 0);
		}
	}
	
	Vector3  GetCenterOffset (){
		return centerOffset;
	}
}