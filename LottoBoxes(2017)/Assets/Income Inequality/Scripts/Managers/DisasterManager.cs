using System;



using System.Collections;



using System.Collections.Generic;



using UnityEngine;



using UnityEngine.UI;







/// <summary>



/// Handles turning on and off Disaster UI and Insurance UI



/// Triggers Disaster UI to popup after a randomized range of time has passed



/// Takes away boxes from the player unless they have purchased insurance



/// Amount of boxes taken varies depending on what disaster triggers



/// Player can be left with a negative box balance

    
    
/// Additionally, whenever you are trying to reset data/going inbetween scenes

    
/// you MUST reset all disaster/insurance data to its default values.

    
    
/// This prevents a variety of issues where the system breaks itself



/// upon loading in again



/// </summary>



public class DisasterManager : MonoBehaviour
{



	private const int DISASTER_COUNTDOWN_TIME_IN_SECONDS = 360;



	private const int INSURANCE_COUNTDOWN_TIME_IN_SECONDS = 750;











	public static event Action OnBuyInsurance;



	public static event Action OnInsuranceExpiration;







	private bool firstRun = true;



	private bool firstInsuranceOffer = true;



	private bool isInsuranceActive;

    public bool IsInsuranceActive
    {
        get
        {
            return isInsuranceActive;
        }
    }



	private bool disasterOccurred;



	private bool firstInsurancePopUpToggle = true;



	private bool secondInsurancePopUpToggle = true;







	private int disasterChance;



	private int disasterType;



	private float boxDeductionModifier = 1.0f;



	private float timeActive;







	private DateTime timeOfDisaster;

	private TimeSpan countdownToDisaster;

	private DateTime timeOfInsuranceExpiration;

	private DateTime timeOfInsurancePurchase;

	private TimeSpan insuranceExpirationTimer;

	private DateTime timeOfGoal12Fail;







	public GameObject canvasContainerRef;

	public GameObject insuranceNotEnoughBoxesUI;







	// Tornado  prefab



	public GameObject tornadoFab;







	public GameObject earthquakeLineOne;



	public GameObject earthquakeLineTwo;



	public GameObject lightningStrike;



	public GameObject RainEmitter;







	// Positions for where the tornado and earthquake animations/images need to go



	// probably should just set as as points later rather than grabbing a ref to the game object itself



	public Camera mainCamera;






	public RectTransform earthquakePosOne;



	public RectTransform earthquakePosTwo;



	public RectTransform earthquakePosThree;



	public RectTransform earthquakePosFour;



	public RectTransform lightningStrikeOne;



	public RectTransform lightningStrikeTwo;



	public RectTransform lightningStrikeThree;



	public RectTransform lightningStrikeFour;



	public RectTransform rain;







	public RectTransform TornadoStart;



	public RectTransform TornadoPosOne;







	// This on the "BoxesOwned" UI element.



	public GameObject BoxPair;



	public GameObject[] gameSystemsUI;



	public GameObject loanUI;



	public GameObject expensesUI;



	public GameObject GoalManager;



	// Entire DisasterUI element in the Unity scene



	public GameObject disasterUI;



	public Text disasterBoxesTaken;



	public Text disasterTypeText;

    public Text disasterInsuranceText;







	public GameObject insuranceTimerBar;



	public GameObject insuranceUI;



	public Text insuranceDeal;

    public Text insuranceQuoteBubbleRich;

    public Text insuranceQuoteBubblePoor;

    public Text boxInsuranceAmount;



	public Text timeUntilInsuranceExpirationText;




	public GameObject DisasterEffectsUI;



	public GameObject WealthyBackgroundUI;



	public GameObject PoorBackgroundUI;

    public GameObject PoorBackgroundElements;

	// Position of the BoxesOwned UI element in the scene.



	private RectTransform boxesUIPosition;


    private GameObject opportunityBox;

    private TimeSpan opportunityBoxTimeToAdd;


    private Coroutine disasterTimerCoroutine;
    private Coroutine insuranceTimerCoroutine;
    public Coroutine disasterEffects;

    private Vector3 wealthyUIPos;
    private Vector3 poorUIPos;

    private GameObject QuakeLineOne;
    private GameObject QuakeLineTwo;
    private GameObject QuakeLineThree;
    private GameObject QuakeLineFour;
    private GameObject tornado;

    public Button workButton;
    public Button settingsButton;



    #region UnityCallbacks







    // Use this for initialization



