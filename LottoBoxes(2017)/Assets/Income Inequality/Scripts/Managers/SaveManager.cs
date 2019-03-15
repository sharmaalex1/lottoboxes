using UnityEngine;

using System.Collections;

using System.Collections.Generic;

using System.Runtime.Serialization.Formatters.Binary;

using System.Runtime.Serialization;

using System.IO;

using System;



/// <summary>

/// Singleton Script that takes care of loading,deleting, and saving data in binary form

/// Also contains

/// </summary>

public class SaveManager : MonoBehaviour

{

    public event Action SendSaveData;

    public event Action GameUnpaused;





    public ClassDifferences[] classDifferences;



    //private instace for the singeton

    private static SaveManager instance;



    private AllSaveData saveData;



    //all Save Data properties

    //used for readability and so we don't have to make the same proptery on multiple scripts



    //adjust quality settings based on RAM

    void SetQualitySettings()

    {

        if (SystemInfo.systemMemorySize > 2000)

        {

            Application.targetFrameRate = 60;

            QualitySettings.SetQualityLevel(2);

        }

        else if (SystemInfo.systemMemorySize > 600)

        {

            Application.targetFrameRate = 60;

            QualitySettings.SetQualityLevel(1);

        }

        else

        {

            Application.targetFrameRate = 48;

            QualitySettings.SetQualityLevel(0);

        }

    }



    //all properties so other scripts can access the save data

    #region Readability Properties

