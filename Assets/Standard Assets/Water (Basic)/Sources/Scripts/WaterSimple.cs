using UnityEngine;

// Sets up transformation matrices to scale&scroll water waves
// for the case where graphics card does not support vertex programs.

//[ExecuteInEditMode]
public class WaterSimple : MonoBehaviour
{
	private bool generated = false;
	private float lifetime = 0.0f;
	private float fadingTime = 1.0f;
	private float startSizeMax= 0.15f;
	private float startSizeMin = 0.05f;
	private float finalSize = 0.5f;
	private float finalSizeMin = 0.25f;
	private Vector3 finalScale;

	void Update()
	{
		Fade ();
		/*if( !GetComponent<Renderer>() )
			return;
		Material mat = GetComponent<Renderer>().sharedMaterial;
		if( !mat )
			return;
			
		Vector4 waveSpeed = mat.GetVector( "WaveSpeed" );
		float waveScale = mat.GetFloat( "_WaveScale" );
		float t = Time.time / 20.0f;
		
		Vector4 offset4 = waveSpeed * (t * waveScale);
		Vector4 offsetClamped = new Vector4(Mathf.Repeat(offset4.x,1.0f), Mathf.Repeat(offset4.y,1.0f), Mathf.Repeat(offset4.z,1.0f), Mathf.Repeat(offset4.w,1.0f));
		mat.SetVector( "_WaveOffset", offsetClamped );
		
		Vector3 scale = new Vector3( 1.0f/waveScale, 1.0f/waveScale, 1 );
		Matrix4x4 scrollMatrix = Matrix4x4.TRS( new Vector3(offsetClamped.x,offsetClamped.y,0), Quaternion.identity, scale );
		mat.SetMatrix( "_WaveMatrix", scrollMatrix );
				
		scrollMatrix = Matrix4x4.TRS( new Vector3(offsetClamped.z,offsetClamped.w,0), Quaternion.identity, scale * 0.45f );
		mat.SetMatrix( "_WaveMatrix2", scrollMatrix );*/
	}

	void Fade()
	{
		if(generated)
		{
			lifetime += Time.deltaTime*fadingTime;
			transform.localScale = Vector3.Lerp(transform.localScale, finalScale, lifetime);
			if(lifetime > 1)
				Destroy(gameObject);
		}
	}

	public void raindropPool()
	{
		generated = true;
		transform.localScale = Vector3.one*Random.Range(startSizeMin, startSizeMax);
		finalScale = Vector3.one * Random.Range (finalSizeMin, finalSize);
	}
}
