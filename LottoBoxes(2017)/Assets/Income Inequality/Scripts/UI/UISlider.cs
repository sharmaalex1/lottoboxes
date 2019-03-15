using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// To set this up for unlimited canvases have each canvas after the first one be initialized.
/// Run a for loop (Going until N canvases) and have their positions be set similar to the newXPos defined within it.
/// In the loop set the multiply value (1 in the case currently) to increase by one each iteration.
/// Ideally you would have the main page as the first element in the array. 
/// This one will be your reference for the remaining position setup etc...
/// This was created because setting up a horizontal group that takes into account resolutions wasn't working.
/// The other issue was having the canvases that you wanted to be in the group offscreen messed up the scaling badly.
/// 
/// To properly maintain this class and its functionality, please maintain the enum at the bottom of this script.
/// Whenever and element is added to either side, ensure that an enum value is setup for it
/// and that they are consisten in increments of 1/-1
/// </summary>
public class UISlider : MonoBehaviour
{
    private const int MAX_MULTIPLE_INCREMENT = 1;

    public static event System.Action<int> OnSlide;

    // This is intended to be the rect transform of an object with a scroll rect.
    // The scroll rect should take up tyhe entirety of the canvas.
    public RectTransform canvasGuide;

    [Header("Canvas Layout Organization")]
    [Tooltip("All other elements are positioned relative to center element.")]
    public Canvas mainCanvas;
    public GameObject centerElement;
    public GameObject[] rightSideElements;
    public GameObject[] leftSideElements;

    private MainSceneUIElements currentElement;

    public MainSceneUIElements CurrentElement
    {
        get
        {
            return currentElement;
        }
        private set
        {
            currentElement = value;
        }
    }

    private void Start()
    {
        OnSlide += Slide;

        AlignCanvases();
        centerElement.SetActive(true);

        if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
            AudioManager.Instance.PlayAudioClip(BGMType.HighClass);
        else
            AudioManager.Instance.PlayAudioClip(BGMType.LowClass);
    }

    private void OnDestroy()
    {
        OnSlide -= Slide;
    }

    private void AlignCanvases()
    {			
        int positionMultiple = MAX_MULTIPLE_INCREMENT;
        int toGoTo = rightSideElements.Length > 0 ? rightSideElements.Length : leftSideElements.Length;
        bool doneWithRight = false;
        centerElement.GetComponent<UIToggle>().idNum = 0;
        for (int i = 0; i < toGoTo;)
        {
            float newXPos = mainCanvas.GetComponent<RectTransform>().rect.width * positionMultiple;
            if (rightSideElements.Length > 0 && !doneWithRight)
            {
                rightSideElements[i].SetActive(true);

                rightSideElements[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(newXPos, centerElement.GetComponent<RectTransform>().anchoredPosition.y);
                // Starts at an ID number of 1.
                rightSideElements[i].GetComponent<UIToggle>().idNum = i + 1;
                i++;
                positionMultiple += MAX_MULTIPLE_INCREMENT;

                if (i >= toGoTo)
                {
                    doneWithRight = true;
                    i = 0;
                    positionMultiple = MAX_MULTIPLE_INCREMENT;
                    toGoTo = leftSideElements.Length;				
                }
                continue;
            }

            if (leftSideElements.Length > 0)
            {
                leftSideElements[i].SetActive(true);
                leftSideElements[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-newXPos, centerElement.GetComponent<RectTransform>().anchoredPosition.y);
                // Starts at an ID number of 1.
                leftSideElements[i].GetComponent<UIToggle>().idNum = -i - 1;
                i++;
                positionMultiple += MAX_MULTIPLE_INCREMENT;	
            }
        }
    }

    public void TriggerSlideEvent(int indexToMoveTo)
    {
        if (System.Enum.GetValues(typeof(MainSceneUIElements)).GetValue(indexToMoveTo) != null)
        {
            currentElement = (MainSceneUIElements)indexToMoveTo;   
        }

        // If we are sliding and the current element changes to something that isn't the main game
        // push the onscreen boxes back into the box count.
        if (currentElement != MainSceneUIElements.MainGame)
        {
            SaveManager.Instance.CurrentBoxCount += MainGameSpawner.instance.NumOnScreenBoxes;
        }

        if (OnSlide != null)
        {
            OnSlide((int)currentElement);
        }
    }

    private void Slide(int indexToMoveTo)
    {
        float newXPos = (mainCanvas.GetComponent<RectTransform>().rect.width - centerElement.GetComponent<RectTransform>().anchoredPosition.x) * indexToMoveTo;
        canvasGuide.GetComponent<RectTransform>().anchoredPosition = new Vector2(-newXPos, centerElement.GetComponent<RectTransform>().anchoredPosition.y);
        
        if(indexToMoveTo == (int)MainSceneUIElements.MainGame)
        {
            if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
            {
                AudioManager.Instance.PlayAudioClip(BGMType.HighClass);
                AudioManager.Instance.PlayAudioClip(AMBType.HighClassAmbience);
            }
            else
            {
                AudioManager.Instance.PlayAudioClip(BGMType.LowClass);
                AudioManager.Instance.PlayAudioClip(AMBType.LowClassAmbience);
            }
        }
        else if (indexToMoveTo == (int)MainSceneUIElements.MiniGame)
        {
            AudioManager.Instance.PlayAudioClip(BGMType.Working);
            AudioManager.Instance.PlayAudioClip(AMBType.ConveyerBelt);
        }
    }
}

public enum MainSceneUIElements
{
    MainGame = 0,
    MiniGame = 1
}
