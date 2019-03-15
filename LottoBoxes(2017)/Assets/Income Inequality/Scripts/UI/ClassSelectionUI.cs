using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ClassSelectionUI : MonoBehaviour
{
    //    private void Update()
    //    {
    //        if (Input.GetKeyDown(KeyCode.Alpha1))
    //        {
    //            SaveManager.Instance.UnlockedClasses.Add(Classes.Rich);
    //            for (int i = 0; i < SaveManager.Instance.UnlockedClasses.Count; i++)
    //            {
    //                Debug.Log(SaveManager.Instance.UnlockedClasses[i]);
    //            }
    //        }
    //
    //        if (Input.GetKeyDown(KeyCode.Alpha2))
    //        {
    //            SaveManager.Instance.UnlockedClasses.Add(Classes.Poor);
    //            for (int i = 0; i < SaveManager.Instance.UnlockedClasses.Count; i++)
    //            {
    //                Debug.Log(SaveManager.Instance.UnlockedClasses[i]);
    //            }
    //        }
    //    }
    //    private List<Button> allClassButtons;
    //
    //    public Button richClassButton;
    //    public Button poorClassButton;
    //    public GameObject poorClassLockedUI;
    //
    //    private void Start()
    //    {
    //        allClassButtons = new List<Button>(System.Enum.GetValues(typeof(Classes)).Length);
    //        allClassButtons.Add(richClassButton);
    //        allClassButtons.Add(poorClassButton);
    //
    //        AddListenerToButton(richClassButton, Classes.Rich);
    //        CheckAvailabilityOfClasses();
    //    }
    //
    //    private void CheckAvailabilityOfClasses()
    //    {
    //        for (int i = 0; i < System.Enum.GetValues(typeof(Classes)).Length; i++)
    //        {
    //            if (!SaveManager.Instance.UnlockedClasses.Contains((Classes)i))
    //            {
    //                allClassButtons[i].gameObject.SetActive(true);
    //                AddListenerToButton(allClassButtons[i], (Classes)i);
    //
    //                continue;
    //            }
    //        }
    //        if (!SaveManager.Instance.UnlockedClasses.Contains(Classes.Poor))
    //        {
    //            poorClassLockedUI.SetActive(true);
    //        }
    //        else
    //        {
    //            poorClassLockedUI.SetActive(false);
    //            AddListenerToButton(poorClassButton, Classes.Poor);
    //        }
    //    }
    //
    //    private void AddListenerToButton(Button classButton, Classes chosenClass)
    //    {
    //        classButton.onClick.AddListener(() =>
    //            {
    //                SceneTransition.Instance.TriggerClassSelectedEvent(chosenClass);
    //                SceneTransition.Instance.TriggerSceneChangeEvent((int)Scenes.MainMiniCombo);
    //            });
    //    }
}