    private void OnEnable ()
	{
        SubscribeToEvents();

        firstRun = SaveManager.Instance.FirstRun;



		firstInsuranceOffer = SaveManager.Instance.FirstInsurnaceOffer;



		disasterOccurred = SaveManager.Instance.DisasterOccurred;



		isInsuranceActive = SaveManager.Instance.IsInsuranceActive;





        Debug.Log ("First Run?: " + firstRun);



		Debug.Log ("First Insurance Offer?: " + firstInsuranceOffer);



		Debug.Log ("First Offer Ever?: " + SettingsManager.Instance.GetFirstOfferEver ());



		Debug.Log ("Is Insurance Active?: " + isInsuranceActive);



		Debug.Log ("First Insurance PopUp Toggle: " + firstInsurancePopUpToggle);



		Debug.Log ("Second Insurance PopUp Toggle: " + secondInsurancePopUpToggle);



		if (!SettingsManager.Instance.GetFirstOfferEver () && !disasterOccurred)
        {

            if (isInsuranceActive)
            {

                timeOfInsuranceExpiration = SaveManager.Instance.TimeOfInsuranceExpiration;

				insuranceExpirationTimer = timeOfInsuranceExpiration.Subtract (DateTime.Now);



				Debug.Log ("Time of insurance expiration: " + timeOfInsuranceExpiration);



				timeOfGoal12Fail = SaveManager.Instance.TimeOfGoal12Fail;



				insuranceTimerCoroutine = StartCoroutine (UpdateInsuranceTimer ());

			}



			timeOfDisaster = SaveManager.Instance.TimeOfDisaster;



			countdownToDisaster = timeOfDisaster.Subtract (DateTime.Now);


            Debug.Log ("Time of Disaster: " + timeOfDisaster);



			disasterTimerCoroutine = StartCoroutine (UpdateDisasterTimer ());



		}



	}







	private void OnDisable ()
	{
        UnSubscribeToEvents();
	}







	private void Start ()
	{

		boxesUIPosition = GameObject.FindGameObjectWithTag ("BoxOwnedUI").GetComponent<RectTransform> ();

        

	}







	private void Update ()
	{

#if UNITY_EDITOR







		if (Input.GetKeyDown (KeyCode.C)) {



			TestDisaster ();



		}







		if (Input.GetKeyDown (KeyCode.X)) {



			ShowInsurancePopup ();



		}







		if (Input.GetKeyDown (KeyCode.M)) {



			TestInsuranceEffect ();



		}







		if (Input.GetKeyDown (KeyCode.V)) {



			disasterUI.SetActive (true);



		}



#endif



	}



	#endregion







	#region Button Methods







	/// <summary>



	/// This method is called on a button press.



	/// Handles deducting the appropriate amount of boxes.



	/// </summary>



	public void AcceptInsurnace ()
	{

        int insuranceCost = 50;

        if(SceneTransition.Instance.SelectedClass == Classes.Rich)
        {
            insuranceCost = 100;
        }
        else if (SceneTransition.Instance.SelectedClass == Classes.Poor)
        {
            insuranceCost = 200;
        }

		if (SaveManager.Instance.CurrentBoxCount < insuranceCost)
        {



			insuranceNotEnoughBoxesUI.SetActive (true);



		} else {



			BuyInsurance ();







			timeOfDisaster = DateTime.Now.Add (new TimeSpan (0, 0, DISASTER_COUNTDOWN_TIME_IN_SECONDS));



			timeOfGoal12Fail = DateTime.Now.Add (new TimeSpan (0, 0, 30));



			timeOfInsuranceExpiration = DateTime.Now.Add (new TimeSpan (0, 0, INSURANCE_COUNTDOWN_TIME_IN_SECONDS));



			timeOfInsurancePurchase = DateTime.Now;







			isInsuranceActive = true;







			insuranceTimerCoroutine = StartCoroutine (UpdateInsuranceTimer ());



			disasterTimerCoroutine = StartCoroutine (UpdateDisasterTimer ());



			SettingsManager.Instance.SetFirstOfferEver (false);







			insuranceUI.SetActive (false);







			StatisticsManager.Instance.UpdateDisasterInfo (timeOfDisaster, timeOfInsuranceExpiration, isInsuranceActive, firstInsuranceOffer, disasterOccurred, firstRun, firstInsurancePopUpToggle,

				secondInsurancePopUpToggle, timeOfGoal12Fail);







			TriggerOnBuyInsuranceEvent ();







			Debug.Log ("Time of Disaster: " + timeOfDisaster);



			Debug.Log ("Time of Insurance Expiration: " + timeOfInsuranceExpiration);



			Debug.Log ("Time of Goal 12 Failure: " + timeOfGoal12Fail);



		}

	}







	/// <summary>



	/// This method is called upon a button press.



	/// If the player denies taking insurance, this method will be called.



	/// </summary>



