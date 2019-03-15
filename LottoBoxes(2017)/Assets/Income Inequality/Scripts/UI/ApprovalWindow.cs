using UnityEngine;
using System.Collections;

public class ApprovalWindow : MonoBehaviour
{

    // Use this for initialization
    void OnEnable()
    {
        SaveManager.Instance.isApproved = true;
        Debug.Log("Approval window has been shown");
		MixpanelManager.FeedbackWindowDisplayed ();
    }
}
