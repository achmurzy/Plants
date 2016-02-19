using UnityEngine;
using System.Collections;

public class alternatingShoulderController : BaseController {
	
	const int ShoulderControl = 4;

	/*********SHOULDER-MOVEMENT VARIABLES*************/
	public bool leftPressed = true;
	public float currentAxisRaw, lastAxisRaw;
	public bool leftDown, rightDown, lastLeft, lastRight;
	public float shoulderInput = 0;

	private float speedDamp = 0.01f;
	private float velocityBonus = 2.5f;

	private float maxInputGap = 0.1f;
	private float inputGap = 5f;		//You're going to want to
	public float inputBuffer = 0.1f;	//represent these graphically

	public bool axisReleased;
	public bool leftReleased, rightReleased;
	/***********************************************************/

	void Awake ()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		
		_animation = GetComponent<Animation>();
		_characterState = ShoulderControl;
		lastState = ShoulderControl;
		
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
		
		if(ShoulderMovement() || lastState == ShoulderControl)
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
		
			lastAxisRaw = currentAxisRaw;
			currentAxisRaw = Input.GetAxis("Z-Axis/Shoulders");

			if(inputBuffer < 0)
			{
				if(currentAxisRaw == 0 && !axisReleased && lastAxisRaw != 0) //Value has snapped back to 0
				{
					inputBuffer = inputGap;
					Debug.Log ("Nope");
					//lastJumpButtonTime = Time.time;
				} //we assume it is from negative input, i.e. both triggers pressed; do nothing.
				//we cannot assume it is from negative input anymore, it is triggering too often.
				//It may be a good solution simply to tie this activity to jumping to discourage it
				//We need to know when negative input has occured rather than input simply falling to 0
				else
				{
					//We need to know when the button has been
					//released and the value is falling back to 0.
					axisReleased = false;
					
					if(currentAxisRaw > 0)
						if(currentAxisRaw < lastAxisRaw)
					{
						axisReleased = true;
						shoulderInput = lastAxisRaw;
					}
					//We capture the shoulderInput when the player releases the axis in
					//order to get pressure-sensitive velocity gains.
					if(currentAxisRaw < 0)
						if(currentAxisRaw > lastAxisRaw)
					{
						axisReleased = true;
						shoulderInput = lastAxisRaw;
					}
				}
				
				//We need to know when the axis buttons are released
				if(axisReleased)
				{	
					//Check which shoulder is supposed to be pressed
					if(leftPressed)
					{
						//See if the correct shoulder is indeed pressed
						//Apply velocity and direction if so
						if(shoulderInput > 0)
						{
							leftPressed = !leftPressed;
							moveSpeed += (velocityBonus * ((moveSpeed+1)/runSpeed) * shoulderInput);
							//moveDirection += right * (shoulderInput/10);
							inputBuffer = inputGap;
							Debug.Log("Yep");
						}
					}
					else 	//It should be the case that speed is harder to garner at higher velocities
					{
						if(shoulderInput < 0)
						{
							leftPressed = !leftPressed;
							moveSpeed += (velocityBonus * ((moveSpeed+1)/runSpeed) * -shoulderInput);
							//moveDirection += right * (shoulderInput/10);
							inputBuffer = inputGap;
							Debug.Log("Yep");
						}
					}
				}			//Changing direction on velocity gain is totally pointless right now because the
			}				//Difference is always made up on activating the other trigger. It would make sense 
			//Only if the triggers did not have to be activated alternately.
			/*
			lastLeft = leftDown;
			leftDown = Input.GetButton("LeftShoulder1");
			lastRight = rightDown;
			rightDown = Input.GetButton("RightShoulder1");


			if(lastLeft)
				if(!leftDown)
				{
					leftReleased = true;
					Debug.Log ("LeftReleased");
				}

			if(lastRight)
				if(!rightDown)
				{
					rightReleased = true;
					Debug.Log ("RightReleased");
				}

			if(leftPressed)
			{
				if(rightReleased)
				{
					leftPressed = !leftPressed;
					moveSpeed += velocityBonus * ((moveSpeed+1)/runSpeed);
					Debug.Log ("Movin");
				}
			}
			else
				if(leftReleased)
				{
					leftPressed = !leftPressed;
					moveSpeed += velocityBonus * ((moveSpeed+1)/runSpeed);
					Debug.Log ("Sailin");
				}
			
			leftReleased = false;
			rightReleased = false;
			*/

			//Drag coefficient increases as our speed does 
			float velocityDamp = moveSpeed/runSpeed;
			moveSpeed -= speedDamp * velocityDamp;
			//Increase inputGap based on velocityDamp here also
			inputGap = maxInputGap * velocityDamp;

			 
			inputBuffer -= Time.deltaTime;

			if(moveSpeed > 0.01f)
				_characterState = ShoulderControl;
			else
			{
				_characterState = Idle;
				lastState = ShoulderControl;
			}

			//Very important line
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
		
		/*if(Input.GetButtonDown("StateToggle"))
		{
			if(ShoulderMovement() || lastState == ShoulderControl)
			{
				lastState = 0;
				_characterState = Idle;
			}
			else
			{
				_characterState = ShoulderControl;
				lastState = ShoulderControl;
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
	
	public bool ShoulderMovement() 
	{
		return _characterState == ShoulderControl;
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