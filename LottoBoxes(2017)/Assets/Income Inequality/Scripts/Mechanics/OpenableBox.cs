using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class handles moving boxes in the main game scene on screen.
/// It also handles animating the opening and generating the contents.
/// Furthermore, objects with this attached handle tap input independently.
/// </summary>
public class OpenableBox : MonoBehaviour, IPointerDownHandler
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

    private GameObject leftLid, rightLid;
    private bool isOpen = false;
    public SpawnPoint assignedSpawnPoint;

    private Vector3 upDestination;
    private Vector3 finalDestination;
    private Vector3 rotateDirection;

    private Coroutine shakeCor;

    public Sprite boxSprite;
    public Sprite emptyBoxPoof;
    public GameObject componentPrefab;
    public GameObject boxesReceivedText;
    public GameObject ticketBurstPrefab;
    private GameObject disasterManager;
    private GameObject mainManager;
    private GameObject tournamentManager;

    public bool giveTicketPiece;
    private bool firstBillNotDone = true;

    #region Unity Callbacks

    private void Awake()
    {
        mainManager = GameObject.FindGameObjectWithTag("MainGameManager");

        if (SaveManager.Instance.TotalBoxesOpened < 25)
        {

            disasterManager = GameObject.FindGameObjectWithTag("DisasterManager");

        }

        tournamentManager = GameObject.FindGameObjectWithTag("TournamentManager");

        ApplyTextures();
        iTween.Init(gameObject);
        // Grabbing left and right lids for animating.
        leftLid = gameObject.transform.GetChild(1).gameObject;
        rightLid = gameObject.transform.GetChild(2).gameObject;
        shakeCor = StartCoroutine(TapMeShake());
    }


    private void OnEnable()
    {
        SubscribeToEvents();
        StartCoroutine(MoveToOnScreenPosition());        
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
        assignedSpawnPoint.IsOccupied = false;
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

        // Left lid rotation.
        iTween.RotateAdd(
            leftLid.gameObject, 
            iTween.Hash(
                "amount", new Vector3(0, 0, -252), 
                "easetype", iTween.EaseType.easeOutBounce, 
                "time", BOX_OPEN_TIME,
                "ignoretimescale", true));
        
        // Right lid rotation.
        iTween.RotateAdd(
            rightLid.gameObject,
            iTween.Hash(
                "amount", new Vector3(0, 0, 252),
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
    /// Need an accessable reference to the destroy animation of the regular boxes
    /// so that we can remove them via the MainGameSpawner script when the 
    /// oppertunity box comes in.
    /// </summary>
    public void PublicDestroyAnimation()
    {
        PlayDestroyAnimation();
    }

    /// <summary>
    /// Function that checks the boxes x position and sets its final destionation.
    /// This function also starts a coroutine that runs the final box animation.
    /// </summary>
    void PlayDestroyAnimation()
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
        Vector3 orgEuler = transform.localEulerAngles;
        float moveTime = 0.7f;

        assignedSpawnPoint = MainGameSpawner.instance.PickSpawnPoint();

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

        yield break;
    }

    private void ApplyTextures()
    {
        Texture wantedTexture = BoxTextureContainer.Instance.ReceiveTexture();
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
        }
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
    /// After filling a list of items to spawn, cit calls a function to spawn them.
    /// </summary>
    private List<StaticVars.BoxContent> GenerateComponents()
    {
        List<StaticVars.BoxContent> componentsToSpawn = new List<StaticVars.BoxContent>();

        for (int i = 0; i < StaticVars.MAX_BOX_COMPONENTS; i++)
        {
            int randNum = Random.Range(0, StaticVars.BOX_PERCENT_CHANCE + StaticVars.GOLDTICKET_PERCENT_CHANCE + StaticVars.NOTHING_PERCENT_CHANCE);
            StaticVars.BoxContent current = StaticVars.GetContents(randNum);
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
                if(tournamentManager.GetComponent<TournamentManager>().InTournamentMode == true)
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
                            int boxesReceived = SaveManager.Instance.GetBoxesReceived(StatisticsManager.Instance.CurrentClass, SaveManager.Instance.TotalBoxesOpened);
                            //MainGameEventManager.TriggerBoxFoundEvent(boxesReceived);
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

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isOpen)
        {
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
            
            if(tournamentManager.GetComponent<TournamentManager>().InTournamentMode)
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

            StartCoroutine(PlayBoxAnimation());
        }
    }

    #endregion

    #region General Utilities

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

    #endregion

    #region Event Subscription

    private void SubscribeToEvents()
    {
        MainGameEventManager.OnGameEnd += DestructionRoutine;

        // When the oppertunity box spawns, play the destruction animation for boxes
        MainGameEventManager.OnOpportunityBoxSpawn += PlayDestroyAnimation;

        if (SceneTransition.Instance.SelectedClass == Classes.Poor && !SaveManager.Instance.CompletedFurnitureTutorial)
        {
            MainGameEventManager.OnFurnitureTutorialStart += DestructionRoutine;
        }
    }

    private void UnsubscribeFromEvents()
    {        
        MainGameEventManager.OnGameEnd -= DestructionRoutine;
        MainGameEventManager.OnOpportunityBoxSpawn -= PlayDestroyAnimation;

        if (SceneTransition.Instance.SelectedClass == Classes.Poor && !SaveManager.Instance.CompletedFurnitureTutorial)
        {
            MainGameEventManager.OnFurnitureTutorialStart -= DestructionRoutine;
        }
    }

    #endregion
}
