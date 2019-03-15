using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

//Global Delegate for other scripts to use
public delegate void Callback();

/*This script is the what handles the tutorial in the main game.
 * This script also handles when Emmett takes you back to the spinner
 * after completing your first ticke
 * 
 */
public class MainGameTutorial : MonoBehaviour
{

    public const float SCREEN_OFFSET = 2.5f;
    //the current CallBackFor the Tutorial to call
    private Callback functionToCallBack = null;


    //TODO make tutorial scripts first in Execution Order

    #region Inspector Set Vars

    [Header("Emmett says 5 Lines, Each line has a 100 character limit.")]
    [Multiline]
    public List<string> emmetSpeechLines;

    [Header("Lines Emmett says when the player completes their first ticket")]
    [Multiline]
    public List<string> emmetUnlockSpeechLines;

    [Header("Lines Emmett says when the player unlocks the furniture shop")]
    [Multiline]
    public List<string> emmetFurnitureSpeechLines;

    [Header("How long to wait at the beginning of the game before it starts")]
    public float timeToWaitTutorialStart = 0.75f;

    [Header("The Emmett Sprites needed for different parts of tht tutorial, there should be 3 sprites")]
    public List<Sprite> emmettSprites;

    [Header("Reference to the Text for the bubble")]
    public Text bubbleText;

    [Header("Emmett's Speech bubble Reference")]
    public Image emmettSpeechBubble;

    [Header("A reference to the Emmett GameObject in the scene")]
    public GameObject emmettReference;

    [Header("The tutorialBoxPrefab")]
    public GameObject tutorialBoxPrefab;

    [Header("Faded black image")]
    public GameObject underlayImage;

    [Header("Managers that need to be turned off during the tutorial")]
    public GameObject[] managersToTurnOff;

    public Button[] buttonsToTurnOff;


    [Header("References to game managers and their associated UI")]
    public GameObject loanRef;
    public GameObject insuranceRef;
    public GameObject billsRef;

    public GameObject loanTimerUI;
    public GameObject insuranceTimerUI;
    public GameObject billsTimerUI;

    [Header("The Canvas for the entire tutorial UI")]
    public Canvas tutorialCanvas;

    [Header("Reference to the in scene the Boxes Own UI")]
    public GameObject boxOverviewUI;

    [Header("Reference to non UI text Display prefab")]
    public GameObject boxesGivenDisplay;

    [Header("Image of the shope button for the furniture tutorial")]
    public GameObject shopButtonImage;

    [Header("Actual Shop Button")]
    public GameObject shopButton;

    [Header("Shop Button Parent Object")]
    public GameObject shopButtonParent;

    [Header("Furniture Shop Interface")]
    public GameObject shopInterface;

    [Header("Furniture Shop Exit Button")]
    public GameObject shopInterfaceExitButton;

    [Header("UniversalUI GameObject Reference")]
    public GameObject universalUIObject;

    [Header("Shop Button Main Location")]
    public GameObject ShopButtonMainPosition;

    [Header("Shop Button Location Tutorial")]
    public GameObject ShopButtonTutorialPosition;

    [Header("Shop UI Main Location")]
    public GameObject shopUIMainPosition;

    [Header("GameObject that contains list of shop items")]
    public GameObject shopItemObject;

    [Header("Speech Bubble Reference for when we need to change sort order")]
    public Image speechBubbleFurniture;

    #endregion

    //current Coroutine instance incase we need to stop it
    private Coroutine currentCoroutine;
    //the list of steps in the tutorial
    Queue<IEnumerator> tutorialSteps;
    Queue<IEnumerator> classUnlockSteps;
    Queue<IEnumerator> furnitureUnlockSteps;

    //have we already shown that we can get a box from a box
    private bool hasShownfirstBox = false;
    //bool to let some coroutines know to end their while loop
    private bool doWeContinue = false;

    private bool startedFurnitureTutorial = false;

    private Coroutine furnitureTutorialRunning;

    private GameObject tournamentManager;

    #region Singleton

    private static MainGameTutorial instance;

    public static MainGameTutorial Instance
    {
        get
        {
            return instance;
        }
        private set { }

    }

    private MainGameTutorial()
    {

    }

    #endregion

    #region General/Utility

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            int destroyTick = 0;

