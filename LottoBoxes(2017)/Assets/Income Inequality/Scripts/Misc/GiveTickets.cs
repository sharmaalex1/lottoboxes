using UnityEngine;
using System.Collections;

public class GiveTickets : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SaveManager.Instance.CurrentOwnedTickets++;
            gameObject.GetComponent<MainGameUI>().UpdateTicketCountUI(SaveManager.Instance.CurrentOwnedTickets);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SaveManager.Instance.CurrentOwnedTickets += 10;
            gameObject.GetComponent<MainGameUI>().UpdateTicketCountUI(SaveManager.Instance.CurrentOwnedTickets);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SaveManager.Instance.CurrentOwnedTickets += 100;
            gameObject.GetComponent<MainGameUI>().UpdateTicketCountUI(SaveManager.Instance.CurrentOwnedTickets);
        }

        if(Input.GetKeyUp(KeyCode.UpArrow))
        {
            MainGameEventManager.TriggerAllTicketsFoundEvent();
        }
    }
}
