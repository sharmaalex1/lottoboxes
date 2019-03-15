using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class handles moving opportunity boxes in the main game scene on screen.
/// It handles data/events related to the box itself, for spawning related things,
/// look in the MainGameSpawner script, as there is some crossover between these two.
/// It also handles animating the opening and generating the contents.
/// Furthermore, objects with this attached handle tap input independently.
/// </summary>
public class OpportunityBox : MonoBehaviour, IPointerDownHandler
{
    /// How long it takes for the box to complete its opening animation.
    /// </summary
    public const float BOX_OPEN_TIME = 0.5f;
    /// <summary>
    /// How long it takes for the box to shake and open.
    /// </summary>
    public const float BOX_SHAKE_TIME = 0.5f;
    /// <summary>
    /// How long it takes for box to leave screen after opening.
    /// </summary>
    public const float BOX_WAIT_TO_LEAVE = 0.5f;

    private GameObject leftLid, rightLid, door;
    private bool isOpen = false;
    public SpawnPoint assignedSpawnPoint;

    StaticVars.BoxContent current;

    private Vector3 upDestination;
    private Vector3 finalDestination;
    private Vector3 rotateDirection;

    private Coroutine shakeCor;
    private Coroutine oppBoxCountdown;

    public Sprite boxSprite;
    public Sprite emptyBoxPoof;
    public GameObject componentPrefab;
    public GameObject boxesReceivedText;
    public GameObject ticketBurstPrefab;
    private GameObject disasterManager;
    private GameObject mainManager;
    private GameObject tournamentManager;
    private GameObject opportunityBoxObject;
    private GameObject safeHealthText;
    private GameObject safeTimerText;
    private GameObject healthBarUI;
    private GameObject timerBarUI;
    private Image healthBarFill;
    private Image timerBarFill;

    private GameObject[] activeOppBoxes;

    public bool giveTicketPiece;
    private bool firstBillNotDone = true;

    public int maxHealth = 100;
    public int baseHealth = 50;
    public int opportunityBoxHealth = 0;
    public int opportunityBoxFullHealth = 0;
    public float opportunityBoxTimer = 0.0f;
    public float opportunityBoxOriginalTimer = 0.0f;
    private bool failedToOpen = false;

    float oppBoxHealthFloat;
    float oppBoxFullHealthFloat;
    [SerializeField]
    private GameObject opportunityBoxTimerText;

    [SerializeField]
    private GameObject opportunityBoxHealthText;

    [SerializeField]
    private GameObject sparksOnTap;

    [SerializeField]
    private GameObject healthBarCanvas;

    [SerializeField]
    private GameObject opportunityBoxTimerBarCanvas;

    public AudioClip tapNoise;

    #region Unity Callbacks

    private void Awake()
    {
        // If the safe was active when the user closed the app
        // give it the values it had when the closure happened
        if (MainGameSpawner.instance.opportunityBoxActive)
        {
            opportunityBoxHealth = SaveManager.Instance.OpportunityBoxHealth;
            opportunityBoxFullHealth = SaveManager.Instance.OpportunityBoxFullHealth;

            opportunityBoxTimer = SaveManager.Instance.OpportunityBoxTimer;
            opportunityBoxOriginalTimer = SaveManager.Instance.OpportunityBoxOriginalTimer;

            oppBoxHealthFloat = opportunityBoxHealth;
            oppBoxFullHealthFloat = opportunityBoxFullHealth;
        }
        else
        {
            if(SceneTransition.Instance.SelectedClass == Classes.Rich)
            {
                opportunityBoxHealth = 100;
                opportunityBoxFullHealth = opportunityBoxHealth;
                opportunityBoxTimer = 20.0f;
                opportunityBoxOriginalTimer = opportunityBoxTimer;
            }
            else if(SceneTransition.Instance.SelectedClass == Classes.Poor)
            {
                opportunityBoxHealth = 100;
                opportunityBoxFullHealth = opportunityBoxHealth;
                opportunityBoxTimer = 10.0f;
                opportunityBoxOriginalTimer = opportunityBoxTimer;
            }
        }

        mainManager = GameObject.FindGameObjectWithTag("MainGameManager");

        if (SaveManager.Instance.TotalBoxesOpened < 25)
        {

            disasterManager = GameObject.FindGameObjectWithTag("DisasterManager");

        }

        tournamentManager = GameObject.FindGameObjectWithTag("TournamentManager");

        ApplyTextures();
        iTween.Init(gameObject);
        // Grabbing left and right lids for animating.
        //leftLid = gameObject.transform.GetChild(1).gameObject;
        //rightLid = gameObject.transform.GetChild(2).gameObject;
        door = gameObject.transform.GetChild(1).gameObject;
        shakeCor = StartCoroutine(TapMeShake());
    }