            tournamentManager = GameObject.FindGameObjectWithTag("TournamentManager");
            shopButtonParent.SetActive(false);

            //check if the Main Game Tutorial is already finished
            if (SaveManager.Instance.CompletedMainTutorial)
            {
                destroyTick++;
            }
            //Setup to run tutorial
            else
            {
                tutorialCanvas.gameObject.SetActive(true);
                Toggle(false);
                InitializeTutorialQueue();
                SetupEmmetUI();
                StartCoroutine(RunTutorial());
            }

            if (SaveManager.Instance.UnlockedClasses.Contains(Classes.Poor))
            {
                destroyTick++;
            }
            //Setup up stuff for when player gets his/her first ticket
            else
            {
                InitializeNewClassUnlockQueue();
                MainGameEventManager.OnAllTicketsFound += RunUnlockClassRoutineWrapper;
            }
            

            if (SceneTransition.Instance.SelectedClass == Classes.Poor)
            {

                if (SaveManager.Instance.CompletedFurnitureTutorial)
                {
                    destroyTick++;
                }
                else
                {
                    // setup for when the player gets their first ticket in poor mode
                    // introduces the furniture shop (poor mode only)                    
                    InitializeFurnitureTutorialQueue();
                    
                    MainGameEventManager.OnAllTicketsFound += RunFurnitureTutorialWrapper;
                }
            }