    #region Opportunity Box
    public int OpportunityBoxHealth
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].opportunityBoxHealth;
        }
        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].opportunityBoxHealth = value;
        }
    }

    public int OpportunityBoxFullHealth
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].opportunityBoxFullHealth;
        }
        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].opportunityBoxFullHealth = value;
        }
    }

    public float OpportunityBoxTimer
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].opportunityBoxTimer;
        }
        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].opportunityBoxTimer = value;
        }
    }

    public float OpportunityBoxOriginalTimer
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].opportunityBoxOriginalTimer;
        }
        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].opportunityBoxOriginalTimer = value;
        }
    }

    public bool OpportunityBoxActive
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].opportunityBoxAcitve;
        }
        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].opportunityBoxAcitve = value;
        }
    }

    public bool FirstOpportunityBoxEver
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].firstOpportunityBoxEver;
        }
        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].firstOpportunityBoxEver = value;
        }
    }
    #endregion

    public bool PassedTournyIntro
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].passedTournyIntro;
        }
        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].passedTournyIntro = value;
        }
    }

    public string TournamentUsername
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].tournamentUsername;
        }

        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].tournamentUsername = value;
        }
   }

    public int BoxesOwed //Save and get the amount of boxes owed to loan shark

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].boxesInDebt;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].boxesInDebt = value;

        }

    }



    public DateTime TimeOfLoanCollection //loan timer

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeOfLoanCollection;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeOfLoanCollection = value;

        }

    }



    public bool IsLoanActive //check to msee if the loan is true

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].isLoanTakenOut;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].isLoanTakenOut = value;

        }

    }

    public bool isApproved //check to see if the approval window has been run

    {

        get

        {

            return saveData.hasApprovalWindowBeenShown;

        }



        set

        {

            saveData.hasApprovalWindowBeenShown = value;

        }

    }
    #region Goal Booleans
    public bool goal01Completion //Box Novice -- 200 Boxes

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal01Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal01Complete = value;
            //Debug.Log("Achievement 01 set true");

        }

    }

    public bool goal02Completion //Box Enthusiast -- 400 Boxes

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal02Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal02Complete = value;

        }

    }

    public bool goal03Completion //Box Afficianado -- 1200 Boxes

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal03Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal03Complete = value;

        }

    }

    public bool goal04Completion //Box Master -- 2500 Boxes

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal04Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal04Complete = value;

        }

    }

    public bool goal05Completion //Box Entrepreneur -- 4000 Boxes

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal05Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal05Complete = value;

        }

    }

    public bool goal06Completion //Box Mogul -- 7500 Boxes

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal06Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal06Complete = value;

        }

    }

    public bool goal07Completion //Big Box -- 10,000 Boxes

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal07Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal07Complete = value;

        }

    }

    public bool goal08Completion //We can do it! -- 200 Boxes in one shift

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal08Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal08Complete = value;

        }

    }

    public bool goal09Completion //You did do it! -- 200 Boxes in one shift five times

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal09Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal09Complete = value;

        }

    }

    public bool goal10Completion //Self Made Box Opener -- Work 50 times

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal10Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal10Complete = value;

        }

    }

    public bool goal11Completion //You were not prepared -- Disaster w/o insurance

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal11Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal11Complete = value;

        }

    }

    public bool goal12Completion //Clairvoyance -- Insurance bought 30 seconds or less before disaster

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal12Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal12Complete = value;

        }

    }

    public bool goal13Completion //You were prepared -- Got hit with a disaster while having insurance

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal13Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal13Complete = value;

        }

    }

	public bool goal15Completion //Roach Resilience -- Hit with 5 insured disasters

	{

		get

		{

			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal15Complete;

		}



		set

		{

			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal15Complete = value;

		}

	}

    public bool goal17Completion //Shark Bait -- Pay off a loan

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal17Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal17Complete = value;

        }

    }

    public bool goal18Completion //Shark Week -- Pay off Seven Loans

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal18Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal18Complete = value;

        }

    }

    public bool goal19Completion //Chum -- Fail to pay a loan

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal19Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal19Complete = value;

        }

    }

    public bool goal20Completion //Testing the Waters -- Take out loan for first time

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal20Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal20Complete = value;

        }

    }

    public bool goal21Completion //Adult Swim -- Pay 21 loans

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal21Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal21Complete = value;

        }

    }

    public bool goal22Completion //Castaway -- Fail a loan 10 times

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal22Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal22Complete = value;

        }

    }

    public bool goal23Completion //Where the Buffalo Roam -- Get visit from buffalo

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal23Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal23Complete = value;

        }

    }

    public bool goal24Completion //Stampede -- 1000k Boxes paid in bills

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal24Complete;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].goal24Complete = value;

        }

    }
    #endregion

	#region Market Booleans
	public bool itemOnStand //Check whether there is already an item on the stand
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].itemPlacedOnStand;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].itemPlacedOnStand = value;
		}
	}

	public bool itemOnTable //Check whether there is already an item on the stand
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].itemPlacedOnTable;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].itemPlacedOnTable = value;
		}
	}

	public bool item01Purchased //Bought 
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item01Bought;
		} 
	
		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item01Bought = value;
			//Debug.Log("Bought 01 set true");
		}
	}

	public bool item02Purchased //Bought 
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item02Bought;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item02Bought = value;
		}
	}

	public bool item03Purchased //Bought 
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item03Bought;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item03Bought = value;
		}
	}

	public bool item04Purchased //Bought 
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item04Bought;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item04Bought = value;
		}
	}
	public bool item05Purchased //Bought 
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item05Bought;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item05Bought = value;
		}
	}

	public bool item06Purchased //Bought 
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item06Bought;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item06Bought = value;
		}
	}

	public bool item07Purchased //Bought Desk
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item07Bought;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item07Bought = value;
		}
	}

	public bool item08Purchased //Bought Fishbowl
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item08Bought;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item08Bought = value;
		}
	}

	public bool item09Purchased //Bought Dog Bed
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item09Bought;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item09Bought = value;
		}
	}

	public bool item10Purchased //Bought Blinds
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item10Bought;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item10Bought = value;
		}
	}

	public bool item11Purchased //Bought Tea Set
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item11Bought;
		} 

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].item11Bought = value;
		}
	}

    public bool isWallFixed
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wallFixed;
        }

        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wallFixed = value;
        }
    }

    public bool isWindowFixed
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].windowFixed;
        }

        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].windowFixed = value;
        }
    }

	public bool floor01bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor01;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor01 = value;
		}
	}

	public bool floor02bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor02;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor02 = value;
		}
	}

	public bool floor03bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor03;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor03 = value;
		}
	}

	public bool floor04bought
	{
		get
		{
			return saveData.classesData [(int)StatisticsManager.Instance.CurrentClass].floor04;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor04 = value;
		}
	}

	public bool floor05bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor05;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor05 = value;
		}
	}

	public bool floor06bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor06;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].floor06 = value;
		}
	}

	public bool wall01bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall01;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall01 = value;
		}
	}

	public bool wall02bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall02;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall02 = value;
		}
	}

	public bool wall03bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall03;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall03 = value;
		}
	}

	public bool wall04bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall04;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall04 = value;
		}
	}

	public bool wall05bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall05;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall05 = value;
		}
	}

	public bool wall06bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall06;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall06 = value;
		}
	}

	public bool wall07bought
	{
		get
		{
			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall07;
		}

		set
		{
			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].wall07 = value;
		}
	}
    #endregion

    #region GoalRelated

    public int loansPaidCount //Tracks how many loans have been paid

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].loansPaid;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].loansPaid = value;

        }

    }

	public int insuredDisasterCount //Tracks how many disasters have struck with insurance

	{

		get

		{

			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].insuredDisaster;

		}



		set

		{

			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].insuredDisaster = value;

		}

	}

    public int loansFailedStreak //Tracks how many loans have been failed consecutively

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].loansFailed;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].loansFailed = value;

        }

    }

    public int timesWorkedCount //Tracks how many times the player has worked

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timesWorked;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timesWorked = value;

        }

    }

    public int timesWorkOver200Count //Tracks how many times the player has finished work with 200 or more boxes

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timesWorkOver200;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timesWorkOver200 = value;

        }

    }

    public int totalBoxesPaidInBillsCount
    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].totalBoxesPaidInBills;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].totalBoxesPaidInBills = value;

        }

    }


    #endregion



    public DateTime TimeOfDisaster //disaster timer

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeOfDisaster;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeOfDisaster = value;

        }

    }

    public DateTime TimeOfInsuranceExpiration //when insurance expires
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeOfInsuranceExpiration;
        }

        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeOfInsuranceExpiration = value;
        }


    }



    public DateTime TimeOfGoal12Fail
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeOfGoal12Fail;
        }

        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeOfGoal12Fail = value;
        }


    }

    public bool IsInsuranceActive //is insurance true
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].isInsuranceActive;
        }

        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].isInsuranceActive = value;
        }
    }

    public bool FirstRun //check to see if this is the first time the game is played

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].isFirstRun;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].isFirstRun = value;

        }

    }



    public bool FirstInsurnaceOffer //is this the first insurance offer

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].firstInsuranceOffer;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].firstInsuranceOffer = value;

        }

    }



    public bool FirstOfferEver //get/save first offer

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].firstOfferEver;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].firstOfferEver = value;

        }

    }

    public bool FirstInsurancePopUpToggle

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].firstInsurancePopUpToggle;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].firstInsurancePopUpToggle = value;

        }

    }

    public bool SecondInsurancePopUpToggle

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].secondInsurancePopUpToggle;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].secondInsurancePopUpToggle = value;

        }

    }

    public bool DisasterOccurred //did a disaster occur

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].disasterOccurred;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].disasterOccurred = value;

        }

    }



    public bool DisasterToggle //enable/disable disaster

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].disasterToggle;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].disasterToggle = value;

        }

    }



    public bool ExpensesToggle //enable/disable bills

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].expensesToggle;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].expensesToggle = value;

        }

    }



    //STB Added functions related to Bills.

    public int BoxesOwedBill

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].boxesInBillDebt;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].boxesInBillDebt = value;

        }

    }

    public DateTime TimeOfBillCollection //when bills are collected

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeOfBillCollection;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeOfBillCollection = value;

        }

    }

    public bool IsBillActive //is bill true

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].areBillsStarted;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].areBillsStarted = value;

        }

    }

    public int CurrentBoxCount //how many boxes do you have
    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].currentBoxTotal;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].currentBoxTotal = value;

        }

    }



    public int TotalBoxesOwned

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].totalBoxesOwned;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].totalBoxesOwned = value;

        }

    }



    public int TotalBoxesOpened

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].totalBoxesOpened;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].totalBoxesOpened = value;

        }

    }



    public bool IsTicketBonusAvailable

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].isTicketBonusAvailable;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].isTicketBonusAvailable = value;

        }

    }



    public DateTime LastLogoutTime

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].lastLogoutTime;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].lastLogoutTime = value;

        }

    }



    public float ShiftTime

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].shiftTime;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].shiftTime = value;

        }

    }



    public int CompletedTicketCycles

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].completedTicketCycles;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].completedTicketCycles = value;

        }

    }

	public int CurrentOwnedTickets

	{

		get

		{

			return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].currentTickets;

		}

		set

		{

			saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].currentTickets = value;

		}

	}



    public List<TicketPieceLocations> TicketPieces

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].ticketLocationsFilled;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].ticketLocationsFilled = value;

        }

    }



    public List<Classes> UnlockedClasses

    {

        get

        {

            return saveData.unlockedClasses;

        }

        private set

        {



        }

    }

    public bool CompletedFurnitureTutorial
    {
        get
        {
            return saveData.isFurnitureTutorialComplete;
        }
        set
        {
            if (value == true)
            {
                saveData.isFurnitureTutorialComplete = true;
            }
        }
    }

    
    public bool StartedFurnitureTutorial
    {
        get
        {
            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].startedFurnitureTutorial;
        }
        set
        {
            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].startedFurnitureTutorial = value;
        }
    }
    

    public bool CompletedMainTutorial

    {

        get

        {

            return saveData.isMainTutorialComplete;

        }

        set

        {

            if (value == true)

            {

                saveData.isMainTutorialComplete = true;

            }



        }

    }



    public bool CompletedMiniTutorial

    {

        get

        {

            return saveData.isMiniGameTutorialComplete;

        }



        set

        {

            if (value == true)

            {

                saveData.isMiniGameTutorialComplete = true;

            }

        }

    }


    public bool CompletedStart

    {

        get

        {

            return saveData.isMainTutorialComplete;

        }

        set

        {

            if (value == true)

            {

                saveData.isMainTutorialComplete = true;

            }



        }

    }



    public bool CompletedIntro

    {

        get

        {

            return saveData.isIntroComplete;

        }



        set

        {

            if (value == true)

            {

                saveData.isIntroComplete = true;

            }

        }

    }


    public bool CompletedLoadTutorial

    {

        get

        {

            return saveData.isLoanTutorialComplete;

        }

        set

        {

            if (value == true)

            {

                saveData.isLoanTutorialComplete = true;

            }

        }

    }



    public TimeSpan TimePlayed

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timePlayed;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timePlayed = value;

        }

    }



    public int BoxesPaidBack

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].boxesPaidBack;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].boxesPaidBack = value;

        }

    }



    public int BillsPaid

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].billsPaid;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].billsPaid = value;

        }

    }



    public int BoxesGainedFromLoans

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].boxesGainedFromLoans;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].boxesGainedFromLoans = value;

        }

    }



    public int BoxesGainedFromWork

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].boxesGainedFromWork;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].boxesGainedFromWork = value;

        }

    }



    public int NumCurrentTicketPieces

    {

        get

        {

            return TicketPieces.Count;

        }

    }



    public int NumLoansTakenOut

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].numLoansTakenOut;

        }



        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].numLoansTakenOut = value;

        }

    }



    public TimeSpan TimeAtWork

    {

        get

        {

            return saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeAtWork;

        }

        set

        {

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].timeAtWork = value;

        }

    }



    #endregion



    #region Singleton Setup



    //public property with a get for other scripts to acces

    public static SaveManager Instance

    {

        get

        {

            return instance;

        }

    }



    //private constructor so other scripts can't make a new instance

    private SaveManager()

    {

    }



    //Function runs when scene is first started

    private void Awake()

    {

        //if no instance, set this as the instance and Load Save Data

        if (instance == null)

        {

            SetQualitySettings();

            instance = this;

            DontDestroyOnLoad(transform.root.gameObject);



            InitializeDifferences();

            Load();

        }

        //if an instance exists, destroy this one 

        else

        {

            Destroy(this.gameObject);

        }

    }



    #endregion



    #region Data Management



    /// <summary>

    /// Save the game data as it is right now to file.

    /// Called this script when the game is closed or leaves the foreground

    /// </summary>

    private void Save()

    {

        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(StaticVars.PATH_SAVE_DATA);

        bf.Serialize(file, saveData);

        file.Close();

    }



    /// <summary>

    /// Load the game data from the file

    /// Called at the beginning of the game.

    /// </summary>

    private void Load()

    {

        if (File.Exists(StaticVars.PATH_SAVE_DATA))

        {

            try

            {

                BinaryFormatter bf = new BinaryFormatter();

                FileStream file = File.Open(StaticVars.PATH_SAVE_DATA, FileMode.Open);

                saveData = bf.Deserialize(file) as AllSaveData;

                file.Close();



            }

            catch

            {

                Delete();

                Load();

            }

        }

        else

        {

            saveData = new AllSaveData(classDifferences);

            Save();

        }

    }



    /// <summary>

    /// Delete the game data file.

    /// </summary>

    private void Delete()

    {

        if (File.Exists(StaticVars.PATH_SAVE_DATA)) // make sure it is there

        {

            BinaryFormatter bf = new BinaryFormatter();

            File.Delete(StaticVars.PATH_SAVE_DATA);

            FileStream file = File.Create(StaticVars.PATH_SAVE_DATA);

            saveData = new AllSaveData(classDifferences);

            bf.Serialize(file, saveData);

            file.Close();



            //Debug.Log("DELETE || " + saveData);

        }

    }



    #endregion



    #region Save Automation



