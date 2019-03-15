using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Item : MonoBehaviour
{
    /// <summary>
    /// Amount of time it takes an item to make its way to the respective UI element
    /// </summary>
    public const float TIME_TO_GET_TO_UI = 1.0f;
    //variables set through constructor	protected Sprite wantedSprite;
    protected GameObject parent;

    public const float ITEM_MOVEMENT_TIME = 0.5f;


    #region Unity Callbacks

    protected virtual void Start()
    { 
        //TODO Create another means of getting the box overview position.
        MainGameEventManager.OnGameEnd += DestructionRoutine;

        iTween.Init(gameObject);
    }

    protected virtual void OnDisable()
    {
        MainGameEventManager.OnGameEnd -= DestructionRoutine;
    }

    #endregion

    #region Setup Methods


    protected virtual void SetPosition()
    {
        float yOffset, zOffset;
        yOffset = 0.5f;
        zOffset = 5f;
       
        transform.position = new Vector3(parent.transform.position.x, parent.transform.position.y + yOffset, parent.transform.position.z - zOffset);
        transform.localScale = Vector3.one * 0.25f;
    }

    #endregion

    protected abstract IEnumerator Move();

    protected abstract void DestructionRoutine();
}