            //if this object and the tutorial canvas if there is no need for it
            if (destroyTick >= 3)
            {
                Destroy(tutorialCanvas.gameObject);
                Destroy(this.gameObject);
            }

        }
        else
        {

            Destroy(this.gameObject);
        }

    }

    private void OnEnable()
    {
        MainGameEventManager.OnFurnitureTutorialStart += SendFurnitureTutorialData;

        StartCoroutine(CheckForTutorialCompletion());
    }

    private void OnDestroy()
    {
        //MainGameEventManager.OnFurnitureTutorialStart -= SendFurnitureTutorialData;
        MainGameEventManager.OnAllTicketsFound -= RunUnlockClassRoutineWrapper;
        MainGameEventManager.OnAllTicketsFound -= RunFurnitureTutorialWrapper;
    }

    //Function to setup Emmetts UI before he is needed
    private void SetupEmmetUI()
    {

        iTween.Init(emmettReference);

        emmettReference.transform.position = new Vector3(Camera.main.transform.position.x - StaticVars.CameraHalfWidth - SCREEN_OFFSET, transform.position.y, transform.position.z);
        emmettReference.SetActive(false);
        bubbleText.text = string.Empty;
        bubbleText.gameObject.SetActive(false);
        emmettSpeechBubble.color = new Color(emmettSpeechBubble.color.r, emmettSpeechBubble.color.g, emmettSpeechBubble.color.b, 0);
        emmettSpeechBubble.gameObject.SetActive(false);
    }

    //Function used for subscription to event
    //calls to run the unlock class routine
    private void RunUnlockClassRoutineWrapper()
    {
        StartCoroutine(RunUnlockClassRoutine());
    }

    private IEnumerator RunUnlockClassRoutine()
    {
        tutorialCanvas.gameObject.SetActive(true);
        SetupEmmetUI();

        // Add the on-screen boxes back to the pool of boxes before destroying.
        SaveManager.Instance.CurrentBoxCount += MainGameSpawner.instance.NumOnScreenBoxes;
        // Turn off spawning manager.
        Toggle(false);

        // Turning off the timer UI so that they don't appear over Emmett in the tutorial
        if (loanRef.GetComponent<LoanManager>().IsLoanActive)
        {
            loanTimerUI.SetActive(false);
        }

        if (insuranceRef.GetComponent<DisasterManager>().IsInsuranceActive)
        {
            insuranceTimerUI.SetActive(false);
        }

        if (billsRef.GetComponent<ExpensesManager>().IsBillActive)
        {
            billsTimerUI.SetActive(false);
        }
        

        SaveManager.Instance.UnlockIncomeClass(Classes.Poor);
        // Used to remove all boxes in the scene as to not block Emmett
        MainGameEventManager.TriggerGameEndEvent();

        //small pause   
        yield return new WaitForSeconds(timeToWaitTutorialStart);

        //wait until all steps are complete
        while (classUnlockSteps.Count > 0)
        {
            yield return currentCoroutine = StartCoroutine(classUnlockSteps.Dequeue());
        }

        SceneTransition.Instance.TriggerSceneChangeEvent(Scenes.ClassRoulette);

        Destroy(tutorialCanvas.gameObject);
        Destroy(this.gameObject);
    }

    //Coroutine that actually runs the tutorial
    private IEnumerator RunTutorial()
    {
        yield return new WaitForSeconds(timeToWaitTutorialStart);
        //Wait till we are done
        while (tutorialSteps.Count > 0)
        {
            yield return currentCoroutine = StartCoroutine(tutorialSteps.Dequeue());
        }

        //toggle the Managers to turn them on
        Toggle(true);
        tutorialCanvas.gameObject.SetActive(false);
        //tell the save Manager that this tutorial is complete
        SaveManager.Instance.CompletedMainTutorial = true;
    }

    // For starting furniture tutorial via event subscription
    private void RunFurnitureTutorialWrapper()
    {
        furnitureTutorialRunning = StartCoroutine(RunFurnitureTutorial());
    }

    private IEnumerator RunFurnitureTutorial()
    {
        // add the amount of boxes that will disappear back into the box count

        tutorialCanvas.gameObject.SetActive(true);

        startedFurnitureTutorial = true;

        MainGameEventManager.TriggerFurnitureTutorialStartEvent();

        SetupEmmetUI();
        // turn specific managers and timers off
        SaveManager.Instance.CurrentBoxCount += MainGameSpawner.instance.NumOnScreenBoxes;
        Toggle(false);

        // Timer UI may nee to be turned back on after the tutorial is done
        // Check to see if that is the case
        if (loanRef.GetComponent<LoanManager>().IsLoanActive)
        {
            loanTimerUI.SetActive(false);
            loanRef.GetComponent<LoanManager>().OpportunityBoxStopLoanTimer();
        }

        if (insuranceRef.GetComponent<DisasterManager>().IsInsuranceActive)
        {
            insuranceTimerUI.SetActive(false);
            insuranceRef.GetComponent<DisasterManager>().OpportunityBoxStopInsuranceTimer();
            insuranceRef.GetComponent<DisasterManager>().OpportunityBoxStopDisasterTimer();
        }

        if (billsRef.GetComponent<ExpensesManager>().IsBillActive)
        {
            billsTimerUI.SetActive(false);
            billsRef.GetComponent<ExpensesManager>().OpportunityBoxStopExpensesTimer();
        }

        // fully turn insurance and bills off so that these mechancis don't bother the player
        // during the tutorial
        insuranceRef.SetActive(false);

        billsRef.SetActive(false);

        yield return new WaitForSeconds(timeToWaitTutorialStart);

        // wait until all steps are complete
        while (furnitureUnlockSteps.Count > 0)
        {
            yield return currentCoroutine = StartCoroutine(furnitureUnlockSteps.Dequeue());
        }

        // turn the managers and timers back on
        Toggle(true);

        insuranceRef.SetActive(true);

        billsRef.SetActive(true);

        if (loanRef.GetComponent<LoanManager>().IsLoanActive)
        {
            loanTimerUI.SetActive(true);
            loanRef.GetComponent<LoanManager>().OpportunityBoxStartLoanTimer();
        }

        if (insuranceRef.GetComponent<DisasterManager>().IsInsuranceActive)
        {
            insuranceTimerUI.SetActive(true);
            insuranceRef.GetComponent<DisasterManager>().OpportunityBoxStartInsuranceTimer();
            insuranceRef.GetComponent<DisasterManager>().OpportunityBoxStartDisasterTimer();
        }

        if (billsRef.GetComponent<ExpensesManager>().IsBillActive)
        {
            billsTimerUI.SetActive(true);
            billsRef.GetComponent<ExpensesManager>().OpportunityBoxStartExpensesTimer();
        }

        // move the shop button from the tutorial UI back to the main UI and size it properly
        shopButtonParent.transform.SetParent(universalUIObject.transform);
        shopButtonParent.transform.position = ShopButtonMainPosition.transform.position;
        shopButtonParent.transform.localScale = new Vector3(.38f, .38f);

        foreach (GameObject furniturePiece in shopItemObject.GetComponent<PopLib>().nonTutorialFurniture.Skip(1))
        {
            furniturePiece.GetComponent<Button>().interactable = true;
        }

        shopInterfaceExitButton.GetComponent<Button>().interactable = true;

        shopInterface.transform.SetParent(universalUIObject.transform);
        shopInterface.transform.position = shopUIMainPosition.transform.position;
        shopInterface.transform.localScale = shopUIMainPosition.transform.localScale;
        shopInterface.SetActive(false);

        tutorialCanvas.gameObject.SetActive(false);
        SaveManager.Instance.CompletedFurnitureTutorial = true;
        startedFurnitureTutorial = false;
        StatisticsManager.Instance.UpdateFurnitureTutorialStartedState(startedFurnitureTutorial);
        MainGameEventManager.OnAllTicketsFound -= RunFurnitureTutorialWrapper;
    }

    private void InitializeFurnitureTutorialQueue()
    {
        furnitureUnlockSteps = new Queue<IEnumerator>();

        // emmett says congrats on ticket
        furnitureUnlockSteps.Enqueue(FurnitureTutorialUISetup());
        furnitureUnlockSteps.Enqueue(EmmettAppears(true, 2));
        furnitureUnlockSteps.Enqueue(TextBubbleFade(true));
        furnitureUnlockSteps.Enqueue(EmmettSaysSomething(false, true));
        furnitureUnlockSteps.Enqueue(WaitForPlayerTapText());

        // shop button fades in, "lets see what you can get with that"
        furnitureUnlockSteps.Enqueue(EmmettSaysSomething(false, true));
        furnitureUnlockSteps.Enqueue(WaitForPlayerTapText());
        furnitureUnlockSteps.Enqueue(SetShopButtonActive());
        furnitureUnlockSteps.Enqueue(CallBackCall());
        furnitureUnlockSteps.Enqueue(WaitForTutorialComponents());
        furnitureUnlockSteps.Enqueue(TextBubbleFade(false));

        // prompts player to buy desk
        furnitureUnlockSteps.Enqueue(SetEmmettSprite(1));
        furnitureUnlockSteps.Enqueue(TextBubbleFade(true));
        furnitureUnlockSteps.Enqueue(EmmettSaysSomething(false, true));
        furnitureUnlockSteps.Enqueue(WaitForPlayerTapText());
        furnitureUnlockSteps.Enqueue(TextBubbleFade(false));
        furnitureUnlockSteps.Enqueue(EmmettDisapears(true, false));
        furnitureUnlockSteps.Enqueue(SetDeskButtonActive());
        furnitureUnlockSteps.Enqueue(CallBackCall());
        furnitureUnlockSteps.Enqueue(WaitForTutorialComponents());

        // lets player know they can get other items with golden tickets
        furnitureUnlockSteps.Enqueue(EmmettAppears(true, 0));
        furnitureUnlockSteps.Enqueue(TextBubbleFade(true));
        furnitureUnlockSteps.Enqueue(EmmettSaysSomething(false, true));
        furnitureUnlockSteps.Enqueue(WaitForPlayerTapText());
        furnitureUnlockSteps.Enqueue(TextBubbleFade(false));
        furnitureUnlockSteps.Enqueue(EmmettDisapears(true, false));
        
    }

    //Initializes Queue for Unlock Class Actions
    private void InitializeNewClassUnlockQueue()
    {
        classUnlockSteps = new Queue<IEnumerator>();

        classUnlockSteps.Enqueue(EmmettAppears(true, 0));
        classUnlockSteps.Enqueue(TextBubbleFade(true));
        //emmett tells you to tap a box
        classUnlockSteps.Enqueue(EmmettSaysSomething(false, false));
        classUnlockSteps.Enqueue(WaitForPlayerTapText());
        classUnlockSteps.Enqueue(EmmettSaysSomething(false, false));
        classUnlockSteps.Enqueue(WaitForPlayerTapText());
        classUnlockSteps.Enqueue(TextBubbleFade(false));
        classUnlockSteps.Enqueue(EmmettDisapears(true, false));     
    }

    //initialize Queue of Tutorial Actions
    private void InitializeTutorialQueue()
    {
        tutorialSteps = new Queue<IEnumerator>();


        //First Part of tutorial of the tutorial when the emmett explains the box
        tutorialSteps.Enqueue(BoxSpawn());
        tutorialSteps.Enqueue(EmmettAppears(true, 0));
        tutorialSteps.Enqueue(TextBubbleFade(true));
        //emmett tells you he is your guide.
        tutorialSteps.Enqueue(EmmettSaysSomething());
        tutorialSteps.Enqueue(WaitForPlayerTapText());
        // Your are told to tap the box in the middle.
        tutorialSteps.Enqueue(EmmettSaysSomething());
        tutorialSteps.Enqueue(WaitForPlayerTapText());
        tutorialSteps.Enqueue(TextBubbleFade(false));
        tutorialSteps.Enqueue(EmmettDisapears(true, false));
        tutorialSteps.Enqueue(CallBackCall());
        tutorialSteps.Enqueue(WaitForTutorialComponents());
        //the box will handle everything from here until part 2


        //Second Step after the player taps first box
        tutorialSteps.Enqueue(BoxSpawn());
        tutorialSteps.Enqueue(EmmettAppears(true, 1));
        tutorialSteps.Enqueue(TextBubbleFade(true));
        //emmett Tells you that you can get boxes from boxes 
        tutorialSteps.Enqueue(EmmettSaysSomething());
        tutorialSteps.Enqueue(WaitForPlayerTapText());
        tutorialSteps.Enqueue(TextBubbleFade(false));
        tutorialSteps.Enqueue(EmmettDisapears(true, false));
        tutorialSteps.Enqueue(CallBackCall());
        tutorialSteps.Enqueue(WaitForTutorialComponents());

        //Step 3 Emmett Explains the Ticket
        tutorialSteps.Enqueue(EmmettAppears(false, 2));
        tutorialSteps.Enqueue(TextBubbleFade(true));
        tutorialSteps.Enqueue(EmmettSaysSomething());
        tutorialSteps.Enqueue(WaitForPlayerTapText());
        //Emmett Says this is his and you must get your own
        tutorialSteps.Enqueue(EmmettSaysSomething());
        tutorialSteps.Enqueue(WaitForPlayerTapText());
        tutorialSteps.Enqueue(CallBackCall());
        tutorialSteps.Enqueue(WaitForTutorialComponents());

        //Emmett Tells you he can put you on the path to getting tickets.
        tutorialSteps.Enqueue(EmmettSaysSomething());
        tutorialSteps.Enqueue(WaitForPlayerTapText());
        //Emmett gives you boxes.
        tutorialSteps.Enqueue(EmmettSaysSomething());
        tutorialSteps.Enqueue(WaitForPlayerTapText());
        tutorialSteps.Enqueue(TextBubbleFade(false));
        tutorialSteps.Enqueue(GiveBoxes());
        tutorialSteps.Enqueue(EmmettDisapears(false, true));

    }

    //Function that toggles Managers and Buttons
    private void Toggle(bool toggleStatus)
    {
        for (int i = 0; i < managersToTurnOff.Length; i++)
        {
            managersToTurnOff[i].SetActive(toggleStatus);
        }

        //turn off buttons
        for (int i = 0; i < buttonsToTurnOff.Length; i++)
        {
            buttonsToTurnOff[i].enabled = toggleStatus;
        }
    }

    //a public function that allows other tutorial scripts to continue the tutorial
    //used if the manager is waiting on other scripts to do something
    //you can pass a call back function into here if needed
    public void ContinueTutorial(Callback funcToCallBack = null)
    {
        functionToCallBack = funcToCallBack;
        doWeContinue = true;
    }

    //Calls the Call Back Function
    //made it a coroutine so it can be apart of the queue
    private IEnumerator CallBackCall()
    {
        if (functionToCallBack != null)
        {
            doWeContinue = false;
            functionToCallBack();
        }
        yield break;
    }

    //function used to spawn Boxes for the tutorial
    private IEnumerator BoxSpawn()
    {
        
        //z is subtracted so the box isn't z fighting with the main canvas.
        Vector3 spawnPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.root.position.z - 2.5f);
        spawnPos.y += Camera.main.orthographicSize + SCREEN_OFFSET;
        GameObject obj = Instantiate(tutorialBoxPrefab, spawnPos, tutorialBoxPrefab.transform.rotation) as GameObject;

        //have we shown the first box 
        if (hasShownfirstBox == false)
        {
            //if not symbolize that we already have for next time
            hasShownfirstBox = true;
        }
        else
        {
            //if we have show the first box already
            TutorialBoxMain box = obj.GetComponent<TutorialBoxMain>() as TutorialBoxMain;
            //tell the box, that this one doesn't give a box(it shows the ticket)
            if (box != null)
            {
                box.givesABox = false;
            }
        }

        //waits until box sends the signal that it is done moving
        while (doWeContinue == false)
        {
            yield return null;
        }
        yield break;
    }

    #endregion

    #region Waiting Coroutines
    //simple coroutine that waits for the player to tap the screen
    private IEnumerator WaitForPlayerTapText()
    {
        while (true)
        {

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                break;
            }
#else

            if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                break;
            }
