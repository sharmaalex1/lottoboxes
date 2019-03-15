using UnityEngine;
using UnityEngine.EventSystems;

public class ClassRouletteUI : MonoBehaviour, IPointerDownHandler
{
    public static event System.Action OnScreenTapped;
    public static event System.Action OnDialSpun;

    private bool hasDialSpun = false;

    public GameObject tapableObj;
    [Header("Each Classes respective UI icon in order of the Classes enum")]
    public GameObject[] unlockableClassUIElements;

    #region Unity Callbacks

    private void Start()
    {
        DialSpinner.OnDialSpinComplete += LoadNextScene;
    }

    private void OnDestroy()
    {
        DialSpinner.OnDialSpinComplete -= LoadNextScene;
    }

    #endregion

    #region IPointerDownHandler implementation

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!hasDialSpun)
        {
            tapableObj.SetActive(false);
            TriggerOnDialSpunEvent();
            hasDialSpun = true;
        }
    }

    #endregion

    //TODO
    // The AnimateClass method was never setup to animate the icon at the bottom of the class roulette scene
    // upon the dial stopping.

    //    /// <summary>
    //    /// Used to wrap a coroutine into a regular function call for event subscription.
    //    /// </summary>
    //    private void AnimateClassIconWrapper()
    //    {
    //        StartCoroutine(AnimateClassIcon());
    //    }
    //
    //
    //    /// <summary>
    //    /// This method is intended to animate the class icon at the bottom of the screen
    //    /// during the class roulette, when it lands on a class.
    //    /// That part is not complete yet.
    //    /// </summary>
    //    private IEnumerator AnimateClassIcon()
    //    {
    //
    //        yield break;
    //    }

    private void LoadNextScene()
    {
        if (SaveManager.Instance.UnlockedClasses.Contains(Classes.Rich) && SaveManager.Instance.UnlockedClasses.Contains(Classes.Poor))
        {
            // If this is the second time reaching the dial scene, both classes will technically be unlocked.
            // So checking for that will allow for an immediate transition to the poor class if it is unlocked.
            SceneTransition.Instance.TriggerClassSelectedEvent(Classes.Poor);
            SceneTransition.Instance.TriggerSceneChangeEvent(Scenes.MainMiniCombo); 
        }
        else
        {
            // If it is the player's first time in this scene, they will be directed to the class selection UI afterwards.
            SceneTransition.Instance.TriggerSceneChangeEvent(Scenes.ClassSelection);   
        }
    }

    #region Event Triggers

    private static void TriggerOnScreenTappedEvent()
    {
        if (OnScreenTapped != null)
        {
            OnScreenTapped();
        }
    }

    private static void TriggerOnDialSpunEvent()
    {
        if (OnDialSpun != null)
        {
            OnDialSpun();
        }
    }

    #endregion
}
