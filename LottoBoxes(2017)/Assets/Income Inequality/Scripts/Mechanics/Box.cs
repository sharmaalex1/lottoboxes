using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum BoxCondition
{
    Damaged,
    Good
}

[RequireComponent(typeof(BoxCollider2D))]
public class Box : MonoBehaviour
{
    private const float DOWNWARD_SPEED = 3.0f;
    private const float SWIPE_SPEED = 15.0f;

    private Coroutine currentCheckRoutine;

    //these two variables are used to prevent errors in editor
    private float camOrthographicSize;
    private Vector3 camPos;
    

    private HandMotor motor;

    private BoxCondition curCondition;
    public List<Sprite> goodConditionBoxes;
    public List<Sprite> defectiveBoxes;

    private bool swiped;
    private bool isRoutineRunning = false;
    private DragInput localInput;

    private Rigidbody2D myRigidBody;
    private bool sorted = false;
    private bool sortedCorrectly;


    void Awake()
    {
        int temp = Random.Range(0, 2);
        curCondition = temp == 0 ? BoxCondition.Damaged : BoxCondition.Good;
        gameObject.tag = temp == 0 ? "Damaged" : "Good";


        swiped = false;
        AssignBoxSprite();

        myRigidBody = GetComponent<Rigidbody2D>();

        if (MiniGameDifficultyManager.Instance != null)
        {
            myRigidBody.velocity = new Vector3(0, MiniGameDifficultyManager.Instance.CurrentSpeed, 0);
        }

        camOrthographicSize = Camera.main.orthographicSize;
        camPos = Camera.main.transform.position;

    }

    void Start()
    {
        //MiniGameEventManager.OnBoxSwiped += MoveBox;
        MiniGameEventManager.OnBoxDrag += StartDrag;
        UISlider.OnSlide += DestructionRoutine;
        MiniGameEventManager.OnSpeedChange += VelocityChange;
    }

    void AssignBoxSprite()
    {
        switch (curCondition)
        {
            case BoxCondition.Good:
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = goodConditionBoxes[Random.Range(0, goodConditionBoxes.Count)];
                }
                break;
            case BoxCondition.Damaged:
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = defectiveBoxes[Random.Range(0, defectiveBoxes.Count)];
                }
                break;
            default:
                {
                    Debug.LogError("A box wasn't assigned a condition.");
                }
                break;
        }
    }


    void OnBecameInvisible()
    {
        if (isRoutineRunning)
        {
            StopCoroutine(currentCheckRoutine);
        }

        if (sorted && swiped)
        {
            if (sortedCorrectly)
            {
                MiniGameEventManager.TriggerCorrectPlacementEvent();
            }
            else
            {
                MiniGameEventManager.TriggerIncorrectPlacementEvent();
            }
        }
        else
        {
            MiniGameEventManager.TriggerOnBoxMissed();
        }

        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        //MiniGameEventManager.OnBoxSwiped -= MoveBox;
        MiniGameEventManager.OnBoxDrag -= StartDrag;
        UISlider.OnSlide -= DestructionRoutine;
        MiniGameEventManager.OnSpeedChange -= VelocityChange;
    }

    //Fuction called back for Itween.MoveTo
    private void SwipeCallBack()
    {
        //Instantiate our motorObj 
        GameObject motorObj = Instantiate(HandContainer.Instance.GetHandPair(), this.transform.position, Quaternion.identity) as GameObject;

        motor = motorObj.GetComponent<HandMotor>();
        if (motor == null)
        {
#if UNITY_EDITOR
            Debug.LogError("motorobj is missing motorscript");
#endif

            return;
        }

        sortedCorrectly = WasBoxCorrectlySorted();

        motor.Initialize(this, sortedCorrectly);
    }

    private void StartDrag(GameObject obj)
    {
        if (obj == this.gameObject && !isRoutineRunning && !swiped)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingOrder += 10;

            // Cache local DragInput reference. Necessary for checking if the object is being dragged
            localInput = gameObject.GetComponent<DragInput>();

            swiped = true;
            currentCheckRoutine = StartCoroutine(CheckDrag());
        }
    }

    private void DestructionRoutine(int nothing)
    {
        Destroy(this.gameObject);
    }

    private IEnumerator CheckDrag()
    {
        isRoutineRunning = true;
        float leftXCheck = (Camera.main.transform.position.x - StaticVars.CameraHalfWidth) + (StaticVars.GetBoxHalfDiagonal(GetComponent<BoxCollider2D>().size, transform.localScale));
        float rightXCheck = (Camera.main.transform.position.x + StaticVars.CameraHalfWidth) - (StaticVars.GetBoxHalfDiagonal(GetComponent<BoxCollider2D>().size, transform.localScale));
        while (true)
        {
            // Only allow Box to be removed if the box is outside of either bound AND the user is not dragging (not touching the box)
            if ((transform.position.x <= leftXCheck || transform.position.x >= rightXCheck) && !localInput.IsDragging)
            {
                //  Debug.Break();
                sorted = true;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder -= 10;
                GetComponent<Rigidbody2D>().isKinematic = true;
                MiniGameEventManager.TriggerOnDragStop(this.gameObject);
                isRoutineRunning = false;
                SwipeCallBack();
                break;
            }
          
            yield return null;
        }

        yield break;
    }

    private void VelocityChange(float newVelocity)
    {
        if (!swiped)
        {
            myRigidBody.velocity = new Vector3(0, newVelocity, 0);
        }
    }

    private bool WasBoxCorrectlySorted()
    {
        if (transform.position.x > camPos.x && curCondition == BoxCondition.Good)
        {
            return true;   
        }
        else if (transform.position.x < camPos.x && curCondition == BoxCondition.Damaged)
        {
            return true;
        }

        return false;
    }

    public void SlapOffScreen()
    {
 
        float yOffset = 2.0f;
        float moveTime = 0.25f;

        Vector3 pos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - camOrthographicSize - yOffset, transform.position.z);
        BoxCollider2D collider = GetComponent<BoxCollider2D>() as BoxCollider2D;
        if (collider == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Couldnt Find Box Collider on mini game box");
#endif
            return;
        }

        pos.y -= StaticVars.GetBoxHalfDiagonal(collider.size, transform.localScale);

        iTween.MoveTo(this.gameObject, iTween.Hash("position", pos, "time", moveTime, "easetype", iTween.EaseType.linear));
    }
}

