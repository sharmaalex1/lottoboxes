using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

// both UnityEngine nad System both have implementations for these
// need to specify which ones we want to use
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

/*ALGORITHM
 *  Start with 5 boxes
 *  1 on screen
 *  4 in the queue waiting to be on screen 
 */

public class MainGameSpawner : MonoBehaviour, ISpawnable
{
    /// <summary>
    /// Maximum possible boxes that can be on screen at once.
    /// </summary>
    private static int maxOnScreenBoxes = 6;
    /// <summary>
    /// Amount of time the spawner waits to check the number of on screen boxes again.
    /// </summary>
    private const float SPAWN_WAIT = 0.8f;
    /// <summary>
    /// Length that hyper mode will last in seconds.
    /// </summary>
    public const float HYPER_MODE_DURATION = 20.0f;

    private IEnumerator countdownRoutine;
    private IEnumerator spawnRoutine;

    [SerializeField]
    private GameObject opportunityBoxSpawnPointObject;

    private SpawnPoint opportunityBoxSpawnPoint;

    public SpawnPoint OpportunityBoxSpawnPoint
    {
        get
        {
            return opportunityBoxSpawnPoint;
        }
    }

    private List<SpawnPoint> onScreenSpawnPoints;
    private List<Transform> offScreenSpawnPoints;

    public static MainGameSpawner instance;

    public GameObject openableBox;
	public GameObject opportunityBox;
    public GameObject opportunityBoxClone;
    public GameObject hyperModeParticles;

    public GameObject disasterManager;
    public GameObject expensesManager;
    public GameObject loanManager;
    public GameObject shiftsManager;

    public GameObject[] regularBoxes;

    public GameObject opportunityBoxWarning;
    public GameObject opportunityBoxTutorial;

    public Button disableWorkButton;
    public Button disableSettingsButton;

    private MeshRenderer warningMesh;
    private Color warningMeshColor;

    private Coroutine warningBlink;

    /// <summary>
    /// Controls spawning of boxes depending on whether the opportunity box is active
    /// </summary>
    public bool opportunityBoxActive = false;

    // Delay the opportunity box moving if it has not been encountered before
    public bool freshOppBoxSpawn = false;

    // Blocks any duplicate safes or non-safe boxes from spawning when loading occurs
    private bool blockSpawn = true;

    public bool firstOpportunityBoxEver;

    public int opportunityUpperLimit = 101;
    public int opportunitySpawnThreshold = 100;

    public float oppBoxBadLuckCounter;

    #region Hyper Mode Vars

    public bool hyperModeisActive = false;
    private int hyperModeBoxAdditive;

    #endregion

    private  int numOnScreenBoxes;

    public int NumOnScreenBoxes
    {	
        get
        {
            return instance.numOnScreenBoxes;
        }
        private set
        {
            instance.numOnScreenBoxes = value;
        }
    }

    #region Unity Callbacks

    void Awake()
    {

        instance = this;

        onScreenSpawnPoints = new List<SpawnPoint>();
        offScreenSpawnPoints = new List<Transform>();

        opportunityBoxActive = SaveManager.Instance.OpportunityBoxActive;
        firstOpportunityBoxEver = SaveManager.Instance.FirstOpportunityBoxEver;

        hyperModeBoxAdditive = 0;
    }

    void Start()
    {
        hyperModeParticles.gameObject.SetActive(false);
        PopulateSpawnPointLists();

        opportunityBoxSpawnPoint = new SpawnPoint(opportunityBoxSpawnPointObject.transform);

        CalcBoxLimit();
    }

    private void OnEnable()
    {

        SubscribeToEvents();

        numOnScreenBoxes = 0;

        StartCoroutine(CheckBoxCount());

        StaticVars.GenerateProbabilityList();

        if (SceneTransition.Instance.SelectedClass == Classes.Rich)
        {
            StaticVars.OppBoxGenerateProbabilityList();
            Debug.Log("Rich Opp Box Drop Rates Active");
        }
        else if(SceneTransition.Instance.SelectedClass == Classes.Poor)
        {
            StaticVars.PoorOppBoxGenerateProbabilityList();
            Debug.Log("Poor Opp Box Drop Rates Active");
        }
    }

    private void OnDisable()
    {
        if(hyperModeisActive)
        {
            DeactivateHyperMode();
        }
        UnsubscribeFromEvents();
    }


    #endregion

    #region Spawning

