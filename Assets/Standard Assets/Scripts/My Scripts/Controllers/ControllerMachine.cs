using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//Why must everything be so general as fuck?
public abstract class ControllerMachine : MonoBehaviour 
{
	protected Dictionary<int, CharacterState> states;	//States are encoded as named constant integers unique to the
	protected CharacterState currentState, lastState;	//particular machine. Transitions occur by passing the named
														//index from a state to the machine, which indexes the dict.
	protected const int NoTransition = -1;
	protected const int BaseState = 0;

	protected Vector3 move_direction;			//We've assumed directionality, movement and input parameters
	public abstract Vector3 MoveDirection { get; set; }		//are all necessarily universal properties of a controller

	protected float move_speed;				//At least an interesting one
	public abstract float MoveSpeed { get; set; }			//fuckin use these
												//Properties will allow each machine to define get/set methods for
	protected float vertical_speed;				//its unique parameters, also letting us decide what happens when
	public abstract float VerticalSpeed { get; set; }			//one of them is changed or accessed by a given state. (For instance
										//updating model rotation independent of any state when direction changes)
	protected float gravity;					
	public abstract float Gravity { get; set; }				//We've assumed gravity is the only really universal force acting
															//on a state machine
	protected CollisionFlags collisionFlags;	//ugh
	protected CharacterController controller;
	public Animation animation;

	protected Camera machineCamera;

	protected float hAxis, vAxis;				//Which is not to say we make interesting controllers
	public abstract float Horizontal { get; set; }
	public abstract float Vertical { get; set; }

	protected delegate void ControllerEvent(ControllerMachine sender, EventArgs e);
	protected event ControllerEvent Message;

	// Use this for initialization				no need to self-deprecate now
	protected void Awake () 
	{
		states = new Dictionary<int, CharacterState> ();
		controller = this.GetComponent<CharacterController> ();
		animation = this.GetComponent<Animation> ();
	}

	protected void Start()
	{

	}
	
	// Update is called once per frame
	protected void Update () 
	{
		currentState.UpdateLogic ();									
	}											

	protected void StateChange(int source, int target)
	{
		if(target != NoTransition && source != target)
		{
			List<int> targetList = new List<int>();
			int rootKey = findCommonRoot (source, target, ref targetList);
			while(currentState.Key != rootKey)	
			{								
				lastState = currentState;				
				lastState.StateExit ();					
				currentState = states [currentState.sourceKey];
			}
			rootKey = targetList.IndexOf (rootKey) + 1;
			currentState = states [targetList [rootKey]];
			while(currentState.Key != target)
			{
				currentState.StateEnter();
				lastState = currentState;
				rootKey++;
				currentState = states[targetList[rootKey]];
			}
			currentState.StateEnter();
		}
	}			

	private int findCommonRoot(int sourceState, int targetState, ref List<int> list2)
	{
		List<int> list1;
		list1 = new List<int> ();

		int i = sourceState;
		while(i != 0)
		{
			list1.Insert(0, i);
			i = states[i].sourceKey;
		}
		list1.Insert (0, 0);

		i = targetState;
		while(i != 0)
		{
			list2.Insert(0, i);
			i = states[i].sourceKey;
		}
		list2.Insert (0, 0);

		i = 0;
		while(list1[i] == list2[i])	//elements different or last element of one list
		{
			if(i == list1.Count-1 || i == list2.Count-1)
				return list1[i];
			i++;
		}
		return list1[i-1];
	}
																//collisions are a universal occurence, and semantics
	protected void OnControllerColliderHit(ControllerColliderHit hit)	//thereof are state-specific
	{
		currentState.Collision (hit);
	}			

	protected void SendEvent(EventArgs e)
	{
		Message (this, e);
	}

	public int getState()
	{
		return currentState.Key;
	}

	protected CharacterState indexState(int key)
	{
		return states[key];
	}
/****************************************************************************************************************
 * **************************************************************************************************************
 * *************************************************************************************************************/
	protected interface Animatable
	{
		AnimationState AnimationState { get; set; } 
		float AnimateSpeed { get; set; }	
		void Animate();
	}

	protected abstract class CharacterState
	{
		protected ControllerMachine machine;
		protected CharacterState source;
		protected int KeyValue;
		public int Key { get{ return KeyValue; } }
		public int sourceKey;

		protected CharacterState(){}
		protected CharacterState(ControllerMachine m)
		{										
			machine = m;
		}

		//Have to put a general event-handling method in here
		public abstract void UpdateLogic();
		public abstract int EventHandler();
		public abstract void StateEnter();					//When we change states, we must call each entry and exit method of each state						
		public abstract void StateExit();					//traversed along the hierarchy to get to the new state. So if we are walking and
		public void Collision(ControllerColliderHit hit)	//begin running, we do not need to traverse the grounded state and do not call its
		{}													//methods. But if we are running and begin jumping we must say 
															//Running->Exit();	 Grounded->Exit();	Aerial->Enter();	Jumping->Enter();
	}
}

