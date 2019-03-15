using UnityEngine;
using System.Collections;

public class LoanRepaymentUI : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 offScreenPos;

    private void Start()
    {
        iTween.Init(gameObject);

        LoanManager.OnLoanDisbursed += SlideLoanUI;
        LoanManager.OnFullLoanCollected += SlideLoanUI;

        originalPos = transform.position;
        offScreenPos = new Vector3(originalPos.x - 6, originalPos.y, originalPos.z);

        if (!SaveManager.Instance.IsLoanActive)
        {
            this.gameObject.transform.position = offScreenPos;
        }
    }

    private void OnDisable()
    {
        LoanManager.OnLoanDisbursed -= SlideLoanUI;
        LoanManager.OnFullLoanCollected -= SlideLoanUI;

        transform.position = originalPos;
    }

    private void SlideLoanUI()
    {
        Vector3 posToMoveTo = SaveManager.Instance.IsLoanActive ? originalPos : offScreenPos;
        iTween.MoveTo(this.gameObject, iTween.Hash("position", posToMoveTo, "time", 0.5f, "easetype", iTween.EaseType.spring));
    }

}
