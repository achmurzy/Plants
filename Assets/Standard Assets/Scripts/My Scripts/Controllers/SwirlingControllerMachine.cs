using UnityEngine;
using System.Collections;

public class SwirlingControllerMachine : ControllerMachine 
{
	private const int Grounded = 1;
	private const int Aerial = 2;
	private const int Idle = 3;
	private const int Hovering = 4;
	private const int Swirling = 5;
	private const int Selecting = 6;
	private const int Falling = 7;

	public override Vector3 MoveDirection{ get { return move_direction; } set{ move_direction = value; } }
	public override float MoveSpeed{ get{ return move_speed; } set{ move_speed = value; } }
	public override float VerticalSpeed{ get{ return vertical_speed; } set{ vertical_speed = value; } }
	public override float Gravity{ get{ return gravity; } set{ gravity = value; } }
	public override float Horizontal { get { return hAxis; } set{ hAxis = value; } }
	public override float Vertical { get { return vAxis; } set{ vAxis = value; } }

/*	//***************************************************************************************************///
										//	BASE CONTROLLER VARIABLES  //
	public bool isMoving = false;
	private bool isControllable = true;
	/************GROUNDED MOVEMENT**************/
	private bool grounded = true;
	private float rotateSpeed = 400.0f;
	private float drag = 0.01f;

	/**************SWIRLING_JUMP_VARIABLES**************/
	//Ground control variables
	public Vector3 lookDirection;
	public Vector3 targetDirection;
	private float lastH;
	
	public float dot;
	public bool selectionOff = false;

	private float runSpeed = 15;
	private float groundDrag = 0.025f;
	public float currentAcceleration;
	private float maxAcceleration = 2.0f;

	private float basicGravity = 50;
	
	//Hovering variables

	private float hoverHeight;
	private float hoverCastDistance = 10000;
	private float lastCastDistance = 0.0f;
	private float lastHeight = 0.0f;
	private float hoverBonus = 3.0f;
	private bool first, second, third;
	private float hoveringAnimationTransitionSpeed =0.1f;
	private float hoveringAnimationTransitionParameter = 0;
	
	//Swirling variables
	private float swirlingGravityCoefficient = 0.075f;
	private float rotationalDistance = 0.5f;

	private Vector3 lookTarget;
	public float maxHeight = 50.0f;
	private bool gravityPop = false;	//By popular demand
	
	//Stick swirling variables
	private float lastRadians, interval, intervalBuffer;
	private float Interval { get { return interval; } }
	private float intervalBufferLength = 0.5f;
	private float RotationResolution = Mathf.PI/32.0f;
	private float RotationSkip = 4*Mathf.PI/8;
	public float mosx;
	public float mosy;
	private bool killSwirling;
	
	//Raycasting information
	private RaycastHit info;
	private int raycastMask = 507;

	private bool killSelecting = false;

	private float waterCapacity = 100.0f;
	public float waterLevel;								//All of our watering parameters
	
	public GameObject TopNode, BottomLeftNode, BottomRightNode;		//Visual guide to activating Swirling

	public float colorParam = 0;
	private bool down = false;
	
	/***************************************************/

	void Awake()
	{
		base.Awake ();
		waterLevel = waterCapacity;
	}

	// Use this for initialization
	void Start () 
	{
		//Grab necessary components
		states.Add (BaseState, new SwirlingBaseState (this));
		states.Add (Grounded, new SwirlingGroundedState (this));
		states.Add (Idle, new SwirlingIdleState (this));
		states.Add (Hovering, new SwirlingHoverState (this));
		states.Add (Swirling, new SwirlingSwirlState (this));
		states.Add (Selecting, new SwirlingSelectState (this));
		states.Add (Falling, new SwirlingFallState (this));
		
		currentState = states [Idle];
		//State variable initializations
		MoveDirection = transform.TransformDirection(Vector3.forward);
		lookDirection = MoveDirection;
		hoverHeight = 1.01f;
		lastH = -1;
	}
	
	// Update is called once per frame
	void Update () 
	{
		base.Update ();



		if (!isControllable)
			Input.ResetInputAxes();
		
		ApplyGravity ();


	}

	private class SwirlingBaseState : CharacterState
	{
		SwirlingControllerMachine machine; 

