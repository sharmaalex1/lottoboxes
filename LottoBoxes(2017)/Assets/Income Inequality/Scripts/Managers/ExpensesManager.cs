using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

/// <summary>
/// Handles turning on and off the Bill UI.
/// Triggers the bill UI to popup upon having zero boxes.
/// After a certain amount of time, the owed boxes are retrieved from the player.
/// </summary>
public class ExpensesManager : MonoBehaviour
{
    GameObject GoalManager;
    //private const float INTEREST_RATE_RICH = 0.08f;
    //private const float INTEREST_RATE_POOR = 0.30f;
    private const int MINIMUM_BILL = 15;
    private const int BILL_COLLECTION_TIME_IN_SECONDS = 300;
    /// <summary>
    /// The minimum time a player has to be in the scene for before the Bill collector can
    /// collect its bill.
    /// </summary>
    private const float MINIMUM_TIME_FOR_COLLECTION = 3.0f;

    public static event Action OnFirstBill;
    public static event Action OnPartialBillCollected;
    public static event Action OnFullBillCollected;

    // Offer that will be distributed if accepted (interest exclusive)
    //private int boxesOffered;

    // What the player will owe (boxesOwed + billBoxesDue)
    private int billBoxesDue = 11;
    private int boxesOwed;
    private bool isBillActive;

    public bool IsBillActive
    {
        get
        {
            return isBillActive;
        }
    }

    private bool firstBillNotDone = true;
    private DateTime timeOfBillCollection;
    private TimeSpan timeUntilCollection;
    private float timeActive;
    //private float curInterestRate;

    // Position of the BoxesOwned UI element in the scene.
    private RectTransform boxesUIPosition;

    // The Collector prefab.
    public GameObject collectorFab;
    // The entire billUI element in the scene
    public GameObject billUI;
    // This on the "BoxesOwned" UI element.
    public GameObject boxPair;

    public GameObject disasterUI;
    public GameObject loanUI;
    public GameObject insuranceUI;

    public GameObject billPaybackBar;
    //public Text interestRateText;
    
    public Text expectedRepayment;
    public Text collectorTimeUntilCollectionText;

    private GameObject opportunityBoxExpenses; 
    private TimeSpan opportunityBoxTimeToAddExpenses;

    private Coroutine expensesTimer;

    #region Unity Callbacks

    // Use this for initialization
    private void OnEnable()
    {

        //boxesOffered = 0;
        isBillActive = SaveManager.Instance.IsBillActive;

        SubscribeToEvents();

        timeActive = Time.time;
        //curInterestRate = StatisticsManager.Instance.CurrentClass == Classes.Rich ? INTEREST_RATE_RICH : INTEREST_RATE_POOR;

        //interestRateText.gameObject.SetActive(false);

        if (isBillActive)
        {
            boxesOwed = SaveManager.Instance.BoxesOwedBill;
            timeOfBillCollection = SaveManager.Instance.TimeOfBillCollection;
            timeUntilCollection = timeOfBillCollection.Subtract(DateTime.Now);

            expensesTimer = StartCoroutine(UpdateTimer());
        }
        MixpanelManager.BillStart();
    }

    private void Start()
    {
        boxesUIPosition = GameObject.FindGameObjectWithTag("BoxOwnedUI").GetComponent<RectTransform>();
        GoalManager = GameObject.Find("GoalManager");
    }
    
    private void OnDisable()
    {
        UnSubscribeToEvents();
    }

    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.N))
        {
            TestAcceptBill();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ShowBillPopup();
        }

