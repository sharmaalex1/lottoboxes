using UnityEngine;
using System.Collections;

public class MainGameManager : MonoBehaviour
{

    public int boxesOpenedThisSession;
    
    private void Start()
    {
        boxesOpenedThisSession = 0;

        Application.targetFrameRate = 60;

        MainGameEventManager.TriggerGameStartEvent();

        UISlider.OnSlide += TriggerGameStateEventChange;

        #if UNITY_EDITOR

        if (SaveManager.Instance.CurrentBoxCount <= 0)
        {
            Debug.Log("Spawning isn't broken! You just have no boxes!");
        }

        #endif
    }

    private void OnDisable()
    {
        UISlider.OnSlide -= TriggerGameStateEventChange;
    }

    private void TriggerGameStateEventChange(int curCanvasIndex)
    {
        if (curCanvasIndex == 0)
        {
            MainGameEventManager.TriggerGameStartEvent();
        }
        else
        {
            MainGameEventManager.TriggerGameEndEvent();
            MainGameEventManager.TriggerHyperModeEnd();
        }
    }

    public void incrementBoxCount()
    {
        boxesOpenedThisSession++;

        if(boxesOpenedThisSession == 10)
        {
            MixpanelManager.tenBoxesOpenedInSession();
            Debug.Log("10 Opened this session!");
        }

        if (boxesOpenedThisSession == 50)
        {
            MixpanelManager.fiftyBoxesOpenedInSession();
            Debug.Log("50 Opened this session!");
        }

        if (boxesOpenedThisSession == 100)
        {
            MixpanelManager.hundredBoxesOpenedInSession();
            Debug.Log("100 Opened this session!");
        }

        if (boxesOpenedThisSession == 250)
        {
            MixpanelManager.twoFiftyBoxesOpenedInSession();
            Debug.Log("250 Opened this session!");
        }
    }


}