#if UNITY_EDITOR

    // This is reliably called in the editor, when the game stops

    void OnApplicationQuit()

    {

        if (MainGameSpawner.instance != null)

        {

            //Add boxes on screen to the count before saving

            saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].currentBoxTotal += MainGameSpawner.instance.NumOnScreenBoxes;

        }



        if (SendSaveData != null)

        {

            SendSaveData();

        }

        Save();

    }

#endif



    //Called when ever the game is pause or unpaused

    void OnApplicationPause(bool status)

    {

        if (status)

        {

            if (MainGameSpawner.instance != null)

            {

                saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].currentBoxTotal += MainGameSpawner.instance.NumOnScreenBoxes;

                //Save Everything

            }



            if (SendSaveData != null)

            {

                SendSaveData();

            }

            Save();



        }

        else if (!status)

        {

            if (MainGameSpawner.instance != null)

            {

                saveData.classesData[(int)StatisticsManager.Instance.CurrentClass].currentBoxTotal -= MainGameSpawner.instance.NumOnScreenBoxes;



                if (GameUnpaused != null)

                {

                    GameUnpaused();

                }

            }

        }

    }



    #endregion



    /// <summary>

    /// Function called by Stats Manager to reset a save data,

    /// Returns the new Save Data

    /// </summary>

    public ClassSaveData ResetDataForClass(Classes classToReset)

    {

        saveData.classesData[(int)classToReset] = new ClassSaveData(classDifferences[(int)classToReset].startingBoxes);

        return saveData.classesData[(int)classToReset];

    }



    //Allow Audio Manager to grab the entire Audio SaveData

    public AudioSaveData GetAudioData()

    {

        return saveData.audioData;

    }



    //no long in use

    public void DeleteAndReload()

    {

        Delete();

        Load();

    }



    //Loads the class differences from the Resources Folder

    private void InitializeDifferences()

    {



        int numClasses = Enum.GetNames(typeof(Classes)).Length;



        ClassDifferences[] tempDifferences = Resources.LoadAll<ClassDifferences>("ClassesData");



        if (tempDifferences == null || tempDifferences.Length == 0)

        {

#if UNITY_EDITOR

            Debug.LogError("Couldn't Find Class Differences in Resources Folder");

#endif

            return;

        }

        else if (tempDifferences.Length < numClasses || tempDifferences.Length > numClasses)

        {

#if UNITY_EDITOR

            Debug.LogError("The number of Class Differences Found doesn't not equal the number of Classes the game supports.");

#endif

            return;

        }



        classDifferences = new ClassDifferences[Enum.GetNames(typeof(Classes)).Length];



        for (int i = 0; i < tempDifferences.Length; i++)

        {

            int currentClass = (int)tempDifferences[i].myClass;

            if (classDifferences[currentClass] == null)

            {

                classDifferences[currentClass] = tempDifferences[i];

            }

            else

            {

#if UNITY_EDITOR

                Debug.LogError("There are multiple Class Differences for the same Class, which means one is missing for a class. Please make sure there is one for each class.");

#endif

                return;

            }

        }

    }



    //Formula used to calculate how many boxes a 

    public int GetBoxesReceived(Classes currentClass, int openedBoxes)

    {

        //if the percentage * the number of opened boxes is less then the starting num to give

        if (openedBoxes * classDifferences[(int)currentClass].openBoxPercent < (float)classDifferences[(int)currentClass].startingNumBoxesReceive)

        {

            //return starting NUm

            return classDifferences[(int)currentClass].startingNumBoxesReceive;

        }



        //return percentage * opened boxes rounded correctly

        float fBoxesCouldGain = openedBoxes * classDifferences[(int)currentClass].openBoxPercent;

        if (fBoxesCouldGain - Mathf.Floor(fBoxesCouldGain) >= 0.5f)

        {

            return Mathf.CeilToInt(fBoxesCouldGain);

        }



        return Mathf.FloorToInt(fBoxesCouldGain);



    }



    //Unlocks class if the class isn't already unlocked

    public void UnlockIncomeClass(Classes classToUnlock)

    {

        if (!UnlockedClasses.Contains(classToUnlock))

        {

            SaveManager.Instance.UnlockedClasses.Add(classToUnlock);

            MixpanelManager.ClassUnlocked(classToUnlock);

        }

    }

    public void IncrementWorkCount()
    {
        SaveManager.Instance.timesWorkedCount += 1;
        Debug.Log("Times Worked: " + SaveManager.Instance.timesWorkedCount.ToString());
    }
}





