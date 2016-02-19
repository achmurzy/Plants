using UnityEngine;
using System.Collections;

public class rotatingStickController : BaseController {

	const int StickMovement = 4;

	/*********STICK-MOVEMENT ROTATION VARIABLES*************/
	public float lastRadians;
	private float fixationDamp = 0.1f;
	public float RotationResolution = Mathf.PI/4;
	public float RotationSkip = Mathf.PI/2;

	public float slideSpeed = 0.005f;

	public float rotationalDistance = 0.01f;
	public Vector3 targetDirection;

	public float fixedCameraSnapSpeed = 1000;
	public float fixedCameraSnapLag = 0.05f;
	/***********************************************************/
	
	void Awake ()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		
		_animation = GetComponent<Animation>();
		_characterState = StickMovement;
		lastState = StickMovement;
		
		if(!_animation)
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
		if(!idleAnimation) {
			_animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if(!walkAnimation) {
			_animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}
		if(!runAnimation) {
			_animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}
		if(!jumpPoseAnimation && canJump) {
			_animation = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
		}

		BaseCameraController fixedCam = GetComponent<fixedCamera> (); 
		if(fixedCam.enabled)
		{
			fixedCam.snapMaxSpeed = fixedCameraSnapSpeed;
			fixedCam.snapSmoothLag = fixedCameraSnapLag;
		}

		
	}
	
	/*Calculate moveDirection, moveSpeed, inAirVelocity based on user input,
	which compose the movement vector passed to CharacterController.Move() in Update()*/
	void UpdateSmoothedMovementDirection ()
	{
		Transform cameraTransform = Camera.main.transform;
		grounded = IsGrounded();
		
		//transform.localScale = new Vector3(1, 1, 1);
		
		// Forward vector relative to the camera along the x-z plane	
		forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		
		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		
		//Store unsmoothed keyboard input
		v = Input.GetAxis("Vertical");
		h = Input.GetAxis ("Horizontal");
		
		// Are we moving backwards or looking backwards
		if (v < -0.2)
			movingBack = true;
		else
			movingBack = false;
		
		bool wasMoving = isMoving;
		//Determine if keyboard input has occured (and hence movement as a result)
		isMoving = Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1;
		if(isMoving && !IsJumping())
		{
			_characterState = lastState;
		}

		if(StickControl() || lastState == StickMovement)
		{
			jumpCanceled = false;
			//targetSpeed = 0;
			
			v = Input.GetAxis("Mouse Y");
			float currentRadians = Mathf.Atan2(v, Input.GetAxis("Mouse X"));
			if(v < 0)
				currentRadians = (Mathf.PI*2) + currentRadians;
			
			if(isMoving)
			{
				float interval = currentRadians - lastRadians;
				
				if(interval < 0)
				{
					moveSpeed -= fixationDamp * 2;
					lastRadians = currentRadians;
				}
				else if(interval > RotationSkip)
					lastRadians = currentRadians;
				else if(interval > RotationResolution)
				{
					Debug.Log (currentRadians + ", " + lastRadians);
					lastRadians = currentRadians;
					moveSpeed += 0.1f;
					moveSpeed += moveSpeed * 0.1f;
				}
			}
			else
			{
				moveSpeed -= fixationDamp;
				lastRadians = currentRadians;
			}
			
			// Lock camera for short period when transitioning moving & standing still
			lockCameraTimer += Time.deltaTime;
			if (isMoving != wasMoving)
				lockCameraTimer = 0.0f;
			
			// We store speed and direction seperately,
			// so that when the character stands still we still have a valid forward direction
			// moveDirection is always normalized, and we only update it if there is user input.
			h = Input.GetAxis("Z-Axis/Shoulders");
			
			// Target direction relative to the camera
			Vector3 slideDirection = Vector3.Cross(moveDirection, Vector3.up);

			if(isMoving)
				transform.position -= slideDirection * h * slideSpeed;

			targetDirection = moveDirection;

			if(Input.GetButton("LeftShoulder1"))
			{
				targetDirection = 
					Quaternion.AngleAxis(-rotationalDistance, Vector3.up) * moveDirection;
			}
			else if(Input.GetButton("RightShoulder1"))
			{
				targetDirection = 
					Quaternion.AngleAxis(rotationalDistance, Vector3.up) * moveDirection;
			}
			//else 	//Do nothing and wait for a valid interval or for the player to overshoot
			
			/*targetSpeed = 0;
			if(v > 0)
				targetSpeed = moveSpeed + v; 
			else if(v < 0)
				targetSpeed = moveSpeed - 2.0f;
			else
				targetSpeed = moveSpeed - 0.1f;
				*/
			
			//If we are really slow, just snap to the target direction
			if (moveSpeed < 5.0f && grounded)
			{
				//You can't change direction without a little bit of speed
				//moveDirection = targetDirection.normalized;
			}
			// Otherwise smoothly turn towards it
			else
			{
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, 
					Mathf.Abs(h) * (1000*rotateSpeed/(moveSpeed + 0.1f)) * Mathf.Deg2Rad * Time.deltaTime, 1000);
				
				moveDirection = moveDirection.normalized;
			}
			
			
			// Smooth the speed based on the current target direction
			//float curSmooth = speedSmoothing * Time.deltaTime;
			
			if(moveSpeed > 0.1f)
				_characterState = StickMovement;
			else
			{
				lastState = StickMovement;
				_characterState = Idle;
			}
			
			//moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
			
			moveSpeed = Mathf.Clamp (moveSpeed, 0.0f, runSpeed);
		}
		// Grounded controls
		else if (grounded)
		{
			// Target direction relative to the camera
			Vector3 targetDirection = v * forward + h * right;
			
			lockCameraTimer += Time.deltaTime;
			
			if (isMoving != wasMoving)
			{
				lockCameraTimer = 0.0f;
			}
			
			if (targetDirection != Vector3.zero) //i.e. if v or h is not 0, meaning there has been input
			{
				//If we are really slow, just snap to the target direction
				if (moveSpeed < 5.0f && grounded)
				{
					moveDirection = targetDirection.normalized;
				}
				// Otherwise smoothly turn towaif(rds it
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, Mathf.Abs(h) * rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
					
					moveDirection = moveDirection.normalized;
				}
			}
			