	public void DenyInsurance ()
	{



		Debug.Log (string.Format ("The player has denied buying insurance"));







		if (!SettingsManager.Instance.GetFirstOfferEver ()) {

			firstInsuranceOffer = false;

			Debug.Log ("First insurance offer: " + firstInsuranceOffer);

			insuranceUI.SetActive (false);

			Debug.Log ("Insurance UI closed: " + insuranceUI.activeSelf);

		} else {

			timeOfDisaster = DateTime.Now.Add (new TimeSpan (0, 0, DISASTER_COUNTDOWN_TIME_IN_SECONDS));



			Debug.Log ("Time of Disaster: " + timeOfDisaster);



			disasterTimerCoroutine = StartCoroutine (UpdateDisasterTimer ());

			SettingsManager.Instance.SetFirstOfferEver (false);

			Debug.Log ("First Offer Ever?: " + SettingsManager.Instance.GetFirstOfferEver ());

			firstInsuranceOffer = false;

			Debug.Log ("First insurance offer: " + firstInsuranceOffer);

			insuranceUI.SetActive (false);

			Debug.Log ("Insurance UI closed: " + insuranceUI.activeSelf);

		}



		StatisticsManager.Instance.UpdateDisasterInfo (timeOfDisaster, timeOfInsuranceExpiration, isInsuranceActive, firstInsuranceOffer, disasterOccurred, firstRun, firstInsurancePopUpToggle,

			secondInsurancePopUpToggle, timeOfGoal12Fail);



	}







	public void DisasterEnd ()
	{


        timeOfDisaster = DateTime.Now.Add (new TimeSpan (0, 0, DISASTER_COUNTDOWN_TIME_IN_SECONDS));



		Debug.Log ("Time of Disaster: " + timeOfDisaster);



		disasterUI.SetActive (false);



		firstInsurancePopUpToggle = true;



		secondInsurancePopUpToggle = true;



		StatisticsManager.Instance.UpdateDisasterInfo (timeOfDisaster, timeOfInsuranceExpiration, isInsuranceActive, firstInsuranceOffer, disasterOccurred, firstRun, firstInsurancePopUpToggle,

			secondInsurancePopUpToggle, timeOfGoal12Fail);



	}











	#endregion







	#region Disaster Manipulation Methods



	private void DisasterStrike ()
	{



		// If the disaster strikes, take a certain percent of the 



		// box count away depending on the type of disaster



		// Doesn't update the box count UI directly



		// Also probably don't need the disasterOccurred bool, might need to remove later



		if (disasterOccurred) {

			disasterChance = UnityEngine.Random.Range (1, 101);



			Debug.Log ("Disaster Chance: " + disasterChance);



			disasterType = UnityEngine.Random.Range (1, 5);



			Debug.Log ("Disaster Type: " + disasterType);

			if (isInsuranceActive) {

				SaveManager.Instance.insuredDisasterCount++;

			}



			if (disasterChance <= 40) 
			{
                // turning some UI buttons off so that the player
                // can't cause unintended issues to occur
                settingsButton.interactable = false;
                workButton.interactable = false;

                if (!isInsuranceActive && !SaveManager.Instance.goal11Completion) {

					GoalManager.GetComponent<GoalManager> ().GoalUnlocked (11, false);

				}



				Debug.Log ("Current time: " + DateTime.Now);

				if (isInsuranceActive && DateTime.Now < timeOfGoal12Fail && !SaveManager.Instance.goal12Completion) {

					GoalManager.GetComponent<GoalManager> ().GoalUnlocked (12, true);

				}





				if (isInsuranceActive && !SaveManager.Instance.goal13Completion) {

					GoalManager.GetComponent<GoalManager> ().GoalUnlocked (13, false);

				}



				/*

                if (disasterType == 1)



                {



                    int newBoxCollectAmount = (int)(SaveManager.Instance.CurrentBoxCount * (.6f / boxDeductionModifier));



                    StatisticsManager.Instance.RemoveFromBoxCount(newBoxCollectAmount, false);



                    disasterBoxesTaken.text = newBoxCollectAmount.ToString() + " Boxes";



                    disasterTypeText.text = "A hurricane has struck! You have lost: ";







                    boxDeductionModifier = 1.0f;







                    Debug.Log("Current box count: " + SaveManager.Instance.CurrentBoxCount);



                }

                */



				if (disasterType == 1 || disasterType == 2) {



					int newBoxCollectAmount = (int)(SaveManager.Instance.CurrentBoxCount * (.7f / boxDeductionModifier));



					StatisticsManager.Instance.RemoveFromBoxCount (newBoxCollectAmount, false);



					disasterBoxesTaken.text = newBoxCollectAmount.ToString () + " Boxes";



					disasterTypeText.text = "An earthquake has struck! You have lost: ";

                    if (isInsuranceActive)
                    {
                        disasterInsuranceText.text = "A good thing you \n had insurance! \n \n You could have \n lost more boxes \n if I hadn't been \n so generous..."; ;
                    }
                    else
                    {
                        disasterInsuranceText.text = "If only someone \n had been around \n to help you out. \n \n Maybe next time?";
                    }





                    boxDeductionModifier = 1.0f;







					Debug.Log ("Current box count: " + SaveManager.Instance.CurrentBoxCount);



				} else if (disasterType == 3 || disasterType == 4) {



					int newBoxCollectAmount = (int)(SaveManager.Instance.CurrentBoxCount * (.8f / boxDeductionModifier));



					StatisticsManager.Instance.RemoveFromBoxCount (newBoxCollectAmount, false);



					disasterBoxesTaken.text = newBoxCollectAmount.ToString () + " Boxes";



					disasterTypeText.text = "A tornado has struck! You have lost: ";

                    if(isInsuranceActive)
                    {
                        disasterInsuranceText.text = "A good thing you \n had insurance! \n \n You could have \n lost more boxes \n if I hadn't been \n so generous...";
                    }
                    else
                    {
                        disasterInsuranceText.text = "If only someone \n had been around \n to help you out. \n \n Maybe next time?";
                    }





					boxDeductionModifier = 1.0f;







					Debug.Log ("Current box count: " + SaveManager.Instance.CurrentBoxCount);



				}



				/*

                else if (disasterType == 4)



                {



                    int newBoxCollectAmount = (int)(SaveManager.Instance.CurrentBoxCount * (.9f / boxDeductionModifier));



                    StatisticsManager.Instance.RemoveFromBoxCount(newBoxCollectAmount, false);



                    disasterBoxesTaken.text = newBoxCollectAmount.ToString() + " Boxes";



                    disasterTypeText.text = "A flood has struck! You have lost: ";







                    boxDeductionModifier = 1.0f;







                    Debug.Log("Current box count: " + SaveManager.Instance.CurrentBoxCount);



                }



            }

            */
				if (!isInsuranceActive && StatisticsManager.Instance.CurrentClass == Classes.Poor) {
					this.GetComponent<DisasterDamageManager> ().rollForDamage ();
				}
			} else {



				timeOfDisaster = DateTime.Now.Add (new TimeSpan (0, 0, DISASTER_COUNTDOWN_TIME_IN_SECONDS));



				disasterOccurred = false;



				Debug.Log ("Time of Disaster: " + timeOfDisaster);



			}



		}



	}