/// <summary>

/// SaveData holds all peristent data for the game. It has two ways to manipulate data.

/// Once the game is live you have to be very careful with changing the AllSaveData Class

/// If the Save Data class is changed while the game is live and it can destroy the players save data

/// and make them start over

/// VERSION TOLERANT SERIALIZATION

/// https://msdn.microsoft.com/en-us/library/ms229752(v=vs.80).aspx

/// New fields should have the attribute [OptionalField] applied to them

/// You will most likely need to use a few of the Seralization Callbacks

/// FYI Deserialization is loading, Serialization is Saving

/// </summary>



[System.Serializable]

public class AllSaveData

{

    //the list of all save data, one for each class/campaign

    public List<ClassSaveData> classesData;



    public List<Classes> unlockedClasses;



    //instance of AudioSaveData

    public AudioSaveData audioData;



    public bool isMainTutorialComplete;

    public bool isLoanTutorialComplete;

    public bool isMiniGameTutorialComplete;

    public bool isFurnitureTutorialComplete;

    public bool isIntroComplete;

    public bool hasApprovalWindowBeenShown;



    /// <summary>

    /// Default Constructor to make a new Save Data

    /// </summary>

    public AllSaveData(ClassDifferences[] classDifferences)

    {

        //make new list and give it a length (-1 because we don't want to include classes.none)

        classesData = new List<ClassSaveData>(Enum.GetNames(typeof(Classes)).Length);



        unlockedClasses = new List<Classes>();



        foreach (Classes cl in Enum.GetValues(typeof(Classes)))

        {

            classesData.Insert((int)cl, new ClassSaveData(classDifferences[(int)cl].startingBoxes));

        }



        audioData = new AudioSaveData();




        isMainTutorialComplete = isLoanTutorialComplete = isMiniGameTutorialComplete = isFurnitureTutorialComplete = false;

}

}



