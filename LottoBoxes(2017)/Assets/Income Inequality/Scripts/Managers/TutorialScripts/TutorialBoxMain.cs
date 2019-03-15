using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

/* Class that Controls the Box in the Main Game Tutorial
 * 
 */ 
public class TutorialBoxMain : MonoBehaviour,IPointerDownHandler
{

    private const float BOX_DROP_TIME = 0.7f;
    private const float BOX_OPEN_TIME = 0.5f;
    private const float BOX_EXISTANCE_TIME = 1.3f;
    private const float BOX_SHAKE_TIME = 0.2f;
    private const float BOX_WAIT_TO_LEAVE = 0.3f;

  
    public bool givesABox = true;
    private bool wasTapped = false;
    private bool canTap = false;

    [Header("Put the box component prefab here")]
    public GameObject componentPrefab;

    [Header("Put the gameobject that represents how many boxes were gain here")]
    public GameObject boxesReceivedText;

    [Header("The Sprite for the box that might be generated")]
    public Sprite boxSprite;

    public Sprite randomTicketSprite;


    private GameObject leftLid;
    private GameObject rightLid;

    private Vector3 upDestination;
    private Vector3 finalDestination;
    private Vector3 rotateDirection;

    private TutorialTicketPiece ticketPiece;

    // Use this for initialization
    private void Awake()
    {
        iTween.Init(gameObject);
        leftLid = gameObject.transform.GetChild(1).gameObject;
        rightLid = gameObject.transform.GetChild(2).gameObject;
        StartCoroutine(MoveToPosition());

    }

    //detects box click and plays the animation, makes sure it can only be tapped once
    public void OnPointerDown(PointerEventData data)
    {
        if (!wasTapped && canTap)
        {
            wasTapped = true;
            //StatisticsManager.Instance.UpdateBoxesTappedUI();
            StartCoroutine(PlayBoxAnimation());
        }
    }

    //Function that actually controls the box animation
    private IEnumerator PlayBoxAnimation()
    {
        // Left lid rotation.
        iTween.RotateAdd(
            leftLid.gameObject,
            iTween.Hash(
                "amount", new Vector3(0, 0, -250),
                "easetype", iTween.EaseType.easeOutBounce,
                "time", BOX_OPEN_TIME,
                "ignoretimescale", true));

        // Right lid rotation.
        iTween.RotateAdd(
            rightLid.gameObject,
            iTween.Hash("amount", new Vector3(0, 0, 250),
                "easetype", iTween.EaseType.easeOutBounce,
                "time", BOX_OPEN_TIME,
                "ignoretimescale", true));

        
        //depending on the Box generate a component
        if (givesABox)
        {
            AudioManager.Instance.PlayAudioClip(SFXType.PointGain);
            GenerateBoxComponent();
        }
        else
        {
            AudioManager.Instance.PlayAudioClip(SFXType.GainGoldenTicket);
            GenerateTicketComponent();
        }

        if (givesABox)
        {

            // Shake entire box for added suspense.
            iTween.ShakeRotation(gameObject,
                iTween.Hash(
                    "amount", new Vector3(0, 0, 10),
                    "time", BOX_SHAKE_TIME,
                    "oncomplete", "PlayDestroyAnimation"));
        }
        else
        {
            iTween.ShakeRotation(gameObject,
                iTween.Hash(
                    "amount", new Vector3(0, 0, 10),
                    "time", BOX_SHAKE_TIME));
        }

        //yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(BOX_EXISTANCE_TIME));

        yield break;


    }

