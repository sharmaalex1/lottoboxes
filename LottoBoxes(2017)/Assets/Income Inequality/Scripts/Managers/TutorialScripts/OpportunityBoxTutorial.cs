using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class OpportunityBoxTutorial : MonoBehaviour
{
    public void EndTutorial()
    {
        this.gameObject.SetActive(false);
        MainGameEventManager.TriggerOpportunityBoxTutorialEndEvent();
    }
}
