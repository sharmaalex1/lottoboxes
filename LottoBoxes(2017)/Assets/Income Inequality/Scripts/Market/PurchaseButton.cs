using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PurchaseButton : MonoBehaviour {

	GameObject MarketManager;

	public GameObject transactionWindow;
	public GameObject transactionText;
	public GameObject transactionValue;

	int identifier;

	void Awake()
	{
		MarketManager = GameObject.Find ("MarketManager");

		transactionWindow = MarketManager.GetComponent<TransactionWindowHelper> ().transactionWindow;
		transactionText = MarketManager.GetComponent<TransactionWindowHelper> ().transactionText;
		transactionValue = MarketManager.GetComponent<TransactionWindowHelper> ().transactionValue;

	}

	public void uiButtonPress()
	{
		identifier = gameObject.GetComponent<MarketLibraryUIChanger> ().itemIdentifier;
		Debug.Log ("Setting identifier as: " + identifier.ToString () + ".");	
		transactionWindow.GetComponent<PurchaseRedirect> ().callingItem = this.gameObject;
		//If the item has not yet been purchased
		Debug.Log ("Button pressed for item " + identifier.ToString () + ".");
		if (MarketManager.GetComponent<MarketLib> ().GetBoughtStatus (identifier) == false) 
		{
			transactionValue.GetComponent<Text>().text = MarketManager.GetComponent<MarketLib> ().GetPrice (identifier).ToString() + " Tickets";
			transactionText.GetComponent<Text> ().text = "Would you like to purchase this " + MarketManager.GetComponent<MarketLib> ().GetTitle (identifier) + "?";
			transactionWindow.SetActive (true);
			Debug.Log ("Opening purchase window for item " + identifier.ToString () + ".");
		} 

		else if (MarketManager.GetComponent<MarketLib> ().GetBoughtStatus (identifier) == true) {
			transactionValue.GetComponent<Text>().text = MarketManager.GetComponent<MarketLib> ().GetSellValue (identifier).ToString() + " Tickets";
			transactionText.GetComponent<Text> ().text = "Would you like to sell this " + MarketManager.GetComponent<MarketLib> ().GetTitle (identifier) + "?";
			transactionWindow.SetActive (true);
			Debug.Log ("Opening sell window for item " + identifier.ToString () + ".");
		}
		this.GetComponent<MarketLibraryUIChanger> ().UpdateMainUI ();
		Debug.Log ("Button Pressed for item " + identifier.ToString ());
	}

	public void commitTransaction()
	{
		switch (MarketManager.GetComponent<MarketLib> ().GetBoughtStatus (identifier)) 
		{
		case false:
			{
				if (MarketManager.GetComponent<MarketLib> ().GetPrice (identifier) > SaveManager.Instance.CurrentOwnedTickets)
					transactionText.GetComponent<Text> ().text = "You don't have enough tickets!";
				else 
				{
					if (MarketManager.GetComponent<MarketLib> ().CanBePurchased (identifier)) 
					{
						if (!MarketManager.GetComponent<MarketLib> ().itemNeedsStand(identifier)) {
							transactionWindow.SetActive (false);
							MarketManager.GetComponent<MarketManager> ().purchaseItem (identifier);
						}
						else if (MarketManager.GetComponent<MarketLib> ().itemNeedsStand(identifier) && !MarketManager.GetComponent<MarketLib> ().isStandOccupied()) {
							transactionWindow.SetActive (false);
							MarketManager.GetComponent<MarketManager> ().purchaseItem (identifier);
							SaveManager.Instance.itemOnStand = true;
						}
						else if (MarketManager.GetComponent<MarketLib> ().itemNeedsStand(identifier) && MarketManager.GetComponent<MarketLib> ().isStandOccupied())
						{
							transactionText.GetComponent<Text>().text = "You need to clear the stand to buy this!";
						}

					} 
					else 
					{
						switch (identifier)
						{
						case 1:
						case 15:
							transactionText.GetComponent<Text> ().text = "You already have something in its place!";
							break;
						case 5:
						case 6:
						case 16:
							transactionText.GetComponent<Text> ().text = "You need to purchase a stand to place this item on!";
							break;
						default:
							Debug.Log ("Something can't be bought and doesn't have a condition in place, has identifier " + identifier.ToString () + ".");
							break;
						}
					}
				}
				break;
			}


		case true:
			{
				if (MarketManager.GetComponent<MarketLib> ().CanBeSold (identifier)) 
				{
					MarketManager.GetComponent<MarketManager> ().sellItem (identifier);
					transactionWindow.SetActive (false);

					if (MarketManager.GetComponent<MarketLib> ().itemNeedsStand(identifier))
						SaveManager.Instance.itemOnStand = false;
				} 
				else 
				{
					switch (identifier)
					{
					case 1:
						transactionText.GetComponent<Text> ().text = "You can't sell the stand when there's something on it!";
						break;
					case 7:
					case 8:
					case 9:
					case 10:
					case 11:
					case 12:
					case 13:
					case 14:
					case 17:
					case 18:
					case 19:
					case 20:
					case 21:
					case 22:
					case 23:
						transactionText.GetComponent<Text> ().text = "You can't return renovations!";
						break;
					default:
						Debug.Log ("Something can't be sold and doesn't have a condition in place, has identifier " + identifier.ToString () + ".");
						break;
					}
				}
				break;
			}
		}


		this.GetComponent<MarketLibraryUIChanger> ().UpdateMainUI ();
	}
}