    private void OnEnable()
    {
        SubscribeToEvents();
        activeOppBoxes = GameObject.FindGameObjectsWithTag("OpportunitySafe");

        // If opportunity boxes spawn back-to-back
        // then Unity sometimes forgets to call our spawn even
        // for this box because two are technically "active".
        // This makes sure the system functions correctly
        if(activeOppBoxes.Length == 2)
        {
            MainGameEventManager.TriggerOpportunityBoxSpawnedEvent();
        }

        StartCoroutine(MoveToOnScreenPosition());
    }

    private void OnDisable()
    {
        StatisticsManager.Instance.UpdateOppertunityBoxInfo(opportunityBoxHealth, opportunityBoxFullHealth, opportunityBoxTimer, opportunityBoxOriginalTimer);
        assignedSpawnPoint.IsOccupied = false;
        UnsubscribeFromEvents();
    }

    #endregion

    #region Box Manipulation Methods

    /// <summary>
    /// Opens the left and right lids attached to the box
    /// </summary>
    private IEnumerator PlayBoxAnimation()
    {
        AudioManager.Instance.PlayAudioClip(SFXType.BoxTap);

        int rotationAmount = Random.Range(2, 4);
        iTween.RotateBy(gameObject,
            iTween.Hash(
                "amount", Vector3.up * ((Random.Range(0, 50) > 25) ? -1.0f : 1.0f),
                "time", BOX_SHAKE_TIME,
                "easetype", iTween.EaseType.easeInCirc,
                "islocal", true,
                "ignoretimescale", true));

        iTween.MoveBy(gameObject,
            iTween.Hash(
                "amount", Vector3.up * 0.4f,
                "time", BOX_SHAKE_TIME,
                "easetype", iTween.EaseType.easeOutCirc,
                "islocal", false,
                "ignoretimescale", true));

        yield return new WaitForSeconds(BOX_SHAKE_TIME);



        List<StaticVars.BoxContent> componentsToSpawn = GenerateComponents();
        for (int i = 0; i < componentsToSpawn.Count; i++)
        {
            switch (componentsToSpawn[i])
            {
                case StaticVars.BoxContent.GoldTicket:
                case StaticVars.BoxContent.Box:
                    AudioManager.Instance.PlayAudioClip(SFXType.BoxItem);
                    break;
                case StaticVars.BoxContent.Nothing:
                    AudioManager.Instance.PlayAudioClip(SFXType.GainNothing);
                    break;
                default:
                    break;
            }
        }

        //make it look like the box is spitting out the item
        //punches position when the box is half open 
        iTween.PunchPosition(this.gameObject,
            iTween.Hash(
                "amount", Vector3.down / 3,
                "time", BOX_OPEN_TIME,
                "ignoretimescale", true));

        /*// Left lid rotation.
		iTween.RotateAdd(
			leftLid.gameObject, 
			iTween.Hash(
				"amount", new Vector3(0, 0, -252), 
				"easetype", iTween.EaseType.easeOutBounce, 
				"time", BOX_OPEN_TIME,
				"ignoretimescale", true));*/

        // Right lid rotation.
        iTween.RotateAdd(
            door.gameObject,
            iTween.Hash(
                "amount", new Vector3(0, 60, 0),
                "easetype", iTween.EaseType.easeOutBounce,
                "time", BOX_OPEN_TIME,
                "ignoretimescale", true));

        //we only want to wait about 1 third of the box open time before generating the components
        yield return new WaitForSeconds(BOX_OPEN_TIME * 0.33f);
        SpawnComponents(componentsToSpawn);
        //yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(BOX_EXISTENCE_TIME - BOX_OPEN_TIME - BOX_SHAKE_TIME));
        PlayDestroyAnimation();

        yield break;
    }