[System.Serializable]

public class ClassSaveData

{

    //Main Game Stats

    public int completedTicketCycles;

    public DateTime lastLogoutTime;

    public int currentBoxTotal;

    public int totalBoxesOwned;

    public int totalBoxesOpened;

    public int boxesInDebt;

    public int boxesInBillDebt;

    public DateTime timeOfLoanCollection;

    public DateTime timeOfBillCollection;

    public bool isLoanTakenOut;

    public bool areBillsStarted;

    public List<TicketPieceLocations> ticketLocationsFilled;

    public float shiftTime;

    public bool isTicketBonusAvailable;

    public bool startedFurnitureTutorial;

    #region Disaster Variables
    public bool isFirstRun;

    public bool firstInsuranceOffer;

    public bool firstOfferEver;

    public bool disasterOccurred;

    public bool isInsuranceActive;

    public DateTime timeOfDisaster;

    public DateTime timeOfInsuranceExpiration;

    public DateTime timeOfGoal12Fail;

    public bool firstInsurancePopUpToggle;

    public bool secondInsurancePopUpToggle;



    public bool disasterToggle;

    public bool expensesToggle;
    #endregion

    #region Tournament Variables

    public string tournamentUsername;

    public bool passedTournyIntro;

    #endregion

    public int opportunityBoxHealth;

