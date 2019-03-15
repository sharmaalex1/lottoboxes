using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class InputGuard : MonoBehaviour, IPointerDownHandler
{
    // Object that the guard will be turning to block input behind.
    private GameObject objectToFocus;
    // If the object is turned on because of an event trigger, this will prevent it from receiving input.
    private bool preventDeactivation = false;
    // Image component attached to the gameobject.
    private Image img;

    private void Awake()
    {
        img = this.GetComponent<Image>();
    }

    private void Start()
    {
        MainGameEventManager.OnTicketRoutineBegin += ActivateGuardOnTimer;
        MainGameEventManager.OnTicketRoutineEnd += DeactivateGuard;
        MainGameEventManager.OnBoxCountExhausted += ActivateGuard;
    }

    private void OnDisable()
    {
        MainGameEventManager.OnTicketRoutineBegin -= ActivateGuardOnTimer;
        MainGameEventManager.OnTicketRoutineEnd -= DeactivateGuard;
        MainGameEventManager.OnBoxCountExhausted -= ActivateGuard;
    }

    /// <summary>
    /// This method is called from a Button Gameobject's OnClick method.
    /// The parameter that is taken in is the object that will be turning on.
    /// </summary>
    public void ActivateGuard(GameObject elementToTurnOn)
    {
        objectToFocus = elementToTurnOn;
        img.enabled = true;
        elementToTurnOn.SetActive(true);
    }

    /// <summary>
    /// Generic version the method to simply
    /// </summary>
    public void ActivateGuard()
    {
        img.enabled = true;
    }

    /// <summary>
    /// For subscribing to an event in case something automated will be happening and
    /// the focus guard needs to be present.
    /// </summary>
    public void ActivateGuardOnTimer()
    {
        StartCoroutine(ActivateGuardRoutine());
    }

    /// <summary>
    /// Turn this into a modular function that can wait any time not just specialized for the ticket routine.
    /// </summary>
    private IEnumerator ActivateGuardRoutine()
    {
        preventDeactivation = true;

       // yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds
          //  (OpenableBox.BOX_OPEN_TIME + OpenableBox.BOX_EXISTENCE_TIME + TicketPiece.TIME_TO_GET_TO_UI));
        
        this.GetComponent<Image>().enabled = true;

        yield break;
    }

    public void DeactivateGuard()
    {
        preventDeactivation = false;

        img.enabled = false;
    }

    /// <summary>
    /// Handles turning off the image of the guard upon being tapped.
    /// Also turns off any buttons associated with the objectToFocus as well
    /// hiding the object itself.
    /// </summary>
    public void OnPointerDown(PointerEventData data)
    {
        if (preventDeactivation)
        {
            return;
        }
            
        if (objectToFocus != null)
        {
            objectToFocus.SetActive(false);
            objectToFocus = null;
        }

        img.enabled = false;
    }
}
