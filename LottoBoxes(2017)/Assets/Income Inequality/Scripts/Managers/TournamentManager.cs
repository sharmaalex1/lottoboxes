using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// This manager is handles most/all of the code pertaining to running tournaments/pvp events for LottoBoxes.
/// Any variable/function definitions for tournament-related variables can be found here.
/// </summary>
public class TournamentManager : MonoBehaviour
{
    #region Local Variables
    public GameObject tournamentIntro1;
    public GameObject tournamentIntro2;
    public GameObject usernameSelection;
    public GameObject personalInfoConfirmation;

    public GameObject localScoreCheckmark;

    public Text eventHighscoresText;
    public Text eventCountdownText;

    // Would probably be more efficient to group these all under a tag and just find any object with that
    // tag. For now, we do it the hard way.
    public GameObject expensesUI;
    public GameObject loanUI;
    public GameObject insuranceUI;
    public GameObject disasterUI;
    public GameObject tutorialUI;

    public InputField usernameInput;

    private string eventTitle;
    private string eventDescription;
    private string eventScoresText;
    private string username;
    private string ios_deviceID;
    private string android_ID;


    private string addScoreURL = "http://ec2-54-89-205-102.compute-1.amazonaws.com/updateEvent?";
    private string getPlayerDataURL = "http://ec2-54-89-205-102.compute-1.amazonaws.com/currentEvent.json";
    private string getEventRanksURL = "http://ec2-54-89-205-102.compute-1.amazonaws.com/currentEventRanks.json";
    private string getEventDataURL = "http://ec2-54-89-205-102.compute-1.amazonaws.com/getEventData.json";
    private string clearEventRanksURL = "http://ec2-54-89-205-102.compute-1.amazonaws.com/currentEvent/clearHighscores";

    List<string> playerData = new List<string>();
    List<string> dbEventData = new List<string>();
    List<string> highscores = new List<string>();


    private DateTimeOffset eventStartTime;
    private DateTimeOffset eventEndTime;

    private TimeSpan timeToEventEnd;
    private TimeSpan timeToEventStart;

    private int tournamentScore;
    private int tournamentMultiplier;

    private bool inTournamentMode;
    private bool passedTournyIntro;
    private bool clearData = true;
    private bool receivedConfirmation = false; // trying to prevent coroutines getting called too much
    #endregion

    #region Public Accessors
    public DateTimeOffset EventStartTime
    {
        get
        {
            return eventStartTime.DateTime;
        }
    }

    public DateTimeOffset EventEndTime
    {
        get
        {
            return eventEndTime.DateTime;
        }
    }

    public TimeSpan TimeToEventEnd
    {
        get
        {
            return timeToEventEnd;
        }
    }

    public int TournamentScore
    {
        get
        {
            return tournamentScore;
        }
        set
        {
            tournamentScore = value;
        }
    }

    public int TournamentMultiplier
    {
        get
        {
            return tournamentMultiplier;
        }
    }

    public bool InTournamentMode
    {
        get
        {
            return inTournamentMode;
        }
    }

    public bool PassedTournyIntro
    {
        get
        {
            return passedTournyIntro;
        }
    }

    public string Username
    {
        get
        {
            return username;
        }
    }
    #endregion

    #region Unity Callbacks

    private void OnEnable()
    {
        MainGameEventManager.OnTournyStart += StartTournamentConfirmationHelper;

        StartCoroutine(GetEventInfo());
        StartCoroutine(GetPlayerData());
        StartCoroutine(GetRanks());

        if (inTournamentMode)
        {
            if (!passedTournyIntro)
            {
                StartCoroutine(TournamentConfirmation());
            }
        }
    }

    private void OnDisable()
    {
        MainGameEventManager.OnTournyStart -= StartTournamentConfirmationHelper;
    }

    #endregion

    #region User Info Functions

    // Have a popup where the user enters their desired screen name and save it
    public void CreateUsername()
    {
        username = usernameInput.text;

#if UNITY_IOS || UNITY_STANDALONE_WIN
        ios_deviceID = SystemInfo.deviceUniqueIdentifier;
#endif
        
#if UNITY_ANDROID
        AndroidJavaClass clsUnity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = clsUnity.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject objResolver = objActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass clsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
        android_ID = clsSecure.CallStatic<string>("getString", objResolver, "android_id");
#endif
        // Give the player some boxes if they don't have enough (maybe, might conflict with message of the game)

        usernameSelection.SetActive(false);

        passedTournyIntro = true;

        Debug.Log("Desired username: " + username);
    }

#endregion

#region Database Functions

