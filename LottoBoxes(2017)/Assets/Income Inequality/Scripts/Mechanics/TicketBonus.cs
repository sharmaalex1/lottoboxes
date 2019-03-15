using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Need to create a save variable for whether or not the player has a bonus available.
// If it is available on startup, set the bonus button object active.
// Otherwise let it be the regular ticket piece viewer button.
public class TicketBonus : MonoBehaviour
{
    public GameObject ticketBonusUI;
    public Text ticketBonusText;

    // Use this for initialization
    void OnEnable()
    {
        //If save manager says bonus is active
        MainGameEventManager.OnAllTicketsFound += ActivateTicketBonusButton;

        if (SaveManager.Instance.IsTicketBonusAvailable)
        {
            ticketBonusUI.SetActive(true);   
        }
        else
        {
            ticketBonusUI.SetActive(false);
        }
            
        ticketBonusText.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        MainGameEventManager.OnAllTicketsFound -= ActivateTicketBonusButton;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            MainGameEventManager.TriggerAllTicketsFoundEvent();
        }
    }

    private void ActivateTicketBonusButton()
    {
        SaveManager.Instance.IsTicketBonusAvailable = true;
        ticketBonusUI.SetActive(true);
    }

    public void GiveTicketBonusWrapper()
    {
        StartCoroutine(GiveTicketBonusRoutine());
    }

    public IEnumerator GiveTicketBonusRoutine()
    {
        int bonus = 50;
        StatisticsManager.Instance.AddToBoxCount(bonus, true);

        StartCoroutine(ShowTicketBonusText(bonus));

        ticketBonusUI.SetActive(false);
        SaveManager.Instance.IsTicketBonusAvailable = false;

        yield break;
    }

    private IEnumerator ShowTicketBonusText(int bonus)
    {
        print("Called");
        GameObject ticketBonusTextObj = ticketBonusText.gameObject;
        Vector3 originalPos = ticketBonusTextObj.transform.position;

        ticketBonusText.gameObject.SetActive(true);
        ticketBonusText.text = string.Format("+{0} Boxes", bonus);

        ticketBonusTextObj.transform.localScale = Vector3.zero;

        iTween.ScaleTo(ticketBonusTextObj, iTween.Hash("scale", Vector3.one, "time", 0.5f));
        iTween.MoveTo(ticketBonusTextObj, iTween.Hash("y", ticketBonusTextObj.transform.position.y + 0.2f, "easetype", iTween.EaseType.easeOutBounce, "time", 0.5f));

        yield return new WaitForSeconds(2.0f);

        ticketBonusTextObj.transform.position = originalPos;
        ticketBonusTextObj.SetActive(false);

        yield break;
    }
}