    /// <summary>
    /// Every second, this checks if the current number of boxes on screen "numBoxes" is maxed.
    /// If it is not, then it runs the Spawn coroutine. If hyper mode isn't active,
    /// then the oppporunity box spawn routine is the one that will be chosen to spawn boxes,
    /// with a small chance to spawn opporunity boxes.
    /// If hyper mode is active, then the routine that ONLY spawns regular boxes is chosen.
    /// </summary>
    /// <returns>The box count.</returns>
    private IEnumerator CheckBoxCount()
    {
        yield return new WaitForSeconds(0.5f);

        while (offScreenSpawnPoints == null || onScreenSpawnPoints == null)
        {
            PopulateSpawnPointLists();

            yield return null;
        }

        while (true)
        {
            if (!hyperModeisActive)
            {
                // Should never be below zero, or else nothing will spawn
                // If it does go below zero, it shouldn't, so always set it back to zero if it goes under
                if(numOnScreenBoxes < 0)
                {
                    numOnScreenBoxes = 0;
                }
                //if (numOnScreenBoxes < maxOnScreenBoxes + hyperModeBoxAdditive && SaveManager.Instance.CurrentBoxCount > 0)

                if (numOnScreenBoxes == 0 && SaveManager.Instance.CurrentBoxCount > 0)
                {
                    // If there is another Spawn coroutine running, interrupt it and resume from the current numOnScreenBoxes.
                    // This is to avoid spawn point and numOnScreen discrepencies.
                    if (spawnRoutine != null)
                    {
                        StopCoroutine(spawnRoutine);
                    }

                    spawnRoutine = SpawnOpportunityBox();
                    StartCoroutine(spawnRoutine);
                }
            }
            else
            {
                if (numOnScreenBoxes < maxOnScreenBoxes + hyperModeBoxAdditive && SaveManager.Instance.CurrentBoxCount > 0)
                {
                    AudioManager.Instance.PlayAudioClip(SFXType.BoxAppearsOnScreen);
                    // If there is another Spawn coroutine running, interrupt it and resume from the current numOnScreenBoxes.
                    // This is to avoid spawn point and numOnScreen discrepencies.
                    if (spawnRoutine != null)
                    {
                        StopCoroutine(spawnRoutine);
                    }

                    spawnRoutine = Spawn();
                    StartCoroutine(spawnRoutine);
                }
            }

            yield return null;
        }
    }

    public void CheckBoxSpawn()
    {
        StartCoroutine(CheckBoxCount());
    }