		public SwirlingBaseState(SwirlingControllerMachine m)
		{	
			KeyValue = BaseState;
			machine = m;
		}
		
		public override void UpdateLogic ()
		{	
			machine.InputAndDirectionLogic ();
			machine.MoveSpeed += machine.currentAcceleration;
			machine.MoveSpeed = Mathf.Clamp (machine.MoveSpeed, 0, machine.runSpeed - 5);
			machine.waterLevel = Mathf.Clamp (machine.waterLevel, 0, machine.waterCapacity);
			machine.currentAcceleration = Mathf.Clamp(machine.currentAcceleration, 0, machine.maxAcceleration);
			
			machine.lookDirection.Normalize ();
			machine.MoveDirection.Normalize ();

			Vector3 movement = machine.MoveDirection * machine.MoveSpeed + new Vector3 (0, machine.VerticalSpeed, 0);
			movement *= Time.deltaTime;
			
			machine.collisionFlags = machine.controller.Move(movement);
			
			machine.gameObject.transform.rotation = Quaternion.LookRotation(machine.lookDirection);
			machine.nodePulse ();
		}
		
		public override int EventHandler()
		{	
			return -1;
		}

		public override void StateEnter ()
		{
			
		}
		
		public override void StateExit ()
		{
			
		}
	}

	private class SwirlingGroundedState : CharacterState
	{
		private new SwirlingControllerMachine machine;
		private bool goFall = false;
		private bool inWater = false;
		private float waterRefillRate;
		
		private float minFallDistance = 3.1f;
		
		private float hoverBufferLength = 2.0f;
		private float hoverBuffer;
		
		private float hoveringParameter;
		private float hoveringSmooth = 0.2f;
		private float hoveringThreshold = 10.0f;
		private float hoveringDegradation = 0.05f;
		
		private GameObject occupiedWater;
		
		public SwirlingGroundedState(SwirlingControllerMachine m)
		{
			KeyValue = Grounded;
			machine = m;
			sourceKey = BaseState;
			source = machine.states[sourceKey];
		}
		
		public override void UpdateLogic()
		{
			machine.GroundedAndHoveringMovement ();
			
			if(hoverBuffer < 0)
			{
				if(machine.Interval > machine.RotationSkip)
				{}
				else if(inWater)
				{
					machine.waterLevel += machine.Interval*waterRefillRate; 
					if(machine.waterLevel > machine.waterCapacity - 1.0f)
						hoveringParameter += hoveringSmooth; 
					machine.hoverHeight = machine.hoverBonus * ((hoveringParameter+0.01f)/hoveringThreshold);
					machine.hoverHeight += occupiedWater.transform.position.y;
					machine.gameObject.transform.position = new Vector3(machine.gameObject.transform.position.x, 
					                                                    machine.hoverHeight, machine.gameObject.transform.position.z);
				}
				else
				{
					if(machine.Interval > machine.RotationResolution && 
					   machine.waterLevel > machine.waterCapacity*0.05f)
					{
						hoveringParameter += hoveringSmooth; 
					}
					
					if(Physics.Raycast (machine.gameObject.transform.position, -machine.gameObject.transform.up, out machine.info, machine.hoverCastDistance, machine.raycastMask-256))
					{
						if(machine.info.distance - machine.lastCastDistance > 0.1f && machine.info.distance > minFallDistance)
						{													
							goFall = true;
						}
						else
						{
							if(machine.info.collider.gameObject.tag == "Terrain")
							{
								machine.hoverHeight = Terrain.activeTerrain.SampleHeight(machine.info.point);
								machine.hoverHeight += 0.1f;
							}
							else
							{
								machine.hoverHeight = machine.info.point.y;
								if(machine.info.collider.gameObject.name == "CityStreet")
									machine.hoverHeight+=0.5f;
							}
							machine.hoverHeight += machine.hoverBonus * ((hoveringParameter+0.01f)/hoveringThreshold);
							machine.lastCastDistance = machine.info.distance;
						}	
					}
					else //Case for steep terrain where the bottom of the ship passes through slope and we
					{    //begin missing raycasts, not scaling the height.
						machine.hoverHeight = Terrain.activeTerrain.SampleHeight(machine.gameObject.transform.position);
						machine.hoverHeight += 1.0f;
					}
				}
				
				if(hoveringParameter > 0)
				{
					if(Mathf.Abs (machine.lastHeight - machine.hoverHeight) > 0.25f)
						machine.hoveringAnimationTransitionParameter = 0;
					
					machine.hoveringAnimationTransitionParameter += Time.deltaTime *
						machine.hoveringAnimationTransitionSpeed*100.0f;
					
					machine.lastHeight = machine.hoverHeight;
					float newHeight = Mathf.Lerp(machine.gameObject.transform.position.y, machine.hoverHeight, 
					                             machine.hoveringAnimationTransitionParameter);
					machine.gameObject.transform.position = new Vector3(machine.gameObject.transform.position.x, 
					                                                    newHeight, 
					                                                    machine.gameObject.transform.position.z);
				}
				else
					machine.gameObject.transform.position = new Vector3(machine.gameObject.transform.position.x, 
					                                                    machine.hoverHeight, 
					                                                    machine.gameObject.transform.position.z);
			}
			
			hoverBuffer -= Time.deltaTime;
			hoveringParameter -= hoveringDegradation;
			hoveringParameter = Mathf.Clamp(hoveringParameter, 0, hoveringThreshold+1);
			
			machine.MoveSpeed -= machine.groundDrag;
		}

