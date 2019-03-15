using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(Image))]
/// <summary>
/// This class manages transitioning between scenes as well as created a smooth black fade in/out during the transition.
/// Screen Space - Overlay is the best fit for this as it is intended to cover everything in a scene.
/// </summary>
public class SceneTransition : MonoBehaviour, IPointerDownHandler
{
    
    private static event System.Action<Scenes> OnSceneChange;
    private static event System.Action<Classes> OnClassSelected;

    private static SceneTransition instance;

    public static SceneTransition Instance
    {
        get
        {
            return instance;
        }
    }

    private Classes selectedClass;

    public Classes SelectedClass
    {
        get
        {
            return selectedClass;
        }
        private set
        {
            selectedClass = value;
        }
    }

    private Image img;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }


    }

    private void Start()
    {
        OnSceneChange += SetupNewLevelWrapper;
        OnClassSelected += DetermineClassToSetup;

        img = this.GetComponent<Image>();
        img.enabled = false;
        img.color = Color.clear;
    }

    private void OnDestroy()
    {
        OnSceneChange -= SetupNewLevelWrapper;
        OnClassSelected -= DetermineClassToSetup;
    }

    /// <summary>
    /// Wraps the SetupNewLevel method in a function call so that it can subscribe to an event.
    /// </summary>
    private void SetupNewLevelWrapper(Scenes sceneToLoad)
    {
        StartCoroutine(SetupNewLevel(sceneToLoad));
    }

    /// <summary>
    /// Function that fades the screen to black, asynchronously loads the next level, 
    /// then fades the screen to clear again.
    /// The "Scenes" enum can be used for loading, and exists for readability.
    /// Make sure to cast the enum value as an int e.g. (int)Scenes.MainMiniCombo
    /// </summary>
    private IEnumerator SetupNewLevel(Scenes sceneToLoad)
    {
        img.enabled = true;

        // Fade in the black overlay.
        while (img.color.a < 1f)
        {
            img.color = Vector4.MoveTowards(img.color, Color.black, 1 * Time.deltaTime);

            yield return null;
        }

        // Load the expected scene
        SceneManager.LoadScene(System.Enum.GetNames(typeof(Scenes)).GetValue((int)sceneToLoad).ToString());

        yield return new WaitForSeconds(1);

        // Fade out the black overlay to reveal the new scene.
        while (img.color.a > 0)
        {
            img.color = Vector4.MoveTowards(img.color, Color.clear, 1 * Time.deltaTime);

            yield return null;
        }

        img.color = Color.clear;
        img.enabled = false;

        yield break;
    }

    private void DetermineClassToSetup(Classes classesToSetup)
    {
        SelectedClass = classesToSetup;
    }

    #region IPointerClickHandler implementation

    public void OnPointerDown(PointerEventData eventData)
    {        
        return;
    }

    #endregion

    public void TriggerSceneChangeEvent(Scenes sceneToLoad)
    {   
        if (OnSceneChange != null)
        {
			Debug.Log("TRYING TO LOAD:" + sceneToLoad.ToString());
            
            if(SceneManager.GetActiveScene().name.ToString() == "MainMiniCombo")
            {

                // Resetting insurance and disaster system when you move between scenes because it currently carries over between income levels

                // potentially could add seperate timers later.

                Debug.Log("Resetting Disaster and Insurance Data");

                StatisticsManager.Instance.UpdateDisasterInfo(new DateTime(), new DateTime(), false, true, false, true, true, true, new DateTime());

                SettingsManager.Instance.SetFirstOfferEver(true);

            }
            
            OnSceneChange(sceneToLoad);
        }
    }

    public void TriggerClassSelectedEvent(Classes selectedClass)
    {        
        if (OnClassSelected != null)
        {
            OnClassSelected(selectedClass);
            MixpanelManager.ClassChosen(selectedClass);
        }
    }
}
