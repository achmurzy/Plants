using UnityEngine;
using System.Collections;

public class shipAnimator : MonoBehaviour {

	private Animation animator;
	AnimationState waterTank;
	AnimationState radar;
	private swirlingJumpController sJC;

	private float waterLevel = 0.0f;
	private bool selecting = false;

	private float radarSpeed = -1.5f;
	private float radarBonus = 2.0f;

	// Use this for initialization
	void Start () 
	{
		sJC = gameObject.GetComponent<swirlingJumpController> ();

		animator = gameObject.GetComponentInChildren<Animation> ();
		waterTank = animator ["Water|WaterFill"];
		waterTank.speed = 0;

		radar = animator ["Radar|RadarSpin"];
	}

	// Update is called once per frame
	void Update () 
	{
		waterTank.time = waterLevelRangeConversion();
		updateStates ();
		animator.Blend ("Water|WaterFill");

		if(selecting)
			radar.speed = 0;
		else if(sJC.IsSwirling())
			radar.speed = radarSpeed+(radarBonus*(sJC.verticalSpeed/sJC.maxSwirlSpeed));
		else
			radar.speed = radarSpeed;
	}

	private void updateStates()
	{
		waterLevel = sJC.waterLevel;

		selecting = sJC.IsSelecting (); 
	}

	private float waterLevelRangeConversion()	//Maybe messy, but we know water is between 0 and 100 and
	{											//that the total length of this animation is 0.958 
		return (waterLevel / 100) * 0.958f;
	}
}
