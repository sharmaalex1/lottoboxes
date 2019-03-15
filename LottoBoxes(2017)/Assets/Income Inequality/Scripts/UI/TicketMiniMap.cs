using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TicketMiniMap : MonoBehaviour
{
    public Image[] ticketPieces;

    void Start()
    {
        MainGameEventManager.OnTicketRoutineEnd += UpdateVisuals;
        IdleRewardManager.OnRewardCollected += UpdateVisuals;
    }

    void OnDestroy()
    {
        MainGameEventManager.OnTicketRoutineEnd -= UpdateVisuals;
        IdleRewardManager.OnRewardCollected -= UpdateVisuals;
    }

    private void OnEnable()
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
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

}