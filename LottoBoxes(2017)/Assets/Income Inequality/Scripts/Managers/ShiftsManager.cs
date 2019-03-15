using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

//This class handles the shift system
//Handles the wait for work timer in the main game
//handles the working timer in the mini game
public class ShiftsManager : MonoBehaviour
{

    public GameObject GoalManager;
    //the time the player can work in seconds
    private float TIME_PLAYER_CAN_WORK = 60f;
    //the time the player has to wait to go to work
    private float TIME_TILL_PLAYER_CAN_WORK = 180f;

    //Timer for when the player is in the
    private float workingTimer;
    //How long the player has left until going to work
    private float waitForWorkTimer;

    private Coroutine currentRoutineRunning;

    //Reference to what the work button originally says
    private string originalToWorkText;

    //reference to UI slider 
    private UISlider sliderReference;

    private GameObject opportunityBoxWork;

    //readonly Property for other scripts to access
    public float WaitForWorkTimer
    {
        get
        {
            return waitForWorkTimer;
        }
        private set
        {

        }
    }

    #region Public UI Vars

    [SerializeField]
    [Header("The tab GamObject for the Return to Work UI")]
    private GameObject tab;

    [SerializeField]
    [Header("The Guy Image for the return to work UI")]
    private Image workingGuy;

    [SerializeField]
    [Header("The Text For the Return to Work UI")]
    private Text ToWorkText;
   
    [SerializeField]
    [Header("The Text for the time in the Mini Game")]
    private Text sortingTimeText;

    #endregion


    #region Singleton

    private static ShiftsManager instance;

    public static ShiftsManager Instance
    {
        get
        {
            return instance;
        }

        private set { }
    }


    private ShiftsManager()
    {
    }

    #endregion

    private void Awake()
    {
        GoalManager = GameObject.Find("GoalManager");
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(this.gameObject);
            SubscribeToEvents();
            originalToWorkText = ToWorkText.text;

            //find UISlider
            sliderReference = GameObject.Find("CanvasContainer").GetComponent<UISlider>();
            if (sliderReference == null)
            {
                #if UNITY_EDITOR
                Debug.LogError("Couldn't find slider reference");
                #endif
            }

            //check the timing
            CheckTimingEnter();       
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    #region Information Automation

    private void OnApplicationPause(bool pauseStatus)
    {
        //if pausing
        if (pauseStatus)
        {
            //check if a routine is running
            if (currentRoutineRunning != null)
            {
                //stop the coroutine and set the reference equal to null
                StopCoroutine(currentRoutineRunning);
                currentRoutineRunning = null;
            }
        }

        //if the player just entered the game
        if (!pauseStatus)
        {
            //if we aren't running the coroutine
            if (currentRoutineRunning == null)
            {
                //if we are in the mini game
                if(sliderReference.CurrentElement == MainSceneUIElements.MiniGame)
                {
                    //restart the mini game timer
                    currentRoutineRunning = StartCoroutine(WorkingCountdown());
                }
                else
                {
                    //if we are in the Main Game check the timer
                    CheckTimingEnter();
                }
            }
        }
    }
#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        if (currentRoutineRunning != null)
        {
            StopCoroutine(currentRoutineRunning);
        }
    }

#endif

    //Function for subscription to send its save data to the save manager before saving
    private void SendShiftData()
    {
        SaveManager.Instance.ShiftTime = waitForWorkTimer;
    }

    #endregion


    #region Event Subscription

    //Subscribe To Relevant Events
    private void SubscribeToEvents()
    {
        //need to find actual event
        //MainGameEventManager.OnSwitchCampaign += AdjustToSwitchCampaigns;
        UISlider.OnSlide += AdjustToSceneChange;
        SaveManager.Instance.SendSaveData += SendShiftData;
        MiniGameTutorial.StartMiniGame += StartWorkTimer;

        MainGameEventManager.OnOpportunityBoxSpawn += AddOpportunityBoxTimerWork;
        MainGameEventManager.OnOpportunityBoxSpawn += OpportunityBoxStopWorkTimer;
        MainGameEventManager.OnOpportunityBoxDespawn += OpportunityBoxStartWorkTimer;
    }