    // Posting scores to the database and web app
    private IEnumerator PostScores(string name, int score, string deviceID, bool passedIntro)
    {
#if UNITY_IOS || UNITY_STANDALONE_WIN
        string post_url = addScoreURL + "username=" + WWW.EscapeURL(name) + "&score=" + score + "&UDID=" + ios_deviceID + "&passedTutorial=" + Convert.ToInt32(passedTournyIntro);
        Debug.Log("Used iOS or Editor URL");
#endif

#if UNITY_ANDROID
        string post_url = addScoreURL + "username=" + WWW.EscapeURL(name) + "&score=" + score + "&UDID=" + android_ID + "&passedTutorial=" + Convert.ToInt32(passedTournyIntro);
        Debug.Log("Used Android URL");
#endif
        Debug.Log(post_url);

        // Post URL to site and create a download object to get the result

        if (name.Length == 0)
        {
            Debug.Log("No name provided, can't send information to server");
        }
        else
        {
            WWW hs_post = new WWW(post_url);
            yield return hs_post; // wait until download is done

            if (hs_post.error != null)
            {
                Debug.LogError("Error posting the highscore: " + hs_post.error);
            }
            else
            {
                Debug.Log("Post successful");
            }
        }
    }

    /// <summary>
    /// Helps other scripts execute the PostScores coroutine when they need to
    /// </summary>
    public void PostScoresHelper()
    {
#if UNITY_IOS || UNITY_STANDALONE_WIN
        StartCoroutine(PostScores(username, tournamentScore, ios_deviceID, passedTournyIntro));
#endif

#if UNITY_ANDROID
        StartCoroutine(PostScores(username, tournamentScore, android_ID, passedTournyIntro));
#endif
    }

    /*
    // Check to see if username already exists for this event
    private IEnumerator CheckUsername(string name)
    {

       string post_url = check name url + "username=" + WWW.EscapeURL(name);

        Debug.Log(post_url);

        // Post URL to site and create a download object to get the result

        WWW hs_post = new WWW(post_url);
        yield return hs_post; // wait until download is done

        if (hs_post.error != null)
        {
            Debug.LogError("Error posting the highscore: " + hs_post.error);
        }
        else
        {
            Debug.Log("Post successful");
        }
    }
    */

    private IEnumerator GetPlayerData()
    {
        WWW hs_get = new WWW(getPlayerDataURL);
        yield return hs_get;

        if (hs_get.error != null)
        {
            Debug.LogError("There was an error getting the highscore: " + hs_get.error);
        }
        else if(hs_get.text == "[]")
        {
            Debug.Log("No scores yet for this event");
        }
        else
        {
            eventScoresText = hs_get.text;

            string[] playerData2;

            playerData.Add(eventScoresText);

            for (int i = 0; i < playerData.Count; i++)
            {
                playerData[i] = Regex.Replace(playerData[i], "[^\\w\\._]", " ");
            }

            playerData2 = playerData[0].Split(' ');

            playerData.Clear();

            for (int i = 0; i < playerData2.Length; i++)
            {
                playerData.Add(playerData2[i]);
            }

            playerData.RemoveAll(item => item == " ");

            playerData.RemoveAll(item => item == string.Empty);

            for (int i = 0; i < playerData.Count; i++)
            {
                Debug.Log(playerData[i]);
            }

            for(int i = 0; i < playerData.Count; i++)
            {
#if UNITY_IOS || UNITY_STANDALONE_WIN
                if(SystemInfo.deviceUniqueIdentifier == playerData[i])
                {
                    username = playerData[i - 2];
                    tournamentScore = int.Parse(playerData[i - 1]);
                    passedTournyIntro = bool.Parse(playerData[i + 1]);

                    Debug.Log("Local Username: " + username);
                    Debug.Log("Score: " + tournamentScore);
                    Debug.Log("UDID: " + SystemInfo.deviceUniqueIdentifier);
                    Debug.Log("Passed Intro?: " + passedTournyIntro);
                    break;
                }
#endif

#if UNITY_ANDROID
                if(android_ID == playerData[i])
                {
                    username = playerData[i - 2];
                    tournamentScore = int.Parse(playerData[i - 1]);
                    passedTournyIntro = bool.Parse(playerData[i + 1]);
                    
                    Debug.Log("Local Username: " + username);
                    Debug.Log("Score: " + tournamentScore);
                    Debug.Log("UDID: " + android_ID);
                    Debug.Log("Passed Intro?: " + passedTournyIntro);
                    break;
                }
#endif
            }
        }
    }

