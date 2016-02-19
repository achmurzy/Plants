using UnityEngine;
using System.Collections;

public class dualFaceButtonController : BaseController {
	//AKA the hilariously complicated state machine
	const int FaceControls = 4;

	/*********FACE_CONTROLLER_VARIABLES**********/
	//We need button listeners for all eight buttons
	public float padHorizontal, padVertical;
	public bool padUp, padLeft, padRight, padDown;
	public bool cross, circle, triangle, square;

	private float moveInterval = 0.01f;
	private float dragCoefficienct = 0.005f;
	private float directionAngle = 0.1f;

	public Vector3 targetDirection;
	/********************************************/
	
	void Awake ()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		
		_animation = GetComponent<Animation>();
		_characterState = FaceControls;
		lastState = FaceControls;

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
		grounded = IsGrounded();
		
		//transform.localScale = new Vector3(1, 1, 1);
		
		// Forward vector relative to the player along the x-z plane	
		forward = transform.TransformDirection(Vector3.forward);
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
		targetDirection = moveDirection;

		if(_characterState == FaceControls || lastState == FaceControls)
		{
			circle = Input.GetButton("Circle");
			triangle = Input.GetButton("Triangle");
			square = Input.GetButton("Square");
			cross = Input.GetButton("Cross");

			padVertical = Input.GetAxisRaw("DPadVertical");
			padHorizontal = Input.GetAxisRaw("DPadHorizontal");

			if(padVertical > 0)
			{
				padUp = true;
				padDown = false;
			}
			else if(padVertical < 0)
			{
				padDown = true;
				padUp = false;
			}
			else
			{
				padDown = false;
				padUp = false;
			}

			if(padHorizontal > 0)
			{
				padRight = true;
				padLeft = false;
			}
			else if(padHorizontal < 0)
			{
				padLeft = true;
				padRight = false;
			}
			else
			{
				padLeft = false;
				padRight = false;
			}

			//Simple left-right, forward-back movement
			if(padUp && triangle)
			{
				targetDirection = forward;
				moveSpeed += moveInterval;
			}
			else if(padLeft && square)
			{
				targetDirection = -right;
				moveSpeed += moveInterval;
			}
			else if(padRight && circle)
			{
				targetDirection = right;
				moveSpeed += moveInterval;
			}
			else if(padDown && cross)
			{
				targetDirection = -forward;
				moveSpeed += moveInterval;
			}

			//Diagonal motion
			else if(padUp && circle || padRight && triangle)
			{
				targetDirection = forward+right;
				moveSpeed += moveInterval;
			}
			else if(padLeft && triangle || padUp && square)
			{
				targetDirection = forward-right;
				moveSpeed += moveInterval;
			}
			else if(padDown && square || padLeft && cross)
			{
				targetDirection = (-forward-right);
				moveSpeed += moveInterval;
			}
			else if(padRight && cross || padDown && circle)
			{
				targetDirection = (right-forward);
				moveSpeed += moveInterval;
			}

			if(targetDirection != moveDirection)
			{
				Debug.Log ("Rot-tote");
				Debug.Log ("MD: " + moveDirection);
				Debug.Log ("TD: " + targetDirection);
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed, 0.0f);
				Debug.Log ("MD: " + moveDirection);
				Debug.Log ("TD: " + targetDirection);
			}

			//Inverse Directions -- We would like to brake if it is pressed in the same axis as the current
			//direction of motion, but I would like different semantics for inverse inputs orthogonal to
			//the current direction of movement in the future.

			if(padUp && cross || padDown && triangle)
			{
				Debug.Log(Vector3.Dot(moveDirection, forward));
				if(Mathf.Abs(Vector3.Dot(moveDirection, forward)) > 0.5f)
					moveSpeed -= dragCoefficienct*10;
				else
				{}
				
			}
			else if(padLeft && circle || padRight && square)
			{
				Debug.Log(Vector3.Dot(moveDirection, right));
				if(Mathf.Abs(Vector3.Dot(moveDirection, right)) > 0.5f)
					moveSpeed -= dragCoefficienct*10;
				else
				{}
			}

			//D-Pad Diagonal Inputs -- These are Rotations
			else if(padDown && padLeft || padUp && padLeft)
			{
				transform.rotation = Quaternion.LookRotation
										(Quaternion.AngleAxis(-directionAngle, Vector3.up) * forward);
			}
			else if(padDown && padRight || padUp && padRight)
			{
				transform.rotation = Quaternion.LookRotation
										(Quaternion.AngleAxis(directionAngle, Vector3.up) * forward);
			}

			//What happens when multiple face buttons are pressed at the same time?

			moveSpeed -= dragCoefficienct;
			moveSpeed = Mathf.Clamp(moveSpeed, 0, runSpeed);
		}

		// Grounded controls
		else if (grounded)
		{
			// Target direction relative to the camera
			targetDirection = v * forward + h * right;
			
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
			
			_characterState = Idle;
			
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
			}
			
			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
		}
		// In air controls
		else
		{
			targetDirection = v * forward + h * right;
			
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
		/*if (Input.GetButtonDown ("Jump"))
		{
			lastJumpButtonTime = Time.time;
		}
		else if(Input.GetButtonUp("Jump") && !jumpCanceled && !jumpingReachedApex && IsJumping() && !doubleJumped)
		{
			jumpCanceled = true;
			jumpButtonLength = (Time.time - lastJumpButtonTime)*0.01f;
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
		/*if (IsGrounded())
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
		}*/	
		
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