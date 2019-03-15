using UnityEngine;
using System.Collections;

public class MarketButtonHelper : MonoBehaviour
{
	public GameObject marketUI;
	public GameObject richWarning;

    private void OnEnable()
    {
        if(SceneTransition.Instance.SelectedClass == Classes.Rich)
        {
            this.gameObject.SetActive(false);
        }


        Debug.Log(SaveManager.Instance.CompletedFurnitureTutorial + " " + SceneTransition.Instance.SelectedClass);
        if(SaveManager.Instance.CompletedFurnitureTutorial && SceneTransition.Instance.SelectedClass == Classes.Poor)
        {
            this.gameObject.SetActive(true);
        }
    }

	public void onButtonPress()
	{
		int currentClass = (int)StatisticsManager.Instance.CurrentClass;
		if (currentClass == 1)
        {
            marketUI.SetActive(true);
        }
	}
}
