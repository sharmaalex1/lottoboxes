using UnityEngine;
using System.Collections;

public class CoroutineUtil : MonoBehaviour
{

	/// <summary>
	/// If timescale is set to 0 and something still needs to be 
	/// waited for/on, this function will handle that situation.
	/// </summary>
	public static IEnumerator WaitForRealSeconds(float duration)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + duration)
		{
			yield return null;
		}

		yield break;
	}

}
