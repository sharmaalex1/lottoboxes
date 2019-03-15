using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles events/timeline of the intro comic
/// that the player sees when they first launch the game
/// </summary>
public class IntroComic : MonoBehaviour, IPointerDownHandler
{
    public GameObject introPageOne;
    public GameObject introPageTwo;
    public GameObject introPageThree;
    public GameObject introPageFour;
    public GameObject introPageFive;

    public GameObject pageFivePanelTwoPos;
    public GameObject pageFivePanelThreePos;

    private GameObject pgTwoPanelThree;

    public GameObject tapToContinue;

    private bool triplePanelStart = false;

    private int tapCounter = 0;

    private Color objectAlpha;

    //a function for other script to subscribe to
    //is then called when the window is closed
    public static event System.Action StartIntroManager;

    private void OnEnable()
    {
        StartCoroutine(SetPanelAlpha(tapToContinue, 4.0f));
    }

    private void OnDisable()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (tapCounter == 0)
        {
            GameObject pgOnePanelTwo = introPageOne.transform.GetChild(1).gameObject;
            StartCoroutine(SetPanelAlpha(pgOnePanelTwo, 4.0f));


            tapCounter++;
        }
        else if(tapCounter == 1)
        {
            introPageOne.SetActive(false);
            introPageTwo.SetActive(true);

            tapCounter++;
        }
        else if (tapCounter == 2)
        {
            StartCoroutine(PageTwoPanelTwoDelay());
        }
        else if (tapCounter == 3)
        {
            pgTwoPanelThree = introPageTwo.transform.GetChild(2).gameObject;
            StartCoroutine(SetPanelAlpha(pgTwoPanelThree, 4.0f));

            tapCounter++;
        }
        else if (tapCounter == 4)
        {

            pgTwoPanelThree = introPageTwo.transform.GetChild(2).gameObject;
            objectAlpha = pgTwoPanelThree.GetComponent<Image>().color;
            objectAlpha.a = 255.0f;
            pgTwoPanelThree.GetComponent<Image>().color = objectAlpha;

            GameObject pgTwoPanelFour = introPageTwo.transform.GetChild(3).gameObject;
            StartCoroutine(SetPanelAlpha(pgTwoPanelFour, 4.0f));

            tapCounter++;
        }
        else if(tapCounter == 5)
        {
            introPageTwo.SetActive(false);
            introPageThree.SetActive(true);

            tapCounter++;
        }
        else if (tapCounter == 6)
        {

            if (!triplePanelStart)
            {
                triplePanelStart = true;
                StartCoroutine(PageThreeTriplePanel());
            }
        }
        else if (tapCounter == 7)
        {
            GameObject pgThreePanelFive = introPageThree.transform.GetChild(4).gameObject;
            StartCoroutine(SetPanelAlpha(pgThreePanelFive, 4.0f));

            tapCounter++;
        }
        else if(tapCounter == 8)
        {

            introPageThree.SetActive(false);
            introPageFour.SetActive(true);

            tapCounter++;
        }
        else if (tapCounter == 9)
        {
            GameObject pgFourPanelTwo = introPageFour.transform.GetChild(1).gameObject;
            StartCoroutine(SetPanelAlpha(pgFourPanelTwo, 4.0f));

            tapCounter++;
        }
        else if(tapCounter == 10)
        {
            GameObject pgFourPanelThree = introPageFour.transform.GetChild(2).gameObject;
            StartCoroutine(SetPanelAlpha(pgFourPanelThree, 4.0f));
            StartCoroutine(PageFourThoughtBubbles());

            tapCounter++;
        }
        else if (tapCounter == 11)
        {
            introPageFour.SetActive(false);
            introPageFive.SetActive(true);

            tapCounter++;
        }
        else if(tapCounter == 12)
        {
            StartCoroutine(PageFiveTwoMoving());

            tapCounter++;
        }
        else if(tapCounter == 13)
        {
            GameObject pgFivePanelFour = introPageFive.transform.GetChild(3).gameObject;
            StartCoroutine(SetPanelAlpha(pgFivePanelFour, 4.0f));

            tapCounter++;
        }
        else if(tapCounter == 14)
        {
            EndIntro();

            Scenes sceneToGoTo = SaveManager.Instance.UnlockedClasses.Contains(Classes.Rich) ? Scenes.ClassSelection : Scenes.ClassRoulette;
            SceneTransition.Instance.TriggerSceneChangeEvent(sceneToGoTo);
            AudioManager.Instance.PlayAudioClip(BGMType.Working);
        }
    }

    /// <summary>
    /// Game object must have an image component for this to work
    /// </summary>
    private IEnumerator SetPanelAlpha(GameObject panelToSet, float timeToSet)
    {
        Color panelAlpha;

        if (panelToSet.GetComponent<Text>())
        {
            panelAlpha = panelToSet.GetComponent<Text>().color;

            while (panelAlpha.a < 255.0f)
            {

                panelAlpha.a += Mathf.SmoothStep(0.0f, 255.0f, Time.deltaTime / timeToSet);

                panelToSet.GetComponent<Text>().color = panelAlpha;

                yield return null;
            }
        }
        else
        {
            panelAlpha = panelToSet.GetComponent<Image>().color;

            while (panelAlpha.a < 255.0f)
            {

                panelAlpha.a += Mathf.SmoothStep(0.0f, 255.0f, Time.deltaTime / timeToSet);

                panelToSet.GetComponent<Image>().color = panelAlpha;

                yield return null;
            }
        }

        yield break;
    }

    private IEnumerator PageTwoPanelTwoDelay()
    {
        GameObject pgTwoPanelTwo = introPageTwo.transform.GetChild(1).gameObject;
        StartCoroutine(SetPanelAlpha(pgTwoPanelTwo, 6.0f));

        yield return new WaitForSeconds(0.5f);

        tapCounter++;

        yield break;
    }

    private IEnumerator PageThreeTriplePanel()
    {
        GameObject pgThreePanelTwo = introPageThree.transform.GetChild(1).gameObject;
        StartCoroutine(SetPanelAlpha(pgThreePanelTwo, 4.0f));
        iTween.ScaleFrom(pgThreePanelTwo, new Vector3(pgThreePanelTwo.transform.localScale.x + 0.5f, pgThreePanelTwo.transform.localScale.y + 0.5f), 1.0f);

        yield return new WaitForSeconds(0.5f);

        GameObject pgThreePanelThree = introPageThree.transform.GetChild(2).gameObject;
        StartCoroutine(SetPanelAlpha(pgThreePanelThree, 4.0f));
        iTween.ScaleFrom(pgThreePanelThree, new Vector3(pgThreePanelThree.transform.localScale.x + 0.5f, pgThreePanelThree.transform.localScale.y + 0.5f), 1.0f);

        yield return new WaitForSeconds(0.5f);

        GameObject pgThreePanelFour = introPageThree.transform.GetChild(3).gameObject;
        StartCoroutine(SetPanelAlpha(pgThreePanelFour, 4.0f));
        iTween.ScaleFrom(pgThreePanelFour, new Vector3(pgThreePanelFour.transform.localScale.x + 0.5f, pgThreePanelFour.transform.localScale.y + 0.5f), 1.0f);

        tapCounter++;

        yield break;
    }

    private IEnumerator PageFourThoughtBubbles()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject goodThoughtBubble = introPageFour.transform.GetChild(3).gameObject;
        StartCoroutine(SetPanelAlpha(goodThoughtBubble, 1.0f));
        iTween.ScaleFrom(goodThoughtBubble, new Vector3(goodThoughtBubble.transform.localScale.x + 1.0f, goodThoughtBubble.transform.localScale.y + 1.0f), 1.0f);

        yield return new WaitForSeconds(0.5f);

        GameObject badThoughtBubble = introPageFour.transform.GetChild(4).gameObject;
        StartCoroutine(SetPanelAlpha(badThoughtBubble, 1.0f));
        iTween.ScaleFrom(badThoughtBubble, new Vector3(badThoughtBubble.transform.localScale.x + 1.0f, badThoughtBubble.transform.localScale.y + 1.0f), 1.0f);

        yield break;
    }

    private IEnumerator PageFiveTwoMoving()
    {
        GameObject pgFivePanelTwo = introPageFive.transform.GetChild(1).gameObject;
        GameObject pgFivePanelThree = introPageFive.transform.GetChild(2).gameObject;

        iTween.MoveTo(pgFivePanelTwo, iTween.Hash("position", new Vector3(pageFivePanelTwoPos.transform.position.x, pageFivePanelTwoPos.transform.position.y), "time", 2.0f));

        yield return new WaitForSeconds(0.2f);

        iTween.MoveTo(pgFivePanelThree, iTween.Hash("position", new Vector3(pageFivePanelThreePos.transform.position.x, pageFivePanelThreePos.transform.position.y), "time", 2.0f));

        yield break;
    }

    //Function called by an event trigger that closes the window
    public void EndIntro()
    {
        SaveManager.Instance.CompletedIntro = true;

        if (StartIntroManager != null)
        {
            StartIntroManager();
        }
    }
}
