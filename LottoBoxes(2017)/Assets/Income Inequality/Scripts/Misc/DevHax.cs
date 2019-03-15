using UnityEngine;
using System.Collections;

public class DevHax : MonoBehaviour
{
    public void UnlockPoorClass()
    {
        if (!SaveManager.Instance.UnlockedClasses.Contains(Classes.Poor))
        {
            SaveManager.Instance.UnlockedClasses.Add(Classes.Poor);   
        }
        else
        {
            Debug.Log("Stop spamming the unlock button!");
        }
    }

    public void AddTicketPiece()
    {
        MainGameEventManager.TriggerAllTicketsFoundEvent();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            int amountToAdd = 200;

            StatisticsManager.Instance.AddToBoxCount(amountToAdd, false);
        }
    }
}
