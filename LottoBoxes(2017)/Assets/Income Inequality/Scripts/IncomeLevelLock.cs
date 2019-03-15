using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IncomeLevelLock : MonoBehaviour
{
    // Version of the UI that has a lock on it (Blocks the actual selection button)
    public GameObject lockedUIVariant;
    public Button classSelectButton;
    // Class this button represents.
    public Classes curClassRepresentation;

    void Start()
    {
        Debug.Log(SaveManager.Instance.UnlockedClasses.Contains(curClassRepresentation));
        if (SaveManager.Instance.UnlockedClasses.Contains(curClassRepresentation))
        {
            if (lockedUIVariant != null)
            {
                lockedUIVariant.SetActive(false);   
            }

            classSelectButton.onClick.AddListener(() =>
                {
                    AudioManager.Instance.PlayAudioClip(SFXType.UIInteraction);
                    SceneTransition.Instance.TriggerClassSelectedEvent(curClassRepresentation);
                    SceneTransition.Instance.TriggerSceneChangeEvent(Scenes.MainMiniCombo); 
                });
        }
    }
}