    /// <summary>
    /// Helps other scripts execute the GetScores coroutine when they need to
    /// </summary>
    public void GetPlayerDataHelper()
    {
        StartCoroutine(GetPlayerData());
    }

    private IEnumerator GetRanks()
    {
        WWW ranks_get = new WWW(getEventRanksURL);
        yield return ranks_get;

        if (ranks_get.error != null)
        {
            Debug.LogError("There was an error getting the highscore: " + ranks_get.error);
        }
        else if(inTournamentMode == false)
        {
            if(DateTime.Compare(DateTime.Now, eventEndTime.DateTime) > 0)
            {
                eventCountdownText.text = "The last event ended at: " + "\n" + eventStartTime.DateTime;
            }

            if (DateTime.Compare(DateTime.Now, eventStartTime.DateTime) < 0)
            {
                eventCountdownText.text = "Next event starts at: " + "\n" + eventStartTime.DateTime;
            }
            // add a case in which a new event has not been set
            // should read: last event was at: start time for that event
            // grab it off of the first set data we get from the server
        }
        else if (ranks_get.text == "[]")
        {
            Debug.Log("No scores yet for this event");
        }
        else
        {
            Debug.Log("Retrieved the top highscores");

            localScoreCheckmark.SetActive(false);

            eventScoresText = ranks_get.text;

            string[] highscores2;

            highscores.Clear();

            highscores.Add(eventScoresText);

            for (int i = 0; i < highscores.Count; i++)
            {
                highscores[i] = Regex.Replace(highscores[i], "[^\\w\\._]", " ");
            }

            highscores2 = highscores[0].Split(' ');

            highscores.Clear();

            for (int i = 0; i < highscores2.Length; i++)
            {
                highscores.Add(highscores2[i]);
            }

            highscores.RemoveAll(item => item == " ");

            highscores.RemoveAll(item => item == string.Empty);
            
            for(int i = 0; i < highscores.Count; i++)
            {
                Debug.Log(highscores[i]);
            }

            Debug.Log(highscores.Count);

                if (highscores.Count == 2)
                {
                    Debug.Log("One score");
                    eventHighscoresText.text = highscores[0] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[1] + "\n" + "\n";
                    
                }
                else if(highscores.Count == 4)
                {
                    Debug.Log("Two score");

                    eventHighscoresText.text = highscores[0] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[1] + "\n" + "\n" +
                    highscores[2] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[3] + "\n" + "\n";


                }
                else if(highscores.Count == 6)
                {
                    Debug.Log("Three score");

                    eventHighscoresText.text = highscores[0] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[1] + "\n" + "\n" +
                    highscores[2] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[3] + "\n" + "\n" +
                    highscores[4] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[5] + "\n" + "\n";


                }
                else if(highscores.Count == 8)
                {
                    Debug.Log("Four score");

                    eventHighscoresText.text = highscores[0] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[1] + "\n" + "\n" +
                    highscores[2] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[3] + "\n" + "\n" +
                    highscores[4] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[5] + "\n" + "\n" +
                    highscores[6] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[7] + "\n" + "\n";

                }
                else if(highscores.Count >= 10)
                {
                    Debug.Log("Five score");

                    eventHighscoresText.text = highscores[0] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[1] + "\n" + "\n" + 
                    highscores[2] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[3] + "\n" + "\n" +
                    highscores[4] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[5] + "\n" + "\n" +
                    highscores[6] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[7] + "\n" + "\n" +
                    highscores[8] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[9] + "\n" + "\n";
                    
                }

        }
    }

    public void GetRanksHelper()
    {
        StartCoroutine(GetRanks());
    }

    private IEnumerator ClearCurrentHighscores()
    {
        WWW clear_highscores = new WWW(clearEventRanksURL);

        yield return clear_highscores;

        Debug.Log("Cleared current highscores");
    }