	#endregion







	#region Insurance Manipulation Methods







	private void BuyInsurance ()
	{


        if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
        {
            StatisticsManager.Instance.RemoveFromBoxCount(100, false);
        }
        else if (StatisticsManager.Instance.CurrentClass == Classes.Poor)
        {
            StatisticsManager.Instance.RemoveFromBoxCount(200, false);
        }



		MainGameUI.instance.UpdateBoxCountUI (SaveManager.Instance.CurrentBoxCount);





		boxDeductionModifier = 3.0f;



	}







	#endregion











	#region Utility Methods



	private IEnumerator UpdateInsuranceTimer ()
	{

		while (isInsuranceActive) {

			insuranceExpirationTimer = timeOfInsuranceExpiration.Subtract (DateTime.Now);







			string time = "";

			int seconds = insuranceExpirationTimer.Seconds;

			int minutes = insuranceExpirationTimer.Minutes;

			float totalSeconds = insuranceExpirationTimer.Seconds + (insuranceExpirationTimer.Minutes * 60);



			if (minutes < 10) {

				time += "0";

			}

			time += minutes + ":";



			if (seconds < 10) {

				time += "0";

			}

			time += seconds;



			timeUntilInsuranceExpirationText.text = time;







			float totalSecondsInsurance = insuranceExpirationTimer.Seconds + (insuranceExpirationTimer.Minutes * 60);



			if (totalSecondsInsurance <= 0) {



				timeUntilInsuranceExpirationText.text = "00:00";







				if (Time.time < timeActive + 2.0f) {



					yield return new WaitForSeconds (timeActive + 2.0f - Time.time);



				}



				isInsuranceActive = false;

				StatisticsManager.Instance.UpdateDisasterInfo (timeOfDisaster, timeOfInsuranceExpiration, isInsuranceActive, firstInsuranceOffer, disasterOccurred, firstRun, firstInsurancePopUpToggle,

					secondInsurancePopUpToggle, timeOfGoal12Fail);

				TriggerOnInsuranceExpirationEvent ();

				boxDeductionModifier = 1.0f;



			}



			yield return null;

		}



		yield break;

	}







