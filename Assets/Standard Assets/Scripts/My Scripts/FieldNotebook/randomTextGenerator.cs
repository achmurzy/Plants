using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System.Text;

//Performance problems associated with this script currently
public class randomTextGenerator : MonoBehaviour {

	public StringBuilder DisplayText, QueueText;
	public string textBuffer;
	public int TextIndex;
	public int CharacterLimit;
	public int LineLimit;
	public Queue<string> Lines;

	// Use this for initialization
	void Start () 
	{
		TextIndex = 0;
		CharacterLimit = 10;
		LineLimit = 5;
		Lines = new Queue<string>();
		QueueText = new StringBuilder ();
		DisplayText = new StringBuilder();
		StartCoroutine ("IntervalElapsed");
		textBuffer = generateString (UnityEngine.Random.Range (1, 10));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (DisplayText.Length >= CharacterLimit)
		{
			DisplayText.Append("\n");
			Lines.Enqueue(DisplayText.ToString());
			QueueText.Append(Lines.Last());			//Add the new line to the Queue string
			if (Lines.Count > LineLimit)
			{
				QueueText.Replace(Lines.Dequeue(), "");	//This is slick, get rid of the old line and its place in the queue text simultaneously
			}
			Debug.Log (QueueText.Capacity);
			Debug.Log(QueueText.Length);
			Debug.Log (DisplayText.Capacity);
			Debug.Log(DisplayText.Length);
			Debug.Log (Lines.Count);
			DisplayText.Length = 0;	
		}
	}

	public string generateString(int length)
	{
		Int32[] ints = new Int32[length];
		for (int i = 0; i < length; i++)
			ints[i] = UnityEngine.Random.Range(32, 128);
		StringBuilder sb = new StringBuilder();
		
		for (int i = 0; i < length; i++)
			sb.Append(Convert.ToChar(ints[i]));

		return sb.ToString();
	}

	private IEnumerator IntervalElapsed()
	{ 
		while(true)
		{
			yield return new WaitForSeconds (1.0f);
			//At an interval we add some number of characters to the last string in the queue from Text
			DisplayText.Append(textBuffer[TextIndex]);
			TextIndex++;
			if (TextIndex == textBuffer.Length)
			{
				//Add text to the buffer here
				textBuffer = generateString(UnityEngine.Random.Range(1, 10));
				TextIndex = 0;
			}
		}
	}

	public string GetText()
	{

		return QueueText.ToString () + DisplayText.ToString();
	}
}
