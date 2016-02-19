using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parametric_L_System : MonoBehaviour {

	public List<Module> returnList;
	public Dictionary <char, Module[]> productions;

	protected float leafGrowth = 1.0f;
	protected float segmentGrowth = 1.0f;
	protected float widthGrowth = 1.0f;
	protected float flowerGrowth = 1.0f;
	protected float axlGrowth = 1.0f;

	protected float currentBranchWidth = 1.0f;

	protected float exponentialParameter = 0.48f;

	protected int numOfProductions;
	protected int[] productionLengths;

	protected float branchAngle = 45.0f;
	protected float divergenceAngle = 60.0f;

	protected GameObject Leaf;
	protected GameObject Flower;

	public bool formSelected = false;
	public bool leavesSelected = false;
	public bool flowersSelected = false;
	
	protected float age = 0.0f;
	protected float maturity = 6.0f;
	protected float senescenceParameter = 0.0f;
	protected float senescenceSpeed = 0.01f;
	protected bool senesced = false;
	protected bool withering = false;

	public bool watering = false;

	private float systemRotation = 360.0f;
	private float rotateTime = 2.0f;

	// Use this for initialization
	void Start () 
	{
		returnList = new List<Module> ();
		productions = new Dictionary<char, Module[]> ();
		currentBranchWidth = 1.0f;
	}
	
	// Update is called once per frame
	protected void Update () 
	{
		if(withering)
		{
			senesce();
		}
	}

	public virtual float growthFunction(char sym, float gfi, float time)
	{
		return 0;
	}	

	public void setBranchWidth(float bw)
	{
		currentBranchWidth = bw;
	}
	
	public float getBranchWidth()
	{
		return currentBranchWidth;
	}

	public bool stopAnimating()
	{
		if(age > maturity)
		{
			animateMaturity();
			return true;
		}
		else
			return false; 
	}

	public void animateMaturity()
	{
		if(rotateTime > 0)
		{
			transform.Rotate(Vector3.up*(Time.deltaTime*systemRotation));
			rotateTime -= Time.deltaTime;
			systemRotation -= Time.deltaTime*(systemRotation/rotateTime);
		}
	}

	public bool killSystem()
	{
		return senesced;
	}

	public void beginDecay()
	{
		withering = true;
	}
	
	public void ageSystem()
	{
		age += Time.deltaTime;
	}

	public GameObject sendFlower()
	{
		return Flower;
	}

	public GameObject sendLeaf()
	{
		return Leaf;
	}

	private void senesce()
	{
		if(senescenceParameter < 1 && transform.localScale.magnitude > 0.01f)
		{
			Input.ResetInputAxes();
			transform.localScale = Vector3.Lerp (transform.localScale, Vector3.zero, senescenceParameter);
			senescenceParameter += Time.deltaTime * senescenceSpeed;
		}
		else
		{
			withering = false;
			senesced = true;
		}
	}
}

public class Module
{
	public char symbol;
	public float age, terminalAge, growthParameter;
	//Age, TerminalAge for production application, and a growth function identifier

	public Module(char sym, float a, float term, float gp)
	{
		symbol = sym;
		age = a;
		terminalAge = term;
		growthParameter = gp;
	}

	public Module(Module m)
	{
		symbol = m.symbol;
		age = m.age;
		terminalAge = m.terminalAge;
		growthParameter = m.growthParameter;
	}

	public virtual void agePerturb()
	{
		terminalAge += Random.Range (-0.25f, 0.5f);
	}
}