    //Utilizes the regular box stuff
    private void GenerateBoxComponent()
    {
        //if this Tutorial box Generates a box, just utilize the already made box item
        GameObject toSpawn = Instantiate(componentPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        SpriteRenderer componentRenderer = toSpawn.GetComponent<SpriteRenderer>();
        if (componentRenderer != null)
        {
            componentRenderer.sortingOrder = 9;
        }
        BoxItem box = toSpawn.AddComponent<BoxItem>();
        box.Setup(boxSprite, this.gameObject, boxesReceivedText, 1);
    }

    //need to make our own Ticket Stuff for the tutorial
    private void GenerateTicketComponent()
    {
        GameObject toSpawn = Instantiate(componentPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        SpriteRenderer componentRenderer = toSpawn.GetComponent<SpriteRenderer>();
        if (componentRenderer != null)
        {
            componentRenderer.sortingOrder = 9;
        }

        ticketPiece = toSpawn.AddComponent<TutorialTicketPiece>();
        ticketPiece.Setup(randomTicketSprite, this.gameObject);
    }

    //Set for Box Destroy Animation
    //Is calledback from Itween on Play Box Animation
    private void PlayDestroyAnimation()
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
        diagonalLength = Mathf.Sqrt(diagonalLength);


        //we always want the direction to go to the left
        upDestination = new Vector3();
        upDestination.x = gameObject.transform.position.x + cameraHalfWidth + 3f;
        upDestination.y = gameObject.transform.position.y + 0.5f;
        upDestination.z = gameObject.transform.position.z;

        finalDestination = new Vector3();
        finalDestination.x = 5.3f;
        finalDestination.y = -4.5f;
        finalDestination.z = gameObject.transform.position.z;

        rotateDirection = new Vector3();
        rotateDirection.x = 329.5f;
        rotateDirection.y = 169.5f;
        rotateDirection.z = 240f;

        StartCoroutine(BoxDestroyAnimation());
    }

    //Function that actually plays the BoxDestroyed Event
    private IEnumerator BoxDestroyAnimation()
    {
        yield return new WaitForSeconds(BOX_WAIT_TO_LEAVE);

        iTween.RotateTo(gameObject, iTween.Hash(
                "rotation", rotateDirection,
                "easetype", iTween.EaseType.easeOutQuad,
                "time", .5f
            )
        );
        iTween.MoveTo(gameObject, iTween.Hash(
                "position", upDestination,
                "time", 1f,
                "easetype", iTween.EaseType.easeInOutBack,
                "oncomplete", "DestructionRoutine"
            )
        );
    }

    //Destroys this game object
    //Is a callback from BoxDestroyAnimation
    private void DestructionRoutine()
    {
        Destroy(this.gameObject);
    }

    private IEnumerator MoveToPosition()
    {
        Vector3 wantedPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.root.position.z);
       
        iTween.MoveTo(this.gameObject, iTween.Hash("position", wantedPos, "time", BOX_DROP_TIME, "easetype", iTween.EaseType.easeOutBack));

        yield return new WaitForSeconds(BOX_DROP_TIME);

        MainGameEventManager.TriggerBoxSpawnedEvent();

        MainGameTutorial.Instance.ContinueTutorial(EnableClickDetect);
        yield break;
    }

    //Call Back function for the Main Game Tutorial to use
    private void EnableClickDetect()
    {
        canTap = true;
    }

    //Function used by TutorialTicketPiece
    public void RunReverseAnimation()
    {
        StartCoroutine(ReverseAnimation());
    }

    //Reverse the boxes animation(close the box)
    private IEnumerator ReverseAnimation()
    {

        // Left lid rotation.
        iTween.RotateAdd(
            leftLid.gameObject,
            iTween.Hash(
                "amount", new Vector3(0, 0, 250),
                "easetype", iTween.EaseType.easeOutBounce,
                "time", BOX_OPEN_TIME,
                "ignoretimescale", true));

        // Right lid rotation.
        iTween.RotateAdd(
            rightLid.gameObject,
            iTween.Hash("amount", new Vector3(0, 0, -250),
                "easetype", iTween.EaseType.easeOutBounce,
                "time", BOX_OPEN_TIME,
                "ignoretimescale", true));

        yield return new WaitForSeconds(BOX_OPEN_TIME);

        //Destroy the ticket
        GameObject.Destroy(ticketPiece.gameObject);

        StartCoroutine(MoveOffScreen());

        yield break;
    }

    //Move the box off screen
    private IEnumerator MoveOffScreen()
    {
        Vector3 wantedPos = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
        wantedPos.y += Camera.main.orthographicSize + MainGameTutorial.SCREEN_OFFSET;
        iTween.MoveTo(this.gameObject, iTween.Hash("position", wantedPos, "time", BOX_DROP_TIME, "easetype", iTween.EaseType.easeOutBack, "oncomplete", "DestructionRoutine"));
        yield return new WaitForSeconds(BOX_DROP_TIME);

        MainGameTutorial.Instance.ContinueTutorial();
        GameObject.Destroy(this.gameObject);

        yield break;
    }
}