		public override int EventHandler ()
		{
			if (Input.GetKeyDown (KeyCode.Tab) && hoveringParameter < 0.5f && !machine.killSelecting)
				return Selecting;
			else if(hoveringParameter > hoveringThreshold && hoverBuffer < 0)
				return Hovering;
			else if(goFall)
			{
				goFall = false;
				return Falling;
			}
			return source.EventHandler();
		}

		public override void StateEnter()
		{
			machine.Gravity = 0;
			hoveringParameter = 0;	
			hoverBuffer = hoverBufferLength;
		}

		public override void StateExit()
		{

		}

		new public void Collision(ControllerColliderHit hit)
		{
			if (hit.gameObject.tag == "Water")
			{
				occupiedWater = hit.gameObject;
				inWater = true;
			}
			else
				inWater = false;
		}
	}

	private class SwirlingIdleState : CharacterState
	{
		private new SwirlingControllerMachine machine;
		
		public SwirlingIdleState(SwirlingControllerMachine m)
		{
			machine = m;
			sourceKey = Grounded;
			KeyValue = Idle;
			source = machine.states[sourceKey];
		}
		
		public override void UpdateLogic()
		{
			
		}
		
		public override int EventHandler()
		{
			return source.EventHandler ();
		}
		
		public override void StateEnter()
		{
			
		}
		
		public override void StateExit()
		{
			
		}
	}

	private class SwirlingHoverState : CharacterState
	{
		private new SwirlingControllerMachine machine;
		private float swirlBufferLength = 0.25f;
		private float swirlBuffer;
		
		private Vector2 xyDir;
		private Vector2 secondDir = new Vector2 (-Mathf.Sqrt (2) / 2, -Mathf.Sqrt (2) / 2);
		private Vector2 thirdDir = new Vector2 (Mathf.Sqrt (2) / 2, -Mathf.Sqrt (2) / 2);
		private Vector2 firstDir = new Vector2 (0, 1);
		private float swirlDotRange = 0.99f;
		private bool oneRevolution; 
		
		private float hoverTimerLength = 3.0f;
		public float hoverTimer = 0.0f;
		
		public SwirlingHoverState(SwirlingControllerMachine m)
		{
			KeyValue = Hovering;
			machine = m;
			sourceKey = Grounded;
			source = machine.states[sourceKey];
		}
		