#endif
        /*
        if (firstBillNotDone)
        {
            if (SaveManager.Instance.TotalBoxesOpened >= 100 && !SaveManager.Instance.IsBillActive && !MainGameSpawner.instance.hyperModeisActive)
            {
                firstBillNotDone = false;
                MainGameEventManager.TriggerFirstHundredBoxCountEvent();
            }
        }
        */
    }

    #endregion

    #region Button Methods

    /// <summary>
    /// This method is called on a button press.
    /// Sets the appropriate variables that pertain to debt collection.
    /// Handles distributing the appropriate amount of boxes.
    /// Sets the timer for debt collection into motion as well.
    /// </summary>
    public void AcceptBill()
    {
        FirstBill(boxesOwed);

        billUI.SetActive(false);
        if (!SaveManager.Instance.goal23Completion)
            GoalManager.GetComponent<GoalManager>().GoalUnlocked(23, false);
        
    }

    
    /// <summary>
    /// This method is called upon a button press.
    /// If the player denies the bill, this method will be called.
    /// </summary>
    public void DenyBill()
    {
        FirstBill(boxesOwed);

        billUI.SetActive(false);
        //Debug.Log(string.Format("The player has denied the bill"));
        // Could probably have a minimum wait time before another bill can be asked for...
        //billUI.SetActive(false);
    }
    

    #endregion

    #region Bill Manipulation Methods

    /// <summary>
    /// When the player accepts the bill, this method will start the billPayment UI and 
    /// start the timer.
    /// </summary>
    private void FirstBill(int billAmount)
    {
        boxesOwed = CalculateBillDues();
        isBillActive = true;
        timeOfBillCollection = DateTime.Now.Add(new TimeSpan(0, 0, BILL_COLLECTION_TIME_IN_SECONDS));
        
        StatisticsManager.Instance.UpdateBillInfo(boxesOwed, timeOfBillCollection, isBillActive);

        expensesTimer = StartCoroutine(UpdateTimer());
        TriggerOnFirstBillEvent();
    }

    /// <summary>
    /// Handles decreasing the CurrentBoxCount, but this does not make changes to the UI in MainGameUI immediately.
    /// </summary>
    private void CollectBill()
    {
        if (SaveManager.Instance.CurrentBoxCount < boxesOwed)
        {
            // If the player doesn't have enough boxes to pay back their bill,
            // take 90% of their current box count.
            // Add new bill to what is left of old bill.
            // Finished up by reseting the bill collection timer.
            int newBoxCollectAmount = (int)(SaveManager.Instance.CurrentBoxCount * 0.9f);
            StatisticsManager.Instance.RemoveFromBoxCount(newBoxCollectAmount, false);
            SaveManager.Instance.BillsPaid += newBoxCollectAmount;

            boxesOwed -= newBoxCollectAmount;
            boxesOwed += CalculateBillDues();

            timeOfBillCollection = DateTime.Now.Add(new TimeSpan(0, 0, BILL_COLLECTION_TIME_IN_SECONDS));
            StatisticsManager.Instance.UpdateBillInfo(boxesOwed, timeOfBillCollection, isBillActive);

            TriggerOnPartialBillCollectedEvent();
        }
        else
        {
            StatisticsManager.Instance.RemoveFromBoxCount(boxesOwed, false);
            timeOfBillCollection = DateTime.Now.Add(new TimeSpan(0, 0, BILL_COLLECTION_TIME_IN_SECONDS));
            SaveManager.Instance.BillsPaid += boxesOwed;

            boxesOwed = CalculateBillDues();

            StatisticsManager.Instance.UpdateBillInfo(boxesOwed, timeOfBillCollection, isBillActive);

            TriggerOnFullBillCollectedEvent();
        }

        Debug.Log("Amount of Boxes Paid in Bills so far: " + SaveManager.Instance.BillsPaid);
    }

    /*
    /// <summary>
    /// Resets all variables associated with a bill since it will have been collected when this is called.
    /// </summary>
    private void ClearBill()
    {
        boxesOwed = 0;
        timeOfBillCollection = new DateTime();
        isBillActive = false;
        StatisticsManager.Instance.UpdateBillInfo(boxesOwed, timeOfBillCollection);

        //SlideBillUI(false);
    }
    /*
    private void ShowInterestAmountAddedWrapper()
    {
        StartCoroutine(ShowInterestAmountAdded());
    }
    
    private IEnumerator ShowInterestAmountAdded()
    {
        GameObject interestRateTextObj = interestRateText.gameObject;
        Vector3 originalPos = interestRateTextObj.transform.position;

        yield return new WaitForSeconds(1.0f);

        interestRateText.gameObject.SetActive(true);
        interestRateText.text = string.Format("+{0}% \nLate Fee", curInterestRate * 100);

        interestRateTextObj.transform.localScale = Vector3.zero;

        iTween.ScaleTo(interestRateTextObj, iTween.Hash("scale", Vector3.one, "time", 0.5f));
        iTween.MoveTo(interestRateTextObj, iTween.Hash("y", interestRateTextObj.transform.position.y + 0.2f, "easetype", iTween.EaseType.easeOutBounce, "time", 0.5f));

        yield return new WaitForSeconds(2.0f);

        interestRateTextObj.transform.position = originalPos;
        interestRateTextObj.SetActive(false);

        yield break;
    }
    */
    #endregion

    #region Utility Methods

    private IEnumerator UpdateTimer()
    {
        while (isBillActive)
        {
            timeUntilCollection = timeOfBillCollection.Subtract(DateTime.Now);

            string time = "";
            int seconds = timeUntilCollection.Seconds;
            int minutes = timeUntilCollection.Minutes;
            float totalSeconds = timeUntilCollection.Seconds + (timeUntilCollection.Minutes * 60);

            if (minutes < 10)
            {
                time += "0";
            }
            time += minutes + ":";

            if (seconds < 10)
            {
                time += "0";
            }
            time += seconds;

            collectorTimeUntilCollectionText.text = time;

            if (totalSeconds <= 0)
            {
                if (!disasterUI.activeSelf && !insuranceUI.activeSelf && !loanUI.activeSelf)
                {
                    collectorTimeUntilCollectionText.text = "00:00";

                    if (Time.time < timeActive + 2.0f)

                    {

                        yield return new WaitForSeconds(timeActive + 2.0f - Time.time);

                    }

                    CollectBill();

                    StartCoroutine(GenerateCollector());
                }
            }

            yield return null;
        }

        yield break;
    }
    
    /// <summary>
    /// Makes the bill UI visible.
    /// Calls a method to calculate the bill offering.
    /// Sets up the text elements that pertain to offering a bill.
    /// This will not be called if the player has a negative box balance.
    /// </summary>
    private void ShowBillPopup()
    {
        // If we don't already have a bill, show the popup.
        if (!isBillActive && SaveManager.Instance.TotalBoxesOpened >= 100)
        {
            //AudioManager.Instance.PlayAudioClip(SFXType.BillScreenAppears);
            boxesOwed = CalculateBillDues();
            expectedRepayment.text = boxesOwed.ToString();
            billUI.SetActive(true);
        }
    }
    
    /// <summary>
    /// Returns the amount of boxes will be offered to the player.
    /// For now the value is set to be whatever the total amount of boxes have been opened by the player.
    /// </summary>      
    private int CalculateBillDues()
    {
        int bill;

        if (GoalManager.GetComponent<GoalLibrary>().GetCompletionStatus(24)) //Modify bill cost to 20% less if 'Stampede' is completed.
            bill = SaveManager.Instance.TotalBoxesOpened < MINIMUM_BILL ? MINIMUM_BILL : Mathf.RoundToInt(SaveManager.Instance.TotalBoxesOpened / 6);
        else
            bill = SaveManager.Instance.TotalBoxesOpened < MINIMUM_BILL ? MINIMUM_BILL : Mathf.RoundToInt(SaveManager.Instance.TotalBoxesOpened / 5);

        return bill;
    }    
    
    /// <summary>
    /// Creates a billShark GameObject which plays an animation for bill collection.
    /// </summary>
    private IEnumerator GenerateCollector()
    {
        //AudioManager.Instance.PlayAudioClip(SFXType.BuffaloStampede);
        GameObject billCollector = Instantiate(collectorFab, boxesUIPosition.position, Quaternion.identity) as GameObject;
        Vector3 currentSharkPos = billCollector.transform.position;
        billCollector.transform.position = new Vector3(currentSharkPos.x - billCollector.GetComponent<SpriteRenderer>().sprite.bounds.max.x, currentSharkPos.y, currentSharkPos.z);

        yield return new WaitForSeconds(0.445f);

        boxPair.SetActive(false);
        // This needs to happen as the shark runs through its animation.
        MainGameUI.instance.UpdateBoxCountUI(SaveManager.Instance.CurrentBoxCount);

        yield return new WaitForSeconds(0.5f);

        boxPair.SetActive(true);

        yield break;
    }

    private void SubscribeToEvents()
    {
        MainGameEventManager.OnFirstHundredBoxCount += ShowBillPopup;
        SaveManager.Instance.SendSaveData += SendBillData;

        MainGameEventManager.OnOpportunityBoxSpawn += AddOpportunityBoxTimerExpenses;
        MainGameEventManager.OnOpportunityBoxSpawn += OpportunityBoxStopExpensesTimer;
        MainGameEventManager.OnOpportunityBoxDespawn += OpportunityBoxStartExpensesTimer;
    }

    private void UnSubscribeToEvents()
    {
        MainGameEventManager.OnFirstHundredBoxCount -= ShowBillPopup;
        SaveManager.Instance.SendSaveData -= SendBillData;

        MainGameEventManager.OnOpportunityBoxSpawn -= AddOpportunityBoxTimerExpenses;
        MainGameEventManager.OnOpportunityBoxSpawn -= OpportunityBoxStopExpensesTimer;
        MainGameEventManager.OnOpportunityBoxDespawn -= OpportunityBoxStartExpensesTimer;
        //OnPartialBillCollected -= ShowInterestAmountAddedWrapper;
    }

    private void SendBillData()
    {
        StatisticsManager.Instance.UpdateBillInfo(boxesOwed, timeOfBillCollection, isBillActive);
    }

    /// <summary>
    /// For expenses timer, adds original timer from exisiting oppertunity box
    /// </summary>
    private void AddOpportunityBoxTimerExpenses()
    {

        while (opportunityBoxExpenses == null)
        {
            opportunityBoxExpenses = GameObject.FindGameObjectWithTag("OpportunitySafe");

            if (opportunityBoxExpenses != null)
            {
                break;
            }
        }

        if (isBillActive)
        {
            Debug.Log("Time to add to expenses: " + opportunityBoxExpenses.GetComponent<OpportunityBox>().opportunityBoxOriginalTimer);

            opportunityBoxTimeToAddExpenses = new TimeSpan(0, 0, Mathf.CeilToInt(opportunityBoxExpenses.GetComponent<OpportunityBox>().opportunityBoxOriginalTimer));

            timeOfBillCollection += opportunityBoxTimeToAddExpenses;

            Debug.Log("Expenses Expiration: " + timeOfBillCollection);
        }

    }

    /// <summary>
    /// Utilized by opportunity box system to regulate "pausing" this mechanic's timer
    /// </summary>
    public void OpportunityBoxStartExpensesTimer()
    {
        if(isBillActive)
        {
            expensesTimer = StartCoroutine(UpdateTimer());
        }
    }

    /// <summary>
    /// Utilized by opportunity box system to regulate "pausing" this mechanic's timer
    /// </summary>
    public void OpportunityBoxStopExpensesTimer()
    {
        Debug.Log("Is bill active?: " + isBillActive);
        if(isBillActive)
        {
            Debug.Log("Stopping Bill Timer");
            StopCoroutine(expensesTimer);
        }
    }

    #endregion

    #region Test Methods

#if UNITY_EDITOR

    public void TestAcceptBill()
    {
        boxesOwed = 60000;
        FirstBill(boxesOwed);

        billUI.SetActive(false);
    }

#endif

    #endregion

    #region Event Trigger Methods

    private static void TriggerOnFirstBillEvent()
    {
        if (OnFirstBill != null)
        {
            Debug.Log("Bill");
            OnFirstBill();
        }
    }

    private static void TriggerOnPartialBillCollectedEvent()
    {
        if (OnPartialBillCollected != null)
        {
            OnPartialBillCollected();
        }
    }

    private static void TriggerOnFullBillCollectedEvent()
    {
        if (OnFullBillCollected != null)
        {
            OnFullBillCollected();
        }
    }

    #endregion
}
