using System;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;



/// <summary>

/// This class is used to communicate class data with other scripts.

/// Properties pertaining to box count, ticket pieces, loans, and idle rewards can be found here.

/// </summary>

public class StatisticsManager : MonoBehaviour

{

    #region Singleton Setup



    //singleton instance, set to private so other scripts don't have direct access to the var

    private static StatisticsManager instance;



    public static StatisticsManager Instance

    {

        //instance for other scripts to grab

        get

        {

            return instance;

        }

        //leave out set so the instance is readonly to other scrips

    }



    //private constructor so other scripts can't make an instance at runtime

    private StatisticsManager()

    {

    }



    #endregion



    //reference to the current Class

    //this variable and property isn't really needed because a new one was made in SceneTransition

    //decided to keep it because a lot of stuff already use this reference

    //if you were to keep one, get rid of this one and change all references from this one to

    //the one in scene transition

    private Classes currentClass;



    public Classes CurrentClass

    {

        get

        {

            return currentClass;

        }

        private set { }

    }



    #region Unity Callbacks



    //Function called when the script is first loaded

    private void Awake()

    {

        //activate singleton

        if (instance == null)

        {

            instance = this;

        }

        else

        {

            UnityEngine.Object.Destroy(this.gameObject);

        }



        SubscribeToRelevantEvents();

        //if we start in the MainMiniCombo in editor, then default class to Rich

        if (SceneTransition.Instance == null)

        {

            currentClass = Classes.Rich;

            Debug.Log("The field SceneTransition.Instance.SelectedClass is null. Defaulting to rich class.");

        }

        else

        {

            currentClass = SceneTransition.Instance.SelectedClass;           

        }



        //reset variables in case they closed the game before tutorial end

        if (!SaveManager.Instance.CompletedMainTutorial)

        {

            SaveManager.Instance.CurrentBoxCount = 1;

            SaveManager.Instance.TotalBoxesOwned = 1;

            SaveManager.Instance.TotalBoxesOpened = 0;

        }

    }



    //function called when Scene is destroyed, unsubscribe from events to prevent memory leaks

    private void OnDestroy()

    {

        UnSubscribeToReleventEvents();

    }



    #endregion

    #region Box Related Methods

    //function called by Main Game Tutorial to give starting boxes and to update UI when tutorial is finished

    public void GiveStartingBoxes()

    {

        SaveManager.Instance.TotalBoxesOwned = SaveManager.Instance.classDifferences[(int)CurrentClass].startingBoxes;

        SaveManager.Instance.CurrentBoxCount = SaveManager.Instance.classDifferences[(int)CurrentClass].startingBoxes;

        MainGameUI.instance.UpdateBoxCountUI(SaveManager.Instance.CurrentBoxCount);

    }



    /// <summary>

    /// Adds to the current box count.

    /// </summary>

    public void AddToBoxCount(int numToAdd, bool updateUI = true)

    {

        SaveManager.Instance.CurrentBoxCount += numToAdd;

        SaveManager.Instance.TotalBoxesOwned += numToAdd;

        if (updateUI)

        {

            MainGameUI.instance.UpdateBoxCountUI(SaveManager.Instance.CurrentBoxCount);

        }            

    }



    /// <summary>

    /// Removes from the current box count.

    /// Mainly used for loan Manager

    /// </summary>

    public void RemoveFromBoxCount(int numToRemove, bool updateUI = true)

    {

        SaveManager.Instance.CurrentBoxCount -= numToRemove;

        if (updateUI)

        {

            MainGameUI.instance.UpdateBoxCountUI(SaveManager.Instance.CurrentBoxCount);

        }            

    }



    //function for subscription, decrements total of current boxes

    //used for when a box is opened

    private void CurrentBoxesDecrement()

    {

        SaveManager.Instance.CurrentBoxCount--;

        MainGameUI.instance.UpdateBoxCountUI(SaveManager.Instance.CurrentBoxCount);

    }



