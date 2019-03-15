using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GoalLibrary : MonoBehaviour
{
    public GameObject defaultIcon;
    //public GameObject template;
    public int numberOfGoals;

    //vvv!!!REMEMBER TO CHANGE THIS IN EDITOR AS YOU ADD MORE GOALS!!!vvv
    public int[] goalIdentifiers;
    //^^^!!!REMEMBER TO CHANGE THIS IN EDITOR AS YOU ADD MORE GOALS!!!^^^

    public GameObject[] goalIcons;
    public GameObject[] libraryEntries;

    void Awake()
    {
        numberOfGoals = goalIdentifiers.GetLength(0);
        Debug.Log("The array for identifiers is: " + goalIdentifiers.GetLength(0).ToString() + " entries long.");
        Debug.Log("The number of goals is: " + numberOfGoals);
        //Array.Sort(goalIdentifiers);
        libraryEntries = new GameObject[numberOfGoals];
        fillIconsWithDefault(); //Fills unassigned icons with default box
    }

    public void entryArrayPopulation(GameObject entryToAdd, int location)
    {
        libraryEntries[location] = entryToAdd;
    }

    public void updateEntryUI()
    {
        Debug.Log("Trying to update UI...");
        for (int i = 0; i <= numberOfGoals; i++)
        {
            libraryEntries[i].GetComponent<GoalLibraryUIChanger>().UpdateUI();
            Debug.Log("Updating UI for goal: " + libraryEntries[i].GetComponent<GoalLibraryUIChanger>().goalTitle.ToString() + " with identifier: " + libraryEntries[i].GetComponent<GoalLibraryUIChanger>().goalIdentifier);
        }
        Debug.Log("UI Updated");
    }

    public GameObject GetIcon(int identifier)
    {

        int temp = 0;

        for (int i = 0; i < goalIdentifiers.Length; i++)
        {
            if (identifier == goalIdentifiers[i])
            {
                temp = i;
            }
        }

        if (identifier > 0)
        {
            Debug.Log("Changing icon of goal #" + identifier + ", which was at location " + temp + " of the goalIdentifiers array");
            return goalIcons[temp];
        }
        else
            return defaultIcon;
    }

    void fillIconsWithDefault()
    {
        for (int i = 0; i < numberOfGoals; i++)
        {
            if (goalIcons[i] == null)
                goalIcons[i] = defaultIcon;
        }
    }

    #region The Library
    public String GetTitle(int identifier)
    {
        if (identifier == 1)
            return "Box Novice";
        if (identifier == 2)
            return "Box Enthusiast";
        if (identifier == 3)
            return "Box Afficianado";
        if (identifier == 4)
            return "Box Master";
        if (identifier == 5)
            return "Box Entrepreneur";
        if (identifier == 6)
            return "Box Mogul";
        if (identifier == 7)
            return "Big Box";
        if (identifier == 8)
            return "We Can Do It!";
        if (identifier == 9)
            return "You DID Do It!";
        if (identifier == 10)
            return "Self-Made Box Opener";
        if (identifier == 11)
            return "You Were Not Prepared";
        if (identifier == 12)
            return "Clairvoyance";
        if (identifier == 13)
            return "You Were Prepared";
        if (identifier == 15)
            return "Roach Resilience";
        if (identifier == 17)
            return "Shark Bait";
        if (identifier == 18)
            return "Shark Week";
        if (identifier == 19)
            return "Chum";
        if (identifier == 20)
            return "Testing the Waters";
        if (identifier == 21)
            return "Adult Swim";
        if (identifier == 22)
            return "Castaway";
        if (identifier == 23)
            return "Where the Buffalo Roam";
        if (identifier == 24)
            return "Stampede";
        else
            return "Goal Title";
    }

    public String GetDescription(int identifier)
    {
        if (identifier == 1)
            return "Opened 200 Boxes!";
        if (identifier == 2)
            return "Opened 400 Boxes!";
        if (identifier == 3)
            return "Opened 1200 Boxes!";
        if (identifier == 4)
            return "Opened 2500 Boxes!";
        if (identifier == 5)
            return "Opened 4000 Boxes!";
        if (identifier == 6)
            return "Opened 7500 Boxes!";
        if (identifier == 7)
            return "Opened 10,000 Boxes!\n\"You are above even The Box. I hereby award you the title of Big Box.\"";
        if (identifier == 8)
            return "Got 200+ boxes in a single shift!!";
        if (identifier == 9)
            return "Got 200+ boxes in a single shift five times!";
        if (identifier == 10)
            return "Went to work 50 times!";
        if (identifier == 11)
            return "Caught in a disaster without insurance...";
        if (identifier == 12)
            return "Caught in a disaster within 30 seconds of buying insurance!";
        if (identifier == 13)
            return "Caught in a disaster with insurance!";
        if (identifier == 15)
            return "Caught in 5 disasters with insurance!";
        if (identifier == 17)
            return "Paid off a loan!";
        if (identifier == 18)
            return "Paid off seven loans!";
        if (identifier == 19)
            return "Failed to pay off a loan...";
        if (identifier == 20)
            return "Took your first loan.";
        if (identifier == 21)
            return "Paid off 21 loans!";
        if (identifier == 22)
            return "Failed to pay the same loan 10 times...";
        if (identifier == 23)
            return "Got your first visit from the bill collector.";
        if (identifier == 24)
            return "Paid 10,000 boxes total in bills!";
        else
            return "Goal Description Goes Here!";
    }

    public String GetRewardTitle(int identifier)
    {
        if (identifier == 8) //Reward for We Can Do It!
            return "Full-Time Sorter";
        if (identifier == 9) //Reward for You DID Do It!
            return "Extra Hours";
        if (identifier == 12) //Reward for Clairvoyance
            return "Foresight";
        if (identifier == 24) //Reward for Stampede
            return "Loyal Customer";
        return "Reward Title";
    }

    public String GetRewardDescription(int identifier)
    {
        if (identifier == 8) //Reward for We Can Do It!
            return "Starting after your next shift, your time between shifts is reduced by a minute!";
        if (identifier == 9) //Reward for You DID Do It!
            return "Starting on your next shift, you can work for 30 seconds longer!";
        if (identifier == 12) //Reward for Clairvoyance
            return "For such miraculous timing, your current insurance timer has been extended another 15 minutes!";
        if (identifier == 24) //Reward for Stampede
            return "Starting on your next bill, you will owe 20% less boxes on each bill!";
        else
            return "Reward Description Goes Here!";
    }

    public bool GetCompletionStatus(int identifier)
    {
        if (identifier == 1)
            return SaveManager.Instance.goal01Completion;
        if (identifier == 2)
            return SaveManager.Instance.goal02Completion;
        if (identifier == 3)
            return SaveManager.Instance.goal03Completion;
        if (identifier == 4)
            return SaveManager.Instance.goal04Completion;
        if (identifier == 5)
            return SaveManager.Instance.goal05Completion;
        if (identifier == 6)
            return SaveManager.Instance.goal06Completion;
        if (identifier == 7)
            return SaveManager.Instance.goal07Completion;
        if (identifier == 8)
            return SaveManager.Instance.goal08Completion;
        if (identifier == 9)
            return SaveManager.Instance.goal09Completion;
        if (identifier == 10)
            return SaveManager.Instance.goal10Completion;
        if (identifier == 11)
            return SaveManager.Instance.goal11Completion;
        if (identifier == 12)
            return SaveManager.Instance.goal12Completion;
        if (identifier == 13)
            return SaveManager.Instance.goal13Completion;
        if (identifier == 15)
            return SaveManager.Instance.goal13Completion;
        if (identifier == 17)
            return SaveManager.Instance.goal17Completion;
        if (identifier == 18)
            return SaveManager.Instance.goal18Completion;
        if (identifier == 19)
            return SaveManager.Instance.goal19Completion;
        if (identifier == 20)
            return SaveManager.Instance.goal20Completion;
        if (identifier == 21)
            return SaveManager.Instance.goal21Completion;
        if (identifier == 22)
            return SaveManager.Instance.goal22Completion;
        if (identifier == 23)
            return SaveManager.Instance.goal23Completion;
        if (identifier == 24)
            return SaveManager.Instance.goal24Completion;
        else
            return true;
    }
    #endregion
}
