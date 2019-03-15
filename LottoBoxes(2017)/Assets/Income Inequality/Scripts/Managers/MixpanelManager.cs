using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

// Mixpanel events are sent through this class
public class MixpanelManager : MonoBehaviour
{
    // Assign to this in the inspector
    private static string versionNumber;

    // today's date
    private static int currentDay;

    // the instance of this script
    public static MixpanelManager instance;

    const string PREFS_FIRST_USE = "FirstUse";
    const string PREFS_DAYS_SINCE_START = "DaysSinceStart";
    const string PREFS_FIRST_DAY = "FirstDay";
    const string PREFS_SESSION_COUNT = "SessionCount";
    const string PREFS_DATE_TIME_FORMAT = "yyyy-MM-dd";
    const string PREFS_CURRENT_DAY = "CurrentDay";

    const string EVENT_DATE_TIME_FORMAT = "yyyy-MM-ddTHH:mm:ss";


    private void Awake()
    {
        // singleton setup - don't destroy this when loading
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            versionNumber = Application.version;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        Mixpanel.Token = "f4b617289a11f4167590ff110e82cfbd";

        Mixpanel.SuperProperties.Clear();

        if (PlayerPrefs.GetInt("FirstUse") == 0)
        {
            Install();
            PlayerPrefs.SetInt("FirstUse", 1);
        }

        GameOpened();

        MainGameEventManager.OnGameStart += EnterHome;

    }

    private void OnDestroy()
    {
        MainGameEventManager.OnGameStart -= EnterHome;
    }

    #region Gameplay Tracking

    private void EnterHome()
    {
        Mixpanel.SendEvent("Enter Home");
    }

    public static void EnterWork()
    {
        Mixpanel.SendEvent("Enter Work");
    }

    public static void ExitWork(int score, int highestMulti)
    {
        Mixpanel.SendEvent("Exit Work", new Dictionary<string, object>
            {
                { "Boxes Sorted", score },
                { "Highest Multiplier", highestMulti },
            });
    }

    public static void BillStart()
    {
        Mixpanel.SendEvent("Bills Start");
    }

    public static void InsuranceStart()
    {
        Mixpanel.SendEvent("Insurance Start");
    }

	public static void DataReset()
	{
		Mixpanel.SendEvent("DataReset");
	}

	public static void FeedbackWindowDisplayed()
	{
		Mixpanel.SendEvent("Feedback Window Displayed");
	}

	public static void Approval()
    {
        Mixpanel.SendEvent("Approval");
    }

    public static void ClassUnlocked(Classes incomeClass)
    {
        Mixpanel.SendEvent("Class Unlocked", new Dictionary<string, object>
            {
                { "Unlocked", incomeClass },
            });
    }

    public static void tenBoxesOpenedInSession()
    {
        Mixpanel.SendEvent("10 Boxes Opened");
    }

    public static void fiftyBoxesOpenedInSession()
    {
        Mixpanel.SendEvent("50 Boxes Opened");
    }

    public static void hundredBoxesOpenedInSession()
    {
        Mixpanel.SendEvent("100 Boxes Opened");
    }

    public static void twoFiftyBoxesOpenedInSession()
    {
        Mixpanel.SendEvent("250 Boxes Opened");
    }

    public static void ClassChosen(Classes incomeClass)
    {
		
        // Add or update the income class when chosen
        if (!Mixpanel.SuperProperties.ContainsKey("Class"))
        {
            Mixpanel.SuperProperties.Add("Class", incomeClass);
        }
        else
        {
            Mixpanel.SuperProperties["Class"] = incomeClass;
        }
    }

    public static void TicketPieceCollected(TicketPiece piece)
    {
        Mixpanel.SendEvent("Ticket Piece Found", new Dictionary<string, object>
            {
                // Piece id's are 0 - 4
                { "Piece Of Ticket", piece.id + 1 },
                { "Ticket Count", SaveManager.Instance.CompletedTicketCycles },
            });
    }

    public static void ShowInfoPopUp(string message)
    {
        Mixpanel.SendEvent("Show Pop Up", new Dictionary<string, object>
            {
                { "Message", message }
            });
    }

    public static void InfoPopUpClicked()
    {
        Mixpanel.SendEvent("Pop Up Clicked");
    }

    #endregion


    // Install
    // When: The game is installed
    // People: see FirstUse()
    public static void Install()
    {
        // Upate people properties
        FirstUse(DateTime.Now.ToString(EVENT_DATE_TIME_FORMAT));

        //TODO These can be made people properties, if desirable.
//        PlayerPrefs.SetString("Date:First Use", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
//        Mixpanel.SuperProperties.Add("Date:First Use", PlayerPrefs.GetString("Date:First Use"));
//        PlayerPrefs.SetString("Initial Version", versionNumber);
//        Mixpanel.SuperProperties.Add("Initial Version", PlayerPrefs.GetString("Initial Version"));
//        PlayerPrefs.SetInt("FirstUse", 1);
//        PlayerPrefs.SetString("FirstDay", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
//        PlayerPrefs.Save();

        Mixpanel.SendEvent("Install");
    }