		public override void UpdateLogic()
		{
			machine.GroundedAndHoveringMovement ();
			swirlBuffer -= Time.deltaTime;
			
			if(swirlBuffer < 0 && machine.waterLevel > 5)							
			{	
				xyDir = new Vector2(machine.mosx, machine.mosy);
				xyDir.Normalize();
				//Check for a single revolution
				if(Vector2.Dot(xyDir, firstDir) > swirlDotRange)
					machine.first = true;
				if(Vector2.Dot(xyDir, secondDir) > swirlDotRange)
					machine.second = true;
				if(Vector2.Dot(xyDir, thirdDir) > swirlDotRange)
					machine.third = true;
				if(machine.first && machine.second && machine.third)
					oneRevolution = true;
			}
			
			if(Physics.Raycast (machine.gameObject.transform.position, -machine.gameObject.transform.up, 
			                    out machine.info, machine.hoverCastDistance, machine.raycastMask-256))
			{
				if(machine.info.distance < machine.lastCastDistance)
				{
					if(machine.info.collider.gameObject.tag == "Terrain")
						machine.hoverHeight = Terrain.activeTerrain.SampleHeight(machine.info.point);
					else
						machine.hoverHeight = machine.info.point.y;
					machine.hoverHeight += machine.hoverBonus;
				}
				machine.lastCastDistance = machine.info.distance;
			}
			
			if(Mathf.Abs (machine.lastHeight - machine.hoverHeight) > 0.25f)
			{
				machine.hoveringAnimationTransitionParameter = 0;
			}
			else
				machine.hoveringAnimationTransitionParameter += Time.deltaTime *
					machine.hoveringAnimationTransitionSpeed;
			machine.lastHeight = machine.hoverHeight;
			
			float newHeight = Mathf.Lerp(machine.gameObject.transform.position.y, machine.hoverHeight, 
			                             machine.hoveringAnimationTransitionParameter);
			
			machine.gameObject.transform.position = new Vector3(machine.gameObject.transform.position.x, 
			                                                    newHeight, 
			                                                    machine.gameObject.transform.position.z);
			
			hoverTimer -= Time.deltaTime;
		}

		public override int EventHandler ()
		{
			if(hoverTimer < 0 || Input.GetKey(KeyCode.X))
			{
				machine.first = false;
				machine.second = false;
				machine.third = false;
				return Falling;
			}
			if(oneRevolution)
				return Swirling;

			return source.EventHandler ();
		}

		public override void StateEnter()
		{
			machine.Gravity = 0;
			hoverTimer = hoverTimerLength;
			machine.isControllable = true;
			swirlBuffer = swirlBufferLength;
		}
		public override void StateExit()
		{
			oneRevolution = false;
		}

		new public void Collision(ControllerColliderHit hit)
		{}
	}

	private class SwirlingAerialState : CharacterState
	{
		private new SwirlingControllerMachine machine;

		public SwirlingAerialState(SwirlingControllerMachine m)
		{
			machine = m;
			sourceKey = BaseState;
			KeyValue = Aerial;
			source = machine.states[sourceKey];
		}

		public override void UpdateLogic()
		{

		}

		public override int EventHandler()
		{
			return source.EventHandler ();
		}

		public override void StateEnter()
		{

		}

		public override void StateExit()
		{

		}
	}
	
	private class SwirlingSwirlState : CharacterState
	{
		private new SwirlingControllerMachine machine;
		private bool groundTransition = false;
		private bool fallingTransition = false;
		
		private float swirlingMagnitude = 0.25f;
		private float swirlingMagnitudeAbsolute = 0.25f;
		private float fallingSpeedBonus = 1.1f;
		private float terminalVelocity = -25.0f;
		private float maxSwirlSpeed = 5.0f;
		
		private float swirlingBrake = 5.0f;
		
		private float swirlingRotateSpeed = 50.0f;
		
		private Parametric_L_System lastPLSYS;							//Governs how and when we rain on 
		private raindropGenerator rdG;							//parametric L_Systems
		public float waterConsumptionRate;
		
		public SwirlingSwirlState(SwirlingControllerMachine m)
		{
			KeyValue = Swirling;
			machine = m;
			sourceKey = Aerial;
			source = machine.states[sourceKey];
			rdG = machine.gameObject.AddComponent<raindropGenerator> ();
			rdG.Start ();
		}
		
