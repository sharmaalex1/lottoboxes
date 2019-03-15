using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GoalManager : MonoBehaviour
{

    public GameObject goalUI;
    public Text goalUITitle;
    public Text goalUIDescription;
    public Text goalType;
    public GameObject goalUIIcon;

    public GameObject DisasterManager;

    public GameObject approvalWindow;

    void Awake()
    {
        
    }

    void LateUpdate() //Use for goals that need constant scanning to see if requirements are met, not called externally
    {
        //GoalUnlocked(Numerical Identifier of the Goal as per Spreadsheet on Drive, Minor/false or Major/true Goal)
        #region Box Opening Goals
        if (SaveManager.Instance.TotalBoxesOpened >= 200 && !SaveManager.Instance.goal01Completion)
            GoalUnlocked(1, false);

        if (SaveManager.Instance.TotalBoxesOpened >= 400 && !SaveManager.Instance.goal02Completion)
            GoalUnlocked(2, false);

        if (SaveManager.Instance.TotalBoxesOpened >= 1200 && !SaveManager.Instance.goal03Completion)
            GoalUnlocked(3, true);

        if (SaveManager.Instance.TotalBoxesOpened >= 2500 && !SaveManager.Instance.goal04Completion)
            GoalUnlocked(4, true);

        if (SaveManager.Instance.TotalBoxesOpened >= 4000 && !SaveManager.Instance.goal05Completion)
            GoalUnlocked(5, true);

        if (SaveManager.Instance.TotalBoxesOpened >= 7500 && !SaveManager.Instance.goal06Completion)
            GoalUnlocked(6, true);

        if (SaveManager.Instance.TotalBoxesOpened >= 10000 && !SaveManager.Instance.goal07Completion)
            GoalUnlocked(7, true);
        #endregion

        #region Bill Based Checks
        if (SaveManager.Instance.BillsPaid >= 10000 && !SaveManager.Instance.goal24Completion)
            GoalUnlocked(24, true);
        #endregion

        #region Loan Based Checks
        if (SaveManager.Instance.loansPaidCount >= 7 && !SaveManager.Instance.goal18Completion)
            GoalUnlocked(18, false);

        if (SaveManager.Instance.loansPaidCount >= 21 && !SaveManager.Instance.goal21Completion)
            GoalUnlocked(21, true);

        if (SaveManager.Instance.loansFailedStreak >= 10 && !SaveManager.Instance.goal22Completion)
            GoalUnlocked(22, true);
        #endregion

        #region Work Based Checks
        if (SaveManager.Instance.timesWorkedCount >= 50 && !SaveManager.Instance.goal10Completion)
            GoalUnlocked(10, true);
        #endregion
    }

    public void GoalUnlocked(int goalIdentifier, bool typeOfGoal)
    {
        if (typeOfGoal)
            goalType.text = "Major Goal";
        else
            goalType.text = "Minor Goal";

        goalUITitle.text = this.GetComponent<GoalLibrary>().GetTitle(goalIdentifier);
        goalUIDescription.text = this.GetComponent<GoalLibrary>().GetDescription(goalIdentifier);

        if (goalIdentifier == 1)
        {
            SaveManager.Instance.goal01Completion = true;
        }

        if (goalIdentifier == 2)
        {
            SaveManager.Instance.goal02Completion = true;
        }

        if (goalIdentifier == 3)
        {
            SaveManager.Instance.goal03Completion = true;
        }

        if (goalIdentifier == 4)
        {
            SaveManager.Instance.goal04Completion = true;
        }

        if (goalIdentifier == 5)
        {
            SaveManager.Instance.goal05Completion = true;
        }

        if (goalIdentifier == 6)
        {
            SaveManager.Instance.goal06Completion = true;
        }

        if (goalIdentifier == 7)
        {
            SaveManager.Instance.goal07Completion = true;
        }

        if (goalIdentifier == 8)
        {
            SaveManager.Instance.goal08Completion = true;
            this.GetComponent<GoalReward>().presentReward(this.GetComponent<GoalLibrary>().GetRewardTitle(goalIdentifier), this.GetComponent<GoalLibrary>().GetRewardDescription(goalIdentifier));
        }

        if (goalIdentifier == 9)
        {
            SaveManager.Instance.goal09Completion = true;
            this.GetComponent<GoalReward>().presentReward(this.GetComponent<GoalLibrary>().GetRewardTitle(goalIdentifier), this.GetComponent<GoalLibrary>().GetRewardDescription(goalIdentifier));
        }

        if (goalIdentifier == 10)
        {
            SaveManager.Instance.goal10Completion = true;
        }

        if (goalIdentifier == 11)
        {
            SaveManager.Instance.goal11Completion = true;
        }

        if (goalIdentifier == 12)
        {
            SaveManager.Instance.goal12Completion = true;
            this.GetComponent<GoalReward>().presentReward(this.GetComponent<GoalLibrary>().GetRewardTitle(goalIdentifier), this.GetComponent<GoalLibrary>().GetRewardDescription(goalIdentifier));
            DisasterManager.GetComponent<DisasterManager>().addTime(new TimeSpan (0,0,900)); //Adds 15 minutes to active insurance timer for Clairvoyance
        }

        if (goalIdentifier == 13)
        {
            SaveManager.Instance.goal13Completion = true;
        }

        if (goalIdentifier == 17)
        {
            SaveManager.Instance.goal17Completion = true;
        }

        if (goalIdentifier == 18)
        {
            SaveManager.Instance.goal18Completion = true;
        }

        if (goalIdentifier == 19)
        {
            SaveManager.Instance.goal19Completion = true;
        }

        if (goalIdentifier == 20)
        {
            SaveManager.Instance.goal20Completion = true;
        }

        if (goalIdentifier == 21)
        {
            SaveManager.Instance.goal21Completion = true;
        }

        if (goalIdentifier == 22)
        {
            SaveManager.Instance.goal22Completion = true;
        }

        if (goalIdentifier == 23)
        {
            SaveManager.Instance.goal23Completion = true;
        }

        if (goalIdentifier == 24)
        {
            SaveManager.Instance.goal24Completion = true;
            this.GetComponent<GoalReward>().presentReward(this.GetComponent<GoalLibrary>().GetRewardTitle(goalIdentifier), this.GetComponent<GoalLibrary>().GetRewardDescription(goalIdentifier));
        }

        goalUI.SetActive(true);

        if(!SaveManager.Instance.isApproved)
            approvalWindow.SetActive(true);
    }
}
