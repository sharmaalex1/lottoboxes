using UnityEngine;
using UnityEditor;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections;

public class ResetSaveData : MonoBehaviour {


    [MenuItem("Petricore Tools/Reset Saved Data")]
    /// <summary>
    /// Deletes all saved data. 
    /// Also clears PlayerPrefs.
    /// </summary>
    public static void ResetData()
    {
        // Clear playerprefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // If the file exists, delete it
        if (File.Exists(StaticVars.PATH_SAVE_DATA))
        {
            File.Delete(StaticVars.PATH_SAVE_DATA);

#if SAVE_DEBUGGING || UNITY_EDITOR
            Debug.Log("File deleted successfully.");
#endif
        }
        else
        {
#if SAVE_DEBUGGING || UNITY_EDITOR
            Debug.LogWarning("There is no saved data to reset.");
#endif
        }
    }
}