    // gameOpened
    // When: Each time the game is opened
    // People: Date: Last seen, Days In App, Sessions Counter, appVersion
    public static void GameOpened()
    {
        // Send event that the game has been opened
        Mixpanel.SendEvent("GameOpened");

        // For "Days in App"
        // Determine current day
        currentDay = DateTime.Today.Day;

        // if the stored current day is not today
        if (PlayerPrefs.GetInt(PREFS_CURRENT_DAY) != currentDay)
        {
            // update it and determine how many days it has been
            PlayerPrefs.SetInt(PREFS_CURRENT_DAY, currentDay);
            IncrementDaysSinceStart();
        }

        // For "Sessions Counter"
        // grab session count, increment it, send it, and store it
        int sessionCount = PlayerPrefs.GetInt(PREFS_SESSION_COUNT);
        sessionCount++;
        PlayerPrefs.SetInt(PREFS_SESSION_COUNT, sessionCount);


        // Send people properties associated with the game being opened
        Mixpanel.SendPeople(new Dictionary<string, object>
            {
                { "Date: Last Seen", DateTime.Now.ToString(EVENT_DATE_TIME_FORMAT) },
                { "Days In App", PlayerPrefs.GetInt(PREFS_DAYS_SINCE_START) },
                { "Sessions Counter", PlayerPrefs.GetInt(PREFS_SESSION_COUNT) },
                { "appVersion", versionNumber },
            });
    }


    #region Gameplay Tracking

    // gamePlay
    // When: Each time the user begins playing
    // replay is true if the player hit a replay button at the end of the previous game
    // People: gamesPlayed counter
    public static void GamePlay(bool replay)
    {
        // Send gameplay event
        Mixpanel.SendEvent("gamePlay", new Dictionary<string, object>
            {
                { "Replay", replay ? "Yes" : "No" },
            });


        // Update counter for total games played
        int gamesPlayed = PlayerPrefs.GetInt("gamesPlayed");
        gamesPlayed++;
        PlayerPrefs.SetInt("gamesPlayed", gamesPlayed);


        // Send counter to mixpanel as a people property
        Mixpanel.SendPeople(new Dictionary<string, object>
            {
                { "gamesPlayed", gamesPlayed },
            });
    }

    // gameResult
    // When: Each time the user reaches the end of game screen
    // bestScore is true if the user beat their previous best score
    // People: userBestScore
    public static void GameResult(bool bestScore, int score)
    {
        // Send gameResult event
        Mixpanel.SendEvent("gameResult", new Dictionary<string, object>
            {
                { "Score", score },
                { "BestScore", bestScore ? "Yes" : "No" },
            });


        // If this is the users best score, update the mixpanel profile with it as well
        if (bestScore)
        {
            // Send counter to mixpanel as a people property
            Mixpanel.SendPeople(new Dictionary<string, object>
                {
                    { "userBestScore", score },
                });
        }
    }

    #endregion

    #region People Properties

    #region General First Use

    // All properties get updated at the same time (when the user first opens the app) we can send them together
    // Event: Install
    private static void FirstUse(string date)
    {
        Mixpanel.SendPeople(new Dictionary<string ,object>
            {
                { "Operating System", SystemInfo.operatingSystem },
                { "Device Model", SystemInfo.deviceModel },
                { "Date First Use", date },
                { "Distinct ID", Mixpanel.DistinctID }
            }, "set_once");
    }

    #endregion

    #endregion

    #region Misc Functions

    // Updates DaysSinceStart for Days In App
    private static void IncrementDaysSinceStart()
    {
        int tempInt = PlayerPrefs.GetInt(PREFS_DAYS_SINCE_START);
        if (PlayerPrefs.GetString(PREFS_FIRST_DAY) != DateTime.Now.ToString(PREFS_DATE_TIME_FORMAT))
        {
            tempInt++;
            PlayerPrefs.SetInt(PREFS_DAYS_SINCE_START, tempInt);
            PlayerPrefs.SetString(PREFS_FIRST_DAY, DateTime.Now.ToString(PREFS_DATE_TIME_FORMAT));
        }
    }

    #endregion

	
    #region Helper Functions

    // changes a string that was a float into time Minutes:Seconds
    public static string ConvertFloatToTimeString(float _time)
    {
        TimeSpan time = TimeSpan.FromSeconds(_time);
        return string.Format("{0:D2}:{1:D2}:{2:D2}", time.Minutes, time.Seconds, time.Milliseconds);
    }

    #endregion
}