		public override void UpdateLogic()
		{
			if (machine.isMoving)
			{
				if(Vector3.Dot(machine.targetDirection, machine.MoveDirection) < 0)
				{
					machine.MoveSpeed -= swirlingMagnitude*swirlingBrake;
					if(machine.MoveSpeed < 0.1f)
						machine.MoveDirection = Vector3.RotateTowards
							(machine.MoveDirection, machine.targetDirection, swirlingRotateSpeed*Time.deltaTime, 0);
				}
				else
				{
					machine.MoveDirection = Vector3.RotateTowards
						(machine.MoveDirection, machine.targetDirection, Time.deltaTime, 0);
				}
				machine.MoveSpeed += swirlingMagnitude;
			}
			else if(lastPLSYS != null)
			{
				if(lastPLSYS.watering)
					machine.MoveSpeed -= machine.groundDrag*1000;
			}
			else
				machine.MoveSpeed -= machine.groundDrag;
			
			if(machine.lookTarget != Vector3.zero)
				machine.lookDirection = Vector3.RotateTowards(machine.lookDirection, machine.lookTarget, 
				                                              swirlingRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
			
			if(machine.Interval == 0)
			{}
			else
			{
				if(machine.Interval > machine.RotationSkip)
				{}
				else if(machine.Interval > machine.RotationResolution)
				{
					float risingSpeed = swirlingMagnitude;				
					if(machine.VerticalSpeed <= 0)								
					{
						risingSpeed += (machine.VerticalSpeed/terminalVelocity)*fallingSpeedBonus;
					}
					
					machine.VerticalSpeed += risingSpeed;
					machine.waterLevel += waterConsumptionRate;
				}
			}
			
			machine.VerticalSpeed = Mathf.Clamp(machine.VerticalSpeed, terminalVelocity, maxSwirlSpeed);
			
			if(Physics.Raycast (machine.gameObject.transform.position, -machine.gameObject.transform.up, 
			                    out machine.info, machine.gameObject.transform.position.y + 1.0f, machine.raycastMask))
			{
				rdG.setLastHeight(machine.info.distance);
				rdG.exposeHeightRatio(machine.gameObject.transform.position.y/machine.maxHeight);
				if(lastPLSYS != null)
					lastPLSYS.watering = false;
				lastPLSYS = machine.info.collider.gameObject.GetComponentInParent<Parametric_L_System>();
				if(lastPLSYS != null && rdG.IsRaining())
				{
					lastPLSYS.watering = true;
					swirlingMagnitude = swirlingMagnitudeAbsolute;
				}			
				else
					swirlingMagnitude = swirlingMagnitudeAbsolute/2;
				
				if(((machine.info.distance < 0.1f) && machine.info.collider.gameObject.layer != 8) || machine.grounded) 
				{ 				
					groundTransition = true;			
					
				}
				else if(machine.waterLevel <= 0)
				{
					fallingTransition = true;
				}
			}
		}

		public override int EventHandler ()
		{
			if(Input.GetButton("LeftShoulder1"))
			{
				machine.lookTarget = 
					Quaternion.AngleAxis(-machine.rotationalDistance, Vector3.up) * machine.lookDirection;
			}
			else if(Input.GetButton("RightShoulder1"))
			{
				machine.lookTarget = 
					Quaternion.AngleAxis(machine.rotationalDistance, Vector3.up) * machine.lookDirection;
			}
			
			if(Input.GetKey(KeyCode.LeftShift))
				machine.gravityPop = true;
			else
				machine.gravityPop = false;

			if(groundTransition)
			{
				groundTransition = false;
				return Idle;
			}
			else if(fallingTransition)
			{
				fallingTransition = false;
				return Falling;
			}
			return NoTransition;
		}

		public override void StateEnter()
		{
			machine.lookTarget = Vector3.zero;
			rdG.DrawLines(true);
			machine.VerticalSpeed += 5.0f;
			machine.currentAcceleration = 0;
			machine.Gravity = machine.basicGravity * machine.swirlingGravityCoefficient;
		}

		public override void StateExit()
		{
			if(lastPLSYS != null)
				lastPLSYS.watering = false;
			rdG.DrawLines(false);
			machine.first = false;
			machine.second = false;
			machine.third = false;
			machine.VerticalSpeed = 0;
		}
	
		new public void Collision(ControllerColliderHit hit)
		{}
	}
	
	private class SwirlingSelectState : CharacterState
	{
		private new SwirlingControllerMachine machine;
		private bool transitionOut = false;
		private float selectRotateSpeed = 100.0f;
		private float swirlingAcceleration = 0.005f;
		
		public selectionLine SL;
		public ModifiedMouseLook ML;
		private Transform radarTransform;
		
		public SwirlingSelectState(SwirlingControllerMachine m)
		{
			KeyValue = Selecting;
			sourceKey = Grounded;
			source = machine.states[sourceKey];
			machine = m;
			radarTransform = machine.gameObject.transform.Find("Ship/Antenna/Radar");
			SL = machine.gameObject.GetComponentInChildren<selectionLine> ();
			ML = machine.gameObject.GetComponentInChildren<ModifiedMouseLook> ();
			ML.gameObject.SetActive (false);
		}
		