    //UnSubscribe to relevant events to prevent memory leaks
    private void UnSubscribeToEvents()
    {
        //need to find the correct event
        //MainGameEventManager.OnSwitchCampaign -= AdjustToSwitchCampaigns;
        UISlider.OnSlide -= AdjustToSceneChange;
        SaveManager.Instance.SendSaveData -= SendShiftData;
        MiniGameTutorial.StartMiniGame -= StartWorkTimer;
        MainGameEventManager.OnOpportunityBoxSpawn -= AddOpportunityBoxTimerWork;
        MainGameEventManager.OnOpportunityBoxSpawn -= OpportunityBoxStopWorkTimer;
        MainGameEventManager.OnOpportunityBoxDespawn -= OpportunityBoxStartWorkTimer;
    }


    private void OnDestroy()
    {
        UnSubscribeToEvents();
        if (currentRoutineRunning != null)
        {
            StopCoroutine(currentRoutineRunning);
        }
        SaveManager.Instance.LastLogoutTime = DateTime.Now;
        SaveManager.Instance.ShiftTime = waitForWorkTimer;
    }

    //Function to Subscription 
    //adjusts the Shift Manager based on If the game is changed between Main and Mini game
    private void AdjustToSceneChange(int sceneChangedTo)
    {
        //if changing to main game 
        if (sceneChangedTo == (int)MainSceneUIElements.MainGame)
        {
            //stop running the mini game coroutine 
            if (currentRoutineRunning != null)
            {
                StopCoroutine(currentRoutineRunning);
            }

            //added the correct amount of time worked to the SaveManager
            if (workingTimer <= 0)
            {
                int minutes = (int)(TIME_PLAYER_CAN_WORK / 60f);
                int seconds = (int)(TIME_PLAYER_CAN_WORK % 60f);
                SaveManager.Instance.TimeAtWork += new TimeSpan(0, minutes, seconds);
            }
            else
            {
                float timeWorked = TIME_PLAYER_CAN_WORK - workingTimer;
                int minutes = (int)(timeWorked / 60f);
                int seconds = (int)(timeWorked % 60f);
                SaveManager.Instance.TimeAtWork += new TimeSpan(0, minutes, seconds);

            }
            //start the wait for work timer
            currentRoutineRunning = StartCoroutine(WaitForWorkCountdown());
        }
        //if we are changing to the mini game 
        else if (sceneChangedTo == (int)MainSceneUIElements.MiniGame)
        {
            //set waitForWorkTimer just in case the player exits the game while in mini game
            if (SaveManager.Instance.goal08Completion)
            {
                TIME_TILL_PLAYER_CAN_WORK = 120f;
                Debug.Log("Setting work time to 2 minutes");
            }
            waitForWorkTimer = TIME_TILL_PLAYER_CAN_WORK;

            if (SaveManager.Instance.goal09Completion)
            {
                TIME_PLAYER_CAN_WORK = 90f;
                Debug.Log("Setting work length to 1:30");
            }
            workingTimer = TIME_PLAYER_CAN_WORK;
            if (MiniGameTutorial.Instance == null)
            {
                currentRoutineRunning = StartCoroutine(WorkingCountdown());
            }
        }
    }

    #endregion

    #region CountDown Functions

    //The countDown in the Main Game 
    private IEnumerator WaitForWorkCountdown()
    {
        ToWorkText.text = FormatSeconds(waitForWorkTimer);
        SetUpUI(false);

        yield return null;

        while (waitForWorkTimer > 0)
        {
            waitForWorkTimer -= Time.deltaTime;
            ToWorkText.text = "Shift In:" + System.Environment.NewLine + FormatSeconds(waitForWorkTimer);
            yield return null;
        }

        waitForWorkTimer = 0;
        SetUpUI(true);

        yield break;

    }

    //the countdown in the Mini Game 
    private IEnumerator WorkingCountdown()
    {
        sortingTimeText.text = FormatSeconds(workingTimer);
        yield return null;

        while (workingTimer > 0)
        {
            workingTimer -= Time.deltaTime;
            sortingTimeText.text = FormatSeconds(workingTimer);
            yield return null;
        }
        workingTimer = 0;
        //force trigger the slide event 
        sliderReference.TriggerSlideEvent((int)MainSceneUIElements.MainGame);
        yield break;
    }

    #endregion

    #region Utility

