using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

public class Parametric_Turtle : Turtle 
{
	private Parametric_L_System p_l_sys;
	Dictionary <int, List<Module>> additionsList;
	private Object[] possibleFlowers, possibleLeaves;
	private Object Node;
	
	protected float senescenceParam = 0.0f;

	// Use this for initialization
	void Start () 
	{
		//We have to start turtle analysis at the same place each time we stir
		initialPosition = new Vector3 (transform.localPosition.x, 
		                               transform.localPosition.y, 
		                               transform.localPosition.z);

		//Many functions are needed from the L_system under analysis
		p_l_sys = this.GetComponentInParent<Parametric_L_System> ();
		
		branchStack = new Stack ();
		lineList = new List<VectorLine> ();
		additionsList = new Dictionary<int, List<Module>>();

		Node = Resources.Load("Prefabs/L_Systems/Node");

		plantParts = new List<GameObject> ();
	}

	void Update()
	{
		//It is here that collision detection logic with the p_l_sys will be used
		//to determine whether turtle analysis occurs once watering is implemented
		if(!p_l_sys.stopAnimating())
		{
			if(p_l_sys.watering)
			{
				animate = true;
			}
			else
				animate = false;
		}
		else
			animate = false; 

		if(animate)	
		{
			destroyLines();
			stir (p_l_sys.returnList);
		}

		if(p_l_sys.killSystem())
		{
			destroySystem();
		}

	}