		public override void UpdateLogic()
		{
			if (machine.isMoving)
			{
				{
					machine.currentAcceleration += swirlingAcceleration;
					machine.MoveDirection = Vector3.RotateTowards
						(machine.MoveDirection, machine.targetDirection, selectRotateSpeed*Time.deltaTime, 0);
				}
			}
			else
			{
				machine.currentAcceleration = 0;
				machine.MoveSpeed -= machine.groundDrag*100;
			}
			
			if(machine.lookTarget != Vector3.zero)
				machine.lookDirection = Vector3.RotateTowards(machine.lookDirection, machine.lookTarget, 
				                                              selectRotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
			ML.SetInputs(machine.mosx, machine.mosy);
			
			if (machine.IsGrounded ())
				machine.Gravity = 0;
			else
				machine.Gravity = machine.basicGravity;
		}

		public override int EventHandler()
		{
			if(Input.GetButton("LeftShoulder1"))
			{
				machine.lookTarget = 
					Quaternion.AngleAxis(-machine.rotationalDistance, Vector3.up) * machine.lookDirection;
			}
			else if(Input.GetButton("RightShoulder1"))
			{
				machine.lookTarget = 
					Quaternion.AngleAxis(machine.rotationalDistance, Vector3.up) * machine.lookDirection;
			}
			
			if ((Input.GetKeyDown (KeyCode.Tab) && machine.VerticalSpeed > -1) || machine.selectionOff)
				return Idle;

			return source.EventHandler ();
		}

		public override void StateEnter()
		{
			questionGenerator.toggleImage();
			ML.gameObject.SetActive(true);
			ML.dropRotation();
			radarTransform.gameObject.SetActive(false);
			machine.lookTarget = Vector3.zero;
		}
		public override void StateExit()
		{
			questionGenerator.toggleImage();
			radarTransform.gameObject.SetActive(true);
			ML.gameObject.transform.rotation = Quaternion.identity;
			ML.gameObject.SetActive(false);
			SL.clearLine();
			machine.selectionOff = false;
		}

		new public void Collision(ControllerColliderHit hit)
		{}
		
		public GameObject GiveLookObject()
		{
			return ML.gameObject;
		}
	}
	
	private class SwirlingFallState : CharacterState
	{
		private new SwirlingControllerMachine machine;
		private bool land = false;
		
		public SwirlingFallState(SwirlingControllerMachine m)
		{
			KeyValue = Falling;
			machine = m;
			sourceKey = Aerial;
			source = machine.states[sourceKey];
		}
		
		public override void UpdateLogic()
		{
			if(Physics.Raycast (machine.gameObject.transform.position, -machine.gameObject.transform.up, out machine.info, machine.hoverCastDistance, machine.raycastMask-256))
			{
				if(machine.info.distance <= machine.lastCastDistance)
				{										//We are already falling and want to maintain
					//land = true;
				}
			}
		}

		public override int EventHandler ()
		{
			if(Input.GetKey(KeyCode.LeftShift))
				machine.gravityPop = true;
			else
				machine.gravityPop = false;

			if (land || machine.IsGrounded())
			{
				land = false;
				return Idle;
			}
			return source.EventHandler ();
		}

		public override void StateEnter()
		{
			machine.Gravity = machine.basicGravity * machine.swirlingGravityCoefficient;
		}

		public override void StateExit()
		{
			machine.VerticalSpeed = 0;
		}

		new public void Collision(ControllerColliderHit hit)
		{}
	}

	bool IsGrounded () 
	{
		return ((collisionFlags & CollisionFlags.CollidedBelow) != 0);
	}

