using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GoalReward : MonoBehaviour {

    public GameObject RewardUI;
    public GameObject RewardParticles;

    public GameObject RewardUITitle;
    public GameObject RewardUIDescription;

    public void presentReward(String rewardTitle, String rewardDescription)
    {
        RewardUITitle.GetComponent<Text>().text = rewardTitle;
        RewardUIDescription.GetComponent<Text>().text = rewardDescription;

        RewardUI.SetActive(true);
        RewardParticles.SetActive(true);
    }
	
}