	void stir(List<Module> word)
	{
		//Each new stir we restart at the local origin.
		transform.localPosition = new Vector3(initialPosition.x,
		                                      initialPosition.y,
		                                      initialPosition.z);

		//This is a calculation for 2D l-systems facing the camera at every position.
		//May be removed since parametric 2D l_systems seem pointless
		if(animate)
		{
			Quaternion undoIt = Quaternion.FromToRotation(lastForward, p_l_sys.transform.forward);
			undoIt = Quaternion.Inverse (undoIt) * Quaternion.Inverse (undoIt);
			transform.rotation *= undoIt;
			lastForward = p_l_sys.transform.forward;
		}

		//serves a similar function to the initialPosition in that it preserves the origin 
		//orientation of the turtle for subsequent symbol analysis
		saveOrientation = new Quaternion(transform.rotation.x,
		                                 transform.rotation.y,
		                                 transform.rotation.z,
		                                 transform.rotation.w);

		foreach(Module mod in word)
		{
			if(p_l_sys.productions.ContainsKey(mod.symbol))
			{
				if(mod.age > mod.terminalAge && mod.terminalAge > 0)
				{
					List<Module> newList = new List<Module>();

					for(int i = 0; i < p_l_sys.productions [mod.symbol].Length; i++)
					{
						Module m = new Module(p_l_sys.productions [mod.symbol][i]);

						if(System.Char.IsLower(m.symbol))
							m.agePerturb();
						newList.Add(m);
					}
					additionsList.Add(word.IndexOf(mod), newList);
				}
			}

			switch(mod.symbol)
			{
				case 'F':
				{
					VectorLine line;

					float length = p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);

														
					//mod.growthParameter += length;
					
					Vector3 heading = length * transform.up;//mod.growthParameter * transform.up;
					
					line = VectorLine.SetLine3D(Color.green, transform.localPosition, 
					                            transform.localPosition + heading);
					line.drawTransform = p_l_sys.transform;
					line.SetWidth(p_l_sys.getBranchWidth());
					//line.Draw3DAuto();
					
					lineList.Add(line);						
					
					transform.localPosition += heading;
				}
					break;
				case '[':
				{
					TransformHolder pushTrans = new TransformHolder(); 
					pushTrans.Store(transform);
					branchStack.Push(pushTrans);
				}
					break;				
				case ']':					
				{ 
					TransformHolder popTrans = (TransformHolder)branchStack.Pop();
					transform.localPosition = new Vector3(popTrans.position.x, popTrans.position.y, popTrans.position.z);
					transform.localRotation = new Quaternion(popTrans.rotation.x, popTrans.rotation.y, popTrans.rotation.z, popTrans.rotation.w);
				}
					break;	
				case '&':
				{
					//mod.growthParameter += p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
					//transform.Rotate(new Vector3(mod.growthParameter, 0, 0));
			transform.Rotate(new Vector3(p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age), 0, 0));
				}
					break;
				case '^':
				{
					//mod.growthParameter += p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
					//transform.Rotate(new Vector3(-mod.growthParameter, 0, 0));
				transform.Rotate(new Vector3(-p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age), 0, 0));
				}
					break;
				case '+':
				{
				//mod.growthParameter += p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
				//transform.Rotate(new Vector3(0, mod.growthParameter, 0));
				transform.Rotate(new Vector3(0, p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age), 0));
				}	
					break;
				case '-':
				{ 
				//mod.growthParameter += p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
				//transform.Rotate(new Vector3(0, -mod.growthParameter, 0));
				transform.Rotate(new Vector3(0, -p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age), 0));
				}	
					break;
				case '%':
				{
				//mod.growthParameter += p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
				//transform.Rotate(new Vector3(0, 0, mod.growthParameter));
				transform.Rotate(new Vector3(0, 0, p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age)));
				}	
					break;
				case '*':
				{
					//mod.growthParameter += p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
					//transform.Rotate(new Vector3(0, 0, -mod.growthParameter));
				transform.Rotate(new Vector3(0, 0, -p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age)));
				}	
					break;
				case '!':
				{	
					//float widthIncrease = p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
					//mod.growthParameter += widthIncrease; 			
				p_l_sys.setBranchWidth(p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age));	
				}
					break;
				case 'L':
				{
					GameObject leaf = Instantiate(p_l_sys.sendLeaf(), transform.position, 
				                                		transform.rotation) as GameObject;
					//mod.growthParameter += p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
				leaf.transform.localScale = Vector3.one * p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
					leaf.transform.parent = transform.parent;
					plantParts.Add(leaf);
				}
					break;		//Both of these need to be changed so that the turtle knows at Start() which
				case 'I':		//leaves and flowers it will be using. 
				{
					GameObject flower = Instantiate(p_l_sys.sendFlower(), transform.position, 
				                                    	transform.rotation) as GameObject;
					//mod.growthParameter += p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
				flower.transform.localScale = Vector3.one * p_l_sys.growthFunction(mod.symbol, mod.growthParameter, mod.age);
					flower.transform.parent = transform.parent;
					plantParts.Add(flower);
				}
					break;
				case 'N':
				{
					GameObject node = (GameObject)Instantiate(Node, transform.position, Quaternion.identity);
					node.transform.parent = transform.parent;
					plantParts.Add(node);
				}
					break;
				default:
				break;
			}

			//Age the module
			//mod.age += Time.deltaTime * p_l_sys.animateSpeed;
			mod.age += Time.deltaTime;
		}
		if(additionsList.Count != 0)
		{
			addNewProductions();
		}
		p_l_sys.ageSystem();

		transform.rotation = new Quaternion(saveOrientation.x, 
		                                    saveOrientation.y,
		                                    saveOrientation.z,
		                                    saveOrientation.w);
	}

	void addNewProductions()
	{
		int i = -1;

		foreach(KeyValuePair<int, List<Module>> kvp in additionsList)
		{
			if(i == -1)
			{
				i = kvp.Value.Count - 1;
				p_l_sys.returnList.RemoveAt(kvp.Key);
				p_l_sys.returnList.InsertRange(kvp.Key, kvp.Value);
			}
			else
			{
				p_l_sys.returnList.RemoveAt(i+kvp.Key);
				p_l_sys.returnList.InsertRange(i+kvp.Key, kvp.Value);
				i += kvp.Value.Count - 1;
			}
		}
		additionsList.Clear ();
	}

	//Used for animation. We cannot possibly transform the VectorLines or the models correctly
	//so we destroy them all and let the turtle create new, correctly oriented ones.
	public new void destroyLines()
	{
		VectorLine.Destroy (lineList);
		foreach(GameObject go in plantParts)
			GameObject.Destroy(go);
		plantParts.Clear ();
	}

	public void destroySystem()
	{
		VectorLine.Destroy(lineList);
		L_System_Field.RemoveKilledLSystem(transform.parent.gameObject);
		GameObject.Destroy(transform.parent.gameObject);
		GameObject.Destroy(gameObject);
	}

	public void HideLines()
	{
		/*foreach(GameObject go in plantParts)
			go.SetActive(!go.activeSelf);*/
		foreach(VectorLine vi in lineList)
			vi.active = !vi.active;
		
	}
}
