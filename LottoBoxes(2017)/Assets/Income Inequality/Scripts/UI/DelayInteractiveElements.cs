using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Add this to buttons/objects whose interactivity you want to only turn back on until
/// a certain amount of time has passed. May need to make special cases if object
/// does not contain a button component
/// </summary>
public class DelayInteractiveElements : MonoBehaviour
{
    private Coroutine interactiveButtonRoutine;
    private Color ButtonColorFade;
    private Color ButtonColorRegular;

    public Sprite ButtonPressed;
    public Sprite ButtonUnpressed;

    private Color TapToContinue;
    public GameObject TapObject;

    private Color WorkTapToContinue;
    private GameObject WorkTapObject;

    private GameObject PostWorkTapObject;

    private void OnEnable()
    {
        StartCoroutine(SetButtonNonInteractive());

        if (this.gameObject.name == "PostMiniGameResults")
        {
            PostWorkTapObject = this.gameObject.transform.GetChild(0).transform.GetChild(3).gameObject;

            PostWorkTapObject.SetActive(false);
        }

        if (this.gameObject.name == "EarningsWindow")
        {
            this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

            this.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }

        if (this.gameObject.name != "OpportunityBoxTutorial" && this.gameObject.name != "TutorialPopUp" && this.gameObject.name != "PostMiniGameResults" && this.gameObject.name != "EarningsWindow")
        {
            //ButtonColorFade = this.gameObject.GetComponent<Image>().color;

            //ButtonColorFade = new Color(126.0f, 126.0f, 126.0f);

            this.gameObject.GetComponent<Image>().sprite = ButtonPressed;

            //this.gameObject.GetComponent<Image>().color = ButtonColorFade;
        }

        interactiveButtonRoutine = StartCoroutine(SetButtonInteractive());
    }

    public void SetWorkTutortialNonInteractive()
    {
        if (this.gameObject.name == "TutorialPopUp")
        {
            this.gameObject.GetComponent<Image>().raycastTarget = false;
            WorkTapObject.SetActive(false);
        }
    }

    public void StartButtonInteractiveRoutine()
    {
        interactiveButtonRoutine = StartCoroutine(SetButtonInteractive());
    }

    private IEnumerator SetButtonInteractive()
    {
        yield return new WaitForSeconds(1.5f);

        if(this.gameObject.name == "TutorialPopUp")
        {
            WorkTapObject = this.gameObject.transform.GetChild(1).gameObject;

            this.gameObject.GetComponent<Image>().raycastTarget = true;

            WorkTapObject.SetActive(true);

            yield break;
        }

        if (this.gameObject.name == "PostMiniGameResults")
        {
            this.gameObject.GetComponent<Image>().raycastTarget = true;

            PostWorkTapObject.SetActive(true);

            yield break;
        }

        if (this.gameObject.name == "EarningsWindow")
        {
            this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;

            this.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);

            yield break;
        }

        if (this.gameObject.name != "OpportunityBoxTutorial")
        {
            //ButtonColorFade = this.gameObject.GetComponent<Image>().color;

            // ButtonColorFade = new Color(255.0f, 255.0f, 255.0f);

            //this.gameObject.GetComponent<Image>().color = ButtonColorFade;

            this.gameObject.GetComponent<Image>().sprite = ButtonUnpressed;

            this.gameObject.GetComponent<Button>().interactable = true;
        }
        else if(this.gameObject.name == "OpportunityBoxTutorial")
        {
            TapToContinue = TapObject.GetComponent<Text>().color;

            TapToContinue.a = 255.0f;

            TapObject.GetComponent<Text>().color = TapToContinue;

            this.gameObject.GetComponent<Button>().interactable = true;
        }

        yield break;
    }

    private IEnumerator SetButtonNonInteractive()
    {
        if (this.gameObject.name == "TutorialPopUp")
        {
            WorkTapObject = this.gameObject.transform.GetChild(1).gameObject;

            this.gameObject.GetComponent<Image>().raycastTarget = false;

            WorkTapObject.SetActive(true);

            yield break;
        }

        if (this.gameObject.name == "PostMiniGameResults")
        {
            this.gameObject.GetComponent<Image>().raycastTarget = false;

            yield break;
        }

        if (this.gameObject.name == "EarningsWindow")
        {
            this.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

            this.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);

            yield break;
        }

        if (this.gameObject.name != "OpportunityBoxTutorial")
        {
            this.gameObject.GetComponent<Image>().sprite = ButtonPressed;
            this.gameObject.GetComponent<Button>().interactable = false;
        }
        else if (this.gameObject.name == "OpportunityBoxTutorial")
        {
            this.gameObject.GetComponent<Button>().interactable = false;
        }

        yield break;
    }
}