    public int opportunityBoxFullHealth;

    public float opportunityBoxTimer;

    public float opportunityBoxOriginalTimer;

    public bool opportunityBoxAcitve;

    public bool firstOpportunityBoxEver;


    public TimeSpan timePlayed;

    public int boxesPaidBack;

    public int billsPaid;

    public int boxesGainedFromLoans;

    public int boxesGainedFromWork;

    public int numLoansTakenOut;

    public TimeSpan timeAtWork;

	public int currentTickets;

    #region GoalCompletion

    public bool goal01Complete;
    public bool goal02Complete;
    public bool goal03Complete;
    public bool goal04Complete;
    public bool goal05Complete;
    public bool goal06Complete;
    public bool goal07Complete;
    public bool goal08Complete;
    public bool goal09Complete;
    public bool goal10Complete;
    public bool goal11Complete;
    public bool goal12Complete;
    public bool goal13Complete;
	public bool goal15Complete;
    public bool goal17Complete;
    public bool goal18Complete;
    public bool goal19Complete;
    public bool goal20Complete;
    public bool goal21Complete;
    public bool goal22Complete;
    public bool goal23Complete;
    public bool goal24Complete;

    #endregion

    #region GoalRelated

    public int loansFailed;
    public int loansPaid;
    public int timesWorked;
    public int timesWorkOver200;
    public int totalBoxesPaidInBills;
	public int insuredDisaster;