    #endregion





    #region Ticket Related Methods



    //Increments ticket cycles  and updates Ui

    public void IncrementCompletedTicketCycles()

    {

        SaveManager.Instance.CompletedTicketCycles++;
		SaveManager.Instance.CurrentOwnedTickets++;

		MainGameUI.instance.UpdateTicketCountUI(SaveManager.Instance.CurrentOwnedTickets);

    }




    //used to reset the ticket pieces

    public void ResetTicketPieces()

    {

        SaveManager.Instance.TicketPieces = new List<TicketPieceLocations>();

    }



    #endregion



    #region Class Data Related Methods



    //function for subscription

    //used for when a player wants to player a class after already finished that class' campaign

    //not in use

    private void ResetCampaignData(Classes classChosen)

    {

        currentClass = classChosen;

        SaveManager.Instance.ResetDataForClass(classChosen);

    }



    #endregion



    #region Event Subscription Handling



    //subscribe to events to track stats

    private void SubscribeToRelevantEvents()

    {

        //Main Game Events

        //UISlider.OnSlide += OnLevelChanged;

        MainGameEventManager.OnBoxSpawned += CurrentBoxesDecrement;

        MainGameEventManager.OnBoxFound += AddToBoxCount;

        MainGameEventManager.OnTicketPieceFound += AddTicketPiece;

        MainGameEventManager.OnAllTicketsFound += IncrementCompletedTicketCycles;

        MainGameEventManager.OnAllTicketsFound += ResetTicketPieces;

        MainGameEventManager.OnCampaignRestart += ResetCampaignData;

    }



    //unsubscribe to events to track stats

    private void UnSubscribeToReleventEvents()

    {

        //Main Game Events

        //UISlider.OnSlide -= OnLevelChanged;

        MainGameEventManager.OnBoxSpawned -= CurrentBoxesDecrement;

        MainGameEventManager.OnBoxFound -= AddToBoxCount;

        MainGameEventManager.OnTicketPieceFound -= AddTicketPiece;

        MainGameEventManager.OnAllTicketsFound -= ResetTicketPieces;

        MainGameEventManager.OnAllTicketsFound -= IncrementCompletedTicketCycles;

        MainGameEventManager.OnCampaignRestart -= ResetCampaignData;

    }



    #endregion



    //updates the loan information and tests if there is any problem with the info

    public void UpdateLoanInfo(int boxesOwed, DateTime timeOfLoanCollection, bool isLoanActive)

    {



        SaveManager.Instance.BoxesOwed = boxesOwed;

        SaveManager.Instance.TimeOfLoanCollection = timeOfLoanCollection;



        if (isLoanActive && SaveManager.Instance.BoxesOwed <= 0)

        {

            Debug.LogError("A loan IS active but no boxes are owed." +

                "\nDefaulting IsLoanActive value to false.");

        }

        else if (!isLoanActive && SaveManager.Instance.BoxesOwed > 0)

        {

            Debug.LogError("A loan is NOT active yet there are boxes owed and/or time left until loan collection." +

                "\nDefaulting IsLoanActive value to true in order to complete the process.");

        }



        SaveManager.Instance.IsLoanActive = isLoanActive;

    }



    //STB Added UpdateBillInfo

    public void UpdateBillInfo(int boxesOwedBill, DateTime timeOfBillCollection, bool isBillActive)

    {

        SaveManager.Instance.BoxesOwedBill = boxesOwedBill;

        SaveManager.Instance.TimeOfBillCollection = timeOfBillCollection;



        if (isBillActive && SaveManager.Instance.BoxesOwedBill <= 0)

        {

            Debug.LogError("A bill IS active but no boxes are owed." +

                "\nDefaulting IsBillActive value to false.");

        }

        else if (!isBillActive && SaveManager.Instance.BoxesOwedBill > 0)

        {

            Debug.LogError("A bill is NOT active yet there are boxes owed and/or time left until loan collection." +

                "\nDefaulting IsBillActive value to true in order to complete the process.");

        }



        SaveManager.Instance.IsBillActive = isBillActive;

    }



