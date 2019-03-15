
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//Screen Shake
//Phone Vibration - done
//Red Vignette effect - done
//Boxes fly off using upward arc
public class MiniGameBoxExplosion : MonoBehaviour {

    [Header("An Object Reference To the Vignette that should be in the scene AND Turned Off")]
    public Image vignetteObject;

    public AnimationClip vignetteAnimation;

    private List<Box> visibleBoxes;

    private Coroutine currentCoroutine;
    private bool isVignetteRunning;

    #region Singleton
    private static MiniGameBoxExplosion instance;
    public static MiniGameBoxExplosion Instance
    {
        get
        {
            return instance;
        }

        private set
        {

        }
    }

    private MiniGameBoxExplosion() { }
    #endregion 

    // Use this for initialization
    void Start() {
        if (instance == null)
        {
            instance = this;
            isVignetteRunning = false;
            SubscribeToEvents();
            visibleBoxes = new List<Box>();
            vignetteObject.gameObject.SetActive(false);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    private void TriggerMissVisualizations()
    { 

        if(!isVignetteRunning)
        {
            //Handheld.Vibrate();
            isVignetteRunning = true;
            currentCoroutine = StartCoroutine(TriggerVignette());
        }
       
        else
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = StartCoroutine(TriggerVignette());
        }

    }

    private IEnumerator TriggerVignette()
    {
        vignetteObject.gameObject.SetActive(true);

        yield return new WaitForSeconds(vignetteAnimation.length);

        vignetteObject.gameObject.SetActive(false);

        isVignetteRunning = false;
    }


    private void SubscribeToEvents()
    {
        MiniGameEventManager.OnBoxMissed += TriggerMissVisualizations;
        MiniGameEventManager.OnIncorrectBoxPlacement += TriggerMissVisualizations;
    }

    private void UnsubscribeFromEvents()
    {
        MiniGameEventManager.OnBoxMissed -= TriggerMissVisualizations;
        MiniGameEventManager.OnIncorrectBoxPlacement -= TriggerMissVisualizations;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    public void AddBoxReference(Box boxToAdd)
    {
        visibleBoxes.Add(boxToAdd);
    }
}