    /// <summary>
    /// Function that checks the boxes x position and sets its final destionation.
    /// This function also starts a coroutine that runs the final box animation.
    /// </summary>
    public void PlayDestroyAnimation()
    {
        //aspect ratio is width/height
        //height is orthographicSize * 2
        //multiply aspect ratio * height to get width
        //divide it by 2 because we only need half of it.
        float cameraHalfWidth = (Camera.main.aspect * (Camera.main.orthographicSize * 2f)) / 2f;
        BoxCollider col = GetComponent<BoxCollider>();
        if (col == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Couldn't find Openable Box's Box Collider");
#endif
        }

        float diagonalLength = (col.size.x * col.size.x) + (col.size.y * col.size.y) + (col.size.z * col.size.z);
        diagonalLength = Mathf.Pow(diagonalLength, 1f / 3f);


        if (gameObject.transform.position.x > 0)
        {
            upDestination.x = cameraHalfWidth + diagonalLength + 3f;
            upDestination.y = gameObject.transform.position.y + 0.5f;
            upDestination.z = gameObject.transform.position.z;

            finalDestination.x = 5.3f;
            finalDestination.y = -4.5f;
            finalDestination.z = gameObject.transform.position.z;

            rotateDirection.x = 329.5f;
            rotateDirection.y = 169.5f;
            rotateDirection.z = 240f;
        }
        else
        {
            upDestination.x = -cameraHalfWidth - diagonalLength - 3;
            upDestination.y = gameObject.transform.position.y + 0.5f;
            upDestination.z = gameObject.transform.position.z;

            finalDestination.x = -5.3f;
            finalDestination.y = -4.5f;
            finalDestination.z = gameObject.transform.position.z;

            rotateDirection.x = 30.4f;
            rotateDirection.y = 169.5f;
            rotateDirection.z = 120f;
        }

        StartCoroutine(BoxDestroyAnimation());
    }

    /// <summary>
    /// Animation function that twists the box and moves it outside of the screen.
    /// The box is destroyed at the end of the MoveTo animation.  
    /// </summary>
    IEnumerator BoxDestroyAnimation()
    {
        yield return new WaitForSeconds(BOX_WAIT_TO_LEAVE);

        iTween.RotateTo(gameObject,
            iTween.Hash(
                "rotation", rotateDirection,
                "easetype", iTween.EaseType.easeOutQuad,
                "time", .5f));

        iTween.MoveTo(gameObject,
            iTween.Hash(
                "position", upDestination,
                "time", 1f,
                "easetype", iTween.EaseType.easeInOutBack,
                "oncomplete", "DestructionRoutine"));
    }

    /// <summary>
    /// After spawning, this picks a spawn point for the box to move towards.
    /// It then animates towards it after waiting a random bit of time to.
    /// This allows a staggering effect when the boxes move on screen.
    /// </summary>
    /// <returns>The to on screen position.</returns>
    private IEnumerator MoveToOnScreenPosition()
    {
        if (MainGameSpawner.instance.freshOppBoxSpawn)
        {
            yield return new WaitForSeconds(2.5f);
        }

        Vector3 orgEuler = transform.localEulerAngles;
        float moveTime = 0.7f;

        assignedSpawnPoint = MainGameSpawner.instance.OpportunityBoxSpawnPoint;
        assignedSpawnPoint.IsOccupied = true;

        if (assignedSpawnPoint == null)
        {
            Debug.LogError("There wasn't a spawn point to move to. Destroying box...");
        }

        Vector3 dir = Vector3.Normalize(assignedSpawnPoint.XForm.position - transform.position);

        //subtract 90 degrees because the oreintation of the 
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90f;

        //rotate on the z axis
        transform.rotation = Quaternion.AngleAxis(angle, transform.forward);
        //because we rotated on the z axis 180 degrees(roughly), the box that was facing the player is not facing away,
        //so invert the x rotation
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x + 20, transform.localEulerAngles.y, transform.localEulerAngles.z);


