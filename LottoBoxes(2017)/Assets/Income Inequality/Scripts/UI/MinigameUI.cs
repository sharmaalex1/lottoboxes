using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class MinigameUI : MonoBehaviour
{

    //empty now, but will eventually need to do stuff for when mini game is finished
    #region Singleton
    //private static instance for singleton
    private static MinigameUI instance;
    //public readonly property for other scripts to access instance
    public static MinigameUI Instance
    {
        get { return instance;}
        private set { }
    }

    //private constructor so other scripts can't make instances
    private MinigameUI() { }
    #endregion


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

}


