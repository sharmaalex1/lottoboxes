using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TicketPiece : Item
{
    private Vector3 ticketUIPos;
    public int id = 0;

    #region Unity Callbacks

    protected override void Start()
    {
        base.Start();
        ticketUIPos = GameObject.FindGameObjectWithTag("TicketOwnedUI").GetComponent<RectTransform>().position;
    }

    #endregion

    /// <summary>
    /// Animation routine that has the item move up out of the box, then move over to its corresponding UI element.
    /// </summary>
    protected override IEnumerator Move()
    {

        //move the ticket above the box for the player to see
        iTween.MoveTo(gameObject, iTween.Hash("y", gameObject.transform.position.y + 1f, 
                "time", ITEM_MOVEMENT_TIME, 
                "ignoretimescale", true));

        //give extra time for the player to see the ticket
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(ITEM_MOVEMENT_TIME + StaticVars.TIME_SHOW_ITEM));

        iTween.MoveTo(gameObject,
            iTween.Hash("position", Vector3.up,
                "time", TIME_TO_GET_TO_UI,
                "easetype", iTween.EaseType.easeInCirc,
                "ignoretimescale", true));

        iTween.ColorTo(gameObject, iTween.Hash("color", new Color(1, 1, 1, 0),
                                                "time", TIME_TO_GET_TO_UI,
                                                "easetype", iTween.EaseType.easeInCirc,
                                                "ignoretimescale", true));

        iTween.ScaleTo(gameObject, 
            iTween.Hash("scale", Vector3.zero,
                "time", TIME_TO_GET_TO_UI,
                "easetype", iTween.EaseType.easeInCirc,
                "ignoretimescale", true));
		
        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(TIME_TO_GET_TO_UI));

        AudioManager.Instance.PlayAudioClip(SFXType.GoldenTicketAnimation);

        Destroy(this.gameObject);

        yield break;
    }

    /// <summary>
    /// Childs the object to the main canvas.
    /// Ensures that the new item is in front of everything.
    /// Assigns its list of sprites whcih represents each ticket piece.
    /// Finishes off by seting the positino of the item.
    /// </summary>
    public void Setup(GameObject _parent, GameObject spinningBurst)
    {
        Destroy(GetComponent<Animator>());
        this.parent = _parent;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = 6;
        spinningBurst.transform.position = this.transform.position;
        spinningBurst.transform.SetParent(this.transform);
        SetPosition();
    }

    /// <summary>
    /// The assigned sprite will come from the ticketPieceSprites array, and will depend on the index passed in.
    /// </summary>
    /// <param name="enumIndex">Enum index.</param>
    public void ConfigureSprite(int pieceID)
    {
       GetComponent<SpriteRenderer>().sprite = TicketSpriteContainer.Instance.ReceiveTicketSprite(pieceID);
    }

    protected override void SetPosition()
    {
        base.SetPosition();
        StartCoroutine(Move());
    }

    protected override void DestructionRoutine()
    {
        GameObject.Destroy(this.gameObject);
    }
}
