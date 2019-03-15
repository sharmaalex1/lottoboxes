using UnityEngine;
using System.Collections;

public class PopulateLibrary : MonoBehaviour
{

    public GameObject goalTemplate;
    public GameObject GoalManager;
    //public GameObject GoalClip;
    int numberOfGoals;


    // Use this for initialization
    void Start()
    {
        RectTransform goalArea = this.GetComponent<RectTransform>() as RectTransform;
        goalArea.pivot = new Vector2(0.5f, 1);

        numberOfGoals = GoalManager.GetComponent<GoalLibrary>().numberOfGoals;

        Vector3 lastLocation = new Vector3(0, ((36*(numberOfGoals-1))/2)-2, 0);

        for (int i = 0; i < numberOfGoals; i++)
        {
            GameObject lastGoalMade = (GameObject)Instantiate(goalTemplate, lastLocation, Quaternion.identity);
            lastGoalMade.transform.SetParent(transform, false);
            lastGoalMade.GetComponent<GoalLibraryUIChanger>().goalIdentifier = GoalManager.GetComponent<GoalLibrary>().goalIdentifiers[i];
            Debug.Log("Made a library entry with identifier: " + GoalManager.GetComponent<GoalLibrary>().goalIdentifiers[i].ToString());
            lastLocation = new Vector3(lastLocation.x, lastLocation.y - 36f, 0);
            goalArea.sizeDelta = new Vector2(100, goalArea.sizeDelta.y + 36);
            lastGoalMade.name = GoalManager.GetComponent<GoalLibrary>().GetTitle(GoalManager.GetComponent<GoalLibrary>().goalIdentifiers[i]);
            lastGoalMade.SetActive(true);
            GoalManager.GetComponent<GoalLibrary>().entryArrayPopulation(lastGoalMade, i);
        }

    }

}