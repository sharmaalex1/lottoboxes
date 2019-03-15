using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Handles turning on and off the Loan UI.
/// Triggers the loan UI to popup upon having zero boxes.
/// Dishes out a certain amount of boxes to the player if they concede.
/// After a certain amount of time, the owed boxes are retrieved from the player.
/// The retrieved boxes also include interest.
/// The boxes are taken and can leave the player with a negaitve box balance.
/// </summary>
public class LoanManager : MonoBehaviour
{
    private const float INTEREST_RATE_RICH = 0.08f;
    private const float INTEREST_RATE_POOR = 0.30f;
    private const int MINIMUM_LOAN = 50;
    private const int DEBT_COLLECTION_TIME_IN_SECONDS = 150;
    /// <summary>
    /// The minimum time a player has to be in the scene for before the shark can
    /// collect its loan.
    /// </summary>
    private const float MINIMUM_TIME_FOR_COLLECTION = 2.0f;

    public static event Action OnLoanDenied;
    public static event Action OnLoanDisbursed;
    public static event Action OnPartialLoanCollected;
    public static event Action OnFullLoanCollected;

    // Offer that will be distributed if accepted (interest exclusive)
    private int boxesOffered;
    // What the player will owe (boxesOwed + (boxesOwed * interestRate))
    private int boxesOwed;
    private bool isLoanActive;
    public bool IsLoanActive
    {
        get
        {
            return isLoanActive;
        }
    }

    private DateTime timeOfLoanCollection;
    private TimeSpan timeUntilCollection;
    private float timeActive;
    private float curInterestRate;

    // Position of the BoxesOwned UI element in the scene.
    private RectTransform boxesUIPosition;

    // The LoanShark prefab.
    public GameObject sharkFab;
    // The entire loanUI element in the scene
    public GameObject loanUI;
    // This on the "BoxesOwned" UI element.
    public GameObject boxPair;

    public GameObject disasterUI;
    public GameObject expensesUI;
    public GameObject insuranceUI;

	public GameObject GoalManager;

    public GameObject loanPaybackBar;
    public Text interestRateText;

    public Text loanOffering;
    public Text expectedRepayment;
    public Text loanSharkTimeUntilCollectionText;

	public int loanBribeUpperLimit;

    private GameObject opportunityBoxLoans;
    private TimeSpan opportunityBoxTimeToAddLoans;

    private Coroutine loansTimer;

    #region Unity Callbacks

    // Use this for initialization
    private void OnEnable()
    {
        SubscribeToEvents();

        boxesOffered = 0;
        isLoanActive = SaveManager.Instance.IsLoanActive;
        timeActive = Time.time;
        curInterestRate = StatisticsManager.Instance.CurrentClass == Classes.Rich ? INTEREST_RATE_RICH : INTEREST_RATE_POOR;

        interestRateText.gameObject.SetActive(false);

        if (isLoanActive)
        {
            boxesOwed = SaveManager.Instance.BoxesOwed;   
            timeOfLoanCollection = SaveManager.Instance.TimeOfLoanCollection;
            timeUntilCollection = timeOfLoanCollection.Subtract(DateTime.Now);

            loansTimer = StartCoroutine(UpdateTimer());
        }
    }

    private void Start()
    {
        boxesUIPosition = GameObject.FindGameObjectWithTag("BoxOwnedUI").GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        UnSubscribeToEvents();
    }