    #endregion

	public bool itemPlacedOnStand;
	public bool itemPlacedOnTable;

	#region ItemBought

	public bool item01Bought;
	public bool item02Bought;
	public bool item03Bought;
	public bool item04Bought;
	public bool item05Bought;
	public bool item06Bought;
	public bool item07Bought;
	public bool item08Bought;
	public bool item09Bought;
	public bool item10Bought;
	public bool item11Bought;
    public bool wallFixed;
    public bool windowFixed;
	public bool floor01;
	public bool floor02;
	public bool floor03;
	public bool floor04;
	public bool floor05;
	public bool floor06;
	public bool wall01;
	public bool wall02;
	public bool wall03;
	public bool wall04;
	public bool wall05;
	public bool wall06;
	public bool wall07;
	#endregion



    /// <summary>

    /// Default Constructor to reset Values

    /// </summary>

    public ClassSaveData(int startingNum)

    {

        completedTicketCycles = 0;

        lastLogoutTime = DateTime.Now;

        totalBoxesOpened = 0;

        currentBoxTotal = totalBoxesOwned = startingNum;

        boxesInDebt = 0;

        timeOfLoanCollection = new DateTime();

        isLoanTakenOut = false;

        areBillsStarted = false;

        ticketLocationsFilled = new List<TicketPieceLocations>();

        shiftTime = 0;

        isTicketBonusAvailable = false;

        startedFurnitureTutorial = false;

        opportunityBoxHealth = 0;

        opportunityBoxFullHealth = 0;

        opportunityBoxTimer = 0.0f;

        opportunityBoxOriginalTimer = 0.0f;

        opportunityBoxAcitve = false;

        firstOpportunityBoxEver = true;

        isFirstRun = true;

        disasterOccurred = false;

        firstInsuranceOffer = true;

        firstOfferEver = true;

        isInsuranceActive = false;

        timeOfDisaster = new DateTime();

        timeOfInsuranceExpiration = new DateTime();

        timeOfGoal12Fail = new DateTime();

        disasterToggle = true;

        expensesToggle = true;

        firstInsurancePopUpToggle = true;

        secondInsurancePopUpToggle = true;



        timePlayed = TimeSpan.Zero;

        timeAtWork = TimeSpan.Zero;

        boxesGainedFromLoans = 0;

        boxesGainedFromWork = 0;

        boxesPaidBack = 0;

        billsPaid = 0;

        numLoansTakenOut = 0;

    }

}



[System.Serializable]

public class AudioSaveData

{

    //what if anything is muted muted or not

    public bool sfxMuted;

    public bool bgmMuted;

    public bool ambMuted;



    //how loud are the sounds

    public float sfxVolume;

    public float bgmVolume;

    public float ambVolume;





    public AudioSaveData()

    {

        sfxVolume = bgmVolume = ambVolume = 0.75f;

        sfxMuted = bgmMuted = ambMuted = false;

    }

}







