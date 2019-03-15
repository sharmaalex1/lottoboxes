using System;
using UnityEngine;

/// <summary>
/// Central Hub for all events related to the Main Game.
/// </summary>
public class MainGameEventManager : MonoBehaviour
{
    public delegate void MainGameEventHandeler();

    public static event MainGameEventHandeler OnGameStart;
    public static event MainGameEventHandeler OnGameEnd;
    public static event MainGameEventHandeler OnBoxTapped;
    public static event MainGameEventHandeler OnBoxSpawned;
    public static event MainGameEventHandeler OnBoxDestroyed;
    public static event MainGameEventHandeler OnBoxCountExhausted;
    public static event MainGameEventHandeler OnFirstHundredBoxCount;
    public static event MainGameEventHandeler OnFirst25BoxCount;
    public static event MainGameEventHandeler OnTicketFound;
    public static event MainGameEventHandeler OnTicketRoutineBegin;
    public static event MainGameEventHandeler OnTicketRoutineEnd;
    public static event MainGameEventHandeler OnAllTicketsFound;
    public static event MainGameEventHandeler OnHyperModeBegin;
    public static event MainGameEventHandeler OnHyperModeEnd;
    public static event MainGameEventHandeler OnOpportunityBoxSpawn;
    public static event MainGameEventHandeler OnOpportunityBoxDespawn;
    public static event MainGameEventHandeler OnOpportunityBoxTutorialEnd;
    public static event MainGameEventHandeler OnFurnitureTutorialStart;
    public static event MainGameEventHandeler OnTournyStart;

    public static event Action<TicketPiece, bool> OnTicketPieceFound;
    public static event Action<GameObject, Vector3, Vector3> OnBoxSwiped;
    public static event Action<Classes> OnChosenCampaign;
    public static event Action<Classes> OnCampaignRestart;
    public static event Action<Classes> OnSwitchCampaign;
    public static event Action<int, bool> OnBoxFound;

    #region Event Wrappers

    public static void TriggerGameStartEvent()
    {
        if (OnGameStart != null)
        {
            OnGameStart();
        }
    }

    public static void TriggerGameEndEvent()
    {
        if (OnGameEnd != null)
        {
            OnGameEnd();
        }
    }

    public static void TriggerBoxTappedEvent()
    {
        if (OnBoxTapped != null)
        {
            OnBoxTapped();
        }
    }

    public static void TriggerBoxFoundEvent(int numBoxesFound, bool updateUI = true)
    {
        if (OnBoxFound != null)
        {
            OnBoxFound(numBoxesFound, updateUI);
        }
    }

    public static void TriggerBoxSpawnedEvent()
    {
        if (OnBoxSpawned != null)
        {
            OnBoxSpawned();
        }
    }

    public static void TriggerFurnitureTutorialStartEvent()
    {
        if(OnFurnitureTutorialStart != null)
        {
            OnFurnitureTutorialStart();
        }
    }

    public static void TriggerOpportunityBoxSpawnedEvent()
    {
        if(OnOpportunityBoxSpawn != null)
        {
            OnOpportunityBoxSpawn();
        }
    }

    public static void TriggerOpportunityBoxDespanwedEvent()
    {
        if(OnOpportunityBoxDespawn != null)
        {
            OnOpportunityBoxDespawn();
        }
    }

    public static void TriggerOpportunityBoxTutorialEndEvent()
    {
        if(OnOpportunityBoxTutorialEnd != null)
        {
            OnOpportunityBoxTutorialEnd();
        }
    }

    public static void TriggerBoxDestroyedEvent()
    {
        if (OnBoxDestroyed != null)
        {
            OnBoxDestroyed();
        }
    }

    public static void TriggerBoxCountExhaustedEvent()
    {
        if (OnBoxCountExhausted != null)
        {
            OnBoxCountExhausted();
        }
    }
    public static void TriggerTournyStartEvent()    {
        if(OnTournyStart != null)
        {
            OnTournyStart();
        }
    }
    public static void TriggerFirst25BoxCountEvent()
    {
        if (OnFirst25BoxCount != null)
        {
            OnFirst25BoxCount();
        }
    }

    public static void TriggerFirstHundredBoxCountEvent()
    {
        if (OnFirstHundredBoxCount != null)
        {
            OnFirstHundredBoxCount();
        }
    }

    public static void TriggerTicketFoundEvent()
    {
        if (OnTicketFound != null)
        {
            OnTicketFound();
        }
    }

    public static void TriggerTicketRoutineBegin()
    {
        if (OnTicketRoutineBegin != null)
        {
            OnTicketRoutineBegin();
        }
    }

    public static void TriggerTicketRoutineEnd()
    {
        if (OnTicketRoutineEnd != null)
        {
            OnTicketRoutineEnd();
        }
    }

    public static void TriggerAllTicketsFoundEvent()
    {
        if (OnAllTicketsFound != null)
        {
            OnAllTicketsFound();
        }
    }

    public static void TriggerHyperModeBegin()
    {
        if (OnHyperModeBegin != null)
        {
            OnHyperModeBegin();
        }
    }

    public static void TriggerHyperModeEnd()
    {
        if (OnHyperModeEnd != null)
        {
            OnHyperModeEnd();
        }
    }

    #endregion

    #region Action Event Wrappers

    public static void TriggerTicketPieceFoundEvent(TicketPiece piece, bool ignoreAnimationRoutine = false)
    {
        if (OnTicketPieceFound != null)
        {
            OnTicketPieceFound(piece, ignoreAnimationRoutine);
        }
    }

    public static void TriggerBoxSwipedEvent(GameObject box, Vector3 startPos, Vector3 endPos)
    {
        if (OnBoxSwiped != null)
        {
            OnBoxSwiped(box, startPos, endPos);
        }
    }

    public static void TriggerChosenCampaignEvent(Classes classChosen)
    {
        if (OnChosenCampaign != null)
        {
            OnChosenCampaign(classChosen);
        }
    }

    public static void TriggerCampaignRestartEvent(Classes classChosen)
    {
        if (OnCampaignRestart != null)
        {
            OnCampaignRestart(classChosen);
        }
    }

    public static void TriggerCampaignSwitchedEvent(Classes newClass)
    {
        if (OnSwitchCampaign != null)
        {
            OnSwitchCampaign(newClass);
        }
    }

    #endregion
}
 