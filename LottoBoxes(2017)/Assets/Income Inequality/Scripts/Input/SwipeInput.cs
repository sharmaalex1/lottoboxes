using UnityEngine;
using System.Collections;

/// <summary>
/// Class that handles all swipe input.
/// This registers if there has been a touch on the screen, then follows up by grabbing 
/// the position of the pointer when it is lifted to create a direction vector.
/// Following the registration of a swipe, the proper event triggers are fired off for listeners to take action.
/// </summary>
public class SwipeInput : AbstractInput
{
    private Vector3 startPos;
    private bool touchingABox = false;
    private GameObject touchedBox;

    #region Unity Callbacks

    private void Start()
    {
        StartCoroutine(ReceiveInput());
    }

    void OnDestroy()
    {
        ResetInput();
    }

    #endregion

    protected override void IterateMainInputLoop()
    {

        #region PC Input

        #if UNITY_EDITOR

        // Handles PC input.
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit h;
            if (Physics.Raycast(r, out h, 100))
            {
                if (h.collider.gameObject.GetType() == typeof(GameObject))
                {
                    touchedBox = h.collider.gameObject;
                    startPos = Input.mousePosition;
                    touchingABox = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (touchingABox)
            {
                //TODO Make a way for swipe input to distinguish between scenes.
                if (touchedBox.name.Contains("Swipeable"))
                {
                    if (Input.mousePosition.y < startPos.y)
                    {
                        ResetVars();
                        Debug.Log("Wrong swipe direction");
                        return;
                    }

                    MainGameEventManager.TriggerBoxSwipedEvent(touchedBox, startPos, Input.mousePosition);
                }
                else
                {
                    MiniGameEventManager.TriggerBoxSwipedEvent(touchedBox, startPos, Input.mousePosition);	
                }

                ResetVars();
            }
            else
            {
                ResetVars();
            }
        }

        #endif

        #endregion

        #region Device Input

        if (Input.touchCount > 0)
        {
            var curTouch = Input.GetTouch(0);
            if (!touchingABox)
            {
                Ray r = Camera.main.ScreenPointToRay(curTouch.position);
                RaycastHit h;
                if (Physics.Raycast(r, out h, 100))
                {
                    if (h.collider.gameObject.name.Contains("Box"))
                    {
                        touchingABox = true;
                        touchedBox = h.collider.gameObject;
                        startPos = curTouch.position;
                    }
                }
            }
				
            if (touchingABox)
            {
                curTouch = Input.GetTouch(0);
                if (curTouch.phase == TouchPhase.Ended)
                {
                    if (touchedBox.GetComponent<OpenableBox>() != null)
                    {
                        // For Main Game swiping of boxes.
                        if (curTouch.position.y < startPos.y)
                        {
                            ResetVars();
                            return;
                        }
                        MainGameEventManager.TriggerBoxSwipedEvent(touchedBox, startPos, curTouch.position);
                    }
                    else if (touchedBox.GetComponent<Box>() != null)
                    {
                        Debug.Log("Start position: " + startPos);
                        Debug.Log("End position: " + curTouch.position);
                        MiniGameEventManager.TriggerBoxSwipedEvent(touchedBox, startPos, curTouch.position);
                    }
                    ResetVars();
                }
            }
        }

        #endregion

    }

    private void ResetVars()
    {
        startPos = Vector3.zero;
        touchingABox = false;
        touchedBox = null;
    }

    protected override void ResetInput()
    {
        iterate = false;
    }
}
