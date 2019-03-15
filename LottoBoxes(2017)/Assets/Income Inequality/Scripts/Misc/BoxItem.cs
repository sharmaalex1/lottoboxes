using UnityEngine;
using System.Collections;

public class BoxItem : Item
{
    private Vector3 boxOverviewUIPos;
    private GameObject boxesReceivedText;
    private int boxesReceived;


    #region Unity Callbacks

    protected override void Start()
    {
        base.Start();
        boxOverviewUIPos = GameObject.FindGameObjectWithTag("BoxOwnedUI").GetComponent<RectTransform>().position;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    #endregion

    protected override IEnumerator Move()
    {
        
        MainGameEventManager.TriggerBoxFoundEvent(boxesReceived);
        

        // Move the item a bit above the box it came from.
        const float Y_OFFSET = 1.0f;
        float zDestination = Camera.main.transform.position.z + 1;
        iTween.MoveTo(gameObject, iTween.Hash("y", gameObject.transform.position.y + Y_OFFSET, 
                "z", zDestination, 
                "time", ITEM_MOVEMENT_TIME));

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(ITEM_MOVEMENT_TIME));

        yield return StartCoroutine(ShowTextCoroutine());

        // Move the box to its respective UI representative and shrink it.
        iTween.MoveTo(gameObject, iTween.Hash("position", boxOverviewUIPos, 
                "time", TIME_TO_GET_TO_UI, 
                "ignoretimescale", true));
		
        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.zero, 
                "time", TIME_TO_GET_TO_UI, 
                "ignoretimescale", true));

        yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(TIME_TO_GET_TO_UI));


        if (MainGameTutorial.Instance != null)
        {
            MainGameTutorial.Instance.ContinueTutorial();
        }
        Destroy(this.gameObject);
    }

    public void Setup(Sprite wantedSprite, GameObject parent, GameObject boxesReceivedText, int boxesReceived)
    {
        Destroy(GetComponent<Animator>());
        gameObject.GetComponent<SpriteRenderer>().sprite = wantedSprite;
        this.parent = parent;
        this.boxesReceivedText = boxesReceivedText;
        this.boxesReceived = boxesReceived;
        SetPosition();
    }

    protected override void SetPosition()
    {
        base.SetPosition();
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
        StartCoroutine(Move());
    }

    protected override void DestructionRoutine()
    {
        Destroy(this.gameObject);
    }


    private IEnumerator ShowTextCoroutine()
    {
        GameObject textObject = Instantiate(boxesReceivedText, transform.position, Quaternion.identity) as GameObject;
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
        mesh.sortingOrder = SaveManager.Instance.CompletedMainTutorial ? 6 : 4;


        text.text = "+ " + boxesReceived.ToString();

        textObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        textObject.transform.SetParent(this.transform);
        textObject.transform.localScale = Vector3.zero;
        

        float yOffset = 0.75f;

        iTween.MoveTo(textObject, iTween.Hash("y", transform.position.y + yOffset, "time", StaticVars.TIME_MOVE_NUMBER, "ignoretimescale", true));
        iTween.ScaleTo(textObject, iTween.Hash("scale", Vector3.one * StaticVars.TEXT_SCALE, "time", StaticVars.TIME_MOVE_NUMBER, "ignoretimescale", true));

        yield return new WaitForSeconds(StaticVars.TIME_MOVE_NUMBER + StaticVars.TIME_SHOW_ITEM);

        yield break;

    }
}
