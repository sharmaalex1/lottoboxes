using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour 
{
	
	#region Singleton
	private static IntroManager instance;
	public Button sceneChangeButton;
	private bool firstTry = true;


	public static IntroManager Instance
	{
		get
		{
			return instance;
		}
		private set
		{

		}
	}

	private IntroManager()
	{

	}
    #endregion

    //a function for other script to subscribe to
    //is then called when the window is closed
    public static event System.Action StartIntroManager;

    public Canvas IntroductionCanvas;

	private void Awake()
	{
		//Destroy this object if we already completed the start scene
		if (SaveManager.Instance.CompletedStart)
		{
			GameObject.Destroy(this.gameObject);
		}

		if (instance == null)
		{
			instance = this;
			//Subscribe to on slide for when we need to show the window
			UISlider.OnSlide += ShowWindow;
		}
		else
		{
			GameObject.Destroy(this.gameObject);
		}
	}

	//show the window if this is the first playthrough
	private void ShowWindow(int level)
	{
		if (firstTry) 
		{
			firstTry = false;
			IntroductionCanvas.gameObject.SetActive (true);
		} 

		else 
		{
			Scenes sceneToGoTo = SaveManager.Instance.UnlockedClasses.Contains(Classes.Rich) ? Scenes.ClassSelection : Scenes.ClassRoulette;

			sceneChangeButton.onClick.AddListener(() =>
				{
					ButtonClick();
					SceneTransition.Instance.TriggerSceneChangeEvent(sceneToGoTo); 
					sceneChangeButton.enabled = false;
				});

			AudioManager.Instance.PlayAudioClip(BGMType.Working);
		}
	}

    //when this object is destroyed, make sure to unsubscribed from the event
    private void OnDestroy()
	{
		UISlider.OnSlide -= ShowWindow;
	}

	public void ButtonClick()
	{
		AudioManager.Instance.PlayAudioClip(SFXType.UIInteraction);
	}
}
	