#endif


            yield return null;
        }



        yield break;
    }

    //just simple function that waits for another tutorial script to finish
    private IEnumerator WaitForTutorialComponents()
    {
        while (!doWeContinue)
        {
            yield return null;
        }

        yield break;
    }

    #endregion

    #region Emmett Text Coroutines
    //Coroutine that actually spells out emmetts lines
    private IEnumerator EmmettSaysSomething(bool isTutorial = true, bool isFurnitureTutorial = true)
    {
        bubbleText.gameObject.SetActive(true);

        bubbleText.text = string.Empty;
        const float TEXT_SPEED = 50;
        string textToShow = "";
        float currentStringLength = 0;

        //grab the correct speech lines depending on if we are in the tutorial or not
        if (isTutorial)
        {
            textToShow = emmetSpeechLines[0];
            emmetSpeechLines.RemoveAt(0);
        }
        else if(isFurnitureTutorial)
        {
            textToShow = emmetFurnitureSpeechLines[0];
            emmetFurnitureSpeechLines.RemoveAt(0);
        }
        else if (!isTutorial && !isFurnitureTutorial)
        {
            textToShow = emmetUnlockSpeechLines[0];
            emmetUnlockSpeechLines.RemoveAt(0);
        }
            
        //Spell out lines
        while (true)
        {
            currentStringLength += TEXT_SPEED * Time.deltaTime;
            bubbleText.text = textToShow.Substring(0, currentStringLength <= (float)textToShow.Length ? Mathf.FloorToInt(currentStringLength) : textToShow.Length);

            //if we reached the full line break the line
            if (currentStringLength >= textToShow.Length)
            {
                break;
            }

            yield return null;
        }

        if(bubbleText.text.Contains("Golden Ticket"))
        {
            bubbleText.text = bubbleText.text.Replace("Golden Ticket", "<color=#ffa500ff>Golden Ticket</color>");
        }

        yield break;
    }

    #endregion

    #region Emmett Movement and Speech Bubble Fade Coroutines

    //Coroutine to Move emett into site
    private IEnumerator EmmettAppears(bool fromLeft, int spriteNum)
    {
        emmettReference.SetActive(true);
        emmettReference.GetComponent<Image>().sprite = emmettSprites[spriteNum];

        //the third sprite needs to be flipped, third sprite is represented the number 2
        if (spriteNum == 2)
        {
            emmettReference.transform.localScale = new Vector3(-emmettReference.transform.localScale.x, emmettReference.transform.localScale.y, emmettReference.transform.localScale.z);
        }


        float travelTime = 0.5f;
        Vector3 wantedPosition = new Vector3(emmettReference.transform.position.x, emmettReference.transform.position.z, emmettReference.transform.position.z);
        wantedPosition.y = Camera.main.transform.position.y - (Camera.main.orthographicSize / 2);
        wantedPosition.x = Camera.main.transform.position.x - (StaticVars.CameraHalfWidth / 2);

        //set emmett at the correct position 
        if (fromLeft)
        {
            emmettReference.transform.position = new Vector3(Camera.main.transform.position.x - StaticVars.CameraHalfWidth - SCREEN_OFFSET, wantedPosition.y, transform.position.z);
        }
        else
        {
            emmettReference.transform.position = new Vector3(wantedPosition.x, Camera.main.transform.position.y - Camera.main.orthographicSize - SCREEN_OFFSET, transform.position.z);
        }

        //move emmett
        iTween.MoveTo(emmettReference, iTween.Hash("position", wantedPosition, "time", travelTime, "easetype", iTween.EaseType.linear));

        yield return new WaitForSeconds(travelTime);

        yield break;

    }

    // for when you want to change the sprite but don't want to emmett to slide in
    private IEnumerator SetEmmettSprite(int spriteNum)
    {
        emmettReference.GetComponent<Image>().sprite = emmettSprites[spriteNum];

        //the third sprite needs to be flipped, third sprite is represented the number 2
        if (spriteNum == 2)
        {
            emmettReference.transform.localScale = new Vector3(-emmettReference.transform.localScale.x, emmettReference.transform.localScale.y, emmettReference.transform.localScale.z);
        }

        yield break;
    }

    //Coroutine that fades the Text Bubble in and out
    private IEnumerator TextBubbleFade(bool fadeIn)
    {
        float alphaFadeInTime = 0.5f;

        //make sure that the gameObject is active, no need to do a fadeInCheck
        emmettSpeechBubble.gameObject.SetActive(true);
        bubbleText.gameObject.SetActive(true);
        bubbleText.text = string.Empty;

        float currentTimer = 0;
        do
        {
            yield return null;
            currentTimer += Time.deltaTime;
            if (fadeIn)
            {
                emmettSpeechBubble.color = new Color(emmettSpeechBubble.color.r, emmettSpeechBubble.color.g, emmettSpeechBubble.color.b, Mathf.Clamp01(currentTimer / alphaFadeInTime));
            }
            else
            {
                emmettSpeechBubble.color = new Color(emmettSpeechBubble.color.r, emmettSpeechBubble.color.g, emmettSpeechBubble.color.b, 1 - Mathf.Clamp01(currentTimer / alphaFadeInTime));
            }

        } while (currentTimer < alphaFadeInTime);

        //makes sure the alpha is actually set to zero or 1
        if (fadeIn)
        {
            emmettSpeechBubble.color = new Color(emmettSpeechBubble.color.r, emmettSpeechBubble.color.g, emmettSpeechBubble.color.b, 1);
        }
        else
        {
            emmettSpeechBubble.color = new Color(emmettSpeechBubble.color.r, emmettSpeechBubble.color.g, emmettSpeechBubble.color.b, 0);
            //set the the objects off if we are fading out
            emmettSpeechBubble.gameObject.SetActive(false);
            bubbleText.gameObject.SetActive(false);
        }
        yield break;
    }

    //Coroutine that moves Emmett Offscreen
    //inverses x scale if need be
    private IEnumerator EmmettDisapears(bool cameFromLeft, bool reverseScale)
    {
        float offset = 3f;
        float travelTime = 0.5f;
        Vector3 wantedPosition;

        if (cameFromLeft)
        {
            wantedPosition = new Vector3(Camera.main.transform.position.x - StaticVars.CameraHalfWidth - offset, emmettReference.transform.position.y, emmettReference.transform.position.z);
        }
        else
        {
            wantedPosition = new Vector3(emmettReference.transform.position.x, Camera.main.transform.position.y - Camera.main.orthographicSize - offset, emmettReference.transform.position.z);
        }

        iTween.MoveTo(emmettReference, iTween.Hash("position", wantedPosition, "time", travelTime, "easetype", iTween.EaseType.linear));

        yield return new WaitForSeconds(travelTime);

        if (reverseScale)
        {
            emmettReference.transform.localScale = new Vector3(-emmettReference.transform.localScale.x, emmettReference.transform.localScale.y, emmettReference.transform.localScale.z);
        }

        emmettReference.SetActive(false);

        yield break;
    }

    #endregion

    //Coroutine used for emmett to give you the starting boxes
    private IEnumerator GiveBoxes()
    {
        GameObject textObject = Instantiate(boxesGivenDisplay, transform.position, Quaternion.identity) as GameObject;
        TextMesh text = textObject.GetComponent<TextMesh>();
        if (text == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Text Mesh object not found on text object.");
#endif

            yield break;
        }

        MeshRenderer mesh = textObject.GetComponent<MeshRenderer>();
        if (mesh == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Mesh Renderer object not found on Text object.");
#endif
        }

        mesh.sortingLayerName = "UI";
        mesh.sortingOrder = 20;


        text.text = "+" + SaveManager.Instance.classDifferences[(int)StatisticsManager.Instance.CurrentClass].startingBoxes;

        textObject.transform.localScale = Vector3.zero;

        AudioManager.Instance.PlayAudioClip(SFXType.PointGain, 1, 7);

        float yOffset = 0.75f;

        iTween.MoveTo(textObject, iTween.Hash("y", transform.position.y + yOffset, "time", StaticVars.TIME_MOVE_NUMBER, "ignoretimescale", true));
        iTween.ScaleTo(textObject, iTween.Hash("scale", Vector3.one * StaticVars.TUTORIAL_TEXT_SCALE, "time", StaticVars.TIME_MOVE_NUMBER, "ignoretimescale", true));

        yield return new WaitForSeconds(StaticVars.TIME_MOVE_NUMBER + StaticVars.TIME_SHOW_ITEM);

        iTween.MoveTo(textObject, iTween.Hash("position", boxOverviewUI.transform.position, "time", StaticVars.TIME_MOVE_NUMBER, "ignoretimescale", true));
        iTween.ScaleTo(textObject, iTween.Hash("scale", Vector3.zero, "time", StaticVars.TIME_MOVE_NUMBER, "ignoretimescale", true));

        yield return new WaitForSeconds(StaticVars.TIME_MOVE_NUMBER + StaticVars.TIME_SHOW_ITEM);
        StatisticsManager.Instance.GiveStartingBoxes();

        if (tournamentManager.GetComponent<TournamentManager>().InTournamentMode)
        {
            Debug.Log("Reaced InTournamentMode Section");
            if (!tournamentManager.GetComponent<TournamentManager>().PassedTournyIntro)
            {
                Debug.Log("Triggered Start Event Coroutine");
                MainGameEventManager.TriggerTournyStartEvent();
            }
        }

        yield break;
    }

    // For fading in shop button
    private IEnumerator SetShopButtonActive()
    {
        shopButton.GetComponent<Button>().interactable = true;        

        yield break;
    }

    private IEnumerator SetDeskButtonActive()
    {
        shopItemObject.GetComponent<PopLib>().nonTutorialFurniture[0].GetComponent<Button>().interactable = true;

        yield break;
    }

    private IEnumerator FurnitureTutorialUISetup()
    {

        shopButtonParent.transform.SetParent(tutorialCanvas.gameObject.transform);
        shopButtonParent.transform.SetSiblingIndex(1);
        shopButtonParent.transform.position = ShopButtonTutorialPosition.transform.position;
        shopButton.transform.localScale = new Vector3(2.5f, 2.5f);

        shopButtonParent.SetActive(true);

        shopInterface.transform.SetParent(tutorialCanvas.gameObject.transform);
        shopInterface.transform.SetSiblingIndex(2);

        GraphicRaycaster addRaycast = tutorialCanvas.gameObject.AddComponent<GraphicRaycaster>() as GraphicRaycaster;

        shopButton.GetComponent<Button>().interactable = false;

        shopInterfaceExitButton.GetComponent<Button>().interactable = false;

        yield break;
    }

    /// <summary>
    /// if they have gotten several but haven't completed the furniture tutorial yet
    /// it means they jumped out sometime in the process. Run it again if that is the case.
    /// </summary>
    private IEnumerator CheckForTutorialCompletion()
    {
        bool wait = true;

        yield return new WaitForSeconds(0.5f);

        startedFurnitureTutorial = SaveManager.Instance.StartedFurnitureTutorial;

        while (wait)
        {
            yield return new WaitForSeconds(0.5f);

            if (SceneTransition.Instance.SelectedClass == Classes.Poor)
            {

                if (!SaveManager.Instance.CompletedFurnitureTutorial && startedFurnitureTutorial)
                {
                    if(furnitureTutorialRunning != null)
                    {
                        // if the tutorial coroutine is already running, we don't need to be here
                        yield break;
                    }
                    Debug.Log("Restarting Tutorial");
                    RunFurnitureTutorialWrapper();
                    yield break;
                }
            }
            yield return null;
        }
    }

    
    public void SendFurnitureTutorialData()
    {
        StatisticsManager.Instance.UpdateFurnitureTutorialStartedState(startedFurnitureTutorial);
    }
    

}
