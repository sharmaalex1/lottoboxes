using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIToggle : MonoBehaviour
{
    public int idNum;
    public List<GameObject> objectsToToggle;

    private void Start()
    {
        UISlider.OnSlide += CheckIfInView;

        //If we aren't the first element, deactivate all elements that shouldn't be active.
        if (idNum != 0)
        {
            ToggleObjects(false);
        }
    }

    private void OnDestroy()
    {
        UISlider.OnSlide -= CheckIfInView;
    }

    private void CheckIfInView(int curIndex)
    {
        if (idNum == curIndex)
        {
            StartCoroutine(ToggleObjects(true, 0));
        }
        else
        {
            ToggleObjects(false);
        }
    }

    private void ToggleObjects(bool status)
    {
        for (int i = 0; i < objectsToToggle.Count; i++)
        {
            objectsToToggle[i].SetActive(status);	
        }
    }

    private IEnumerator ToggleObjects(bool status, float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < objectsToToggle.Count; i++)
        {
            objectsToToggle[i].SetActive(status);	
        }

        yield break;
    }
}