	private IEnumerator UpdateDisasterTimer ()
	{



		while (!disasterOccurred) {



			countdownToDisaster = timeOfDisaster.Subtract (DateTime.Now);



			// Debug.Log("Countdown to disaster: " + countdownToDisaster);



			float totalSeconds = countdownToDisaster.Seconds + (countdownToDisaster.Minutes * 60);



			if (!isInsuranceActive && !firstInsuranceOffer && totalSeconds <= 180) {



				if (Time.time < timeActive + 2.0f) {



					yield return new WaitForSeconds (timeActive + 2.0f - Time.time);



				}



				if (firstInsurancePopUpToggle) {

					ShowInsurancePopup ();

					firstInsurancePopUpToggle = false;

					StatisticsManager.Instance.UpdateDisasterInfo (timeOfDisaster, timeOfInsuranceExpiration, isInsuranceActive, firstInsuranceOffer, disasterOccurred, firstRun, firstInsurancePopUpToggle,

						secondInsurancePopUpToggle, timeOfGoal12Fail);

				}



			}



			if (!isInsuranceActive && !firstInsuranceOffer && totalSeconds <= 35) {



				if (Time.time < timeActive + 2.0f) {



					yield return new WaitForSeconds (timeActive + 2.0f - Time.time);



				}



				if (secondInsurancePopUpToggle) {

					ShowInsurancePopup ();

					secondInsurancePopUpToggle = false;

					StatisticsManager.Instance.UpdateDisasterInfo (timeOfDisaster, timeOfInsuranceExpiration, isInsuranceActive, firstInsuranceOffer, disasterOccurred, firstRun, firstInsurancePopUpToggle,

						secondInsurancePopUpToggle, timeOfGoal12Fail);

				}



			}







			if (totalSeconds <= 0) {



				if (!loanUI.activeSelf && !expensesUI.activeSelf && !insuranceUI.activeSelf) {



					yield return new WaitForSeconds (4.0f);



					if (Time.time < timeActive + 2.0f) {



						yield return new WaitForSeconds (timeActive + 2.0f - Time.time);



					}







					disasterOccurred = true;



					Debug.Log ("Disaster occurred?: " + disasterOccurred);



					StatisticsManager.Instance.UpdateDisasterInfo (timeOfDisaster, timeOfInsuranceExpiration, isInsuranceActive, firstInsuranceOffer, disasterOccurred, firstRun, firstInsurancePopUpToggle,

						secondInsurancePopUpToggle, timeOfGoal12Fail);



					DisasterStrike ();

                    







					if (disasterChance <= 40)
                    {

						Debug.Log ("Disaster effects are happening.");

						disasterEffects = StartCoroutine (DisasterEffects ());

						//yield return new WaitForSeconds(20.0f);



						Debug.Log ("disasterUI is active");



						disasterUI.SetActive (true);



					}



				}



			}



			yield return null;



		}



		yield break;



	}







	/// <summary>



	/// Handle the animations for the various disasters that will occur



	/// </summary>