			targetSpeed = 0;
			
			targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
			
			float curSmooth = speedSmoothing * Time.deltaTime;
			
			if(moveSpeed > 0.1f)
			{
				if (Input.GetButton("Sprint"))
				{
					targetSpeed *= runSpeed;
					_characterState = Running;
				}
				else
				{
					targetSpeed *= walkSpeed;
					_characterState = Walking;
				}
				lastState = _characterState;
			}
			else
				_characterState = Idle;
			
			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
		}
		// In air controls
		else
		{
			Vector3 targetDirection = v * forward + h * right;
			
			//We need to figure out when we are falling e.g. when we've walked off a ledge because collision flags are complete and utter shit
			if((_characterState == Idle || _characterState == Walking || _characterState == Running) && verticalSpeed <= -5.0f)
				_characterState = Jumping;
			
			if (isMoving)
			{
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, Mathf.Abs(h) * rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				moveDirection = moveDirection.normalized;
			}
		}
	}
	
	void ApplyJumping ()
	{
		// Prevent jumping too fast after each other
		if (lastJumpTime + jumpRepeatTime > Time.time)
		{
			return;
		}
		
		if (IsGrounded() || (IsJumping() && !doubleJumped)) {
			// Jump
			// - Only when pressing the button down
			// - With a timeout so you can press the button slightly before landing		
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout) {
				verticalSpeed = CalculateJumpVerticalSpeed (jumpHeight);
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	
	void ApplyGravity ()
	{
		if (isControllable)	// don't move player at all if not controllable.
		{
			// Apply gravity
			//var jumpButton = Input.GetButton("Jump");
			
			
			// When we reach the apex of the jump we send out a message
			if (IsJumping() && !jumpingReachedApex && verticalSpeed <= 1.0)
			{
				jumpingReachedApex = true;
				jumpCanceled = false;
				SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}
			
			if (IsGrounded ())
			{
				verticalSpeed = 0.0f;
			}
			else if(jumpCanceled)
			{
				verticalSpeed = (gravity*Time.deltaTime)*Mathf.SmoothDamp(verticalSpeed, 0.0f, ref jumpCancelArc, jumpButtonLength);
				jumpCanceled = false;
				jumpingReachedApex = true;
			}
			else
			{
				verticalSpeed -= gravity * Time.deltaTime;
				
			}
		}
	}
	
	float CalculateJumpVerticalSpeed (float targetJumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * targetJumpHeight * gravity);
	}
	
	void DidJump ()
	{
		if (IsJumping ())
			doubleJumped = true;
		jumpingReachedApex = false;
		jumpCanceled = false;
		lastJumpTime = Time.time;
		lastJumpButtonTime = -10;
		
		_characterState = Jumping;
	}
	
	void Update() {
		
		if (!isControllable)
		{
			// kill all inputs if not controllable.
			Input.ResetInputAxes();
		}
		if (Input.GetButtonDown ("Jump"))
		{
			lastJumpButtonTime = Time.time;
		}
		else if(Input.GetButtonUp("Jump") && !jumpCanceled && !jumpingReachedApex && IsJumping() && !doubleJumped)
		{
			jumpCanceled = true;
			jumpButtonLength = (Time.time - lastJumpButtonTime)*0.01f;
		}

		/*
		if(Input.GetButtonDown("StateToggle"))
		{
			if(StickControl() || lastState == StickMovement)
			{
				lastState = 0;
				_characterState = Idle;
			}
			else
			{
				_characterState = StickMovement;
				lastState = StickMovement;
			}
		}*/
		
		UpdateSmoothedMovementDirection();
		
		// Apply gravity
		// - extra power jump modifies gravity
		// - controlledDescent mode modifies gravity
		ApplyGravity ();
		
		// Apply jumping logic
		ApplyJumping ();
		
		// Calculate actual motion
		Vector3 movement = moveDirection * moveSpeed + new Vector3 (0, verticalSpeed, 0) /*+ inAirVelocity*/;
		movement *= Time.deltaTime;
		
		// Move the controller
		CharacterController  controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		
		// ANIMATION sector
		if(_animation) {
			if(_characterState == Jumping) 
			{
				if(!jumpingReachedApex) {
					_animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
					_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name);
				} else {
					_animation[jumpPoseAnimation.name].speed = -landAnimationSpeed;
					_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					_animation.CrossFade(jumpPoseAnimation.name);				
				}
			} 
			else 
			{
				if(controller.velocity.sqrMagnitude < 0.1) {
					_animation.CrossFade(idleAnimation.name);
				}
				else 
				{
					if(_characterState == Running) {
						_animation[runAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, runMaxAnimationSpeed);
						_animation.CrossFade(runAnimation.name);	
					}
					else if(_characterState == Walking) {
						_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0.0f, walkMaxAnimationSpeed);
						_animation.CrossFade(walkAnimation.name);	
					}
					
				}
			}
		}
		// ANIMATION sector
		
		// Set rotation to the move direction
		if (IsGrounded())
		{
			transform.rotation = Quaternion.LookRotation(moveDirection);
		}	
		else
		{
			Vector3 xzMove = movement;
			xzMove.y = 0;
			if (xzMove.sqrMagnitude > 0.001)
			{
				transform.rotation = Quaternion.LookRotation(xzMove);
			}
		}	
		
		// We are in jump mode but just became grounded
		if (IsGrounded())
		{
			inAirVelocity = Vector3.zero;
			if (IsJumping ())
			{
				//jumping = false;
				doubleJumped = false;
				SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		
	}
	
	float GetSpeed () {
		return moveSpeed;
	}

	public bool StickControl() 
	{
		return _characterState == StickMovement;
	}
	
	/*Uses CollisionFlag bitmask xxxx (None(0,1), Sides(0,1), Above(0,1), Below(0,1)) and 
	performs AND comparison with 0001. If Below is 0, the operation returns 0 and the
	character is believed to be in the air. If it is 1, the bitmask should be xxx1, meaning
	a collision has occured below the controller*/
	bool IsGrounded () {
		return ((collisionFlags & CollisionFlags.CollidedBelow) != 0);
	}
	
	Vector3 GetDirection () {
		return moveDirection;
	}
	
	bool IsMoving ()
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
	}
	
	bool HasJumpReachedApex ()
	{
		return jumpingReachedApex;
	}
	
	void Reset ()
	{
		gameObject.tag = "Player";
	}
}