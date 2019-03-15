using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*This class takes care of 
 * the MIni Game tutorial 
 */ 
public class MiniGameTutorial : MonoBehaviour {

    #region Singleton
    private static MiniGameTutorial instance;
    public static MiniGameTutorial Instance
    {
        get
        {
            return instance;
        }
        private set
        {

        }
    }

    private MiniGameTutorial()
    {

    }
    #endregion

    //a function for other script to subscribe to
    //is then called when the tutorial ends
    public static event System.Action StartMiniGame;

    public Canvas tutorialCanvas;

    private void Awake()
    {
        //Destroy this object if we already completed the Mini Game Tutorial
        if (SaveManager.Instance.CompletedMiniTutorial)
        {
            GameObject.Destroy(this.gameObject);
        }

        if (instance == null)
        {
            instance = this;
            //Subscribe to on slide for when we need to show the window
            UISlider.OnSlide += ShowWindow;

            this.gameObject.GetComponent<Image>().enabled = false;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    //show the window if this is the mini game
    private void ShowWindow(int level)
    {
        if(level == (int)MainSceneUIElements.MiniGame)
        {
            tutorialCanvas.gameObject.SetActive(true);
            this.gameObject.GetComponent<Image>().enabled = true;
        }
    }

    //Function called by an event trigger that ends the tutorial
    public void EndTutorial()
    {
        SaveManager.Instance.CompletedMiniTutorial = true;
        tutorialCanvas.gameObject.SetActive(false);
        if(StartMiniGame != null)
        {
            StartMiniGame();
        }
        GameObject.Destroy(this.gameObject);
    }
    //when this object is destroyed, make sure to unsubscribed from the event
    private void OnDestroy()
    {
        UISlider.OnSlide -= ShowWindow;
    }
}

