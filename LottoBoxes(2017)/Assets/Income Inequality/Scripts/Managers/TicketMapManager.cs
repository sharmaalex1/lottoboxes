using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// This class handles activation of the TicketMapUI element based on and event trigger.
/// </summary>
public class TicketMapManager : MonoBehaviour
{
    public TicketMapUI ticketMapUI;
    public bool debug;

    private void Start()
    {
        MainGameEventManager.OnTicketPieceFound += RetrieveNewTicketPiece;
    }

    private void OnDestroy()
    {
        MainGameEventManager.OnTicketPieceFound -= RetrieveNewTicketPiece;
    }

    /// <summary>
    /// Subscribes to the event that handles signaling when 
    /// a ticket piece is found "MainGameEventManager.OnTicketPieceFound".
    /// Beings the routine of adding the new ticket obtained from the callback to the ticket map.
    /// </summary>
    private void RetrieveNewTicketPiece(TicketPiece piece, bool ignoreAnimationRoutine)
    {
        if (debug || ignoreAnimationRoutine)
        {
            return;
        }

        Time.timeScale = 0;
        MainGameEventManager.TriggerTicketRoutineBegin();
        ticketMapUI.gameObject.SetActive(true);
        StartCoroutine(ticketMapUI.PlaceNewTicket(piece));   
    }
}
