using UnityEngine;
using System.Collections;

public class BillPaymentUI : MonoBehaviour
{
    private Vector3 originalPos;
    private Vector3 offScreenPos;

    private void Start()
    {
        iTween.Init(gameObject);

        ExpensesManager.OnFirstBill += SlideBillUI;

        originalPos = transform.position;
        offScreenPos = new Vector3(originalPos.x - 10, originalPos.y, originalPos.z);

        if (!SaveManager.Instance.IsBillActive)
        {
            this.gameObject.transform.position = offScreenPos;
        }
    }

    private void OnDisable()
    {
        ExpensesManager.OnFirstBill -= SlideBillUI;

        transform.position = originalPos;
    }

    private void SlideBillUI()
    {
        Vector3 posToMoveTo = SaveManager.Instance.IsBillActive ? originalPos : offScreenPos;
        iTween.MoveTo(this.gameObject, iTween.Hash("position", posToMoveTo, "time", 0.5f, "easetype", iTween.EaseType.spring));
    }

}
