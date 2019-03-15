using UnityEngine;
using System.Collections;
using System;

/* This class takes care of the ticket found
 * int the tutorial
 */ 
public class TutorialTicketPiece : Item
{

    //how far for the ticket to move in the y
    private const float TICKET_MOVEMENT_IN_Y = 1.5f;
    //tickets y position offset from the parent
    private const float TICKET_Y_OFFSET = 0.5f;


    private Vector3 ticketUIPos;
    private TutorialBoxMain tutorialBox;
    private float originalY;

    // Use this for initialization
    protected override void Start()
    {
        MainGameEventManager.OnGameEnd += DestructionRoutine;
        iTween.Init(gameObject);
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 9;
    }

    //Setup for the Ticket Piece, called be tutorial box
    public void Setup(Sprite ticketSprite, GameObject _parent)
    {
        Destroy(GetComponent<Animator>());
        tutorialBox = _parent.GetComponent<TutorialBoxMain>();
        gameObject.GetComponent<SpriteRenderer>().sprite = ticketSprite;
        this.parent = _parent;
        SetPosition();
    }

    //set the position 
    protected override void SetPosition()
    {
        transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y + TICKET_Y_OFFSET, parent.transform.position.z);
        originalY = this.transform.position.y;
        transform.localScale = Vector3.one * 0.25f;
        StartCoroutine(Move());
    }

    //Destroy this object
    protected override void DestructionRoutine()
    {
        GameObject.Destroy(this.gameObject);
    }

    //Coroutine that moves the object
    protected override IEnumerator Move()
    {
        gameObject.transform.localScale = Vector3.zero;

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(OpenableBox.BOX_OPEN_TIME));

        //float zDestination = Camera.main.transform.position.z + 1;

        iTween.MoveTo(gameObject, iTween.Hash("y", gameObject.transform.position.y + TICKET_MOVEMENT_IN_Y,
               // "z", zDestination,
                "time", ITEM_MOVEMENT_TIME,
                "ignoretimescale", true));

        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * 0.5f, "time", ITEM_MOVEMENT_TIME, "ignoretimescale", true));
        

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(ITEM_MOVEMENT_TIME));

        MainGameTutorial.Instance.ContinueTutorial(RunReverseAnimation);

        

        /*
        iTween.MoveTo(gameObject,
            iTween.Hash("position", ticketUIPos,
                "time", TIME_TO_GET_TO_UI,
                "ignoretimescale", true));


        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(TIME_TO_GET_TO_UI));

        Destroy(this.gameObject);

        yield break;*/
    }

    //Function sent as a call back to Main Game Tutorial 
    //This tells the ticket to run the reverse of the function above
    private void RunReverseAnimation()
    {
        StartCoroutine(ReverseAnimation());
    }


    //Reverses the tickets animation 
    private IEnumerator ReverseAnimation()
    {
        //float destZ = transform.position.z - 1;

        iTween.MoveTo(gameObject, iTween.Hash("y", originalY,
              //"z", destZ,
                "time", ITEM_MOVEMENT_TIME,
                "ignoretimescale", true));

        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.zero, "time", ITEM_MOVEMENT_TIME, "ignoretimescale", true));


        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(ITEM_MOVEMENT_TIME));

        tutorialBox.RunReverseAnimation();


    }
    
}
