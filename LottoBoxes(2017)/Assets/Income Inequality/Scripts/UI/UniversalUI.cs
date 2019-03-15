using UnityEngine;
using UnityEngine.UI;

public class UniversalUI : MonoBehaviour
{
    public Color poorClassLightColor, poorClassDarkColor, richClassLightColor, richClassDarkColor;
    public Image workTab, ticketTab, bonusTicketTab, boxesTab, billTab, billCircle, billUnderlayTab, loanSharkTab, loanSharkCircle, loanSharkUnderlayTab, settingsButton, settingsCog, insuranceTab, insuranceUnderlayTab,
                 insuranceCircle;
    public Text workText, ticketText, bonusTicketText, boxesText, billUIAmountText, loanSharkAmountText, loanSharkStillOwedText, insuranceTitleText, insuranceExpiresInText;

    private void Start()
    {        
        workText.color = Color.white;
        ticketText.color = Color.white;
        bonusTicketText.color = Color.white;
        boxesText.color = Color.white;
        settingsCog.color = Color.white;
        loanSharkAmountText.color = Color.white;
        billUIAmountText.color = Color.black;
        loanSharkStillOwedText.color = Color.white;
        insuranceExpiresInText.color = Color.white;

        switch (StatisticsManager.Instance.CurrentClass)
        {
            case Classes.Poor:
                {
                    workTab.color = poorClassLightColor;
                    ticketTab.color = poorClassLightColor;
                    bonusTicketTab.color = poorClassLightColor;
                    boxesTab.color = poorClassLightColor;
                    settingsButton.color = poorClassLightColor;
                    billTab.color = poorClassLightColor;
                    billCircle.color = poorClassLightColor;
                    billUnderlayTab.color = poorClassDarkColor;
                    loanSharkTab.color = poorClassLightColor;
                    loanSharkCircle.color = poorClassLightColor;
                    loanSharkUnderlayTab.color = poorClassDarkColor;
                    insuranceTab.color = poorClassLightColor;
                    insuranceCircle.color = poorClassLightColor;
                    insuranceUnderlayTab.color = poorClassDarkColor;
                }
                break;
            case Classes.Rich:
                {
                    workTab.color = richClassLightColor;
                    ticketTab.color = richClassLightColor;
                    bonusTicketTab.color = richClassLightColor;
                    boxesTab.color = richClassLightColor;
                    settingsButton.color = richClassLightColor;
                    billTab.color = richClassLightColor;
                    billCircle.color = richClassLightColor;
                    billUnderlayTab.color = richClassDarkColor;
                    loanSharkTab.color = richClassLightColor;
                    loanSharkCircle.color = richClassLightColor;
                    loanSharkUnderlayTab.color = richClassDarkColor;
                    insuranceTab.color = richClassLightColor;
                    insuranceCircle.color = richClassLightColor;
                    insuranceUnderlayTab.color = richClassDarkColor;
                }
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        loanSharkAmountText.text = SaveManager.Instance.BoxesOwed.ToString();
        billUIAmountText.text = SaveManager.Instance.BoxesOwedBill.ToString();
    }
}
