using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Determine what the box/time ratio will be to distribute items
/// Create a means to generate a bunch of items according to the boxes opened while away.
/// Update UI following distribution.
/// </summary>
public class IdleRewardManager : MonoBehaviour
{
    private const float BOXES_PER_MINUTE_INTERVAL = 0.5f;
    private const int MAXIMUM_IDLE_BOXES = 1000;

    public static event Action OnRewardCollected;

    private DateTime loginDate;
    private DateTime logoutDate;
    private int minutesElapsedSinceLastLogin;

    private int boxesToOpen;
    private int boxRewards;
    private int ticketRewards;

    public GameObject earningsWindow;
    public Text titleText;
    public Text boxesEarnedText;
    public Text ticketsEarnedText;

    public GameObject componentPrefab;

    private void Start()
    {
        loginDate = DateTime.Now;
        logoutDate = SaveManager.Instance.LastLogoutTime;
        minutesElapsedSinceLastLogin = (int)(loginDate - logoutDate).TotalMinutes;
        boxesToOpen = (int)((SaveManager.Instance.CompletedTicketCycles * BOXES_PER_MINUTE_INTERVAL) * minutesElapsedSinceLastLogin);

        // STB - Places a cap on the amount of boxes that can be earned through IdleRewards.
        if (boxesToOpen > 1000)
            boxesToOpen = 1000;
        // STB - Ensures that CurrentBoxCount does not grow past a certain threshold through IdleRewards
        if ((SaveManager.Instance.CurrentBoxCount + boxesToOpen) >= 110000000)
            boxesToOpen = 0;

        // The reward window should be shown only when the player has completed one whole ticket.
        // Even if nothing is obtained from boxes being opened while afk, the window will still be shown.
        // If no boxes are opened the window will not be shown.
        if (SaveManager.Instance.CompletedTicketCycles > 0 && boxesToOpen > 0)
        {
            earningsWindow.SetActive(true);

            // If the amound of boxes to open exceeds the amount the player has, then it will cap out at their amount.
            if (boxesToOpen > SaveManager.Instance.CurrentBoxCount)
            {
                boxesToOpen = SaveManager.Instance.CurrentBoxCount;
            }

            StatisticsManager.Instance.RemoveFromBoxCount(boxesToOpen);
            GenerateRewards();
            DisplayEarnedRewards();
        }

        SaveManager.Instance.SendSaveData += SendIdleData;
    }

    /// <summary>
    /// Determines how many of each reward the player will receive.
    /// </summary>
    private void GenerateRewards()
    {
        for (int i = 0; i < boxesToOpen; i++)
        {
            int randNum = UnityEngine.Random.Range(0, StaticVars.BOX_PERCENT_CHANCE + StaticVars.GOLDTICKET_PERCENT_CHANCE + StaticVars.NOTHING_PERCENT_CHANCE);
            StaticVars.BoxContent current = StaticVars.GetContents(randNum);

            switch (current)
            {
                case StaticVars.BoxContent.Box:
                    {
                        boxRewards += SaveManager.Instance.GetBoxesReceived(StatisticsManager.Instance.CurrentClass, SaveManager.Instance.TotalBoxesOpened);
                    }
                    break;
                case StaticVars.BoxContent.GoldTicket:
                    {
                        ticketRewards++;
                        GameObject component = Instantiate(componentPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                        TicketPiece piece = component.AddComponent<TicketPiece>();
                        MainGameEventManager.TriggerTicketPieceFoundEvent(piece, true);
                        UnityEngine.Object.Destroy(component);

                        if (SaveManager.Instance.NumCurrentTicketPieces >= 5)
                        {
                            StatisticsManager.Instance.ResetTicketPieces();
                            StatisticsManager.Instance.IncrementCompletedTicketCycles();
                        }
                    }
                    break;
                case StaticVars.BoxContent.Nothing:
                    {
                        // May do something for exmpty boxes.
                    }
                    break;
            }
            SaveManager.Instance.TotalBoxesOpened++;
        }            
    }

    /// <summary>
    /// Shows the earnings window, displaying to the player how many boxes were opened
    /// and what they had received from those boxes.
    /// </summary>
    private void DisplayEarnedRewards()
    {
        MainGameEventManager.TriggerBoxFoundEvent(boxRewards);
        titleText.text = string.Format("Opened {0} boxes while away", boxesToOpen);
        boxesEarnedText.text = boxRewards.ToString();
        ticketsEarnedText.text = ticketRewards.ToString();
    }

    private void SendIdleData()
    {
        SaveManager.Instance.LastLogoutTime = DateTime.Now;
    }

    private void OnDestroy()
    {
        SaveManager.Instance.SendSaveData -= SendIdleData;
    }

    public void TriggerOnRewardCollectedEvent()
    {
        if (OnRewardCollected != null)
        {
            OnRewardCollected();
        }
    }
}