	private void InputAndDirectionLogic()
	{
		Vector3 forward;

		grounded = IsGrounded();
		
		forward = lookDirection;
		forward.y = 0;
		forward = forward.normalized;
		
		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		
		//Store unsmoothed keyboard input
		Vertical = Input.GetAxis("Vertical");
		Horizontal = Input.GetAxis ("Horizontal");
		if(Mathf.Abs(Horizontal) != 0)
			lastH = Horizontal;
		
		//Determine if keyboard input has occured (and hence movement as a result)
		isMoving = Mathf.Abs (Horizontal) > 0.1 || Mathf.Abs (Vertical) > 0.1;
		
		mosy = Input.GetAxis("Mouse Y");
		mosx = Input.GetAxis ("Mouse X");
		
		float rotV = mosy;
		float currentRadians = Mathf.Atan2(rotV, mosx);
		
		if(rotV < 0 && !killSwirling)
		{
			currentRadians = (Mathf.PI*2) + currentRadians;
			interval = currentRadians - lastRadians;
			intervalBuffer = intervalBufferLength;
			interval = Mathf.Abs (interval);
		}
		else if(intervalBuffer < 0)
			interval = 0;
		intervalBuffer -= Time.deltaTime;
		lastRadians = currentRadians;

		targetDirection = Vertical * forward + Horizontal * right;
	}

	private void GroundedAndHoveringMovement()
	{
		float forwardDotThreshold = 0.85f;
		 float bankingDotThreshold = 0.3f;
		 float cuttingDotThreshold = -0.8f;
		
		 float forwardAccelerationBonus = 0.25f;
		 float bankingAccelerationBonus = 0.1f;
		 float cuttingAccelerationPenalty = -0.1f;
		 float brakeMagnitude = -0.25f;
		
		 float forwardRotationalSpeed = 2.0f;
		 float bankingRotationalSpeed = 0.5f;
		 float cuttingRotationalSpeed = 1.5f;
		 float stationaryRotationalSpeed = 4.0f;

		//Calculate the dot product of the target and the current movement directions
		dot = Vector3.Dot(targetDirection.normalized, lookDirection.normalized);
		
		if(dot > forwardDotThreshold)
		{
			currentAcceleration = forwardAccelerationBonus;
			MoveDirection = Vector3.RotateTowards(MoveDirection, targetDirection, 
			                                      forwardRotationalSpeed*Time.deltaTime, 0.0F);
		}
		else if(dot > bankingDotThreshold)
		{
			currentAcceleration = bankingAccelerationBonus;
			lookDirection = Vector3.RotateTowards(lookDirection, targetDirection, 
			                                      bankingRotationalSpeed*Time.deltaTime, 0.0F);
			MoveDirection = Vector3.RotateTowards(MoveDirection, targetDirection, 
			                                      forwardRotationalSpeed*Time.deltaTime, 0.0F);
		}
		else if(dot > cuttingDotThreshold)
		{
			currentAcceleration = cuttingAccelerationPenalty;
			MoveSpeed += cuttingAccelerationPenalty;
			lookDirection = Vector3.RotateTowards(lookDirection, targetDirection, 
			                                      cuttingRotationalSpeed*Time.deltaTime, 0.0F);
			MoveDirection = Vector3.RotateTowards(MoveDirection, targetDirection, 
			                                      forwardRotationalSpeed*Time.deltaTime, 0.0F);
		}
		else
		{
			currentAcceleration = brakeMagnitude;
			MoveSpeed += brakeMagnitude;
			
			if(MoveSpeed <= 1.0f)
			{
				targetDirection = Quaternion.AngleAxis(lastH*179, transform.up)*lookDirection; 
				
				lookDirection = Vector3.RotateTowards(lookDirection, targetDirection, 
				                                      stationaryRotationalSpeed*Time.deltaTime, 0.0F);
				
				MoveDirection = lookDirection;
			}
		}
	}

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

	void ApplyGravity ()
	{
		float grav = Gravity * Time.deltaTime;
		
		VerticalSpeed -= grav;
		if(transform.position.y > maxHeight)
			VerticalSpeed -= grav;
		if(gravityPop)
			VerticalSpeed -= grav;
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

	public bool IsHovering()
	{ return currentState.Key == Hovering; }
	
	public bool IsSwirling()
	{ return currentState.Key == Swirling; }
	
	public bool IsSelecting()
	{ return currentState.Key == Selecting; }
	
	public bool IsFalling()
	{ return currentState.Key == Falling; }

	public GameObject MouseLookObject()
	{
		SwirlingSelectState state = states [Selecting] as SwirlingSelectState;
		return state.GiveLookObject ();
	}

	public void togglePlayerControllable(bool state)
	{
		isControllable = state;
	}
}
