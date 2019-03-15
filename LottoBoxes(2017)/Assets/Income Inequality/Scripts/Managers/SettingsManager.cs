using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles non-audio settings, such as whether
/// disaster or expenses systems should be functional, etc.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    #region Singleton Setup
    private static SettingsManager instance;

    public static SettingsManager Instance
    {
        get
        {
            return instance;
        }
        private set { }
    }

    private SettingsManager()
    {
    }

    #endregion

    #region Game Settings

    public GameObject expensesManagerRef;
    public GameObject disasterMangerRef;

    private bool disasterToggle;
    private bool expensesToggle;
    
    // toggle used in DisasterManager
    private bool firstOfferEver;

    public void SetDisasterToggle(bool toggleState)
    {
        disasterToggle = toggleState;
        SaveManager.Instance.DisasterToggle = toggleState;
    }

    public bool GetDisasterToggle()
    {
        return disasterToggle;
    }

    public void SetExpensesToggle(bool toggleState)
    {
        expensesToggle = toggleState;
        SaveManager.Instance.ExpensesToggle = toggleState;
    }

    public bool GetExpensesToggle()
    {
        return expensesToggle;
    }

    public void SetFirstOfferEver(bool firstOfferToggle)
    {
        firstOfferEver = firstOfferToggle;
        SaveManager.Instance.FirstOfferEver = firstOfferToggle;
    }

    public bool GetFirstOfferEver()
    {
        return firstOfferEver;
    }

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        //regular singleton checks
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(transform.root.gameObject);   

            // grab relevant save data
            disasterToggle = SaveManager.Instance.DisasterToggle;
            expensesToggle = SaveManager.Instance.ExpensesToggle;
            firstOfferEver = SaveManager.Instance.FirstOfferEver;

            if(disasterToggle)
            {
                disasterMangerRef.SetActive(true);
            }
            else
            {
                disasterMangerRef.SetActive(false);
            }

            if(expensesToggle)
            {
                expensesManagerRef.SetActive(true);
            }
            else
            {
                expensesManagerRef.SetActive(false);
            }

            
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        expensesManagerRef = GameObject.Find("ExpensesManager");
        disasterMangerRef = GameObject.Find("DisasterManager");
    }

    #endregion
}
