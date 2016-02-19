using UnityEngine;
using System.Collections;

public class swirlingJumpController : BaseController {	

	const int Hovering = 4;
	const int Swirling = 5;
	const int InWater = 6;
	const int Selecting = 7;
	const int Falling = 8;

	/**************SWIRLING_JUMP_VARIABLES**************/
	//Ground control variables
	public Vector3 lookDirection;
	public Vector3 targetDirection;
	private float lastH;

	public float dot;
	public bool selectionOff = false;
	private float forwardDotThreshold = 0.85f;
	private float bankingDotThreshold = 0.3f;
	private float cuttingDotThreshold = -0.8f;

	private float forwardAccelerationBonus = 0.25f;
	private float bankingAccelerationBonus = 0.1f;
	private float cuttingAccelerationPenalty = -0.1f;
	private float brakeMagnitude = -0.25f;

	private float forwardRotationalSpeed = 2.0f;
	private float bankingRotationalSpeed = 0.5f;
	private float cuttingRotationalSpeed = 1.5f;
	private float stationaryRotationalSpeed = 4.0f;

	private float groundDrag = 0.025f;
	public float currentAcceleration;
	private float maxAcceleration = 2.0f;

	private float minFallDistance = 2.0f;

	//Hovering variables
	private float hoverTimerLength = 3.0f;
	public float hoverTimer = 0.0f;
	private float hoverHeight;
	private float hoverCastDistance = 10000;
	private float lastCastDistance = 0.0f;
	private float lastHeight = 0.0f;
	private float hoverBonus = 3.0f;
	public bool oneRevolution, first, second, third;
	private Vector2 xyDir;
	private float swirlDotRange = 0.99f;
	//--Hovering activation in ground-state
		public float hoveringParameter;
		private Vector3 hoveringPosition;
		private float hoveringAnimationTransitionSpeed =0.1f;
		private float hoveringAnimationTransitionParameter = 0;
		private float hoveringThreshold = 10.0f;
		private float hoveringDegradation = 0.05f;
		private float hoveringSmooth = 0.2f;
		private float hoverBufferLength = 2.0f;
		private float hoverBuffer;
		private float swirlBufferLength = 0.25f;
		public float swirlBuffer;

	//Swirling variables
	private float swirlingAcceleration = 0.001f;
	private float accelerationEquilibrium = 0.05f;
	private float swirlingMagnitude = 0.25f;
	private float swirlingMagnitudeAbsolute = 0.25f;
	private float horizontalSwirlingSpeedMagnitude = 0.01f;
	private float fallingSpeedBonus = 1.1f;
	private float swirlingBrake = 5.0f;
	public float maxSwirlSpeed = 5.0f;
	private float terminalVelocity = -25.0f;
	private float swirlingGravityCoefficient = 0.075f;
	private float rotationalDistance = 0.5f;
	private float swirlingRotateSpeed = 100.0f;
	private Vector3 lookTarget;
	public float maxHeight = 50.0f;
	private bool gravityPop = false;	//By popular demand

	//Stick swirling variables
	public float lastRadians, currentRadians, interval, intervalBuffer;
	private float intervalBufferLength = 0.5f;
	private bool sign;
	private float RotationResolution = Mathf.PI/32.0f;
	private float RotationSkip = 6*Mathf.PI/8;
	public float mosx;
	public float mosy;
	private bool killSwirling;

	//Raycasting information
	private RaycastHit info;
	private int raycastMask = 507;

	//Selection variables
	//A first pass with a single straight line
	public selectionLine SL;
	public ModifiedMouseLook ML;
	private Transform radarTransform;
	private bool killSelecting = false;

	Parametric_L_System lastPLSYS;							//Governs how and when we rain on 
	private raindropGenerator rdG;							//parametric L_Systems

	private float waterCapacity = 100.0f;
	public float waterLevel;								//All of our watering parameters
	public float waterConsumptionRate;
	public float waterRefillRate;
	private int beforeWater;
	private GameObject occupiedWater;

