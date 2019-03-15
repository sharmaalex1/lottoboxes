using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Was for the original scene transition fade out.
/// This class' functionality is specifically geared towards fading CanvasGroups via their alpha component.
/// The wrapper is only necessary if subscribing to an event that isn't of the IEnumerator type.
/// The fading can be done at the user's discretion.
/// The original intent was for this to be subscribed to a callback that takes in the CanvasGroup from the current
/// scene's main UI implementation. 
/// For an example, see MainGameUI.cs
/// </summary>
public interface IFadeable
{
	void FadeCanvasGroupWrapper(CanvasGroup curSceneCanvasGroup);

	IEnumerator FadeCanvasGroup(CanvasGroup curSceneCanvasGroup);
}