    private void Update()
    {
        #if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TestAcceptLoan();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            ShowLoanPopup();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(GenerateShark());
        }


#endif
    }

    #endregion

    #region Button Methods

    /// <summary>
    /// This method is called on a button press.
    /// Sets the appropriate variables that pertain to debt collection.
    /// Handles distributing the appropriate amount of boxes.
    /// Sets the timer for debt collection into motion as well.
    /// </summary>
    public void AcceptLoan()
    {
        DistributeLoan(boxesOffered);

        loanUI.SetActive(false);

		if(!SaveManager.Instance.goal20Completion)
			GoalManager.GetComponent<GoalManager> ().GoalUnlocked (20, false);
    }

    /// <summary>
    /// This method is called upon a button press.
    /// If the player denies the loan, this method will be called.
    /// </summary>
    public void DenyLoan()
    {
        Debug.Log(string.Format("The player has denied the loan"));
        // Could probably have a minimum wait time before another loan can be asked for...
        loanUI.SetActive(false);
        if(OnLoanDenied != null)
        {
            OnLoanDenied();
        }
    }


    #endregion

    #region Loan Manipulation Methods

    /// <summary>
    /// If the player accepts the loan, this method will add boxes to 
    /// their current count according to the offer made.
    /// </summary>
    private void DistributeLoan(int loanAmount)
    {
        SaveManager.Instance.NumLoansTakenOut++;
        boxesOwed = boxesOffered + Mathf.RoundToInt(boxesOffered * curInterestRate);

        SaveManager.Instance.BoxesGainedFromLoans += loanAmount;
        isLoanActive = true;
        timeOfLoanCollection = DateTime.Now.Add(new TimeSpan(0, 0, DEBT_COLLECTION_TIME_IN_SECONDS));

        StatisticsManager.Instance.AddToBoxCount(loanAmount);
        StatisticsManager.Instance.UpdateLoanInfo(boxesOwed, timeOfLoanCollection, isLoanActive);

        loansTimer = StartCoroutine(UpdateTimer());
        TriggerOnLoanDisbursedEvent();

        //This range of strings regarding info text are all applicable for this situation.
        PopupWindowManager.Instance.ActivatePopup((InfoStrings)Enum.GetValues(typeof(InfoStrings)).GetValue(UnityEngine.Random.Range(1, 4)));
    }

    /// <summary>
    /// Handles decreasing the CurrentBoxCount, but this does not make changes to the UI in MainGameUI immediately.
    /// </summary>
	public void CollectLoan(bool bypassBribe)
    {
        if (SaveManager.Instance.CurrentBoxCount < boxesOwed)
        {
            // If the player doesn't have enough boxes to pay back their loan,
            // take 90% of their current box count.
            // Also, apply the compounding interest rate due to full repaymanet failure.
            // Finished up by adding another two minutes to the loan collection timer.
			int bribeRoll = Random.Range (1, loanBribeUpperLimit);
			if (bribeRoll == (int)(loanBribeUpperLimit / 2) && !bypassBribe) 
			{
				this.GetComponent<LoanFurnitureManager> ().determineItem ();
			}
			else
			{
				int newBoxCollectAmount = (int)(SaveManager.Instance.CurrentBoxCount * 0.9f);
				StatisticsManager.Instance.RemoveFromBoxCount (newBoxCollectAmount, false);
				SaveManager.Instance.BoxesPaidBack += newBoxCollectAmount;

				boxesOwed -= newBoxCollectAmount;
				boxesOwed += (int)(boxesOwed * curInterestRate);
			}
            timeOfLoanCollection = DateTime.Now.Add(new TimeSpan(0, 0, DEBT_COLLECTION_TIME_IN_SECONDS));
            StatisticsManager.Instance.UpdateLoanInfo(boxesOwed, timeOfLoanCollection, isLoanActive);

            TriggerOnPartialLoanCollectedEvent();

			SaveManager.Instance.loansFailedStreak += 1;
			Debug.Log ("Failed Streak: " + SaveManager.Instance.loansFailedStreak);

            PopupWindowManager.Instance.ActivatePopup(InfoStrings.AvoidDeeperDebt);

			if (!SaveManager.Instance.goal19Completion) {
				GoalManager.GetComponent<GoalManager> ().GoalUnlocked (19, false);
			}
        }
        else
        {
            StatisticsManager.Instance.RemoveFromBoxCount(boxesOwed, false);
            SaveManager.Instance.BoxesPaidBack += boxesOwed;

            ClearLoan();

            // Calling this twice because the game has issues sliding the
            // loan timer out when you only call it once for some reason.
            TriggerOnFullLoanCollectedEvent();
            TriggerOnFullLoanCollectedEvent();

            SaveManager.Instance.loansPaidCount += 1;
			SaveManager.Instance.loansFailedStreak = 0;

			if (!SaveManager.Instance.goal17Completion)
            {
				GoalManager.GetComponent<GoalManager> ().GoalUnlocked (17, false);
			}
        }
    }

    /// <summary>
    /// Resets all variables associated with a loan since it will have been collected when this is called.
    /// </summary>
    private void ClearLoan()
    {
        boxesOffered = 0;
        boxesOwed = 0;
        timeOfLoanCollection = new DateTime();
        isLoanActive = false;
        StatisticsManager.Instance.UpdateLoanInfo(boxesOwed, timeOfLoanCollection, isLoanActive);
    }

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

    #endregion

    #region Utility Methods

    private IEnumerator UpdateTimer()
    {
        while (isLoanActive)
        {            
            timeUntilCollection = timeOfLoanCollection.Subtract(DateTime.Now);

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

            loanSharkTimeUntilCollectionText.text = time;

            if (totalSeconds <= 0)
            {
                if(!disasterUI.activeSelf && !insuranceUI.activeSelf && !expensesUI.activeSelf)
                {

                    loanSharkTimeUntilCollectionText.text = "00:00";

                    if (Time.time < timeActive + 2.0f)

                    {

                        yield return new WaitForSeconds(timeActive + 2.0f - Time.time);

                    }

					CollectLoan(false);

                    StartCoroutine(GenerateShark());

                }
            }   

            yield return null;
        }

        yield break;
    }

    /// <summary>
    /// Makes the loan UI visible.
    /// Calls a method to calculate the loan offering.
    /// Sets up the text elements that pertain to offering a loan.
    /// This will not be called if the player has a negative box balance.
    /// </summary>
    private void ShowLoanPopup()
    {
        // If we don't already have a loan, show the popup.
        if (!isLoanActive && SaveManager.Instance.CurrentBoxCount >= 0)
        {
            AudioManager.Instance.PlayAudioClip(SFXType.SharkAppears);
            boxesOffered = CalculateBoxOffering();
            loanOffering.text = boxesOffered.ToString();
            expectedRepayment.text = Mathf.RoundToInt(boxesOffered + (boxesOffered * curInterestRate)).ToString();
            loanUI.SetActive(true);   
        }
    }

    /// <summary>
    /// Returns the amount of boxes will be offered to the player.
    /// For now the value is set to be whatever the total amount of boxes have been opened by the player.
    /// </summary>      
    private int CalculateBoxOffering()
    {
        int offer = SaveManager.Instance.TotalBoxesOpened < MINIMUM_LOAN ? MINIMUM_LOAN : SaveManager.Instance.TotalBoxesOpened;
                
        return offer;
    }

    /// <summary>
    /// Creates a LoanShark GameObject which plays an animation for loan collection.
    /// </summary>
    private IEnumerator GenerateShark()
    {
        AudioManager.Instance.PlayAudioClip(SFXType.SharkAppears);
        GameObject loanShark = Instantiate(sharkFab, boxesUIPosition.position, Quaternion.identity) as GameObject;
        Vector3 currentSharkPos = loanShark.transform.position;
        loanShark.transform.position = new Vector3(currentSharkPos.x - loanShark.GetComponent<SpriteRenderer>().sprite.bounds.max.x, currentSharkPos.y, currentSharkPos.z);

        iTween.MoveTo(loanShark, iTween.Hash("position", new Vector3(boxPair.transform.position.x - 1, boxPair.transform.position.y, 5),
                "time", 0.8f,
                "easetype", iTween.EaseType.linear));

        yield return new WaitForSeconds(0.8f);
        AudioManager.Instance.PlayAudioClip(SFXType.SharkStealsBoxes);
        yield return new WaitForSeconds(0.1f);
        boxPair.SetActive(false);
        // This needs to happen as the shark runs through its animation.
        MainGameUI.instance.UpdateBoxCountUI(SaveManager.Instance.CurrentBoxCount);

        yield return new WaitForSeconds(0.5f);
        iTween.MoveTo(loanShark, iTween.Hash("position", new Vector3(boxPair.transform.position.x + 10, boxPair.transform.position.y, loanShark.transform.position.z),
                "time", 1.8f,
                "easetype", iTween.EaseType.linear));

        iTween.ColorTo(loanShark.gameObject, new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.5f);
        yield return new WaitForSeconds(0.5f);

        Destroy(loanShark.gameObject);
        iTween.PunchScale(boxPair.gameObject, Vector3.one * 0.15f, 0.35f);
        boxPair.SetActive(true);

        yield break;
    }

    private void SubscribeToEvents()
    {
        MainGameEventManager.OnBoxCountExhausted += ShowLoanPopup;
        SaveManager.Instance.SendSaveData += SendLoanData;
        OnPartialLoanCollected += ShowInterestAmountAddedWrapper;

        MainGameEventManager.OnOpportunityBoxSpawn += AddOpportunityBoxTimerLoans;
        MainGameEventManager.OnOpportunityBoxSpawn += OpportunityBoxStopLoanTimer;
        MainGameEventManager.OnOpportunityBoxDespawn += OpportunityBoxStartLoanTimer;
    }

    private void UnSubscribeToEvents()
    {
        MainGameEventManager.OnBoxCountExhausted -= ShowLoanPopup;
        SaveManager.Instance.SendSaveData -= SendLoanData;
        OnPartialLoanCollected -= ShowInterestAmountAddedWrapper;

        MainGameEventManager.OnOpportunityBoxSpawn -= AddOpportunityBoxTimerLoans;
        MainGameEventManager.OnOpportunityBoxSpawn -= OpportunityBoxStopLoanTimer;
        MainGameEventManager.OnOpportunityBoxDespawn -= OpportunityBoxStartLoanTimer;
    }

    private void SendLoanData()
    {
        StatisticsManager.Instance.UpdateLoanInfo(boxesOwed, timeOfLoanCollection, isLoanActive);
    }

    /// <summary>
    /// For the loans timer, adds original timer from exisiting oppertunity box
    /// </summary>
    private void AddOpportunityBoxTimerLoans()
    {

        while (opportunityBoxLoans == null)
        {
            opportunityBoxLoans = GameObject.FindGameObjectWithTag("OpportunitySafe");

            if (opportunityBoxLoans != null)
            {
                break;
            }
        }

        if (isLoanActive)
        {
            Debug.Log("Time to add to Loans: " + opportunityBoxLoans.GetComponent<OpportunityBox>().opportunityBoxOriginalTimer);

            opportunityBoxTimeToAddLoans = new TimeSpan(0, 0, Mathf.CeilToInt(opportunityBoxLoans.GetComponent<OpportunityBox>().opportunityBoxOriginalTimer));

            timeOfLoanCollection += opportunityBoxTimeToAddLoans;

            Debug.Log("Loans Expiration: " + timeOfLoanCollection);
        }

    }

    /// <summary>
    /// Used to start the loans timer, primarily called by the opportunity box system
    /// </summary>
    public void OpportunityBoxStartLoanTimer()
    {
        if(isLoanActive)
        {
            loansTimer = StartCoroutine(UpdateTimer());
        }
    }

    /// <summary>
    /// Used to stop the loans timer, primarily called by the opportunity box system
    /// </summary>
    public void OpportunityBoxStopLoanTimer()
    {
        if(isLoanActive)
        {
            StopCoroutine(loansTimer);
        }
    }

    #endregion

    #region Test Methods

#if UNITY_EDITOR

    public void TestAcceptLoan()
    {
        boxesOffered = 60000;
        DistributeLoan(boxesOffered);

        loanUI.SetActive(false);
    }

    #endif

    #endregion

    #region Event Trigger Methods

    private static void TriggerOnLoanDisbursedEvent()
    {
        if (OnLoanDisbursed != null)
        {
            Debug.Log("Loan taken out");
            OnLoanDisbursed();
        }
    }

    private static void TriggerOnPartialLoanCollectedEvent()
    {
        if (OnPartialLoanCollected != null)
        {
            OnPartialLoanCollected();
        }
    }

    /// <summary>
    /// This is called twice when a player fully pays back a loan to avoid
    /// UI issues. Keep this in mind when you are calling or adding functions to this.
    /// </summary>
    private static void TriggerOnFullLoanCollectedEvent()
    {
        if (OnFullLoanCollected != null)
        {
            OnFullLoanCollected();
        }
    }

    #endregion
}