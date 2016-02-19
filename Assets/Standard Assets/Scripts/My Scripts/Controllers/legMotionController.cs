using UnityEngine;
using System.Collections;

public class legMotionController : BaseController {
	
	const int StickControl = 4;
	
	/*********STICK-PULLING VARIABLES*************/
	public float dot, leftForwardDot, rightForwardDot, leftSideDot, rightSideDot;
	public bool stride, strideOut, leftUp = false;
	private bool leftForward, rightForward;
	private float pairedAccceleration = 1f;
	private float drag = 0.01f;
	private float dragBonus = 0.001f;
	public Vector3 leftTargetDirection, rightTargetDirection;
	private float corridorWidth = 0.75f;
	/***********************************************************/
	
	void Awake ()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		
		_characterState = Idle;
		_animation = GetComponent<Animation>();
		
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
		
		bool wasMoving = isMoving;
		//Determine if keyboard input has occured (and hence movement as a result)
		isMoving = (Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1);
		if(isMoving && !IsJumping())
		{
			_characterState = lastState;
		}
		
		// Target direction relative to the camera
		leftTargetDirection = v * forward + h * right;
		
		v = Input.GetAxis ("Mouse Y");
		h = Input.GetAxis ("Mouse X");
		
		rightTargetDirection = v * forward + h * right;
		
		if(StickMovement() || lastState == StickControl)
		{
			rightForward = false;
			leftForward = false;

			if(leftTargetDirection == Vector3.zero && rightTargetDirection == Vector3.zero)
			{
				strideOut = false;	//Reenable striding when we reach neutrality
				stride = false;
			}
			else
			{
				leftTargetDirection.Normalize();
				rightTargetDirection.Normalize();

				dot = Vector3.Dot(leftTargetDirection, rightTargetDirection);
				leftForwardDot = Vector3.Dot(leftTargetDirection, forward);
				rightForwardDot = Vector3.Dot(rightTargetDirection, forward);
				leftSideDot = Vector3.Dot(leftTargetDirection, -right);
				rightSideDot = Vector3.Dot(rightTargetDirection, right);

				if(leftForwardDot > 0.8f)
					leftForward = true;
				if(rightForwardDot > 0.8f)
					rightForward = true;
			
				//We've been neutral and can start moving towards a stride
				if(!stride)
				{
					if(!strideOut)
					{
						if(Mathf.Abs(leftForwardDot) > 0 && Mathf.Abs(rightForwardDot) > 0)
							stride = true;
					}
				}
				else if(stride)
				{
					//Enforce the corridor
					if(Mathf.Abs(leftSideDot) < corridorWidth && Mathf.Abs(rightSideDot) < corridorWidth)
					{
						//SPEEDGETTING
						//Check which directions go where: left foot forward, right foot forward
						if(leftUp)
						{
							//Check the vertical axes to see if we can apply acceleration
							//Ideally a perfect "step" is straight forward on the stick,
							//or Vector3(0, 0, 1). So we dot the actual direction with this
							//vector and check that it doesn't diverge too much
							if(leftForward)
							{
								if(rightForwardDot < -0.8f)
								{
								//We're good for both -- Apply velocity
									Debug.Log ("Mover");
									leftUp = !leftUp;
									moveSpeed += pairedAccceleration;
								}
							}
						}
						else 
						{
							if(rightForward)
							{
								if(leftForwardDot < -0.8f)
								{
									//We're good for both
									leftUp = !leftUp;
									moveSpeed += pairedAccceleration;
									Debug.Log ("Mover");
								}
							}
						}
					}
					else 	//Punish those who stray
					{
						stride = false;
						strideOut = true;
					}
				}

				//DIRECTIONCHANGING
				if(leftForward)
				{
					//The left stick is forward but we aren't accelerating so perhaps we're turning
					if(rightSideDot > 0)
					{
						//Use the dot product to RotateTowards
						//The term (1 - dot) makes it easier to rotate at more divergent input directions
						moveDirection = Vector3.RotateTowards(moveDirection, rightTargetDirection, 
						            (1 - rightForwardDot) * rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
						Debug.Log ("Rottote");
					}
				}
				else if(rightForward)
				{
					if(leftSideDot > 0)
					{
						//Use the dot product to RotateTowards
						moveDirection = Vector3.RotateTowards(moveDirection, leftTargetDirection, 
						            (1 - leftForwardDot) * rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
						Debug.Log ("Rottote");
					}
				}
			}

			//A strange addition surely, but meant to kick in when they're trying to turn;
			 // turning more sharply yields a higher drag penalty.
			//Apply drag
			if(Mathf.Abs(dot) < 0.5)
				moveSpeed -= drag + dragBonus * (1 - dot);
			else
				moveSpeed -= drag;

		
			//Very important line
			moveSpeed = Mathf.Clamp (moveSpeed, 0.0f, runSpeed);
		}
		// Grounded controls
		else if (grounded)
		{	
			lockCameraTimer += Time.deltaTime;
			
			if (isMoving != wasMoving)
			{
				lockCameraTimer = 0.0f;
			}
			
			if (leftTargetDirection != Vector3.zero) //i.e. if v or h is not 0, meaning there has been input
			{
				//If we are really slow, just snap to the target direction
				if (moveSpeed < 5.0f && grounded)
				{
					moveDirection = leftTargetDirection.normalized;
				}
				// Otherwise smoothly turn towaif(rds it
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, leftTargetDirection, Mathf.Abs(h) * rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
					
					moveDirection = moveDirection.normalized;
				}
			}
			
			targetSpeed = 0;
			
			targetSpeed = Mathf.Min(leftTargetDirection.magnitude, 1.0f);
			
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
			//We need to figure out when we are falling e.g. when we've walked off a ledge because collision flags are complete and utter shit
			if((_characterState == Idle || _characterState == Walking || _characterState == Running) && verticalSpeed <= -5.0f)
				_characterState = Jumping;
			
			if (isMoving)
			{
				moveDirection = Vector3.RotateTowards(moveDirection, leftTargetDirection, Mathf.Abs(h) * rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
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
		
		if(Input.GetButtonDown("StateToggle"))
		{
			if(StickMovement() || lastState == StickControl)
			{
				lastState = 0;
				_characterState = Idle;
			}
			else
			{
				_characterState = StickControl;
				lastState = StickControl;
			}
		}
		
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
	
	public bool StickMovement() 
	{
		return _characterState == StickControl;
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