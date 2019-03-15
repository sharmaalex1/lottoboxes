using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    public Button sceneChangeButton;
    public GameObject introPara;
    public GameObject comicHolder;
	public Button introButton;

    private void Start()
    {
        Scenes sceneToGoTo = SaveManager.Instance.UnlockedClasses.Contains(Classes.Rich) ? Scenes.ClassSelection : Scenes.ClassRoulette;

        sceneChangeButton.onClick.AddListener(() =>
            {
                ButtonClick();
                if (!SaveManager.Instance.UnlockedClasses.Contains(Classes.Rich))
                {
                    comicHolder.SetActive(true);
                    introPara.SetActive(true);
                }                                       
                else
                {
                    SceneTransition.Instance.TriggerSceneChangeEvent(sceneToGoTo);
                }
                sceneChangeButton.enabled = false;
            });
				

		introButton.onClick.AddListener(() =>
			{
			ButtonClick();
			SceneTransition.Instance.TriggerSceneChangeEvent(sceneToGoTo); 
			introButton.enabled = false;
			});

		AudioManager.Instance.PlayAudioClip(BGMType.Working);
	}

    public void ButtonClick()
    {
        AudioManager.Instance.PlayAudioClip(SFXType.UIInteraction);
    }
}
