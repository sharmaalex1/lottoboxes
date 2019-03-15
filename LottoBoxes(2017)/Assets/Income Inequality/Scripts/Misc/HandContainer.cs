using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Container Class that contains all the possible hands that can be used
/// </summary>
public class HandContainer : MonoBehaviour 
{
    [Header("Hand Motors")]
    [SerializeField]
    private GameObject handPrefab;

    #region Singleton Setup
    private static HandContainer instance;

    public static HandContainer Instance
    {
        get
        {
            return instance;
        }
        private set { }
    }

    private HandContainer()
    {

    }
    #endregion
    
    private void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public GameObject GetHandPair()
    {
        return handPrefab;
    }
}
