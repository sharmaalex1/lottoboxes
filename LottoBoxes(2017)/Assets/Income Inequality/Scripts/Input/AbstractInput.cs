using UnityEngine;
using System.Collections;

public abstract class AbstractInput : MonoBehaviour
{
	protected bool iterate;


	/// <summary>
	/// This is the place to reset any variables that were
	/// triggered or set during the input loop.
	/// </summary>
	/// <returns>The input.</returns>
	protected abstract void ResetInput();

	/// <summary>
	/// This is only necessary if subscription to an event is desired, unless the delegate is an IEnumerator.
	/// </summary>
	protected void ReceiveInputWrapper()
	{
		StartCoroutine(ReceiveInput());
	}

	/// <summary>
	/// This is subscribed to the event that handles any kind of input.
	/// </summary>
	protected virtual IEnumerator ReceiveInput()
	{
		iterate = true;
		while (iterate)
		{
			IterateMainInputLoop();	
			yield return null;
		}
			
		yield break;

	}


	/// <summary>
	/// This is where the input is handled for anything kind of input
	/// </summary>
	protected abstract void IterateMainInputLoop();


	//protected abstract void StopInput();
}