	public GameObject TopNode, BottomLeftNode, BottomRightNode;		//Visual guide to activating Swirling
	private Vector2 secondDir = new Vector2 (-Mathf.Sqrt (2) / 2, -Mathf.Sqrt (2) / 2);
	private Vector2 thirdDir = new Vector2 (Mathf.Sqrt (2) / 2, -Mathf.Sqrt (2) / 2);
	private Vector2 firstDir = new Vector2 (0, 1);
	public float colorParam = 0;
	private bool down = false;

	/***************************************************/
	
	void Awake ()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		lookDirection = moveDirection;
		_animation = GetComponent<Animation>();
		_characterState = Idle;
		lastState = Idle;
		swirlBuffer = 0;

		hoverHeight = 1.01f;

		lastH = -1;

		beforeWater = -1;

		waterLevel = waterCapacity;

		rdG = gameObject.AddComponent<raindropGenerator> ();
		rdG.Start ();

		radarTransform = this.transform.Find("Ship/Antenna/Radar");

		SL = GetComponentInChildren<selectionLine> ();

		ML = GetComponentInChildren<ModifiedMouseLook> ();

	}

	void Start()
	{
		ML.gameObject.SetActive (false);
	}
	
	/*Calculate moveDirection, moveSpeed, inAirVelocity based on user input,
	which compose the movement vector passed to CharacterController.Move() in Update()*/
	void UpdateSmoothedMovementDirection ()
	{
		grounded = IsGrounded();
		if(grounded && IsFalling())
		{
			lastState = Falling;
			_characterState = Idle;
		}

		forward = lookDirection;
		forward.y = 0;
		forward = forward.normalized;

		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);

		//Store unsmoothed keyboard input
		v = Input.GetAxis("Vertical");
		h = Input.GetAxis ("Horizontal");
		if(Mathf.Abs(h) != 0)
			lastH = h;

		//Determine if keyboard input has occured (and hence movement as a result)
		isMoving = Mathf.Abs (h) > 0.1 || Mathf.Abs (v) > 0.1;

		mosy = Input.GetAxis("Mouse Y");
		mosx = Input.GetAxis ("Mouse X");

		float rotV = mosy;
		currentRadians = Mathf.Atan2(rotV, mosx);

		if(Input.GetButton("Triangle"))
			gravityPop = true;
		else
			gravityPop = false;

		if(!killSwirling)
		{
			currentRadians = (Mathf.PI*2) + currentRadians;
			interval = currentRadians - lastRadians;
			intervalBuffer = intervalBufferLength;
		}
		else if(intervalBuffer < 0)
			interval = 0;
		intervalBuffer -= Time.deltaTime;
	
		// Grounded controls
		if (_characterState != Swirling )
		{
			// Target direction relative to the camera
			targetDirection = v * forward + h * right;

			if(Input.GetButton("LeftShoulder1"))
			{
				lookTarget = 
					Quaternion.AngleAxis(-rotationalDistance, Vector3.up) * lookDirection;
			}
			else if(Input.GetButton("RightShoulder1"))
			{
				lookTarget = 
					Quaternion.AngleAxis(rotationalDistance, Vector3.up) * lookDirection;
			}
			
			if(lookTarget != Vector3.zero)
				lookDirection = Vector3.RotateTowards(lookDirection, lookTarget, 
				                                      swirlingRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
			lookTarget = Vector3.zero;

			//*******MOVEMENT CONTROLS APPLIED WITH INPUT TO GROUND AND HOVERING STATES********//
			if (isMoving && !IsSelecting()) //i.e. if v or h is not 0, meaning there has been input
			{
				//Calculate the dot product of the target and the current movement directions
				dot = Vector3.Dot(targetDirection.normalized, lookDirection.normalized);

				if(dot > forwardDotThreshold)
				{
					currentAcceleration = forwardAccelerationBonus;
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, 
					                                      		forwardRotationalSpeed*Time.deltaTime, 0.0F);
				}
				else if(dot > bankingDotThreshold)
				{
					currentAcceleration = bankingAccelerationBonus;
					lookDirection = Vector3.RotateTowards(lookDirection, targetDirection, 
					                                      bankingRotationalSpeed*Time.deltaTime, 0.0F);
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, 
					                                      forwardRotationalSpeed*Time.deltaTime, 0.0F);
				}
				else if(dot > cuttingDotThreshold)
				{
					currentAcceleration = cuttingAccelerationPenalty;
					moveSpeed += cuttingAccelerationPenalty;
					lookDirection = Vector3.RotateTowards(lookDirection, targetDirection, 
					                                      cuttingRotationalSpeed*Time.deltaTime, 0.0F);
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, 
					                                      forwardRotationalSpeed*Time.deltaTime, 0.0F);
				}
				else
				{
					currentAcceleration = brakeMagnitude;
					moveSpeed += brakeMagnitude;

					if(moveSpeed <= 1.0f)
					{
						targetDirection = Quaternion.AngleAxis(lastH*179, transform.up)*lookDirection; 

						lookDirection = Vector3.RotateTowards(lookDirection, targetDirection, 
						                	stationaryRotationalSpeed*Time.deltaTime, 0.0F);
					
						moveDirection = lookDirection;
					}
				}
			}
			else if(IsSelecting())
			{
				targetDirection = v * forward + h * right;
				
				if (isMoving)
				{
					{
						currentAcceleration += swirlingAcceleration*5;
						moveDirection = Vector3.RotateTowards
							(moveDirection, targetDirection, swirlingRotateSpeed*Time.deltaTime, 0);
					}
				}
				else
				{
					currentAcceleration = 0;
					moveSpeed -= groundDrag*100;
				}
				
				if(Input.GetButton("LeftShoulder1"))
				{
					lookTarget = 
						Quaternion.AngleAxis(-rotationalDistance, Vector3.up) * lookDirection;
				}
				else if(Input.GetButton("RightShoulder1"))
				{
					lookTarget = 
						Quaternion.AngleAxis(rotationalDistance, Vector3.up) * lookDirection;
				}
				
				if(lookTarget != Vector3.zero)
					lookDirection = Vector3.RotateTowards(lookDirection, lookTarget, 
					                                      swirlingRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
			}
			else 
			{
				currentAcceleration = 0;
				moveSpeed -= groundDrag*10;
			}
			//******************************************************************************************//

			//***************HOVERING REGULATIONS*******************************************************//
			if(_characterState == Hovering)
			{
				hoverTimer -= Time.deltaTime;

				if(hoverTimer < 0 || Input.GetButton("SelectBeam"))				//Time to start falling to the ground
				{
					lastState = Hovering;
					_characterState = Falling;
					hoverBuffer = hoverBufferLength;
					first = false;
					second = false;
					third = false;
				}
				else if(swirlBuffer < 0 && waterLevel > 5)							
				{	
					if(oneRevolution)
					{
						lastState = Hovering;							
						_characterState = Swirling;	
						lookTarget = Vector3.zero;
						rdG.DrawLines(true);
						oneRevolution = false;
						verticalSpeed += 5.0f;
						currentAcceleration = 0;
					}
					xyDir = new Vector2(mosx, mosy);
					xyDir.Normalize();
					//Check for a single revolution
					if(Vector2.Dot(xyDir, firstDir) > swirlDotRange)
						first = true;
					if(Vector2.Dot(xyDir, secondDir) > swirlDotRange)
						second = true;
					if(Vector2.Dot(xyDir, thirdDir) > swirlDotRange)
						third = true;
					if(first && second && third && !IsSwirling())
						oneRevolution = true;
				}
				swirlBuffer -= Time.deltaTime;

				//Ensures height is maintained during hovering, and that it is updated when encountering
				//terrain higher than the currently maintained height.
				if(Physics.Raycast (transform.position, -transform.up, out info, hoverCastDistance, raycastMask-256))
				{
					if(info.distance < lastCastDistance)
					{
						if(info.collider.gameObject.tag == "Terrain")
							hoverHeight = Terrain.activeTerrain.SampleHeight(info.point);
						else
							hoverHeight = info.point.y;
						hoverHeight += hoverBonus;
					}
					lastCastDistance = info.distance;
				}

				if(lastHeight != hoverHeight)
				{
					hoveringAnimationTransitionParameter = 0;
				}
				else
					hoveringAnimationTransitionParameter += Time.deltaTime *
						hoveringAnimationTransitionSpeed;
				lastHeight = hoverHeight;

				float newHeight = Mathf.Lerp(transform.position.y, hoverHeight, 
				                             hoveringAnimationTransitionParameter);
				
				transform.position = new Vector3(transform.position.x, 
				                                 newHeight, 
				                                 transform.position.z);
			}
			else if(_characterState == InWater)
			{
				if(Input.GetButtonDown("StateToggle") && hoveringParameter < 0.5f && !killSelecting)
				{
					ML.gameObject.SetActive(true);
					ML.dropRotation();
					radarTransform.gameObject.SetActive(false);
					lookTarget = Vector3.zero;
					_characterState = Selecting;
				}
				else if(killSelecting && Input.GetButtonDown("StateToggle"))
					if(this.GetComponent<fieldNotebook>().overlayActive())
						if(this.GetComponent<fieldNotebook>().ObjectivesBackground.activeSelf)
							this.GetComponent<fieldNotebook>().OverlayDisable();

				if(waterLevel <= waterCapacity)
				{
					interval = Mathf.Abs(interval);	//permit rotation either way
					if(interval > RotationSkip)
					{}
					else if(interval > RotationResolution)
					{
						waterLevel += interval*waterRefillRate; 
						if(hoverBuffer < 0)
						{
							if(waterLevel > waterCapacity - 10.0f)
								hoveringParameter += hoveringSmooth; 
							hoverHeight = hoverBonus * ((hoveringParameter+0.01f)/hoveringThreshold);
							hoverHeight += occupiedWater.transform.position.y;
							transform.position = new Vector3(transform.position.x, 
							                                 hoverHeight, transform.position.z);
						}
					}
				}
			}
			else if(IsSelecting())
			{
				ML.SetInputs(mosx, mosy);

				if(((Input.GetButtonDown("StateToggle") && verticalSpeed > -1) && !killSelecting) || selectionOff)
				{
					questionGenerator.toggleImage();
					radarTransform.gameObject.SetActive(true);
					ML.gameObject.transform.rotation = Quaternion.identity;
					ML.gameObject.SetActive(false);
					SL.clearLine();
					lastState = Selecting;
					_characterState = Idle;
					selectionOff = false;
				}
				else if(killSelecting && Input.GetButtonDown("StateToggle"))
					if(this.GetComponent<fieldNotebook>().overlayActive())
						if(this.GetComponent<fieldNotebook>().ObjectivesBackground.activeSelf)
							this.GetComponent<fieldNotebook>().OverlayDisable();
			}
			else
			{
				if(Input.GetButtonDown("StateToggle") && hoveringParameter < 0.5f && !IsFalling() && !killSelecting)
				{
					questionGenerator.toggleImage();
					ML.gameObject.SetActive(true);
					ML.dropRotation();
					radarTransform.gameObject.SetActive(false);
					lookTarget = Vector3.zero;
					_characterState = Selecting;
				}
				else if(killSelecting && Input.GetButtonDown("StateToggle"))
					if(this.GetComponent<fieldNotebook>().overlayActive())
						if(this.GetComponent<fieldNotebook>().ObjectivesBackground.activeSelf)
							this.GetComponent<fieldNotebook>().OverlayDisable();
			


				if(hoverBuffer < 0)
				{
					interval = Mathf.Abs(interval);	//permit rotation either way
					if(interval > RotationSkip || IsFalling())
					{}
					else if(interval > RotationResolution && waterLevel > waterCapacity*0.05f)
					{
							hoveringParameter += hoveringSmooth; 
					}

					if(Physics.Raycast (transform.position, -transform.up, out info, hoverCastDistance, raycastMask-256))
					{
						if(IsFalling())
						{
							if(info.distance <= lastCastDistance)
							{										//We are already falling and want to maintain
								_characterState = lastState;		//a smooth descent
								hoverHeight = transform.position.y;
								lastHeight = hoverHeight;
							}
						}
						else if(info.distance - lastCastDistance > 0.1f && info.distance > minFallDistance)
						{													//We've walked off a ledge 
							_characterState = Falling;						//and want to begin falling
							lastState = Idle;
						}
						else
						{
							if(info.collider.gameObject.tag == "Terrain")
							{
								hoverHeight = Terrain.activeTerrain.SampleHeight(info.point);
								hoverHeight += 0.1f;
							}
							else
							{
								hoverHeight = info.point.y;
								if(info.collider.gameObject.name == "CityStreet")
									hoverHeight+=0.5f;
							}
							hoverHeight += hoverBonus * ((hoveringParameter+0.01f)/hoveringThreshold);
							lastCastDistance = info.distance;
						}	
					}
					else //Case for steep terrain where the bottom of the ship passes through slope and we
					{    //begin missing raycasts, not scaling the height.
						hoverHeight = Terrain.activeTerrain.SampleHeight(transform.position);
						hoverHeight += 1.0f;
					}
					if(!IsFalling())
					{
						if(hoveringParameter > 0)
						{
							if(lastHeight != hoverHeight)
								hoveringAnimationTransitionParameter = 0;

							hoveringAnimationTransitionParameter += Time.deltaTime *
									hoveringAnimationTransitionSpeed*100.0f;

							lastHeight = hoverHeight;
							float newHeight = Mathf.Lerp(transform.position.y, hoverHeight, 
							                             hoveringAnimationTransitionParameter);
							transform.position = new Vector3(transform.position.x, 
							                                 newHeight, 
							                                 transform.position.z);
						}
						else
							transform.position = new Vector3(transform.position.x, 
							                                 hoverHeight, 
							                                 transform.position.z);
					}
				}
			}//*********************************************************************************************//
			hoverBuffer -= Time.deltaTime;

			if(hoveringParameter > hoveringThreshold && hoverBuffer < 0)
			{
				hoveringParameter = 0;				
				hoverTimer = hoverTimerLength;
				isControllable = true;
				swirlBuffer = swirlBufferLength;
				_characterState = Hovering;
				lastState = Idle;
			}
			else if (!IsFalling())
			{
				hoveringParameter -= hoveringDegradation;
				hoveringParameter = Mathf.Clamp(hoveringParameter, 0, hoveringThreshold+1);
			}

			moveSpeed -= groundDrag;
		}
		// In air controls
		else
		{
			targetDirection = v * forward + h * right;

			/*if(Vector3.Dot(targetDirection, right) > 0.9f)
				lookTarget = 
					Quaternion.AngleAxis(rotationalDistance, Vector3.up) * lookDirection;
			else if(Vector3.Dot(targetDirection, -right) > 0.9f)
				lookTarget = 
					Quaternion.AngleAxis(-rotationalDistance, Vector3.up) * lookDirection;*/

			if (isMoving)
			{
				if(Vector3.Dot(targetDirection, moveDirection) < 0)
				{
				   moveSpeed -= swirlingMagnitude*swirlingBrake;
					if(moveSpeed < 0.1f)
						moveDirection = Vector3.RotateTowards
							(moveDirection, targetDirection, swirlingRotateSpeed*Time.deltaTime, 0);
				}
				else
				{
					moveDirection = Vector3.RotateTowards
						(moveDirection, targetDirection, Time.deltaTime, 0);
				}
				moveSpeed += swirlingMagnitude;
			}
			else if(lastPLSYS != null)
			{
				if(lastPLSYS.watering)
				{
					moveSpeed -= groundDrag*1000;
					currentAcceleration = 0;
				}
			}
			else
				moveSpeed -= groundDrag;

			if(Input.GetButton("LeftShoulder1"))
			{
				lookTarget = 
					Quaternion.AngleAxis(-rotationalDistance, Vector3.up) * lookDirection;
			}
			else if(Input.GetButton("RightShoulder1"))
			{
				lookTarget = 
					Quaternion.AngleAxis(rotationalDistance, Vector3.up) * lookDirection;
			}

			if(lookTarget != Vector3.zero)
				lookDirection = Vector3.RotateTowards(lookDirection, lookTarget, 
									swirlingRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);


			if(interval == 0)
			{}
			else
			{
				interval = Mathf.Abs(interval);	//permit rotation either way

				if(interval > RotationSkip)
				{}
				else if(interval > RotationResolution)
				{
					float risingSpeed = swirlingMagnitude;				
					if(verticalSpeed <= 0)								
					{
						risingSpeed += (verticalSpeed/terminalVelocity)*fallingSpeedBonus;
					}

					verticalSpeed += risingSpeed;
					waterLevel += waterConsumptionRate;
				}
			}

			verticalSpeed = Mathf.Clamp(verticalSpeed, terminalVelocity, maxSwirlSpeed);

			if(Physics.Raycast (transform.position, -transform.up, out info, transform.position.y + 1.0f, raycastMask))
			{
				rdG.setLastHeight(info.distance);
				rdG.exposeHeightRatio(transform.position.y/maxHeight);
				if(lastPLSYS != null)
					lastPLSYS.watering = false;
				lastPLSYS = info.collider.gameObject.GetComponentInParent<Parametric_L_System>();
				if(lastPLSYS != null && rdG.IsRaining())
				{
					lastPLSYS.watering = true;
					swirlingMagnitude = swirlingMagnitudeAbsolute;
				}			
				else
					swirlingMagnitude = swirlingMagnitudeAbsolute/2;

				if(((info.distance < 0.1f) && info.collider.gameObject.layer != 8) || grounded) 
				{ 				
					_characterState = Idle;			
					lastState = Swirling;			
					if(lastPLSYS != null)
						lastPLSYS.watering = false;
					hoverBuffer = hoverBufferLength;
					rdG.DrawLines(false);
					first = false;
					second = false;
					third = false;
					swirlBuffer = swirlBufferLength;
				}
				else if(waterLevel <= 0)
				{
					if(lastPLSYS != null)
						lastPLSYS.watering = false;
					rdG.DrawLines(false);
					lastState = Swirling;
					_characterState = Falling;
					hoverBuffer = hoverBufferLength + 2.0f;
					first = false;
					second = false;
					third = false;
				}
			}
		}

		lastRadians = currentRadians;
		moveSpeed += currentAcceleration;
		moveSpeed = Mathf.Clamp (moveSpeed, 0, runSpeed - 5);
		waterLevel = Mathf.Clamp (waterLevel, 0, waterCapacity);
		currentAcceleration = Mathf.Clamp(currentAcceleration, 0, maxAcceleration);

		lookDirection.Normalize ();
		moveDirection.Normalize ();
	}
	
	void ApplyGravity ()
	{
		if(_characterState == Hovering || _characterState == Idle)
			verticalSpeed = 0.0f;
		else if(IsSwirling() || IsFalling())
		{
			float grav = gravity*swirlingGravityCoefficient * Time.deltaTime;

			verticalSpeed -= grav;
			if(transform.position.y > maxHeight)
				verticalSpeed -= grav;
			if(gravityPop)
				verticalSpeed -= grav;
		}
		else if(IsSelecting())
		{
			if(IsGrounded())
				verticalSpeed = 0;
			else
				verticalSpeed -= gravity*swirlingGravityCoefficient * Time.deltaTime;
		}
	}
	
	void Update() 
	{	
		if (!isControllable)
		{
			// kill all inputs if not controllable.
			Input.ResetInputAxes();
		}

		UpdateSmoothedMovementDirection();
		
		// Apply gravity
		// - extra power jump modifies gravity
		// - controlledDescent mode modifies gravity
		ApplyGravity ();
		
		// Calculate actual motion
		Vector3 movement = moveDirection * moveSpeed + new Vector3 (0, verticalSpeed, 0);
		movement *= Time.deltaTime;
		
		// Move the controller
		CharacterController  controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		
		// ANIMATION sector
		//	see separate script
		// ANIMATION sector
		
		// Set rotation to the move direction
		//if (IsGrounded())
		//{
		transform.rotation = Quaternion.LookRotation(lookDirection);
		nodePulse ();
	}
	
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if(!IsSelecting() && !IsSwirling())
		{
			if(hit.gameObject.tag == "Water")
			{
				if(_characterState != InWater)
					beforeWater = _characterState;
				_characterState = InWater;
				occupiedWater = hit.gameObject;
			}
			else if(beforeWater != -1)
			{
				_characterState = beforeWater;
				beforeWater = -1;
			}
		}
	}

	void OnTriggerEnter(Collider trigger)
	{
		if(trigger.gameObject.name == "MovementObjective")
			SendMessage("checkMovementUpdate");
		else if(trigger.gameObject.name == "FunRockTrigger")
			SendMessage ("checkSwirlingUpdate");
		else if(trigger.gameObject.name == "SystemsTrigger")
			SendMessage("depriveWater");
	}
	
	float GetSpeed () 
	{ return moveSpeed; }
	
	/*Uses CollisionFlag bitmask xxxx (None(0,1), Sides(0,1), Above(0,1), Below(0,1)) and 
	performs AND comparison with 0001. If Below is 0, the operation returns 0 and the
	character is believed to be in the air. If it is 1, the bitmask should be xxx1, meaning
	a collision has occured below the controller*/
	bool IsGrounded () 
	{ return ((collisionFlags & CollisionFlags.CollidedBelow) != 0); }

	public void togglePlayerControllable(bool state)
	{
		isControllable = state;
	}
	
	Vector3 GetDirection () 
	{ return moveDirection; }
	
	bool IsMoving ()
	{ return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5; }
	
	bool HasJumpReachedApex ()
	{ return jumpingReachedApex; }
	
	void Reset ()
	{ gameObject.tag = "Player"; }

	public bool IsInWater()
	{ return _characterState == InWater; }

	public bool IsHovering()
	{ return _characterState == Hovering; }

	public bool IsSwirling()
	{ return _characterState == Swirling; }

	public bool IsSelecting()
	{ return _characterState == Selecting; }

	public bool IsFalling()
	{ return _characterState == Falling; }

	public void killSwirl(bool kill)
	{
		killSwirling = kill;
	}

	public void killSelect(bool kill)
	{
		killSelecting = kill;
	}

	private void nodePulse()
	{
		Color nodeColor = Color.Lerp (Color.white, Color.blue, colorParam);

		if(colorParam >= 1.0f)
			down = true;
		else if(colorParam < 0)
			down = false;

		if(down)
			colorParam -= Time.deltaTime;
		else
			colorParam += Time.deltaTime;

		BottomLeftNode.GetComponent<Renderer>().material.color = nodeColor;
		BottomRightNode.GetComponent<Renderer>().material.color = nodeColor;
		TopNode.GetComponent<Renderer>().material.color = nodeColor;

		if(IsHovering())
		{
			TopNode.GetComponent<Renderer>().material.color = Color.red;
			BottomLeftNode.GetComponent<Renderer>().material.color = Color.red;
			BottomRightNode.GetComponent<Renderer>().material.color = Color.red;
		}

		if(!grounded)
		{
			if(first)
				TopNode.GetComponent<Renderer>().material.color = Color.green;
			if(second)
				BottomLeftNode.GetComponent<Renderer>().material.color = Color.green;
			if(third)
				BottomRightNode.GetComponent<Renderer>().material.color = Color.green;
		}
	}
}