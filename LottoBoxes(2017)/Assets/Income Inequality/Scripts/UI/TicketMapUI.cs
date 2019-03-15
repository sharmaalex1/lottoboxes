using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// This class takes care of handling placement of new tickets earned in the TicketMapUI. 
/// It's activation is controlled by the TicketMapManager.
/// </summary>
public class TicketMapUI : MonoBehaviour
{
    /// <summary>
    /// Used to allow for a brief pause before the ticket map dissappears.
    /// </summary>
    public const float MENU_IDLE_TIME = 1.5f;
    /// <summary>
    /// Amount to rotate the z-axis of the new ticket.
    /// </summary>
    public const float TICKET_Z_ROTATION = -1080.0f;
    /// <summary>
    /// Time it takes for the ticket to complete its animation routine.
    /// </summary>
    public const float TICKET_ROUTINE_TIME = 1.0f;
    /// <summary>
    /// Time it takes for the ticket to fade in.
    /// </summary>
    public const float TICKET_FADE_IN_TIME = 0.75f;
    public const int MAX_TICKET_PIECES = 5;

    // For handling visibility.
    private CanvasGroup cGroup;
    // For handling input reception.
    private EventTrigger eTrigger;

    private GameObject tournamentManager;

    [Tooltip("1st: UL, 2nd: BL, 3rd: C, 4th: UR, 5th: BR")]
    public Image[] ticketPieces;

    private void Awake()
    {        
        cGroup = this.GetComponent<CanvasGroup>();
        eTrigger = this.GetComponent<EventTrigger>();
    }

    private void OnEnable()
    {
        tournamentManager = GameObject.FindGameObjectWithTag("TournamentManager");

        // Highlight whatever tickets have already been obtained.
        for (int i = 0; i < ticketPieces.Length; i++)
        {
            if (i < SaveManager.Instance.NumCurrentTicketPieces)
            {
                ticketPieces[i].canvasRenderer.SetAlpha(1);
            }
            else
            {
                ticketPieces[i].canvasRenderer.SetAlpha(0);
            }
        }	
    }

    /// <summary>
    /// Handles placement of the new ticket.
    /// The placement is a concoction of fading in, rotating, and scaling up from a 0 scale.
    /// </summary>
    public IEnumerator PlaceNewTicket(TicketPiece newPiece)
    {
        // Don't let the player see it yet so that is can start blocking input immediately.
        cGroup.alpha = 0;
        // Don't let it take input, which would utlimately turn it off.
        eTrigger.enabled = false;

        ticketPieces[newPiece.id].gameObject.transform.localScale = Vector3.one * 0.0f;
        ticketPieces[newPiece.id].canvasRenderer.SetAlpha(0);

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds
            (Item.ITEM_MOVEMENT_TIME + OpenableBox.BOX_OPEN_TIME + TicketPiece.TIME_TO_GET_TO_UI));

        // Allow for visibility of the entire UI after waiting for the 
        // received ticket to reach the designated UI element.
        cGroup.alpha = 1;

        // Increase the scale and rotation to create a spin-in effect for the 
        // ticket piece being added to the TicketMapUI.
        // Also fades in the ticket.
        iTween.ScaleTo(ticketPieces[newPiece.id].gameObject, iTween.Hash("scale", Vector3.one, 
                "time", TICKET_ROUTINE_TIME, 
                "ignoretimescale", true, 
                "easetype", iTween.EaseType.spring));
        iTween.RotateAdd(ticketPieces[newPiece.id].gameObject, iTween.Hash("z", TICKET_Z_ROTATION, 
                "time", TICKET_ROUTINE_TIME / 2, 
                "ignoretimescale", true, 
                "easetype", iTween.EaseType.easeInCirc));
        ticketPieces[newPiece.id].CrossFadeAlpha(1, TICKET_FADE_IN_TIME, true);

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(TICKET_ROUTINE_TIME));
        

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(MENU_IDLE_TIME));

        if (CheckTicketCount())
        {
            StartCoroutine(PlayTicketCompletionAnimation());
        }
        else
        {
            eTrigger.enabled = true;
            this.gameObject.SetActive(false);
            Time.timeScale = 1;

            MainGameEventManager.TriggerTicketRoutineEnd();
        }       

        yield break;
    }

    private IEnumerator PlayTicketCompletionAnimation()
    {
        Debug.Log("All tickets have been earned");

        eTrigger.enabled = true;
        this.gameObject.SetActive(false);
        Time.timeScale = 1;

        MainGameEventManager.TriggerTicketRoutineEnd();

        yield break;
    }

    /// <summary>
    /// Returns true if all pieces have been obtained.
    /// </summary>
    private bool CheckTicketCount()
    {
        if (SaveManager.Instance.NumCurrentTicketPieces >= MAX_TICKET_PIECES)
        {
            MainGameEventManager.TriggerAllTicketsFoundEvent();

            if(tournamentManager.GetComponent<TournamentManager>().InTournamentMode == true)
            {
                tournamentManager.GetComponent<TournamentManager>().AddScore(1, 50);
            }

            return true;
        }

        return false;
    }
}
