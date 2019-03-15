using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConveyorBeltScrolling : MonoBehaviour
{
    private Renderer rend;
    private RawImage img;
    private float offsetY;

    void Awake()
    {
//		rend = GetComponent<Renderer>();
//		rend.sortingLayerName = "UI";
//		rend.sortingOrder = 0;
        img = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        offsetY = Mathf.Repeat(offsetY + (MiniGameDifficultyManager.Instance.CurrentSpeed / 10) * Time.deltaTime, 1);
        img.uvRect = new Rect(0, -offsetY, 1.0f, 1.0f);
//		img.material.SetTextureOffset("_MainTex", new Vector2(0, offsetY));
//		rend.material.SetTextureOffset("_MainTex", new Vector2(0, offsetY));
    }
}
