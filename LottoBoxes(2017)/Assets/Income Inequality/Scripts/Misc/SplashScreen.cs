using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System;

public class SplashScreen : MonoBehaviour
{
    public Image[] logoMasks;
    public Image blackFade;
    private float timer = 0.0f;

    void Start()
    {
        for(int i = 0; i < logoMasks.Length; i++)
        {
            if(i % 2 == 0)
            {
                logoMasks[i].fillOrigin = 1;
                logoMasks[i].transform.GetChild(0).transform.localPosition = Vector3.down * 400;
            }
            else
            {
                logoMasks[i].transform.GetChild(0).transform.localPosition = Vector3.up * 400;
                logoMasks[i].fillOrigin = 0;
            }

            logoMasks[i].fillAmount = 0;
        }

        blackFade.canvasRenderer.SetAlpha(0);
        StartCoroutine(AnimateSplashRoutine());
    }

    protected virtual IEnumerator AnimateSplashRoutine()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1.0f);
        AsyncOperation async = SceneManager.LoadSceneAsync(1);
        async.allowSceneActivation = false;

        //Had to write these one by one instead of a for loop because of lambda + onupdate interaction conflicting with each other and the index

        float delayBetweenEachMask = 0.15f;
        #region Fill amount mask
        iTween.ValueTo(this.gameObject, iTween.Hash
        (
            "delay", 0f,
            "from", 0,
            "to", 1.0f,
            "onupdate", (Action<object>)(newVal =>
            {
                logoMasks[0].fillAmount = (float)newVal;
            }),
            "time", 0.6f,
            "easetype", iTween.EaseType.easeInCubic
        ));

        yield return new WaitForSeconds(delayBetweenEachMask);

        iTween.ValueTo(this.gameObject, iTween.Hash
        (
            "delay", 0f,
            "from", 0,
            "to", 1.0f,
            "onupdate", (Action<object>)(newVal =>
            {
                logoMasks[1].fillAmount = (float)newVal;
            }),
            "time", 0.6f,
            "easetype", iTween.EaseType.easeInCubic
        ));

        yield return new WaitForSeconds(delayBetweenEachMask);

        iTween.ValueTo(this.gameObject, iTween.Hash
        (
            "delay", 0f,
            "from", 0,
            "to", 1.0f,
            "onupdate", (Action<object>)(newVal =>
            {
                logoMasks[2].fillAmount = (float)newVal;
            }),
            "time", 0.6f,
            "easetype", iTween.EaseType.easeInCubic
        ));

        yield return new WaitForSeconds(delayBetweenEachMask);

        iTween.ValueTo(this.gameObject, iTween.Hash
        (
            "delay", 0f,
            "from", 0,
            "to", 1.0f,
            "onupdate", (Action<object>)(newVal =>
            {
                logoMasks[3].fillAmount = (float)newVal;
            }),
            "time", 0.6f,
            "easetype", iTween.EaseType.easeInCubic
        ));

        #endregion

        for (int i = 0; i < logoMasks.Length; i++)
        {
            iTween.MoveTo(logoMasks[i].transform.GetChild(0).gameObject,
                iTween.Hash
                (
                    "position", Vector3.zero,
                    "time", 0.5f,
                    "delay", 0.15 + 0.15f * i,
                    "islocal", true,
                    "easetype", iTween.EaseType.easeOutBack
                ));
        }

        yield return new WaitForSeconds(2.85f);
        blackFade.CrossFadeAlpha(1.0f, 0.35f, false);
        yield return new WaitForSeconds(0.35f);
        async.allowSceneActivation = true;
        yield return async;
    }
}