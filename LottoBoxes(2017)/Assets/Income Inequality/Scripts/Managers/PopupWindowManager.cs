using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum InfoStrings
{
    AssetPoor,
    OrdinancesLimitingPaydayLending,
    MoreLoanStoresThanStarbucks,
    MayPaydayLoan,
    LendingCircleAlternatives,
    AvoidDeeperDebt,
}

public class PopupWindowManager : MonoBehaviour
{
    private static PopupWindowManager instance;

    public static PopupWindowManager Instance
    {
        get
        {
            return instance;
        }
    }

    private Vector3 originalPos;
    private Vector3 offScreenPos;

    [Header("Popup UI Elements")]
    public GameObject popupUI;
    public Text infoText;
    public Button cancelButton;
    public Button webpageVisitButton;

    [Header("Potential Info Strings")]
    public string[] infoString;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Object.Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        originalPos = popupUI.transform.position;
        offScreenPos = new Vector3(originalPos.x - 3.5f, originalPos.y, originalPos.z);

        popupUI.transform.position = offScreenPos;
        popupUI.SetActive(true);

        cancelButton.onClick.AddListener(() => SlideChatBubble(false));
        webpageVisitButton.onClick.AddListener(() =>
            {
                Application.OpenURL("http://www.siliconvalleycf.org/lottoboxes");
                SlideChatBubble(false);
                MixpanelManager.InfoPopUpClicked();
            });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ActivatePopup(InfoStrings.AssetPoor);
        }
    }

    private void SlideChatBubble(bool isSlidingIn = true)
    {
        Vector3 posToMoveTo = isSlidingIn ? originalPos : offScreenPos;
        iTween.MoveTo(popupUI, iTween.Hash("position", posToMoveTo, "time", 0.5f, "easetype", iTween.EaseType.spring));
    }

    /// <summary>
    /// Function to be called by other scripts when certain things occur and a popup
    /// with specific text desired.
    /// Ex: Taking out a loan and wanting to show a correspnding popup with info about loans.
    /// </summary>
    public void ActivatePopup(InfoStrings info)
    {
        string text = infoString[(int)info];

        infoText.text = text;
        popupUI.SetActive(true);
        SlideChatBubble();

        MixpanelManager.ShowInfoPopUp(text);
    }
}