    public void FindLocalRank()
    {
        if(localScoreCheckmark.activeSelf)
        {
            localScoreCheckmark.SetActive(false);
            StartCoroutine(GetRanks());
        }
        else if(!localScoreCheckmark.activeSelf)
        {
            localScoreCheckmark.SetActive(true);

            for (int i = 0; i < highscores.Count; i++)
            {
                if (i == highscores.Count - 1)
                {
                    break;
                }
                else
                {
                    if (highscores[i] == username)
                    {
                        eventHighscoresText.text = highscores[i] + "\t" + "\t" + "\t" + "\t" + "\t" + highscores[i + 1] + "\n" + "\n";
                        Debug.Log("Found the user's score");
                        //tournamentScore = int.Parse(highscores[i + 1]);
                    }
                }
            }
        }
    }

    private IEnumerator GetEventInfo()
    {
        WWW event_get = new WWW(getEventDataURL);

        yield return event_get;

        if (event_get.error != null)
        {
            Debug.LogError("There was an error getting the event data: " + event_get.error);
        }
        else
        {
            foreach (Match m in Regex.Matches(event_get.text, @"(?<=([\'\""])).*?(?=\1)"))
            {
                dbEventData.Add(m.Value);
            }

            dbEventData.RemoveAll(item => item == ",");

            dbEventData.RemoveAll(item => item == "],[");


            for (int i = 0; i < dbEventData.Count; i++)
            {
                Debug.Log(dbEventData[i]);

                // If we haven't found a match by the time we get close to the end
                // then just break out of the loop to avoid an Out of Range Excepton
                if(i == (dbEventData.Count) - 1)
                {
                    eventStartTime = DateTimeOffset.Parse(dbEventData[2]);
                    eventEndTime = DateTimeOffset.Parse(dbEventData[3]);
                    inTournamentMode = false;

                    StartCoroutine(UpdateEventTimer());

                    break;
                }

                DateTimeOffset.TryParse(dbEventData[i], out eventStartTime);

                if(DateTimeOffset.TryParse(dbEventData[i], out eventStartTime))
                {
                    if (DateTime.Compare(DateTime.Now, eventStartTime.DateTime) < 0)
                    {
                        eventStartTime = DateTimeOffset.Parse(dbEventData[i]);
                        timeToEventStart = eventStartTime.DateTime.Subtract(DateTime.Now);
                        eventEndTime = DateTimeOffset.Parse(dbEventData[i + 1]);
                        inTournamentMode = false;

                        StartCoroutine(UpdateEventTimer());

                        break;
                    }
                    else if(DateTime.Compare(DateTime.Now, eventStartTime.DateTime) > 0)
                    {
                        DateTimeOffset.TryParse(dbEventData[i + 1], out eventEndTime);

                        if (DateTime.Compare(DateTime.Now, eventStartTime.DateTime) > 0 && DateTime.Compare(DateTime.Now, eventEndTime.DateTime) < 0)
                        {
                            eventStartTime = DateTimeOffset.Parse(dbEventData[i]);
                            eventEndTime = DateTimeOffset.Parse(dbEventData[i + 1]);
                            timeToEventEnd = eventEndTime.DateTime.Subtract(DateTime.Now);
                            inTournamentMode = true;

                            eventCountdownText.text = "This event ends at: " + "\n" + eventEndTime.DateTime;

                            StartCoroutine(UpdateEventTimer());

                            break;
                        }
                    }
                }
                        

                Debug.Log("Parse attempt for START time: " + eventStartTime.DateTime);
                Debug.Log("Parse attempt for END time: " + eventEndTime.DateTime);
                Debug.Log(DateTime.Now);

                // Check to see if the current time is before the end date that we parsed
            }

            if(inTournamentMode)
            {
                Debug.Log("Acutal difference between now and end time on server: " + timeToEventEnd);
            }
            else
            {
                Debug.Log("Acutal difference between now and start time on server: " + timeToEventStart);
            }

            Debug.Log("No Offset Start: " + eventStartTime.DateTime);
            Debug.Log("No Offset End: " + eventEndTime.DateTime);
            Debug.Log(DateTime.Now);
            Debug.Log("Tournament Active?: " + inTournamentMode);       
        }
    }

    private IEnumerator UpdateEventTimer()
    {
        
        while(inTournamentMode)
        {
             if(DateTime.Compare(DateTime.Now, eventEndTime.DateTime) > 0)
             {
                 Debug.Log("Reached Clear Data");
                 Debug.Log("Cleared because there is no event active");
                 StartCoroutine(ClearCurrentHighscores());
                 eventHighscoresText.text = " ";
                 inTournamentMode = false;
             }
             else if(DateTime.Compare(DateTime.Now, eventStartTime.DateTime) > 0 && DateTime.Compare(DateTime.Now, eventEndTime.DateTime) < 0)
             {
                if (!passedTournyIntro && !receivedConfirmation)
                {
                    StartCoroutine(TournamentConfirmation());
                    receivedConfirmation = true;
                }
            }

            yield return null;

        }
        

        while(!inTournamentMode)
        {
            if(clearData)
            {
                Debug.Log("Cleared because there is no event active");
                StartCoroutine(ClearCurrentHighscores());
                eventHighscoresText.text = " ";
                clearData = false;
            }
            else
            {
                Debug.Log("Cleared and waiting for event to start");
            }


            if (DateTime.Compare(DateTime.Now, eventStartTime.DateTime) > 0 && DateTime.Compare(DateTime.Now, eventEndTime.DateTime) < 0)
            {
                Debug.Log("Reached Tournament Setup");
                StartCoroutine(GetEventInfo());
                StartCoroutine(GetPlayerData());
                StartCoroutine(GetRanks());

                inTournamentMode = true;

                if (inTournamentMode)
                {
                    if (!passedTournyIntro)
                    {
                        StartCoroutine(TournamentConfirmation());
                    }
                }
            }

            yield return null;
        }
        
        yield break;

    }

    /// <summary>
    /// Helps other scripts execute the GetEventInfo coroutine when they need to
    /// </summary>
    public void GetEventInfoHelper()
    {
        StartCoroutine(GetEventInfo());
    }

