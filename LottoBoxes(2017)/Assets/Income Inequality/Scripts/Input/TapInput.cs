using UnityEngine;
using System.Collections;

/// <summary>
/// Class that handles all tap input.
/// This registers if there has been a touch on the screen.
/// Following the registration of a tap, the proper event triggers are fired off for listeners to take action.
/// </summary>
public class TapInput// : AbstractInput
{
    //    #region Unity Callbacks
    //
    //    private void OnEnable()
    //    {
    //        MainGameEventManager.OnGameEnd += ResetInput;
    //        StartCoroutine(ReceiveInput());
    //    }
    //
    //    void OnDisable()
    //    {
    //        MainGameEventManager.OnGameEnd -= ResetInput;
    //    }
    //
    //    #endregion
    //
    //    protected override void IterateMainInputLoop()
    //    {
    //
    //        #if UNITY_EDITOR
    //
    //        #region PC Input
    //
    //        if (Input.GetMouseButtonDown(0))
    //        {
    //            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
    //            RaycastHit h;
    //            if (Physics.Raycast(r, out h, 100))
    //            {
    //                if (h.collider.gameObject.name.Contains("Box"))
    //                {
    //                    MainGameEventManager.TriggerBoxTappedEvent(h.collider.gameObject);
    //                }
    //            }
    //        }
    //
    //        #endregion
    //
    //        #endif
    //
    //
    //        #region Device Input
    //
    //        if (Input.touchCount > 0)
    //        {
    //            var touch = Input.GetTouch(0);
    //            Ray r = Camera.main.ScreenPointToRay(touch.position);
    //            RaycastHit h;
    //            if (Physics.Raycast(r, out h, 100))
    //            {
    //                if (h.collider.gameObject.name.Contains("Box"))
    //                {
    //                    MainGameEventManager.TriggerBoxTappedEvent(h.collider.gameObject);
    //                }
    //            }
    //        }
    //
    //        #endregion
    //
    //    }
    //
    //    protected override void ResetInput()
    //    {
    //        iterate = false;
    //    }
}
