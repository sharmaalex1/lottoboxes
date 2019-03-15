using UnityEngine;
using System.Collections;

public class InsuranceExpirationUI : MonoBehaviour
{

    private Vector3 originalPos;
    private Vector3 offScreenPos;

    private void Start()
    {
        iTween.Init(gameObject);

        DisasterManager.OnBuyInsurance += SlideInsuranceUI;
        DisasterManager.OnInsuranceExpiration += SlideInsuranceUI;

        originalPos = transform.position;
        offScreenPos = new Vector3(originalPos.x - 6, originalPos.y, originalPos.z);

        if (!SaveManager.Instance.IsInsuranceActive)
        {
            this.gameObject.transform.position = offScreenPos;
        }
    }


    private void OnDisable()
    {
        DisasterManager.OnBuyInsurance -= SlideInsuranceUI;
        DisasterManager.OnInsuranceExpiration -= SlideInsuranceUI;

        transform.position = originalPos;
    }

    private void SlideInsuranceUI()
    {
        Vector3 posToMoveTo = SaveManager.Instance.IsInsuranceActive ? originalPos : offScreenPos;
        iTween.MoveTo(this.gameObject, iTween.Hash("position", posToMoveTo, "time", 0.5f, "easetype", iTween.EaseType.spring));
    }
}