        // Allows the box to move to a point with a bit of bounce back.
        iTween.MoveTo(gameObject, iTween.Hash("position", assignedSpawnPoint.XForm.position, "time", moveTime, "easetype", iTween.EaseType.easeOutBack));

        yield return new WaitForSeconds(moveTime / 2);

        iTween.RotateTo(this.gameObject, iTween.Hash("rotation", orgEuler,
            "time", moveTime / 2,
            "easetype", iTween.EaseType.spring,
            "islocal", true));

        yield return new WaitForSeconds(moveTime / 2);

        // Start the countdown after the opportunity box is in position

        opportunityBoxObject = this.gameObject;

        OpportunityBoxUI();

        Debug.Log("First opportunity box ever?: " + MainGameSpawner.instance.firstOpportunityBoxEver);
        if (SaveManager.Instance.FirstOpportunityBoxEver)
        {
            MainGameSpawner.instance.opportunityBoxTutorial.SetActive(true);
            MainGameSpawner.instance.firstOpportunityBoxEver = false;

            SaveManager.Instance.FirstOpportunityBoxEver = false;
        }
        else
        {
            oppBoxCountdown = StartCoroutine(OpportunityBoxCountdown());
        }

        yield break;
    }

    private void ApplyTextures()
    {
        /*Texture wantedTexture = BoxTextureContainer.Instance.ReceiveTexture();
		for (int i = 0; i < transform.childCount; i++)
		{
			MeshRenderer currentMeshRenderer = transform.GetChild(i).gameObject.GetComponent<MeshRenderer>();
			if (currentMeshRenderer == null)
			{
				#if UNITY_EDITOR
				Debug.LogError("Couldn't Find Child's mesh renderer");
				#endif
				return;
			}

			currentMeshRenderer.material.mainTexture = wantedTexture;
		}*/
    }

    //shake the box a little bit to get the players attention
    private IEnumerator TapMeShake()
    {
        while (true)
        {
            //wait a random amount of time to shake
            yield return new WaitForSeconds(Random.Range(3f, 7f));
            //shake position with random force
            //iTween.ShakePosition(this.gameObject, iTween.Hash("amount", Vector3.one * Random.Range(0.05f, 0.15f), "time", 0.25f, "ignoretimescale", true));
            //shake rotation with random torque
            iTween.ShakeRotation(this.gameObject, iTween.Hash("amount", Vector3.one * Random.Range(2f, 4f), "time", 0.25f, "ignoretimescale", true));
            yield return null;
        }
    }



    #endregion

    #region Contents Generation

    /// <summary>
    /// Picks a random element in a list of components.
    /// Depending on what the designated max amount of components is, this can pick multiple elements.
    /// After filling a list of items to spawn, it calls a function to spawn them. Different item lists for poor/rich modes.
    /// </summary>
    private List<StaticVars.BoxContent> GenerateComponents()
    {
        List<StaticVars.BoxContent> componentsToSpawn = new List<StaticVars.BoxContent>();

        for (int i = 0; i < StaticVars.MAX_BOX_COMPONENTS; i++)
        {
            if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
            {
                int randNum = Random.Range(0, StaticVars.OPPBOX_BOX_PERCENT_CHANCE + StaticVars.OPPBOX_GOLDTICKET_PERCENT_CHANCE + StaticVars.OPPBOX_NOTHING_PERCENT_CHANCE);
                current = StaticVars.OppBoxGetContents(randNum);
                Debug.Log("Filling Rich Components");
            }
            else if (StatisticsManager.Instance.CurrentClass == Classes.Poor)
            {
                int randNum = Random.Range(0, StaticVars.POOR_OPPBOX_BOX_PERCENT_CHANCE + StaticVars.POOR_OPPBOX_GOLDTICKET_PERCENT_CHANCE + StaticVars.POOR_OPPBOX_NOTHING_PERCENT_CHANCE);
                current = StaticVars.PoorOppBoxGetContents(randNum);
                Debug.Log("Filling Poor Components");
            }
            componentsToSpawn.Add(current);
        }

        return componentsToSpawn;
    }

    /// <summary>
    /// Takes in a list of components to spawn in a box and spawns one currently.
    /// Can be adjusted to spawn all items in the list.
    /// </summary>
    private void SpawnComponents(List<StaticVars.BoxContent> contentsList)
    {
        if (giveTicketPiece)
        {
            AudioManager.Instance.PlayAudioClip(SFXType.GainGoldenTicket);
            if (SaveManager.Instance.NumCurrentTicketPieces < 5)
            {
                GameObject spinningBurst = Instantiate(ticketBurstPrefab, transform.position, Quaternion.identity) as GameObject;
                GameObject toSpawn = Instantiate(componentPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                TicketPiece piece = toSpawn.AddComponent<TicketPiece>();
                piece.name = "PieceItem";
                MainGameEventManager.TriggerTicketPieceFoundEvent(piece);
                piece.Setup(this.gameObject, spinningBurst);
                MainGameEventManager.TriggerHyperModeBegin();

                if (tournamentManager.GetComponent<TournamentManager>().InTournamentMode == true)
                {
                    tournamentManager.GetComponent<TournamentManager>().AddScore(1, 10);
                    tournamentManager.GetComponent<TournamentManager>().PostScoresHelper();
                }
            }
        }
        else
        {
            for (int i = 0; i < contentsList.Count; i++)
            {
                switch (contentsList[i])
                {
                    case StaticVars.BoxContent.GoldTicket:
                        {
                            AudioManager.Instance.PlayAudioClip(SFXType.GainGoldenTicket);
                            if (SaveManager.Instance.NumCurrentTicketPieces < 5)
                            {
                                GameObject spinningBurst = Instantiate(ticketBurstPrefab, transform.position, Quaternion.identity) as GameObject;
                                GameObject toSpawn = Instantiate(componentPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                                TicketPiece piece = toSpawn.AddComponent<TicketPiece>();
                                piece.name = "PieceItem";
                                MainGameEventManager.TriggerTicketPieceFoundEvent(piece);
                                piece.Setup(this.gameObject, spinningBurst);
                                MainGameEventManager.TriggerHyperModeBegin();

                                if (tournamentManager.GetComponent<TournamentManager>().InTournamentMode == true)
                                {
                                    tournamentManager.GetComponent<TournamentManager>().AddScore(1, 10);
                                    tournamentManager.GetComponent<TournamentManager>().PostScoresHelper();
                                }
                            }
                        }
                        break;
                    case StaticVars.BoxContent.Box:
                        {
                            AudioManager.Instance.PlayAudioClip(SFXType.PointGain, 1, AudioManager.Instance.boxGetIndex);

                            AudioClip[] audioClip;
                            AudioManager.Instance.sfxAudioClipDictionary.TryGetValue(SFXType.PointGain, out audioClip);
                            if (AudioManager.Instance.boxGetIndex < audioClip.Length - 1)
                            {
                                AudioManager.Instance.boxGetIndex++;
                            }
                            else
                            {
                                AudioManager.Instance.boxGetIndex = 0;
                            }

                            GameObject toSpawn = Instantiate(componentPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                            BoxItem box = toSpawn.AddComponent<BoxItem>() as BoxItem;
                            box.name = "BoxItem";
                            box.GetComponent<SpriteRenderer>().sortingOrder = 6;

                            int opportunityBoxReward = Random.Range(0, 101);
                            int boxesReceived = 0;

                            if(SceneTransition.Instance.SelectedClass == Classes.Rich)
                            {
                                Debug.Log("Generating Rich Box Amount");
                                if (opportunityBoxReward < 15)
                                {
                                    boxesReceived = Random.Range(76, 151);
                                }
                                else if (opportunityBoxReward >= 15 && opportunityBoxReward < 74)
                                {
                                    boxesReceived = Random.Range(250, 376);
                                }
                                else if (opportunityBoxReward >= 74)
                                {
                                    boxesReceived = Random.Range(425, 601);
                                }
                            }
                            else if(SceneTransition.Instance.SelectedClass == Classes.Poor)
                            {
                                Debug.Log("Generating Poor Box Amount");
                                if (opportunityBoxReward < 33)
                                {
                                    boxesReceived = Random.Range(1, 51);
                                }
                                else if (opportunityBoxReward > 33 && opportunityBoxReward < 74)
                                {
                                    boxesReceived = Random.Range(150, 301);
                                }
                                else if (opportunityBoxReward >= 74)
                                {
                                    boxesReceived = Random.Range(375, 501);
                                }
                            }
                            box.Setup(boxSprite, this.gameObject, boxesReceivedText, boxesReceived);
                        }
                        break;
                    case StaticVars.BoxContent.Nothing:
                        {
                            GameObject toSpawn = Instantiate(componentPrefab, Vector3.up, Quaternion.identity) as GameObject;
                            PoofItem poof = toSpawn.AddComponent<PoofItem>() as PoofItem;
                            poof.name = "PoofItem";
                            poof.Setup(emptyBoxPoof, this.gameObject);
                            poof.GetComponent<SpriteRenderer>().sortingOrder = 9;
                        }
                        break;
                    default:
                        {
                            break;
                        }
                }
            }
        }
    }

    #endregion

    #region IPointerDownHandeler implementation

    /// <summary>
    /// Anything related to opporutnity box health bar
    /// and on-tap effects goes in here
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isOpen)
        {
            if (opportunityBoxHealth > 0)
            {
                opportunityBoxHealth--;
                safeHealthText.GetComponent<TextMesh>().text = opportunityBoxHealth.ToString();

                float oppBoxHealthFloat = opportunityBoxHealth;
                float oppBoxFullHealthFloat = opportunityBoxFullHealth;

                healthBarFill.fillAmount = oppBoxHealthFloat / oppBoxFullHealthFloat;

                iTween.ShakeRotation(this.gameObject, iTween.Hash("amount", Vector3.one * Random.Range(2f, 4f), "time", 0.25f, "ignoretimescale", true));

                this.GetComponent<AudioSource>().pitch = Random.Range(0.85f, 1.15f);
                this.GetComponent<AudioSource>().PlayOneShot(tapNoise);

                GameObject sparksToSpawn;

                sparksToSpawn = Instantiate(sparksOnTap, eventData.pointerCurrentRaycast.worldPosition, Quaternion.identity) as GameObject;

                ParticleSystemRenderer sparksLayers = sparksToSpawn.GetComponent<ParticleSystemRenderer>();

                sparksLayers.sortingLayerName = "UI";
                sparksLayers.sortingOrder = 9;

                ParticleSystem sparksPlay = sparksToSpawn.GetComponent<ParticleSystem>();

                float sparksDuration = sparksPlay.duration + sparksPlay.startLifetime;

                sparksPlay.Play();

                Destroy(sparksToSpawn, sparksDuration);
            }

            if ((opportunityBoxHealth - 1) < 0) //This is when the box opens
            {
                StopCoroutine(OpportunityBoxCountdown());

                // turn back on the UI elemens that were disabled when the box spawned
                MainGameSpawner.instance.disableWorkButton.interactable = true;
                MainGameSpawner.instance.disableSettingsButton.interactable = true;

                MainGameSpawner.instance.opportunityBoxActive = false;

                StatisticsManager.Instance.UpdateOppertunityBoxActiveState(MainGameSpawner.instance.opportunityBoxActive);


                Debug.Log("Opp Box active state set to false");

                this.GetComponent<AudioSource>().pitch = 1.75f;
                this.GetComponent<AudioSource>().PlayOneShot(tapNoise);
                isOpen = true;
                StopCoroutine(shakeCor);
                MainGameEventManager.TriggerBoxTappedEvent();

                SaveManager.Instance.TotalBoxesOpened++;
                mainManager.GetComponent<MainGameManager>().incrementBoxCount();

                if (SettingsManager.Instance.expensesManagerRef.activeSelf)
                {

                    if (firstBillNotDone)
                    {

                        if (SaveManager.Instance.TotalBoxesOpened >= 100 && !SaveManager.Instance.IsBillActive && !MainGameSpawner.instance.hyperModeisActive)
                        {
                            firstBillNotDone = false;

                            MainGameEventManager.TriggerFirstHundredBoxCountEvent();

                        }

                    }

                }

                if (SettingsManager.Instance.disasterMangerRef.activeSelf)
                {

                    if (SettingsManager.Instance.GetFirstOfferEver() && SaveManager.Instance.TotalBoxesOpened >= 25 && !MainGameSpawner.instance.hyperModeisActive)
                    {
                        MainGameEventManager.TriggerFirst25BoxCountEvent();
                    }

                }

                if (tournamentManager.GetComponent<TournamentManager>().InTournamentMode)
                {
                    if (MainGameSpawner.instance.hyperModeisActive)
                    {
                        tournamentManager.GetComponent<TournamentManager>().AddScore(1, 5);
                    }
                    else
                    {
                        tournamentManager.GetComponent<TournamentManager>().AddScore(1, 1);
                    }

                    tournamentManager.GetComponent<TournamentManager>().PostScoresHelper();
                }

                Debug.Log("OpportunityBox Active?: " + MainGameSpawner.instance.opportunityBoxActive);

                MainGameEventManager.TriggerOpportunityBoxDespanwedEvent();

                DeactivateOpportunityBoxUI();

                StartCoroutine(PlayBoxAnimation());

                Debug.Log("How many boxes on screen?: " + MainGameSpawner.instance.NumOnScreenBoxes);

                //MainGameSpawner.instance.tempCheckBoxSpawn();
            }
        }
    }

    #endregion

    #region General Utilities

    /// <summary>
    /// Setup for the UI (healh bar and timer) related to the Opportunity boxes
    /// </summary>
    private void OpportunityBoxUI()
    {
        Vector3 aboveOpportunityBoxTimer = new Vector3(this.transform.position.x, this.transform.position.y + 2.97f, this.transform.position.z);
        Vector3 aboveOpportunityBoxTimerBar = new Vector3(this.transform.position.x, this.transform.position.y + 2.4f, this.transform.position.z);
        Vector3 aboveOpportunityBoxHealth = new Vector3(this.transform.position.x, this.transform.position.y + 2.4f, this.transform.position.z);
        Vector3 aboveOpportunityBoxHealthBar = new Vector3(this.transform.position.x, this.transform.position.y + 1.85f, this.transform.position.z);

        healthBarUI = Instantiate(healthBarCanvas, aboveOpportunityBoxHealthBar, Quaternion.identity) as GameObject;
        timerBarUI = Instantiate(opportunityBoxTimerBarCanvas, aboveOpportunityBoxTimerBar, Quaternion.identity) as GameObject;
        safeHealthText = Instantiate(opportunityBoxHealthText, aboveOpportunityBoxHealth, Quaternion.identity) as GameObject;
        safeTimerText = Instantiate(opportunityBoxTimerText, aboveOpportunityBoxTimer, Quaternion.identity) as GameObject;

        healthBarUI.GetComponent<Canvas>().sortingLayerName = "UI";
        healthBarUI.GetComponent<Canvas>().sortingOrder = 8;

        timerBarUI.GetComponent<Canvas>().sortingLayerName = "UI";
        timerBarUI.GetComponent<Canvas>().sortingOrder = 8;

        healthBarFill = healthBarUI.transform.GetChild(1).GetComponent<Image>();
        timerBarFill = timerBarUI.transform.GetChild(1).GetComponent<Image>();

        oppBoxHealthFloat = opportunityBoxHealth;
        oppBoxFullHealthFloat = opportunityBoxFullHealth;

        healthBarFill.fillAmount = oppBoxHealthFloat / oppBoxFullHealthFloat;
        timerBarFill.fillAmount = opportunityBoxTimer / opportunityBoxOriginalTimer;

        MeshRenderer healthMesh = safeHealthText.GetComponent<MeshRenderer>();
        MeshRenderer timerMesh = safeTimerText.GetComponent<MeshRenderer>();

        healthMesh.sortingLayerName = "UI";
        healthMesh.sortingOrder = 10;

        timerMesh.sortingLayerName = "UI";
        timerMesh.sortingOrder = 10;

        // Update health to match current
        safeHealthText.GetComponent<TextMesh>().text = opportunityBoxHealth.ToString();
    }

    private void DeactivateOpportunityBoxUI()
    {
        Destroy(healthBarUI);
        Destroy(timerBarUI);
        Destroy(safeHealthText);
        Destroy(safeTimerText);
    }

    private IEnumerator OpportunityBoxCountdown()
    {
        while (opportunityBoxTimer > 0)
        {
            opportunityBoxTimer -= Time.deltaTime;
            safeTimerText.GetComponent<TextMesh>().text = (Mathf.CeilToInt(opportunityBoxTimer)).ToString();
            timerBarFill.fillAmount = opportunityBoxTimer / opportunityBoxOriginalTimer;
            yield return null;
        }

        opportunityBoxTimer = 0;
        opportunityBoxOriginalTimer = 0;

        DeactivateOpportunityBoxUI();

        MainGameSpawner.instance.disableWorkButton.interactable = true;
        MainGameSpawner.instance.disableSettingsButton.interactable = true;
        MainGameSpawner.instance.opportunityBoxActive = false;
        StatisticsManager.Instance.UpdateOppertunityBoxActiveState(MainGameSpawner.instance.opportunityBoxActive);
        Debug.Log("Opp Box active state set to false");
        MainGameEventManager.TriggerOpportunityBoxDespanwedEvent();

        PlayDestroyAnimation();

        Debug.Log("How many boxes on screen?: " + MainGameSpawner.instance.NumOnScreenBoxes);

        yield break;
    }

    // For when we need to start the coroutine via an event
    private void StartOpportunityBoxCountdown()
    {
        oppBoxCountdown = StartCoroutine(OpportunityBoxCountdown());
    }

	private void DestructionRoutine()
	{
        MainGameEventManager.TriggerBoxDestroyedEvent();
        Destroy(this.gameObject);
        
	}

	/// <summary>
	/// This version is for subscribing to the OnSlide event which just destroys the box because
	/// a transition to a new canvas is occuring.
	/// </summary>
	/// <param name="temp">Temp.</param>
	private void DestructionRoutine(int temp)
	{
		Destroy(this.gameObject);
	}

    private void SendOppertunityBoxInfo()
    {
        StatisticsManager.Instance.UpdateOppertunityBoxInfo(opportunityBoxHealth, opportunityBoxFullHealth, opportunityBoxTimer, opportunityBoxOriginalTimer);
    }

	#endregion

	#region Event Subscription

	private void SubscribeToEvents()
	{
		MainGameEventManager.OnGameEnd += DestructionRoutine;
        MainGameEventManager.OnOpportunityBoxTutorialEnd += StartOpportunityBoxCountdown;
        SaveManager.Instance.SendSaveData += SendOppertunityBoxInfo;

        if (SceneTransition.Instance.SelectedClass == Classes.Poor && !SaveManager.Instance.CompletedFurnitureTutorial)
        {
            MainGameEventManager.OnFurnitureTutorialStart += DestructionRoutine;
        }
    }

	private void UnsubscribeFromEvents()
	{        
		MainGameEventManager.OnGameEnd -= DestructionRoutine;
        MainGameEventManager.OnOpportunityBoxTutorialEnd -= StartOpportunityBoxCountdown;
        SaveManager.Instance.SendSaveData -= SendOppertunityBoxInfo;

        if (SceneTransition.Instance.SelectedClass == Classes.Poor && !SaveManager.Instance.CompletedFurnitureTutorial)
        {
            MainGameEventManager.OnFurnitureTutorialStart -= DestructionRoutine;
        }
    }

	#endregion
}
