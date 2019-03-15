using UnityEngine;
using System.Collections;

public class PoofItem : Item
{

    private Animator puffController;

    #region implemented abstract members of Item

    protected override IEnumerator Move()
    {

        puffController = GetComponent<Animator>();
        if(puffController == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Couldn't find Poof Animator");
            yield break;
#endif
        }

        puffController.SetBool("Play", true);

        // Move the item a bit above the box it came from.
        float yOffset = 1f;
        float zDestination = Camera.main.transform.position.z + 1;
        iTween.MoveTo(
            gameObject, 
            iTween.Hash(
                "y", gameObject.transform.position.y + yOffset, 
                "z", zDestination, 
                "time", ITEM_MOVEMENT_TIME));

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(ITEM_MOVEMENT_TIME + StaticVars.TIME_SHOW_ITEM));


        //yield return new WaitForSeconds(0.45f);

        GameObject.Destroy(this.gameObject);
    }

    protected override void DestructionRoutine()
    {
        GameObject.Destroy(this.gameObject);
    }

    #endregion

    protected override void SetPosition()
    {
        //base.SetPosition();
        float yOffset, zOffset;
        zOffset = 0.5f;
        yOffset = 1f;

        transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y + yOffset, parent.transform.position.z - zOffset);
        transform.localScale = Vector3.one * 0.25f;
        StartCoroutine(Move());
    }


    public void Setup(Sprite _wantedSprite, GameObject _parent)
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = _wantedSprite;
        this.parent = _parent;
        SetPosition();
    }
}
