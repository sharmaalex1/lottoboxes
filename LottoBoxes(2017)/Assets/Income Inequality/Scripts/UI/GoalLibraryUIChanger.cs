using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GoalLibraryUIChanger : MonoBehaviour
{

    public int goalIdentifier;
    GameObject goalManager;
    public GameObject goalTitle;
    public GameObject goalDescription;
    public GameObject icon;
    public GameObject background;

    bool bypassCompletionReqs = false; //Dev bypass to make all goals appear completed;

    GameObject thisIcon;

    // Use this for initialization
    void Start()
    {
        //bypassCompletionReqs = true; //!!!Remember to comment out line when done!!! Also will throw errors when updating the UI.
        //thisIcon = icon.GetComponent<Image>();
        goalManager = GameObject.Find("GoalManager");

        UpdateUI();

    }
		
    public void UpdateUI()
    {
        if (goalManager.GetComponent<GoalLibrary>().GetCompletionStatus(goalIdentifier) || bypassCompletionReqs)
        {
            goalDescription.GetComponent<Text>().text = goalManager.GetComponent<GoalLibrary>().GetDescription(goalIdentifier);
            goalTitle.GetComponent<Text>().text = "\"" + goalManager.GetComponent<GoalLibrary>().GetTitle(goalIdentifier) + "\"";
            background.GetComponent<Image>().color = Color.white;
            goalTitle.GetComponent<Text>().color = new Color(.5625f, 1, .6f);
            goalDescription.GetComponent<Text>().color = Color.white;
            icon.GetComponent<Image>().sprite = goalManager.GetComponent<GoalLibrary>().GetIcon(goalIdentifier).GetComponent<Image>().sprite;
            icon.GetComponent<Image> ().color = Color.white;
        }
        else
        {
            goalDescription.GetComponent<Text>().text = "Not yet unlocked...";
            goalTitle.GetComponent<Text>().text = "Locked!";
            thisIcon = goalManager.GetComponent<GoalLibrary>().defaultIcon;
            Debug.Log("Setting up default icon");
            icon.GetComponent<Image>().color = Color.gray;
            background.GetComponent<Image>().color = Color.gray;
            goalTitle.GetComponent<Text>().color = Color.gray;
            goalDescription.GetComponent<Text>().color = Color.gray;
        }
    }
}