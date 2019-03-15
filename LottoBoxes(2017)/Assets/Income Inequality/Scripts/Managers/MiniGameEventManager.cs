using UnityEngine;
using System.Collections;

public class MiniGameEventManager : MonoBehaviour 
{
	public delegate void MinigameEventHandeler();

	public static event MinigameEventHandeler OnCorrectBoxPlacement;
	public static event MinigameEventHandeler OnIncorrectBoxPlacement;
	public static event System.Action<GameObject, Vector3, Vector3> OnBoxSwiped;
    public static event System.Action<int> OnMultiplierChange;
    public static event MinigameEventHandeler OnMiniGameEnd;
    public static event MinigameEventHandeler OnBoxMissed;

    public static event System.Action<GameObject> OnBoxDrag;
    public static event System.Action<GameObject> OnDragStop;

    public static event System.Action<float> OnSpeedChange;

	public static void TriggerCorrectPlacementEvent()
	{
		if (OnCorrectBoxPlacement != null)
		{
			OnCorrectBoxPlacement();
		}
	}

	public static void TriggerIncorrectPlacementEvent()
	{
		if (OnIncorrectBoxPlacement != null)
		{
			OnIncorrectBoxPlacement();
		}
	}

	public static void TriggerBoxSwipedEvent(GameObject swipedBox, Vector3 startPos, Vector3 endPos)
	{
		if (OnBoxSwiped != null)
		{
			OnBoxSwiped(swipedBox, startPos, endPos);
		}
	}

    public static void TriggerMultiplierChange(int multiplier)
    {
        if(OnMultiplierChange != null)
        {
            OnMultiplierChange(multiplier);
        }
    }

    public static void TriggerOnMiniGameEnd()
    {
        if(OnMiniGameEnd != null)
        {
            OnMiniGameEnd();
        }
    }

    public static void TriggerOnBoxMissed()
    {
        if(OnBoxMissed != null)
        {
            OnBoxMissed();
        }
    }

    public static void TriggerOnBoxDrag(GameObject obj)
    {
        if(OnBoxDrag != null)
        {
            OnBoxDrag(obj);
        }
    }

    public static void TriggerOnDragStop(GameObject obj)
    {
        if(OnDragStop != null)
        {
            OnDragStop(obj);
        }
    }

    public static void TriggerOnSpeedChange(float newSpeed)
    {
        if(OnSpeedChange != null)
        {
            OnSpeedChange(newSpeed);
        }
    }
}
