using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class StatsPageTracker : MonoBehaviour {

    #region Singleton
    private static StatsPageTracker instance;

    public static StatsPageTracker Instance
    {
        get
        {
            return instance;
        }
        private set { }
    }

    private StatsPageTracker()
    {

    }
    #endregion

    /// <summary>
    /// These variables don't need to be set
    /// we can find them by doing operations on other saved variables
    /// </summary>
    #region Properties
    public int NumTicketPiecesFound
    {
        get
        {
            return SaveManager.Instance.CompletedTicketCycles * System.Enum.GetNames(typeof(TicketPieceLocations)).Length + SaveManager.Instance.NumCurrentTicketPieces;
        }
    }

    public string AverageTimePerTicket
    {
        get
        {
            TimeSpan timeThisSession = (DateTime.Now - startTime);
            if(NumTicketPiecesFound == 0)
            {
                return FormatTime((float)((SaveManager.Instance.TimePlayed + timeThisSession).TotalSeconds));
            }

            float totalSeconds = (float)((SaveManager.Instance.TimePlayed + timeThisSession).TotalSeconds);
            return FormatTime(totalSeconds/NumTicketPiecesFound);
        }
    }

    public float BoxesPerTicketPiece
    {
        get
        {
            if (NumTicketPiecesFound == 0)
            {
                return (float)SaveManager.Instance.TotalBoxesOpened;
            }
           
            return SaveManager.Instance.TotalBoxesOpened / NumTicketPiecesFound;
            
        }
    }

    public float BoxesGainedFromOpening
    {
        get
        {
            return SaveManager.Instance.TotalBoxesOwned - (SaveManager.Instance.BoxesGainedFromLoans + SaveManager.Instance.BoxesGainedFromWork);
        }
    }
    #endregion

    //publicly set Text variables for when the Stats Page is shown
    #region Public Text Properties
    
    public Text classText;
    public Text timePlayedText;

    public Text ticketPiecesText;
    public Text averageTimeText;
    public Text boxesPerTicketText;
    public Text boxesFoundText;

    public Text boxesOpenedText;
    public Text loansTakenOutText;
    public Text boxesFromWorkText;
    public Text boxesFromLoansText;
    public Text timeAtWorkText;
    public Text boxesPaidBackText;
    #endregion


    private DateTime startTime;

    // Use this for initialization
    void Awake ()
    {
	    if(instance == null)
        {
            instance = this;
            startTime = DateTime.Now;
            SaveManager.Instance.SendSaveData += CalculateTime;
            SaveManager.Instance.GameUnpaused += NewStartTime;
        }

        else
        {
            Destroy(this.gameObject);
        }
	}
	
    //Function used for subscription for when the game is unpaused
	private void NewStartTime()
    {
        startTime = DateTime.Now;
    }

    //function used to format times 
    //time should be displayed as hh:mm:ss
    private string FormatTime(float timeInSeconds)
    {
        //there are 3600 seconds in hour
        int hours = (int)(timeInSeconds / 3600f);
        timeInSeconds -= hours * 3600f;
        int minutes = (int)(timeInSeconds / 60f);
        timeInSeconds -= minutes * 60;
        int seconds = (int)timeInSeconds;

        string toReturn;
        if(hours <= 9)
        {
            toReturn = "0" + hours.ToString() + ":";
        }
        else
        {
            toReturn = hours.ToString() + ":";
        }

        if(minutes <= 9)
        {
            toReturn += "0" + minutes.ToString() + ":";
        }
        else
        {
            toReturn += minutes.ToString() + ":";
        }

        if(seconds <=9)
        {
            toReturn += "0" + seconds.ToString();
        }
        else
        {
            toReturn += seconds.ToString();
        }

        return toReturn;
    }
    
    //function used for Subscription for SendSaveData Event
    //used to calculate time fwhen 
    private void CalculateTime()
    {
        DateTime endTime = DateTime.Now;
        TimeSpan time = endTime - startTime;
        SaveManager.Instance.TimePlayed += time;
    }

    //if this is destoryed, then game is done or we are changing classes
    //so save data and and unsubscribe from events 
    private void OnDestroy()
    {
        DateTime endTime = DateTime.Now;
        TimeSpan time = (endTime - startTime);
        SaveManager.Instance.TimePlayed += time; 

        SaveManager.Instance.SendSaveData -= CalculateTime;
        SaveManager.Instance.GameUnpaused -= NewStartTime;
    }

    //actually called by main game ui to set the variables for the stats page
    public void SetStats()
    {
        if(StatisticsManager.Instance.CurrentClass == Classes.Rich)
        {
            classText.text = "Lotto Boxes";
        }

        else
        {
            classText.text = "Little Boxes";
        }

        TimeSpan timePlayed = SaveManager.Instance.TimePlayed + (DateTime.Now - startTime);
        timePlayedText.text = FormatTime((float)timePlayed.TotalSeconds);

        averageTimeText.text = AverageTimePerTicket;
        ticketPiecesText.text = NumTicketPiecesFound.ToString("N0");
        boxesPerTicketText.text = BoxesPerTicketPiece.ToString("N0");
        boxesFoundText.text = BoxesGainedFromOpening.ToString("N0");
        boxesOpenedText.text = SaveManager.Instance.TotalBoxesOpened.ToString("N0");
        loansTakenOutText.text = SaveManager.Instance.NumLoansTakenOut.ToString("N0");
        boxesFromWorkText.text = SaveManager.Instance.BoxesGainedFromWork.ToString("N0");
        boxesFromLoansText.text = SaveManager.Instance.BoxesGainedFromLoans.ToString("N0");
        timeAtWorkText.text = FormatTime((float)SaveManager.Instance.TimeAtWork.TotalSeconds);
        boxesPaidBackText.text = SaveManager.Instance.BoxesPaidBack.ToString("N0"); 

    }
}
