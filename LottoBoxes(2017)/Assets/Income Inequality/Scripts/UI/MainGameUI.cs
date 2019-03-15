using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Collections;

/// <summary>
/// This class handles what text is being displayed in the main game.
/// It also handles swapping out the main UI elements such as background, 
/// foreground items, etc... depending on the class.
/// </summary>
public class MainGameUI : MonoBehaviour
{
    public static MainGameUI instance;

    private GameObject curClassCanvas;
    [Tooltip("First: Poor, Last: Rich")]
    public GameObject[] availableClassCanvases;

    public Text ownedBoxCountText, tikcetsOwnedText;

    public Text SwitchCampaignText;

    public Canvas statsPage;

    #region Unity Callbacks

    // Use this for initialization
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
			
        MainGameEventManager.OnGameStart += UpdateUIText;
    }

    private void Start()
    {
        SetupClassCanvas();
        UpdateUIText();
    }

    private void OnDisable()
    {
        MainGameEventManager.OnGameStart -= UpdateUIText;
    }

    #endregion

    #region UI Updates

    public void UpdateBoxCountUI(int boxCount)
    {
        ownedBoxCountText.text = boxCount.ToString("N0");
    }

    public void UpdateTicketCountUI(int ticketCount)
    {
        tikcetsOwnedText.text = ticketCount.ToString();
    }

    private void UpdateUIText()
    {
        UpdateBoxCountUI(SaveManager.Instance.CurrentBoxCount);
        //UpdateTicketCountUI(SaveManager.Instance.CompletedTicketCycles);
		UpdateTicketCountUI(SaveManager.Instance.CurrentOwnedTickets);
    }

    #endregion

    /// <summary>
    /// In the Classes enum, Poor should always be first and rich should always be last.
    /// If this is not the case there will be issues. 
    /// This method takes into account what the current class is and the array of
    /// availableClassCanvases should be a complete reflection of the Classes enum 
    /// values in regards to what is assigned at its indices.
    /// </summary>
    private void SetupClassCanvas()
    {
        switch (StatisticsManager.Instance.CurrentClass)
        {
            case Classes.Poor:
                {
                    curClassCanvas = availableClassCanvases[(int)Classes.Poor];
                    // TODO The below line is temporary. Need a more adaptable way to do this.
                    availableClassCanvases[(int)Classes.Rich].SetActive(false);
                }
                break;
            case Classes.Rich:
                {
                    curClassCanvas = availableClassCanvases[(int)Classes.Rich];
                    // TODO The below line is temporary. Need a more adaptable way to do this.
                    availableClassCanvases[(int)Classes.Poor].SetActive(false);
                }
                break;
            default:
                break;
        }
        curClassCanvas.SetActive(true);

    }

    public void SwitchRichPoorCampaign()
    {

        SaveManager.Instance.CurrentBoxCount += MainGameSpawner.instance.NumOnScreenBoxes;

        SaveManager.Instance.LastLogoutTime = System.DateTime.Now;

        if (StatisticsManager.Instance.CurrentClass == Classes.Poor)
        {
            MainGameEventManager.TriggerChosenCampaignEvent(Classes.Rich);
        }
        else if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
        {
            MainGameEventManager.TriggerChosenCampaignEvent(Classes.Poor);
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    private void DisplaySwitchText()
    {
        string text = "Go To" + System.Environment.NewLine;
        text += StatisticsManager.Instance.CurrentClass == Classes.Poor ? "Rich" : "Poor";
        text += System.Environment.NewLine + "Class";

        SwitchCampaignText.text = text;
    }

    public void ResetData()
    {
		MixpanelManager.DataReset();
        
        StatisticsManager.Instance.UpdateDisasterInfo(new DateTime(), new DateTime(), false, true, false, true, true, true, new DateTime());
        SettingsManager.Instance.SetFirstOfferEver(true);

        SaveManager.Instance.DeleteAndReload();
        ShiftsManager.Instance.ResetShiftTime();
        MainGameEventManager.TriggerChosenCampaignEvent(StatisticsManager.Instance.CurrentClass);
        

        SceneManager.LoadScene((int)Scenes.StartScene);
    }

    public void sendApproval()
    {
        MixpanelManager.Approval();
    }

    public void ShowStatsPage()
    {
        statsPage.gameObject.SetActive(true);
        StatsPageTracker.Instance.SetStats();
    } 

}