    /// <summary>
    /// Handles spawning boxes up until the number of on screen boxes is maxed out.
    /// Has a chance to spawn the opportunity safe, but mostly just spawns regular boxes.
    /// </summary>
    public IEnumerator SpawnOpportunityBox()
    {
        yield return new WaitForEndOfFrame();

        AudioManager.Instance.PlayAudioClip(SFXType.BoxAppearsOnScreen);
        int dif = maxOnScreenBoxes + hyperModeBoxAdditive - numOnScreenBoxes;
        if (dif > SaveManager.Instance.CurrentBoxCount)
        {
            dif = SaveManager.Instance.CurrentBoxCount;
        }

        while (dif > 0)
        {
            int chosenIndex = Random.Range(0, offScreenSpawnPoints.Count);

            opportunityBoxClone = GameObject.FindGameObjectWithTag("OpportunitySafe");

            // If the player closed the app while an oppertunity box was active
            // we want to make sure that it is the first, and only, box that spawns.
            // also make sure that the timers are properly paused
            if (opportunityBoxActive && opportunityBoxClone == null)
            {
                if (blockSpawn)
                {
                    // turn off UI buttons so that the player is forced to pay attention to the box
                    disableWorkButton.interactable = false;
                    disableSettingsButton.interactable = false;

                    freshOppBoxSpawn = false;
                    Object.Instantiate(opportunityBox, offScreenSpawnPoints[chosenIndex].position, opportunityBox.transform.rotation);

                    disasterManager.GetComponent<DisasterManager>().OpportunityBoxStopDisasterTimer();
                    disasterManager.GetComponent<DisasterManager>().OpportunityBoxStopInsuranceTimer();

                    expensesManager.GetComponent<ExpensesManager>().OpportunityBoxStopExpensesTimer();

                    loanManager.GetComponent<LoanManager>().OpportunityBoxStopLoanTimer();

                    shiftsManager.GetComponent<ShiftsManager>().OpportunityBoxStopWorkTimer();

                    Debug.Log("Spawned a safe");
                    blockSpawn = false;
                    numOnScreenBoxes = 1;
                }
            }

            if (!opportunityBoxActive)
            {
                if(!hyperModeisActive)
                {
                    if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
                    {
                        opportunitySpawnThreshold = 98;
                    }
                    else
                    {
                        opportunitySpawnThreshold = 100;
                    }

                    int opportunitySpawnChance = Random.Range(1, opportunityUpperLimit);

                    if(firstOpportunityBoxEver && oppBoxBadLuckCounter >= 30)
                    {
                        opportunitySpawnChance = 100;
                    }

                    if (opportunitySpawnChance >= opportunitySpawnThreshold)
                    {
                        disableWorkButton.interactable = false;
                        disableSettingsButton.interactable = false;

                        OpportunityBoxWarningSetup();
                        freshOppBoxSpawn = true;

                        Object.Instantiate(opportunityBox, offScreenSpawnPoints[chosenIndex].position, opportunityBox.transform.rotation);
                        MainGameEventManager.TriggerOpportunityBoxSpawnedEvent();
                        opportunityBoxActive = true;
                        StatisticsManager.Instance.UpdateOppertunityBoxActiveState(opportunityBoxActive);
                        oppBoxBadLuckCounter = 0;
                    }
                    else
                    {
                        Object.Instantiate(openableBox, offScreenSpawnPoints[chosenIndex].position, openableBox.transform.rotation);

                        if(firstOpportunityBoxEver)
                        {
                            if (SceneTransition.Instance.SelectedClass == Classes.Rich)
                            {
                                oppBoxBadLuckCounter += 1.0f;
                            }
                            else
                            {
                                oppBoxBadLuckCounter += 0.5f;
                            }
                        }
                    }
                }
            }
            

            MainGameEventManager.TriggerBoxSpawnedEvent();
            numOnScreenBoxes++;
            dif--;

            // Making sure that the oppertunity box is the only one left on the screen
            // numOnScreenBoxes must be set to 1 when this is complete, as the game has trouble
            // keeping track of how many "boxes" are on screen when we get rid of the extras
            if (opportunityBoxActive)
            {
                regularBoxes = GameObject.FindGameObjectsWithTag("OpenableBox");
                int boxesAddBack = 0;
                if (regularBoxes != null)
                {
                   
                    foreach (GameObject regBox in regularBoxes)
                    {
                        regBox.GetComponent<OpenableBox>().PublicDestroyAnimation();
                        boxesAddBack++;
                    }
                    SaveManager.Instance.CurrentBoxCount += boxesAddBack;
                }
                boxesAddBack = 0;
                numOnScreenBoxes = 1;
            }


            yield return new WaitForSeconds(0.15f);

        }

        yield break;
    }

    /// <summary>
    /// Handles spawning boxes up until the number of on screen boxes is maxed out.
    /// Only spawns regular boxes
    /// </summary>
    public IEnumerator Spawn()
    {
        yield return new WaitForEndOfFrame();

        AudioManager.Instance.PlayAudioClip(SFXType.BoxAppearsOnScreen);
        int dif = maxOnScreenBoxes + hyperModeBoxAdditive - numOnScreenBoxes;
        if (dif > SaveManager.Instance.CurrentBoxCount)
        {
            dif = SaveManager.Instance.CurrentBoxCount;
        }

        while (dif > 0)
        {
            int chosenIndex = Random.Range(0, offScreenSpawnPoints.Count);

            Object.Instantiate(openableBox, offScreenSpawnPoints[chosenIndex].position, openableBox.transform.rotation);

            MainGameEventManager.TriggerBoxSpawnedEvent();
            numOnScreenBoxes++;
            dif--;

            yield return new WaitForSeconds(0.15f);
            
        }

        yield break;
    }


    #endregion

    #region Utilities

