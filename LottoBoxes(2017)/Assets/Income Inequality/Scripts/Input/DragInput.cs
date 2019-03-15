using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DragInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public LayerMask draggableLayers;

    public static float dragDamping = 7f;

    public static bool snapToCenter = true;
    public static float snapSpeed = 3.0f;

    private const float MAX_DRAG_SPEED = 20;

    public GameObject dragPrefab;

    private Rigidbody2D rigid2D;
    private HingeJoint2D joint2D;

    private Vector3 localPickOffset;

    private bool isDragging;

    public bool IsDragging
    {
        get { return isDragging; }
    }

    private PointerEventData prevEventData;

    void Start()
    {
        if (dragDamping == 0) // Should never equal zero (defaults to 1 if that happens)
        {
            dragDamping = 1.0f;
        }

        rigid2D = transform.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDragging)
        {
            // Since there is no GetKey equivalent of OnPointer (i.e. called every frame the touch / mouse is down)
            // We store the previous in the last OnDrag (time the touch / mouse moved) and keep calling OnDrag with it.
            OnDrag(prevEventData);
        }
    }

    #region IPointerDownHandler implementation

    public void OnPointerDown(PointerEventData eventData)
    {
        AudioManager.Instance.PlayAudioClip(SFXType.PlayerGrabsBox);
        isDragging = true;
        prevEventData = eventData;

        //Get the current offset
        localPickOffset = transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(eventData.position));

        // Hinge joint for rotation
        GameObject jointTrans = Instantiate(dragPrefab, Camera.main.ScreenToWorldPoint(eventData.position), transform.rotation) as GameObject;
        joint2D = jointTrans.GetComponent<HingeJoint2D>();

        //Populate the hinge joint
        joint2D.anchor = Vector2.zero;
        joint2D.connectedAnchor = localPickOffset;
        joint2D.connectedBody = rigid2D;

        MiniGameEventManager.TriggerOnBoxDrag(gameObject);
    }

    #endregion

    #region IDragHandler implementation

    public void OnDrag(PointerEventData eventData)
    {
//        Debug.Log("Ondrag" + Time.frameCount);
        if (rigid2D != null && joint2D != null)
        {
            if (snapToCenter)
            {
                joint2D.connectedAnchor = Vector2.Lerp(joint2D.connectedAnchor, Vector2.zero, snapSpeed * Time.deltaTime);
            }

            Vector3 objectCoords = Camera.main.WorldToScreenPoint(joint2D.transform.position);
            float distance = Vector2.Distance(objectCoords, eventData.position);
            Vector2 vector = ((Vector3)eventData.position - objectCoords).normalized * (distance / dragDamping);

            // Restrict max speed
            if (vector.magnitude > MAX_DRAG_SPEED)
            {
                vector = vector.normalized * MAX_DRAG_SPEED;
            }

            joint2D.GetComponent<Rigidbody2D>().velocity = vector;

            // Store this movement as previous, so that update can take over (only an issue if the touch is not moving)
            prevEventData = eventData;
        }
    }

    #endregion

    #region IPointerUpHandler implementation

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        DropDraggedItem(gameObject);
        MiniGameEventManager.TriggerOnDragStop(gameObject);
    }

    #endregion

    private void DropDraggedItem(GameObject obj)
    {
        if (obj == gameObject && joint2D != null)
        {
            //gameObject.GetComponent<SpriteRenderer>().sortingOrder -= 10;
            Destroy(joint2D.gameObject);
        }
    }
}