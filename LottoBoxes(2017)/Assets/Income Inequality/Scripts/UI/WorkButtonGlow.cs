using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class WorkButtonGlow : MonoBehaviour {

    private Image image;
    //time to go to yellow and change back
    private const float COLOR_CHANGE_TIME = 0.5f;

    private Color colorToTurn;
    private Coroutine glowRoutine = null;

    private Color startingColor;
    private Color colorDifference;

    private IEnumerator Start()
    {
        image = GetComponent<Image>();
        LoanManager.OnLoanDenied += StartGlow;
        ExpensesManager.OnPartialBillCollected += StartGlow;

        yield return new WaitForEndOfFrame();
        if (SaveManager.Instance.CurrentBoxCount == 0)
        {
            StartGlow();
        }

    }

    //we will not be using iTween here, due to the fact that in StopGlowing, we would need to call itween.Stop which might causes issues
    //So we do the glowing manually
    private IEnumerator Glow(float timeToWait)
    { 

        float timer = 0;
        bool glow = true;

        if(timeToWait > 0)
        {
            yield return new WaitForSeconds(timeToWait);
        }


        while(true)
        {
            if(glow)
            {
                timer += Time.deltaTime;
                if(timer >= COLOR_CHANGE_TIME)
                {
                    timer = COLOR_CHANGE_TIME;
                    glow = false;
                }
                SetColor(timer);
            }

            else if (!glow)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    timer = 0;
                    glow = true;
                }
                SetColor(timer);
            }

            yield return null;
        }
    }

    private void StartGlow()
    {
        //get starting color
        startingColor = image.color;
        //divide the color in half to make it darker
        colorToTurn = startingColor / 2f;
        //keep the same alpha 
        colorToTurn.a = startingColor.a;
        colorDifference = colorToTurn - startingColor;
        glowRoutine = StartCoroutine(Glow(ShiftsManager.Instance.WaitForWorkTimer));
    }

    private void OnDestroy()
    {
        LoanManager.OnLoanDenied -= StartGlow;
        ExpensesManager.OnPartialBillCollected -= StartGlow;
    }

    public void StopGlowing()
    {
        if (glowRoutine != null)
        {
            StopCoroutine(glowRoutine);
            image.color = startingColor;
            glowRoutine = null;
        }
    }


    private void SetColor(float time)
    {
        time = Mathf.Clamp(time, 0, COLOR_CHANGE_TIME); 

        float percent = Mathf.Clamp01(time / COLOR_CHANGE_TIME);

        Color newCol = startingColor + (percent * colorDifference);
        newCol.a = startingColor.a;
        image.color = newCol;
    }
}