    /// <summary>
    /// Grabs on screen and off screen spawn points in the scene.
    /// </summary>
    private void PopulateSpawnPointLists()
    {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("OnScreen"))
        {
            SpawnPoint sp = new SpawnPoint(go.transform);
            onScreenSpawnPoints.Add(sp);
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("OffScreen"))
        {
            offScreenSpawnPoints.Add(go.transform);
        }
    }

    /// <summary>
    /// Returns the coordinates of an available on-screen spawn point.
    /// This spawn point will be removed from the list so that something 
    /// else cannot choose it until the spawning is reset.
    /// </summary>
    public SpawnPoint PickSpawnPoint()
    {
        // Pick and index out of the available on scree spawn points.
        int chosenIndex = Random.Range(0, onScreenSpawnPoints.Count);
        int temp = chosenIndex;
        while (onScreenSpawnPoints[chosenIndex].IsOccupied)
        {
            chosenIndex++;
            if (chosenIndex > onScreenSpawnPoints.Count - 1)
            {
                // If the index is out of bounds, reset it.
                chosenIndex = 0;
            }

            if (chosenIndex == temp)
            {
                // After resetting the index to 0, if it ends up back at its original value, then all spawn points are occupied.
                Debug.LogWarning("There are no open spawn points...");
                return null;
            }
        } 
			
        SpawnPoint spawnPointRef = onScreenSpawnPoints[chosenIndex];
        spawnPointRef.IsOccupied = true;
        onScreenSpawnPoints[chosenIndex] = spawnPointRef;

        return onScreenSpawnPoints[chosenIndex];
    }

    private void DecreaseOnScreenBoxCount()
    {
        numOnScreenBoxes--;
        if (numOnScreenBoxes <= 0 && SaveManager.Instance.CurrentBoxCount <= 0)
        {
            MainGameEventManager.TriggerBoxCountExhaustedEvent();
        }
    }

    // Helper function for CalcBoxLimit
    private bool IsInRange(int num, int min, int max)
    {
        return (num >= min && num <= max);
    }

    // Determine how many boxes we can show on screen.
    private void CalcBoxLimit()
    {
        int boxCount = SaveManager.Instance.CurrentBoxCount;
        if (boxCount <= 25)
        {
            maxOnScreenBoxes = 1;
        }
        else if (IsInRange(boxCount, 26, 50))
        {
            maxOnScreenBoxes = 2;
        }
        else if (IsInRange(boxCount, 51, 100))
        {
            maxOnScreenBoxes = 3;
        }
        else if (IsInRange(boxCount, 101, 250))
        {
            maxOnScreenBoxes = 4;
        }
        else if (boxCount > 250)
        {
            maxOnScreenBoxes = 5;
        }
        //else if (IsInRange(boxCount, 251, 500))
        //{
        //    maxOnScreenBoxes = 5;
        //}
        //else if (IsInRange(boxCount, 501, 1000))
        //{
        //    maxOnScreenBoxes = 6;
        //}
        //else if (IsInRange(boxCount, 1001, 2000))
        //{
        //    maxOnScreenBoxes = 7;
        //}
        //else if (IsInRange(boxCount, 2001, 4000))
        //{
        //    maxOnScreenBoxes = 8;
        //}
        //else if (boxCount > 4000)
        //{
        //    maxOnScreenBoxes = 9;
        //}
    }

    private void SendOppertunityBoxActiveState()
    {
        StatisticsManager.Instance.UpdateOppertunityBoxActiveState(opportunityBoxActive);
    }

    /// <summary>
    /// Warning for opportunity box spawn
    /// </summary>
    private void OpportunityBoxWarningSetup()
    {
        warningMesh = opportunityBoxWarning.GetComponent<MeshRenderer>();

        warningMesh.sortingLayerName = "UI";
        warningMesh.sortingOrder = 9;

        opportunityBoxWarning.SetActive(true);

        warningBlink = StartCoroutine(BlinkOppertunityBoxWarning());        
    }

    private IEnumerator BlinkOppertunityBoxWarning()
    {

        Debug.Log("Warning happening now");

        int timesBlinked = 0;

        while(timesBlinked <= 4)
        {
            yield return new WaitForSeconds(0.5f);

            opportunityBoxWarning.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            opportunityBoxWarning.SetActive(true);

            timesBlinked++;
        }

        opportunityBoxWarning.SetActive(false);

        yield break;
    }

    public void SetOnScreenBoxCountToZero()
    {
        numOnScreenBoxes = 0;
    }

    // Making sure that the opportunity box and associated UI is fully deactivated
    public void OpportunityBoxCleanup()
    {
        if(opportunityBoxActive)
        {
            disableWorkButton.interactable = true;
            disableSettingsButton.interactable = true;
            opportunityBoxActive = false;

            StatisticsManager.Instance.UpdateOppertunityBoxActiveState(opportunityBoxActive);

            disasterManager.GetComponent<DisasterManager>().OpportunityBoxStartDisasterTimer();
            disasterManager.GetComponent<DisasterManager>().OpportunityBoxStartInsuranceTimer();

            expensesManager.GetComponent<ExpensesManager>().OpportunityBoxStartExpensesTimer();

            loanManager.GetComponent<LoanManager>().OpportunityBoxStartLoanTimer();

            shiftsManager.GetComponent<ShiftsManager>().OpportunityBoxStartWorkTimer();

            StopCoroutine(warningBlink);
            opportunityBoxWarning.SetActive(false);
        }
    }
    #endregion

    #region Hyper Mode

    // Sets hyper mode variables that affect frequency of box spawning and number of boxes on-screen.
    private void ActivateHyperMode()
    {
        hyperModeisActive = true;
        hyperModeBoxAdditive = 3;

        hyperModeParticles.gameObject.SetActive(true);

        // Stops the current countdown routine if one has been assigned.
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
        }

        // Destroy any extra safes that happen to inadvertently spawn just as hyper mode beings;
        Debug.Log("Destroying safes");
        GameObject[] oppBoxesToDestroy = GameObject.FindGameObjectsWithTag("OpportunitySafe");

        if (oppBoxesToDestroy.Length > 0)
        {
            foreach (GameObject oppBox in oppBoxesToDestroy)
            {
                Debug.Log(oppBox.name);
            }
        }
    

        if (oppBoxesToDestroy.Length > 0)
        {
            if(opportunityBoxWarning.activeSelf)
            {
                StopCoroutine(warningBlink);
                opportunityBoxWarning.SetActive(false);
            }

            foreach (GameObject oppBox in oppBoxesToDestroy)
            {
                if (oppBox != null)
                {
                    oppBox.GetComponent<OpportunityBox>().PlayDestroyAnimation();
                }
            }
        }
        
        countdownRoutine = HyperModeCountdown();
        StartCoroutine(countdownRoutine);
    }

    // Rests hyper mode variables that affect frequency of box spawning and number of boxes on-screen.
    private void DeactivateHyperMode()
    {
        hyperModeisActive = false;
        hyperModeParticles.gameObject.SetActive(false);
        hyperModeBoxAdditive = 0;
    }

    // Method that handles deactivating hyper mode after time.
    private IEnumerator HyperModeCountdown()
    {        
        yield return new WaitForSeconds(HYPER_MODE_DURATION);
        Debug.Log("Ending hyper mode.");
        MainGameEventManager.TriggerHyperModeEnd();
    }

    #endregion

    #region Event Subscription

    private void SubscribeToEvents()
    {
        MainGameEventManager.OnBoxDestroyed += DecreaseOnScreenBoxCount;
        MainGameEventManager.OnHyperModeBegin += ActivateHyperMode;
        MainGameEventManager.OnHyperModeBegin += OpportunityBoxCleanup;
        MainGameEventManager.OnHyperModeEnd += DeactivateHyperMode;
        MainGameEventManager.OnHyperModeEnd += CheckBoxSpawn;
        MainGameEventManager.OnBoxTapped += CalcBoxLimit;
        MainGameEventManager.OnOpportunityBoxDespawn += SetOnScreenBoxCountToZero;
        SaveManager.Instance.SendSaveData += SendOppertunityBoxActiveState;

        if (SceneTransition.Instance.SelectedClass == Classes.Poor && !SaveManager.Instance.CompletedFurnitureTutorial)
        {
            MainGameEventManager.OnFurnitureTutorialStart += OpportunityBoxCleanup;
        }
    }

    private void UnsubscribeFromEvents()
    {		
        MainGameEventManager.OnBoxDestroyed -= DecreaseOnScreenBoxCount;
        MainGameEventManager.OnHyperModeBegin -= ActivateHyperMode;
        MainGameEventManager.OnHyperModeBegin -= OpportunityBoxCleanup;
        MainGameEventManager.OnHyperModeEnd -= DeactivateHyperMode;
        MainGameEventManager.OnHyperModeEnd -= CheckBoxSpawn;
        MainGameEventManager.OnBoxTapped -= CalcBoxLimit;
        MainGameEventManager.OnOpportunityBoxDespawn -= SetOnScreenBoxCountToZero;
        SaveManager.Instance.SendSaveData -= SendOppertunityBoxActiveState;

        if (SceneTransition.Instance.SelectedClass == Classes.Poor && !SaveManager.Instance.CompletedFurnitureTutorial)
        {
            MainGameEventManager.OnFurnitureTutorialStart -= OpportunityBoxCleanup;
        }
    }

    #endregion

}