#endregion

#region Utility Methods

    // These functions are a temp intro to the tournament mode until
    // we can get something more robust in there.
/*
    private IEnumerator StartTournamentIntro()
    {
        yield return new WaitForSeconds(1.1f);
        if (inTournamentMode == true)
        {
            if(tutorialUI == null)
            {
                if (loanUI.activeSelf || insuranceUI.activeSelf || expensesUI.activeSelf)
                {
                    yield return null;
                }
                else
                {
                    tournamentIntro1.SetActive(true);
                    yield break;
                }
            }
            else
            {
                if (tutorialUI.activeSelf || loanUI.activeSelf || insuranceUI.activeSelf || expensesUI.activeSelf)
                {
                    yield return null;
                }
                else
                {
                    tournamentIntro1.SetActive(true);
                    yield break;
                }
            }
        }

        yield return null;
    }

    public void StartTournamentIntroHelper()
    {
        StartCoroutine(StartTournamentIntro());
    }
    */

    private IEnumerator TournamentConfirmation()
    {
        if (inTournamentMode)
        {
            Debug.Log("Reached here");
            if (tutorialUI == null)
            {
                Debug.Log("And here");
                if (loanUI.activeSelf || insuranceUI.activeSelf || expensesUI.activeSelf)
                {
                    Debug.Log("Also here");
                    yield return null;
                }
                else
                {
                    Debug.Log("This place too");
                    personalInfoConfirmation.SetActive(true);
                    yield break;
                }
            }
            else
            {
                if (tutorialUI.activeSelf || loanUI.activeSelf || insuranceUI.activeSelf || expensesUI.activeSelf)
                {
                    Debug.Log("As well as here");
                    yield return null;
                }
                else
                {
                    Debug.Log("How?");
                    personalInfoConfirmation.SetActive(true);
                    yield break;
                }
            }
        }

        yield break;
    }

    public void StartTournamentConfirmationHelper()
    {
        StartCoroutine(TournamentConfirmation());
    }

    public void TournamentActive()
    {
        personalInfoConfirmation.SetActive(false);

        tournamentIntro1.SetActive(true);
    }

    public void TournamentRejection()
    {
        personalInfoConfirmation.SetActive(false);
    }

    public void FirstIntroBox()
    {
        tournamentIntro1.SetActive(false);

        tournamentIntro2.SetActive(true);
    }

    public void SecondIntroBox()
    {
        tournamentIntro2.SetActive(false);

        usernameSelection.SetActive(true);
    }

    public void AddScore(int scoreToAdd, int scoreMultiplier)
    {
        TournamentScore = TournamentScore + (scoreToAdd * scoreMultiplier);
    }

#endregion
}
