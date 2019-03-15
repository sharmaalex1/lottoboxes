using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// This class handles all changes made to score and multiplier within the minigame.
/// Special effects such as the multiplier flash, scaling and shaking are included.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // Non-persistent singleton.
    public static ScoreManager instance;

    public GameObject GoalManager;

    public GameObject miniGameResultsUI;
    public Text miniGameResultsText;

    private int streak;
    private int multiplier;
    private int score;

    private int highestSessionMulti = 1;

    public int Score
    { 
        get
        { 
            if (instance != null)
            {
                return instance.score; 
            }
            Debug.LogError("The singleton being accessed is null (ScoreManager)");
            return 0;
        } 
    }

    #region UI Items

    public Text scoreText;
    public GameObject multiplierImages;
    public Text multiplierText;
    public Image multiplierFill;
    public ParticleSystem particle;
    public Color goodColor;
    public Color missColor;

    #endregion

    #region Unity Callbacks

    void Awake()
    {
        GoalManager = GameObject.Find("GoalManager");
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    //Setup Variables and start Coroutines
    private void OnEnable()
    {
        SubscribeToEvents();

        streak = 0;
        score = 0;
        multiplier = 1;

        ParticleSystem.EmissionModule em = particle.emission;
        em.enabled = false;
        particle.Stop();
        multiplierFill.fillAmount = 0;

        scoreText.text = Mathf.Ceil(score).ToString();
        multiplierText.text = string.Format("x{0}", multiplier);

        StartCoroutine(UpdateFill());

        MixpanelManager.EnterWork();
    }

    /// <summary>
    /// Events are unsubscribed here.
    /// </summary>
    void OnDisable()
    {
        //Adds score the the class' current box count
        ShowPostGameScore(score);
        UnsubscribeFromEvents();
    }

    #endregion

    #region Score Utilities

    //function used to reset streak and show multiplier shake if multiplier is greater than 1
    //used to reset multiplier
    private void ResetStreak()
    {
        streak = 0;
        multiplierFill.fillAmount = 0;

        if (multiplier >= 2)
        {
            StartCoroutine(FlashParticle(missColor));
            iTween.ShakePosition(multiplierImages, iTween.Hash("x", 0.1f, "time", 0.4f));
        }

        multiplier = 1;
        multiplierText.text = string.Format("x{0}", multiplier);
    }

    /// <summary>
    /// Function called when it is time to update the multiplier 
    /// Only used to increase multiplier
    /// </summary>
    private IEnumerator UpdateMultiplier()
    {
        //only update multiplier if it is less than the highest multiplier allowed
        if (multiplier < MiniGameDifficultyManager.Instance.HighestMultiplier)
        {

            // Wait a tenth of a second to allow the multiplier to fill completely
            // This function will reset it before then, otherwise.
            yield return new WaitForSeconds(0.1f);

            multiplier++;
            if (multiplier > highestSessionMulti)
            {
                highestSessionMulti = multiplier;
            }

            //reset streak
            streak = 0;

            //Multiplier Gameplay juice
            StartCoroutine(FlashParticle(goodColor));
            StartCoroutine(JoltMultiplierSize());

            //trigger multiplier change event
            MiniGameEventManager.TriggerMultiplierChange(multiplier);
            multiplierText.text = string.Format("x{0}", multiplier);
        }

        yield break;
    }
    //Function used for subscription
    //called when a player places a box correctly
    private void IncreaseScoreAndStreak()
    {
        //increase score and streak
        score += multiplier;
        streak++;
        //update score text
        scoreText.text = Mathf.Ceil(score).ToString();

        //if the updated streak is equal to the wanted streak
        if (streak == MiniGameDifficultyManager.Instance.WantedStreak)
        {
            //update the multiplier
            StartCoroutine(UpdateMultiplier());
        }	
    }

    public void ShowPostGameScore(int boxesEarned)
    {
        if (boxesEarned > 0)
        {
            if (miniGameResultsUI == null || miniGameResultsText == null)
            {
                miniGameResultsUI = GameObject.Find("PostMiniGameResults");
                miniGameResultsText = GameObject.Find("BoxesEarnedAmount").GetComponent<Text>();   
            }

            miniGameResultsText.text = boxesEarned.ToString();
            miniGameResultsUI.SetActive(true);
            StatisticsManager.Instance.AddToBoxCount(boxesEarned);
            SaveManager.Instance.BoxesGainedFromWork += boxesEarned;
        }

        MixpanelManager.ExitWork(boxesEarned, highestSessionMulti);
        highestSessionMulti = 1;

        if (boxesEarned >= 200)
        {
            SaveManager.Instance.timesWorkOver200Count++;
            if (!SaveManager.Instance.goal08Completion)
                GoalManager.GetComponent<GoalManager>().GoalUnlocked(8, true);

            if (!SaveManager.Instance.goal09Completion && SaveManager.Instance.timesWorkOver200Count >= 5)
                GoalManager.GetComponent<GoalManager>().GoalUnlocked(9, true);
        }
    }

    #endregion

    #region Multiplier Utility Methods

    /// <summary>
    /// Called during Start and runs the entire time the mini game is active.
    /// Takes care of constantly updating the fill that corresponds to multiplier 
    /// progress to be as accurate as possible.
    /// </summary>
    /// <returns>The fill.</returns>
    private IEnumerator UpdateFill()
    {
        while (true)
        {            
            float newFillAmount = streak / MiniGameDifficultyManager.Instance.WantedStreak;
            multiplierFill.fillAmount = iTween.FloatUpdate(multiplierFill.fillAmount, newFillAmount, 15.0f);	

            yield return null;
        }
    }

    /// <summary>
    /// Quickly flashes a single particle.
    /// the color taken in is used to distinguish between good/bad flashes.
    /// </summary>
    private IEnumerator FlashParticle(Color particleFlashColor)
    {
        Vector3 newScale = new Vector3(1.2f, 1.2f, 0); 
        ParticleSystem.EmissionModule em = particle.emission;
        multiplierFill.fillAmount = 0;
        particle.startColor = particleFlashColor;
        particle.Play();
        em.enabled = true;

        iTween.ScaleTo(particle.gameObject, iTween.Hash("scale", newScale, "time", 0.2f, "easetype", iTween.EaseType.easeOutBack));

        yield return new WaitForSeconds(0.2f);

        iTween.ScaleTo(particle.gameObject, iTween.Hash("scale", Vector3.zero, "time", 0.2f));

        yield return new WaitForSeconds(0.2f);

        em.enabled = false;
        particle.Stop();

        yield break;
    }

    /// <summary>
    /// Quickly shakes the container holding images pertaining to the multiplier.
    /// Used to indicate the incorrect placement of a box.
    /// </summary>
    /// <returns>The multiplier size.</returns>
    private IEnumerator JoltMultiplierSize()
    {
        Vector3 newScale = new Vector3(1.2f, 1.2f, 0); 

        iTween.ScaleTo(multiplierImages, iTween.Hash("scale", newScale, "time", 0.2f, "easetype", iTween.EaseType.easeOutBack));

        yield return new WaitForSeconds(0.2f);

        iTween.ScaleTo(multiplierImages, iTween.Hash("scale", Vector3.one, "time", 0.2f));

        yield break;
    }

    #endregion

    #region Event Subscription

    private void SubscribeToEvents()
    {
        MiniGameEventManager.OnCorrectBoxPlacement += IncreaseScoreAndStreak;
        MiniGameEventManager.OnIncorrectBoxPlacement += ResetStreak;
        MiniGameEventManager.OnBoxMissed += ResetStreak;

    }

    private void UnsubscribeFromEvents()
    {
        MiniGameEventManager.OnCorrectBoxPlacement -= IncreaseScoreAndStreak;
        MiniGameEventManager.OnIncorrectBoxPlacement -= ResetStreak;
        MiniGameEventManager.OnBoxMissed -= ResetStreak;
    }

    #endregion
}