    public void UpdateDisasterInfo(DateTime timeOfDistaster, DateTime timeOfInsuranceExpiration, bool isInsuranceActive, bool firstInsuranceOffer, bool disasterOccurred, bool firstRun,
                                    bool firstInsurancePopUpToggle, bool secondInsurancePopUpToggle, DateTime timeOfGoal12Fail)

    {

        SaveManager.Instance.TimeOfInsuranceExpiration = timeOfInsuranceExpiration;
        SaveManager.Instance.TimeOfDisaster = timeOfDistaster;
        SaveManager.Instance.TimeOfGoal12Fail = timeOfGoal12Fail;

        SaveManager.Instance.IsInsuranceActive = isInsuranceActive;
        SaveManager.Instance.FirstInsurnaceOffer = firstInsuranceOffer;


        SaveManager.Instance.DisasterOccurred = disasterOccurred;

        SaveManager.Instance.FirstRun = firstRun;
        SaveManager.Instance.FirstInsurancePopUpToggle = firstInsurancePopUpToggle;
        SaveManager.Instance.SecondInsurancePopUpToggle = secondInsurancePopUpToggle;

    }

    public void UpdateTournamentInfo(string tournamentUsername, bool passedTournyIntro)
    {
        SaveManager.Instance.TournamentUsername = tournamentUsername;
        SaveManager.Instance.PassedTournyIntro = passedTournyIntro;
    }



    public void UpdateDisasterToggleState(bool disasterToggle)

    {

        SaveManager.Instance.DisasterToggle = disasterToggle;

    }



    public void UpdateExpensesToggleState(bool expensesToggle)

    {

        SaveManager.Instance.ExpensesToggle = expensesToggle;

    }

    public void UpdateOppertunityBoxInfo(int opportunityBoxHealth, int opportunityBoxFullHealth, float opportunityBoxTimer, float opportunityBoxOriginalTimer)
    {
        SaveManager.Instance.OpportunityBoxHealth = opportunityBoxHealth;
        SaveManager.Instance.OpportunityBoxFullHealth = opportunityBoxFullHealth;
        SaveManager.Instance.OpportunityBoxTimer = opportunityBoxTimer;
        SaveManager.Instance.OpportunityBoxOriginalTimer = opportunityBoxOriginalTimer;
    }

    public void UpdateOppertunityBoxActiveState(bool opportunityBoxActive)
    {
        SaveManager.Instance.OpportunityBoxActive = opportunityBoxActive;
    }

    
    public void UpdateFurnitureTutorialStartedState(bool startedFurnitureTutorial)
    {
        SaveManager.Instance.StartedFurnitureTutorial = startedFurnitureTutorial;
    }
    


    /// <summary>

    /// Adds a ticket piece to income class if that piece hasn't already been found. 

    /// Function returns true if all pieces are found

    /// Handles determining what the next piece will be and sends that data to the 

    /// ticket piece itself so that it can set its sprite accordingly.

    /// </summary>

    public void AddTicketPiece(TicketPiece piece, bool ignoreAnimationRoutine)

    {

        TicketPieceLocations locationBeingChecked = 0;

        // While the ticketPiecesOwn list has the piece for a particluar location, 

        // keep moving forward in the list and find one that isn't added yet.

        while (SaveManager.Instance.TicketPieces.Contains(locationBeingChecked))

        {

            locationBeingChecked += 1;

            piece.id = (int)locationBeingChecked;

        }



        // If the animation routine for obtaining a ticket is to be run, then a sprite must be assigned to it.

        // This is an unnecessary operation otherwise.

        if (!ignoreAnimationRoutine)

        {

            piece.ConfigureSprite(piece.id);

        }



        SaveManager.Instance.TicketPieces.Add(locationBeingChecked);



        MixpanelManager.TicketPieceCollected(piece);

    }



}