	private IEnumerator DisasterEffects ()
	{

		Debug.Log ("Disaster occurred in DisasterEffects: " + disasterOccurred);

		if (disasterOccurred)
        {
            // Need to make sure that the UI is in its proper position when
            // the disaster is over, as iTween can accidentally leave them out of position;
            if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
            {
                Debug.Log(WealthyBackgroundUI.transform.position);
                wealthyUIPos = WealthyBackgroundUI.transform.position;
                Debug.Log(wealthyUIPos);
            }
            else if (StatisticsManager.Instance.CurrentClass == Classes.Poor)
            {
                poorUIPos = PoorBackgroundElements.transform.position;
            }

			/*

            if(disasterType == 1)

            {

                AudioManager.Instance.PlayAudioClip(SFXType.Hurricane);





                GameObject StrikeOne = Instantiate(lightningStrike, lightningStrikeOne.position, Quaternion.Euler(0,0,270)) as GameObject;



                //GameObject StrikeTwo = Instantiate(lightningStrike, lightningStrikeTwo.position, Quaternion.identity) as GameObject;



                //GameObject StrikeThree = Instantiate(lightningStrike, lightningStrikeThree.position, Quaternion.identity) as GameObject;



                //GameObject StrikeFour = Instantiate(lightningStrike, lightningStrikeFour.position, Quaternion.identity) as GameObject;



                GameObject Rain = Instantiate(RainEmitter, rain.position, Quaternion.identity) as GameObject;



                Rain.GetComponent<Canvas>().worldCamera = mainCamera;

                Rain.GetComponent<Canvas>().sortingLayerName = "UI";

                Rain.GetComponent<Canvas>().sortingOrder = 3;

                Rain.GetComponent<ParticleSystem>().Play();





            }

            */



			if (disasterType == 1 || disasterType == 2) {

				AudioManager.Instance.PlayAudioClip (SFXType.Earthquake);


                if(StatisticsManager.Instance.CurrentClass == Classes.Rich)
                {
                    iTween.ShakePosition(WealthyBackgroundUI, iTween.Hash("x", .25f, "y", .25f, "time", 2.0f));
                }
                else if(StatisticsManager.Instance.CurrentClass == Classes.Poor)
                {
                    iTween.ShakePosition(PoorBackgroundElements, iTween.Hash("x", .25f, "y", .25f, "time", 2.0f));
                }



				yield return new WaitForSeconds (1.0f);



				QuakeLineOne = Instantiate (earthquakeLineOne, earthquakePosOne.position, Quaternion.Euler (0, 0, -45)) as GameObject;



				QuakeLineTwo = Instantiate (earthquakeLineTwo, earthquakePosTwo.position, Quaternion.Euler (0, 0, 180)) as GameObject;



				QuakeLineThree = Instantiate (earthquakeLineOne, earthquakePosThree.position, Quaternion.Euler (0, 0, -95)) as GameObject;



				QuakeLineFour = Instantiate (earthquakeLineTwo, earthquakePosFour.position, Quaternion.Euler (0, 0, 60)) as GameObject;





                if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
                {
                    iTween.ShakePosition(WealthyBackgroundUI, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 4.0f));
                }
                else if (StatisticsManager.Instance.CurrentClass == Classes.Poor)
                {
                    iTween.ShakePosition(PoorBackgroundElements, iTween.Hash("x", 1.0f, "y", 1.0f, "time", 4.0f));
                }

				iTween.ShakePosition (QuakeLineOne, iTween.Hash ("x", 1.0f, "y", 1.0f, "time", 4.0f));

				iTween.ShakePosition (QuakeLineTwo, iTween.Hash ("x", 1.0f, "y", 1.0f, "time", 4.0f));

				iTween.ShakePosition (QuakeLineThree, iTween.Hash ("x", 1.0f, "y", 1.0f, "time", 4.0f));

				iTween.ShakePosition (QuakeLineFour, iTween.Hash ("x", 1.0f, "y", 1.0f, "time", 4.0f));



				yield return new WaitForSeconds (3.0f);







				iTween.ColorTo (QuakeLineOne, new Color (1.0f, 1.0f, 1.0f, 0.0f), 1.0f);



				iTween.ColorTo (QuakeLineTwo, new Color (1.0f, 1.0f, 1.0f, 0.0f), 1.0f);



				iTween.ColorTo (QuakeLineThree, new Color (1.0f, 1.0f, 1.0f, 0.0f), 1.0f);



				iTween.ColorTo (QuakeLineFour, new Color (1.0f, 1.0f, 1.0f, 0.0f), 1.0f);







				yield return new WaitForSeconds (2.0f);







				Destroy (QuakeLineOne.gameObject);



				Destroy (QuakeLineTwo.gameObject);



				Destroy (QuakeLineThree.gameObject);



				Destroy (QuakeLineFour.gameObject);

                // turning UI buttons back on
                workButton.interactable = true;
                settingsButton.interactable = true;



                MainGameUI.instance.UpdateBoxCountUI (SaveManager.Instance.CurrentBoxCount);



			} else if (disasterType == 3 || disasterType == 4) {

				AudioManager.Instance.PlayAudioClip (SFXType.Tornado);



				tornado = Instantiate (tornadoFab, TornadoStart.position, Quaternion.identity) as GameObject;







				if (StatisticsManager.Instance.CurrentClass == Classes.Rich) {



					tornado.transform.localScale = new Vector3 (.5f, .5f);







					tornado.transform.SetParent (WealthyBackgroundUI.transform);







					tornado.transform.SetSiblingIndex (0);



				} else {



					tornado.transform.localScale = new Vector3 (.45f, .45f);







					tornado.transform.SetParent (PoorBackgroundUI.transform);







					tornado.transform.SetSiblingIndex (0);



				}









                if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
                {
                    iTween.ShakePosition(WealthyBackgroundUI, iTween.Hash("x", .15f, "y", .15f, "time", 6.0f));
                }
                else if (StatisticsManager.Instance.CurrentClass == Classes.Poor)
                {
                    iTween.ShakePosition(PoorBackgroundElements, iTween.Hash("x", .15f, "y", .15f, "time", 6.0f));
                }



				iTween.MoveTo (tornado, iTween.Hash ("position", new Vector3 (TornadoPosOne.transform.position.x - 30, TornadoPosOne.transform.position.y),



					"time", 80.0f));





				yield return new WaitForSeconds (5.0f);



				tornado.transform.SetParent (DisasterEffectsUI.transform);



				tornado.transform.localScale = new Vector3 (2.0f, 2.0f);



				tornado.GetComponent<SpriteRenderer> ().sortingOrder = 8;


                if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
                {
                    iTween.ShakePosition(WealthyBackgroundUI, iTween.Hash("x", .2f, "y", .2f, "time", 5.0f));
                }
                else if (StatisticsManager.Instance.CurrentClass == Classes.Poor)
                {
                    iTween.ShakePosition(PoorBackgroundElements, iTween.Hash("x", .2f, "y", .2f, "time", 5.0f));
                }



				iTween.MoveTo (tornado, iTween.Hash ("position", new Vector3 (TornadoStart.transform.position.x + 30, TornadoStart.transform.position.y),



					"time", 60.0f));





				yield return new WaitForSeconds (3.0f);





				iTween.ColorTo (tornado.gameObject, new Color (1.0f, 1.0f, 1.0f, 0.0f), 2.0f);



				yield return new WaitForSeconds (1.0f);







				Destroy (tornado.gameObject);

                // turning UI buttons back on
                workButton.interactable = true;
                settingsButton.interactable = true;



                MainGameUI.instance.UpdateBoxCountUI (SaveManager.Instance.CurrentBoxCount);



			}

            // Putting the UI elements back where they belong
            if (StatisticsManager.Instance.CurrentClass == Classes.Rich)
            {
                WealthyBackgroundUI.transform.position = wealthyUIPos;
            }
            else if (StatisticsManager.Instance.CurrentClass == Classes.Poor)
            {
                PoorBackgroundElements.transform.position = poorUIPos;
            }


        }



