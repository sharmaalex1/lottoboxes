/*using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

public class GoalManager : MonoBehaviour 
{
	private int lottoBoxes;
	private int littleBoxes;
	private int timesWorked;
	private bool haveInsurance;
	private bool disasterStrike;
	private int hitNoInsurance;
	private int timesAcceptedLoan;
	private int timesFailedLoan;
	private int billVisits;
	private int boxesPaidBills;
	private int tickets;
	private bool isLotto;
	private bool isLittle;
	private bool fever;

	public Text goalText;
	public GameObject goalUI;

	private void FixedUpdate()
	{
		if (SaveManager.Instance.TotalBoxesOpened == 200 && GetComponent<GoalCheck>().goal01 == false) 
		{

			goalUI.SetActive (true);
			GetComponent<GoalCheck>().goal01 = true;
		}

	}
}
*/