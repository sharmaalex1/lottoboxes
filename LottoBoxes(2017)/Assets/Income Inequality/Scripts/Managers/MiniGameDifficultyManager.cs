using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//This is a small class that handles the Fibonacci classes which affect the Difficulty of the mini game
public class MiniGameDifficultyManager : MonoBehaviour
{
    //what was the previous multiplier
    private int previousMultiplier;

    //set the highest multiplier possible in inspector
    [SerializeField]
    private int highestMultiplier = 12;

    //readonly property for highest multiplier
    public int HighestMultiplier
    {
        get
        {
            return highestMultiplier;
        }

        private set
        {

        }
    }

    //fields to set in inspector for difficulty curves

    #region Fibonacci Vars

    [SerializeField]
    private StreakFibonacci streakSequence;
    [SerializeField]
    private SpeedFibonacci speedFibonacci;
    [SerializeField]
    private SpawnFibonacci spawnTimeFibonacci;

    #endregion

    //readonly property for the current wanted streak
    public float WantedStreak
    {
        get
        {
            return streakSequence.WantedStreak;
        }

        private set
        {

        }
    }

    //readonly property for the current Speed
    public float CurrentSpeed
    {
        get
        {
            return speedFibonacci.CurrentSpeed;
        }

        private set
        {

        }
    }

    //readonly property for the current Spawn Rate
    public float SpawnRate
    {
        get
        {
            return spawnTimeFibonacci.CurrentSpawnTime;
        }

        private set
        {

        }
    }

    #region Singleton

    //private static instance
    private static MiniGameDifficultyManager instance;
    //readonly Property for other scripts to access instance
    public static MiniGameDifficultyManager Instance
    {
        get
        {
            return instance;
        }

        private set { }
    }
    //private constructor
    private MiniGameDifficultyManager()
    {
    }

    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            previousMultiplier = 0;
            //DontDestroyOnLoad(this.transform.root);
            //don't want gameobject to persist
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    //when enabled, subscribe to events
    private void OnEnable()
    {
        SubscribeToEvents();
    }
    
    //subscribe to relevant events
    private void SubscribeToEvents()
    {
        UISlider.OnSlide += InitializeSequences;
        MiniGameEventManager.OnMultiplierChange += IncreaseSequences;
        MiniGameEventManager.OnIncorrectBoxPlacement += ResetSequences;
        MiniGameEventManager.OnBoxMissed += ResetSequences;
      
    }

    //unsubscribe to events
    private void UnSubscribeFromEvents()
    {
        UISlider.OnSlide -= InitializeSequences;
        MiniGameEventManager.OnMultiplierChange -= IncreaseSequences;
        MiniGameEventManager.OnIncorrectBoxPlacement -= ResetSequences;
        MiniGameEventManager.OnBoxMissed -= ResetSequences;
    }

    //when this object is destory, make sure to unsubscribe
    private void OnDestroy()
    {
        UnSubscribeFromEvents();
    }

    //if we are entering the Mini Game, Initialize our difficulty curves
    private void InitializeSequences(int i)
    {
        if ((MainSceneUIElements)i == MainSceneUIElements.MiniGame)
        {
            streakSequence.Initialize();
            speedFibonacci.Initialize();
            spawnTimeFibonacci.Initialize();
        }
    }

    //if the multiplier has increased, we need to increase the sequences
    private void IncreaseSequences(int newMultiplier)
    {
        //only do this if the new multipler is greater than the previous
        if (newMultiplier > previousMultiplier)
        {
            AudioManager.Instance.PlayAudioClip(SFXType.MultiplierIncrease, index: newMultiplier);
            streakSequence.IncrementSequence();
            speedFibonacci.IncrementSequence();
            spawnTimeFibonacci.IncrementSequence();
            previousMultiplier = newMultiplier;

            //Trigger speed change so that current boxes on screen can increase speed
            MiniGameEventManager.TriggerOnSpeedChange(CurrentSpeed);
        }
    }

    //If we place something incorrectly or miss a box
    //we will need to reset the sequences
    private void ResetSequences()
    {
        streakSequence.Reset();
        speedFibonacci.Reset();
        spawnTimeFibonacci.Reset();
        previousMultiplier = 0;

        AudioManager.Instance.PlayAudioClip(SFXType.MultiplierReset);

        //trigger speed change so boxes on screen can slow down
        MiniGameEventManager.TriggerOnSpeedChange(CurrentSpeed);
    }

}