        /*

        else if (disasterType == 4)

        {

            AudioManager.Instance.PlayAudioClip(SFXType.Flood);



        }

        */


        disasterOccurred = false;



		firstRun = false;



		firstInsuranceOffer = true;



		StatisticsManager.Instance.UpdateDisasterInfo (timeOfDisaster, timeOfInsuranceExpiration, isInsuranceActive, firstInsuranceOffer, disasterOccurred, firstRun, firstInsurancePopUpToggle,

			secondInsurancePopUpToggle, timeOfGoal12Fail);



		Debug.Log ("Disaster occurred?: " + disasterOccurred);



		Debug.Log ("First insurnace offer: " + firstInsuranceOffer);



		yield break;



	}






	private void ShowInsurancePopup ()
	{



		// if this is not the first offer, make it sound more urgent on the popup UI



		if (firstInsuranceOffer) {



			insuranceDeal.text = "This is the cost. Deal?";

		} else if (!firstInsuranceOffer) {



			insuranceDeal.text = "Deal? The disaster may strike soon!!!";



		}







		Debug.Log ("InsuranceUI is active");





		insuranceUI.SetActive (true);

        if(StatisticsManager.Instance.CurrentClass == Classes.Rich)
        {
            if(insuranceQuoteBubbleRich.enabled == false)
            {
                boxInsuranceAmount.text = "100 Boxes";
                insuranceQuoteBubblePoor.enabled = false;
                insuranceQuoteBubbleRich.enabled = true;
            }
        }
        else if(StatisticsManager.Instance.CurrentClass == Classes.Poor)
        {
            boxInsuranceAmount.text = "200 Boxes";
            insuranceQuoteBubblePoor.enabled = true;
            insuranceQuoteBubbleRich.enabled = false;
        }

		Debug.Log ("Insurance UI open: " + insuranceUI.activeSelf);





	}



    private void SubscribeToEvents()
    {
        MainGameEventManager.OnOpportunityBoxSpawn += AddOpportunityBoxTimer;

        MainGameEventManager.OnOpportunityBoxSpawn += OpportunityBoxStopDisasterTimer;

        MainGameEventManager.OnOpportunityBoxSpawn += OpportunityBoxStopInsuranceTimer;

        MainGameEventManager.OnOpportunityBoxDespawn += OpportunityBoxStartDisasterTimer;

        MainGameEventManager.OnOpportunityBoxDespawn += OpportunityBoxStartInsuranceTimer;


        MainGameEventManager.OnFirst25BoxCount += ShowInsurancePopup;

        SaveManager.Instance.SendSaveData += SendDisasterData;
    }

    private void UnSubscribeToEvents()
    {
        MainGameEventManager.OnOpportunityBoxSpawn -= AddOpportunityBoxTimer;

        MainGameEventManager.OnOpportunityBoxSpawn -= OpportunityBoxStopDisasterTimer;

        MainGameEventManager.OnOpportunityBoxSpawn -= OpportunityBoxStopInsuranceTimer;

        MainGameEventManager.OnOpportunityBoxDespawn -= OpportunityBoxStartDisasterTimer;

        MainGameEventManager.OnOpportunityBoxDespawn -= OpportunityBoxStartInsuranceTimer;



        MainGameEventManager.OnFirst25BoxCount -= ShowInsurancePopup;

        SaveManager.Instance.SendSaveData -= SendDisasterData;
    }

	private void SendDisasterData ()
	{



		StatisticsManager.Instance.UpdateDisasterInfo (timeOfDisaster, timeOfInsuranceExpiration, isInsuranceActive, firstInsuranceOffer, disasterOccurred, firstRun, firstInsurancePopUpToggle,

			secondInsurancePopUpToggle, timeOfGoal12Fail);



	}

    /// <summary>
    /// For disaster + insurance timer, adds original timer from an existing oppertunity box
    /// Also adds time to the goal 12 timer as this is tied to the insurance timer.
    /// </summary>
    private void AddOpportunityBoxTimer()
    {

        while (opportunityBox == null)
        {
            opportunityBox = GameObject.FindGameObjectWithTag("OpportunitySafe");

            if(opportunityBox != null)
            {
                break;
            }
        }

        
        if (isInsuranceActive)
        {
            Debug.Log("Time to add to disaster/insurance: " + opportunityBox.GetComponent<OpportunityBox>().opportunityBoxOriginalTimer);

            opportunityBoxTimeToAdd = new TimeSpan(0, 0, Mathf.CeilToInt(opportunityBox.GetComponent<OpportunityBox>().opportunityBoxOriginalTimer));

            timeOfInsuranceExpiration += opportunityBoxTimeToAdd;

            Debug.Log("Insurance Expiration: " + timeOfInsuranceExpiration);
        }

        if(timeOfDisaster != DateTime.MinValue)
        {
            opportunityBoxTimeToAdd = new TimeSpan(0, 0, Mathf.CeilToInt(opportunityBox.GetComponent<OpportunityBox>().opportunityBoxOriginalTimer));

            timeOfDisaster += opportunityBoxTimeToAdd;

            Debug.Log("Disaster Expiration: " + timeOfDisaster);
        }

        if(timeOfGoal12Fail != DateTime.MinValue)
        {
            opportunityBoxTimeToAdd = new TimeSpan(0, 0, Mathf.CeilToInt(opportunityBox.GetComponent<OpportunityBox>().opportunityBoxOriginalTimer));

            timeOfGoal12Fail += opportunityBoxTimeToAdd;

            Debug.Log("Goal 12 Fail Expiration: " + timeOfGoal12Fail);
        }
    }

    /// <summary>
    /// Utilized by opportunity box system to regulate "pausing" this mechanic's timer
    /// </summary>    
    public void OpportunityBoxStartDisasterTimer()
    {
        if(timeOfDisaster != DateTime.MinValue)
        {
            disasterTimerCoroutine = StartCoroutine(UpdateDisasterTimer());
        }
    }

    /// <summary>
    /// Utilized by opportunity box system to regulate "pausing" this mechanic's timer
    /// </summary>
    public void OpportunityBoxStopDisasterTimer()
    {
        if(timeOfDisaster != DateTime.MinValue)
        {
            StopCoroutine(disasterTimerCoroutine);
        }
    }

    /// <summary>
    /// Utilized by opportunity box system to regulate "pausing" this mechanic's timer
    /// </summary>
    public void OpportunityBoxStartInsuranceTimer()
    {
        if(isInsuranceActive)
        {
            insuranceTimerCoroutine = StartCoroutine(UpdateInsuranceTimer());
        }
    }

    /// <summary>
    /// Utilized by opportunity box system to regulate "pausing" this mechanic's timer
    /// </summary>
    public void OpportunityBoxStopInsuranceTimer()
    {
        if(isInsuranceActive)
        {
            Debug.Log("Stopping Insurance Timer");
            StopCoroutine(insuranceTimerCoroutine);
        }
    }


    #endregion



    #region Goal Related

    public void addTime (TimeSpan durationToAdd) //Call externally when rewarding bonus time on insurance.
	{

		timeOfInsuranceExpiration = timeOfInsuranceExpiration.Add (durationToAdd);

		Debug.Log ("Time of insurance expiration: " + timeOfInsuranceExpiration);

		StatisticsManager.Instance.UpdateDisasterInfo (timeOfDisaster, timeOfInsuranceExpiration, isInsuranceActive, firstInsuranceOffer, disasterOccurred, firstRun, firstInsurancePopUpToggle,

			secondInsurancePopUpToggle, timeOfGoal12Fail);

	}

	#endregion



	#region Test Methods







	#if UNITY_EDITOR







	public void TestDisaster ()
	{



		disasterOccurred = true;



		DisasterStrike ();



		StartCoroutine (DisasterEffects ());



		StartCoroutine (TestDisasterPopUp ());



	}







	private IEnumerator TestDisasterPopUp ()
	{



		if (disasterChance <= 30) {



			yield return new WaitForSeconds (5.0f);



			Debug.Log ("disasterUI is active");



			disasterUI.SetActive (true);



		}







		yield break;



	}







	public void TestInsuranceEffect ()
	{



		BuyInsurance ();



	}















	#endif











	#endregion







	#region Event Trigger Methods







	private static void TriggerOnBuyInsuranceEvent ()
	{



		if (OnBuyInsurance != null) {



			Debug.Log ("Insurance has been bought");



			OnBuyInsurance ();



		}



	}







	public static void TriggerOnInsuranceExpirationEvent ()
	{



		if (OnInsuranceExpiration != null) {



			Debug.Log ("Insurance has expired");



			OnInsuranceExpiration ();



		}



	}







	#endregion



}