    //sets up the UI in the main game depending on if the player can go to work or not
    private void SetUpUI(bool isOn)
    {
        Image tabImage = tab.GetComponent<Image>();
        if (tabImage == null)
        {

            #if UNITY_EDITOR
            Debug.Log("Couldn't Find Image Component on Return to Work Tab");
            #endif
            return;
        }

        Button tabButton = tab.GetComponent<Button>();
        if (tabImage == null)
        {

            #if UNITY_EDITOR
            Debug.Log("Couldn't Find Button Component on Return to Work Tab");
            #endif
            return;
        }

        tabButton.interactable = isOn;
        tabImage.raycastTarget = isOn;

        if (!isOn)
        {
            //make it the same color as the button disabled color(used when interacable is false;
            workingGuy.color = tabButton.colors.disabledColor;
            ToWorkText.text = string.Empty;
            //quick fix until we get new UI
            ToWorkText.fontSize = 40;
        }
        else if (isOn)
        {
            workingGuy.color = new Color(1f, 1f, 1f);
            ToWorkText.text = originalToWorkText;
            //Quick Fix until we get new UI
            ToWorkText.fontSize = 50;
        }

    }

    //format seconds for text 
    private String FormatSeconds(float secondsToConvert)
    {
        int seconds = Mathf.CeilToInt(secondsToConvert);
        int minutes = seconds / 60;
        seconds -= minutes * 60;
        
        //make sure the seconds have two digits
        if (seconds < 10)
        {
            return minutes.ToString() + ":0" + seconds.ToString();
        }

        return minutes.ToString() + ":" + seconds.ToString();
    }


    private void CheckTimingEnter()
    {
        //variables necessary to see how long we were outside of the game 
        DateTime loginTime = DateTime.Now;

        //seconds passes
        float secondsPassed = (float)(loginTime - SaveManager.Instance.LastLogoutTime).TotalSeconds;

        // Prevent any weirdness with shifting timezones
        if (secondsPassed > TIME_TILL_PLAYER_CAN_WORK)
        {
            secondsPassed = TIME_TILL_PLAYER_CAN_WORK;
        }

        //are we in the MainMiniComboScene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == (int)Scenes.MainMiniCombo)
        {
            //are we in the MainGame Portion of that scene
            if (sliderReference.CurrentElement == (int)MainSceneUIElements.MainGame)
            {
                //has the player waited long enough
                if (SaveManager.Instance.ShiftTime - secondsPassed <= 0)
                {
                    waitForWorkTimer = 0;
                    SaveManager.Instance.ShiftTime = 0;
                    SetUpUI(true);
                    if (currentRoutineRunning != null)
                    {
                        StopCoroutine(currentRoutineRunning);
                    }
                }
                //the player needs to wait longer
                else
                {
                    waitForWorkTimer = SaveManager.Instance.ShiftTime - secondsPassed;
                    currentRoutineRunning = StartCoroutine(WaitForWorkCountdown());
                    SaveManager.Instance.ShiftTime = waitForWorkTimer;
                }
            }
        }
    }

    /// <summary>
    /// For the work timer, adds original timer from exisiting oppertunity box
    /// </summary>
    private void AddOpportunityBoxTimerWork()
    {

        while (opportunityBoxWork == null)
        {
            opportunityBoxWork = GameObject.FindGameObjectWithTag("OpportunitySafe");

            if (opportunityBoxWork != null)
            {
                break;
            }
        }

        if (waitForWorkTimer > 0)
        {
            Debug.Log("Time to add to Work: " + opportunityBoxWork.GetComponent<OpportunityBox>().opportunityBoxOriginalTimer);

            waitForWorkTimer += opportunityBoxWork.GetComponent<OpportunityBox>().opportunityBoxOriginalTimer;

            Debug.Log("Time left until work: " + waitForWorkTimer);
        }

    }

    /// <summary>
    /// Utilized by opportunity box system to regulate "pausing" this mechanic's timer
    /// </summary>
    public void OpportunityBoxStartWorkTimer()
    {
        if(waitForWorkTimer > 0)
        {
            currentRoutineRunning = StartCoroutine(WorkingCountdown());
        }
    }

    /// <summary>
    /// Utilized by opportunity box system to regulate "pausing" this mechanic's timer
    /// </summary>
    public void OpportunityBoxStopWorkTimer()
    {
        if(waitForWorkTimer > 0)
        {
            StopCoroutine(currentRoutineRunning);
        }
    }

    #endregion

    public void ResetShiftTime()
    {
        if (currentRoutineRunning != null)
        {
            StopCoroutine(currentRoutineRunning);
        }
        waitForWorkTimer = 0.0f;
    }

    private void StartWorkTimer()
    {
        currentRoutineRunning = StartCoroutine(WorkingCountdown());
    }
}
