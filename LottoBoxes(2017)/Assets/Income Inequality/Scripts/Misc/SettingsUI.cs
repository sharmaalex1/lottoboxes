using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsUI : MonoBehaviour
{
    public Button changeClassButton;
	public Slider musicSlider;
	public Slider sfxSlider;

    public GameObject disasterCheckmark;
    public GameObject expensesCheckmark;

    public GameObject disasterManagerRef;
    public GameObject expensesManagerRef;

    // Use this for initialization
    void OnEnable()
    {
        changeClassButton.onClick.AddListener(() => SceneTransition.Instance.TriggerSceneChangeEvent(Scenes.ClassSelection));

        if (SettingsManager.Instance.GetDisasterToggle())
        {
            disasterCheckmark.SetActive(true);
        }
        else
        {
            disasterCheckmark.SetActive(false);
        }

        if (SettingsManager.Instance.GetExpensesToggle())
        {
            expensesCheckmark.SetActive(true);
        }
        else
        {
            expensesCheckmark.SetActive(false);
        }

        musicSlider.value = AudioManager.Instance.GetBGMVolume();
		sfxSlider.value = AudioManager.Instance.GetSFXVolume();
	}

    public void PlayButtonSound()
    {
        AudioManager.Instance.PlayAudioClip(SFXType.UIInteraction);
    }

	public void OnChangingMusicVolume()
	{
		AudioManager.Instance.SetBGMVolume(musicSlider.value);
		AudioManager.Instance.SetAMBVolume(musicSlider.value);
	}

	public void OnChangingSFXVolume()
	{
		AudioManager.Instance.SetSFXVolume(sfxSlider.value);
	}

    public void DisasterToggler()
    {
        if(SettingsManager.Instance.GetDisasterToggle())
        {
            disasterCheckmark.SetActive(false);
            SettingsManager.Instance.SetDisasterToggle(false);
            disasterManagerRef.SetActive(false);
            Debug.Log("Disaster Toggle: " + SettingsManager.Instance.GetDisasterToggle());
        }
        else
        {
            disasterCheckmark.SetActive(true);
            disasterManagerRef.SetActive(true);
            SettingsManager.Instance.SetDisasterToggle(true);
            Debug.Log("Disaster Toggle: " + SettingsManager.Instance.GetDisasterToggle());
        }
    }

    public void ExpensesToggler()
    {
        if (SettingsManager.Instance.GetExpensesToggle())
        {
            expensesCheckmark.SetActive(false);
            SettingsManager.Instance.SetExpensesToggle(false);
            expensesManagerRef.SetActive(false);
            Debug.Log("Expenses Toggle: " + SettingsManager.Instance.GetExpensesToggle());
        }
        else
        {
            expensesCheckmark.SetActive(true);
            expensesManagerRef.SetActive(true);
            SettingsManager.Instance.SetExpensesToggle(true);
            Debug.Log("Expenses Toggle: " + SettingsManager.Instance.GetExpensesToggle());
        }
    }
}
