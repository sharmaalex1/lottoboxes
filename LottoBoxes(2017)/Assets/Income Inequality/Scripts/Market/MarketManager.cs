using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MarketManager : MonoBehaviour
{
    public GameObject itemUI;
    public Text itemPopUpTitle;
    public Text itemPopUpDescription;
    public Text itemPrice;
    public GameObject itemUIIcon;

    public GameObject approvalWindow;

    public GameObject shopButtonObject;
    public GameObject shopButtonPermLocation;

    void Awake()
    {

    }

    private void OnEnable()
    {
        if (SaveManager.Instance.CompletedFurnitureTutorial && SceneTransition.Instance.SelectedClass == Classes.Poor)
        {
            shopButtonObject.SetActive(true);
        }
    }

    public void purchaseItem(int itemIdentifier)
    {
        //if(SaveManager.Instance.
        if (this.GetComponent<MarketLib>().GetPrice(itemIdentifier) <= SaveManager.Instance.CurrentOwnedTickets)
        {
            switch (itemIdentifier)
            {
                case 1:
                    SaveManager.Instance.item01Purchased = true;
                    break;
                case 2:
                    SaveManager.Instance.item02Purchased = true;
                    break;
                case 3:
                    SaveManager.Instance.item03Purchased = true;
                    break;
                case 4:
                    SaveManager.Instance.item04Purchased = true;
                    break;
                case 5:
                    SaveManager.Instance.item05Purchased = true;
                    break;
                case 6:
                    SaveManager.Instance.item06Purchased = true;
                    break;
                case 7:
                    SaveManager.Instance.isWindowFixed = true;
                    break;
                case 8:
                    SaveManager.Instance.isWallFixed = true;
                    break;
			case 9:
				SaveManager.Instance.wall01bought = true;
				SaveManager.Instance.wall02bought = false;
				SaveManager.Instance.wall03bought = false;
				SaveManager.Instance.wall04bought = false;
				SaveManager.Instance.wall05bought = false;
				SaveManager.Instance.wall06bought = false;
				SaveManager.Instance.wall07bought = false;
				break;
			case 10:
				SaveManager.Instance.wall02bought = true;
				SaveManager.Instance.wall01bought = false;
				SaveManager.Instance.wall03bought = false;
				SaveManager.Instance.wall04bought = false;
				SaveManager.Instance.wall05bought = false;
				SaveManager.Instance.wall06bought = false;
				SaveManager.Instance.wall07bought = false;
				break;
			case 11:
				SaveManager.Instance.wall03bought = true;
				SaveManager.Instance.wall01bought = false;
				SaveManager.Instance.wall02bought = false;
				SaveManager.Instance.wall04bought = false;
				SaveManager.Instance.wall05bought = false;
				SaveManager.Instance.wall06bought = false;
				SaveManager.Instance.wall07bought = false;
				break;
			case 12:
				SaveManager.Instance.floor01bought = true;
				SaveManager.Instance.floor02bought = false;
				SaveManager.Instance.floor03bought = false;
				SaveManager.Instance.floor04bought = false;
				SaveManager.Instance.floor05bought = false;
				SaveManager.Instance.floor06bought = false;
				break;
			case 13:
				SaveManager.Instance.floor01bought = false;
				SaveManager.Instance.floor02bought = true;
				SaveManager.Instance.floor03bought = false;
				SaveManager.Instance.floor04bought = false;
				SaveManager.Instance.floor05bought = false;
				SaveManager.Instance.floor06bought = false;
				break;
			case 14:
				SaveManager.Instance.floor01bought = false;
				SaveManager.Instance.floor02bought = false;
				SaveManager.Instance.floor03bought = true;
				SaveManager.Instance.floor04bought = false;
				SaveManager.Instance.floor05bought = false;
				SaveManager.Instance.floor06bought = false;
				break;
			case 15:
				SaveManager.Instance.item07Purchased = true;
				break;
			case 16:
				SaveManager.Instance.item08Purchased = true;
				break;
			case 17:
				SaveManager.Instance.wall01bought = false;
				SaveManager.Instance.wall02bought = false;
				SaveManager.Instance.wall03bought = false;
				SaveManager.Instance.wall04bought = true;
				SaveManager.Instance.wall05bought = false;
				SaveManager.Instance.wall06bought = false;
				SaveManager.Instance.wall07bought = false;
				break;
			case 18:
				SaveManager.Instance.wall01bought = false;
				SaveManager.Instance.wall02bought = false;
				SaveManager.Instance.wall03bought = false;
				SaveManager.Instance.wall04bought = false;
				SaveManager.Instance.wall05bought = true;
				SaveManager.Instance.wall06bought = false;
				SaveManager.Instance.wall07bought = false;
				break;
			case 19:
				SaveManager.Instance.wall01bought = false;
				SaveManager.Instance.wall02bought = false;
				SaveManager.Instance.wall03bought = false;
				SaveManager.Instance.wall04bought = false;
				SaveManager.Instance.wall05bought = false;
				SaveManager.Instance.wall06bought = true;
				SaveManager.Instance.wall07bought = false;	
				break;
			case 20:
				SaveManager.Instance.wall01bought = false;
				SaveManager.Instance.wall02bought = false;
				SaveManager.Instance.wall03bought = false;
				SaveManager.Instance.wall04bought = false;
				SaveManager.Instance.wall05bought = false;
				SaveManager.Instance.wall06bought = false;
				SaveManager.Instance.wall07bought = true;			
				break;
			case 21:
				SaveManager.Instance.floor01bought = false;
				SaveManager.Instance.floor02bought = false;
				SaveManager.Instance.floor03bought = false;
				SaveManager.Instance.floor04bought = true;
				SaveManager.Instance.floor05bought = false;
				SaveManager.Instance.floor06bought = false;
				break;
			case 22:
				SaveManager.Instance.floor01bought = false;
				SaveManager.Instance.floor02bought = false;
				SaveManager.Instance.floor03bought = false;
				SaveManager.Instance.floor04bought = false;
				SaveManager.Instance.floor05bought = true;
				SaveManager.Instance.floor06bought = false;
				break;
			case 23:
				SaveManager.Instance.floor01bought = false;
				SaveManager.Instance.floor02bought = false;
				SaveManager.Instance.floor03bought = false;
				SaveManager.Instance.floor04bought = false;
				SaveManager.Instance.floor05bought = false;
				SaveManager.Instance.floor06bought = true;
				break;
			case 24:
				SaveManager.Instance.item09Purchased = true;
				break;
			case 25:
				SaveManager.Instance.item10Purchased = true;
				break;
			case 26:
				SaveManager.Instance.item11Purchased = true;
				break;
            default:
                    Debug.Log("Tried to purchase something not in the purchase item list with identifier " + itemIdentifier.ToString());
                    break;
            }
            SaveManager.Instance.CurrentOwnedTickets -= this.GetComponent<MarketLib>().GetPrice(itemIdentifier);
        }
        else
        {
            Debug.Log("Tried to buy item " + itemIdentifier.ToString() + " but did not have enough tickets.");
            Debug.Log("Have: " + SaveManager.Instance.CurrentOwnedTickets.ToString() + ". Need: " + this.GetComponent<MarketLib>().GetPrice(itemIdentifier).ToString() + ".");
        }
        MainGameUI.instance.UpdateTicketCountUI(SaveManager.Instance.CurrentOwnedTickets);
        gameObject.GetComponent<FurnitureManager>().UpdateRoom();
    }

    public void sellItem(int itemIdentifier)
    {
        Debug.Log("Received sell request for item with identifier " + itemIdentifier.ToString() + ".");
		if (gameObject.GetComponent<MarketLib> ().CanBeSold (itemIdentifier)) {
			switch (itemIdentifier) {
			case 1:
				SaveManager.Instance.item01Purchased = false;
				break;
			case 2:
				SaveManager.Instance.item02Purchased = false;
				break;
			case 3:
				SaveManager.Instance.item03Purchased = false;
				break;
			case 4:
				SaveManager.Instance.item04Purchased = false;
				break;
			case 5:
				SaveManager.Instance.item05Purchased = false;
				break;
			case 6:
				SaveManager.Instance.item06Purchased = false;
				break;
			case 15:
				SaveManager.Instance.item07Purchased = false;
				break;
			case 16:
				SaveManager.Instance.item08Purchased = false;
				break;
			case 24:
				SaveManager.Instance.item09Purchased = false;
				break;
			case 25:
				SaveManager.Instance.item10Purchased = false;
				break;
			case 26:
				SaveManager.Instance.item11Purchased = false;
				break;
			default:
				Debug.Log ("Tried to sell something not in the sell item list with identifier " + itemIdentifier.ToString ());
				break;
			}
			float tempPrice = this.GetComponent<MarketLib> ().GetPrice (itemIdentifier);
			SaveManager.Instance.CurrentOwnedTickets += (int)Mathf.Round (tempPrice / 3);
			MainGameUI.instance.UpdateTicketCountUI (SaveManager.Instance.CurrentOwnedTickets);
			gameObject.GetComponent<FurnitureManager> ().UpdateRoom ();
		} else {
			//transactionError (itemIdentifier);
		}
			
    }

	public void bribeItem(int itemIdentifier)
	{
		Debug.Log("Received Shark bribe request for item with identifier " + itemIdentifier.ToString() + ".");
		if (gameObject.GetComponent<MarketLib> ().CanBeSold (itemIdentifier)) {
			switch (itemIdentifier) {
			case 1:
				SaveManager.Instance.item01Purchased = false;
				break;
			case 2:
				SaveManager.Instance.item02Purchased = false;
				break;
			case 3:
				SaveManager.Instance.item03Purchased = false;
				break;
			case 4:
				SaveManager.Instance.item04Purchased = false;
				break;
			case 5:
				SaveManager.Instance.item05Purchased = false;
				break;
			case 6:
				SaveManager.Instance.item06Purchased = false;
				break;
			case 15:
				SaveManager.Instance.item07Purchased = false;
				break;
			case 16:
				SaveManager.Instance.item08Purchased = false;
				break;
			case 24:
				SaveManager.Instance.item09Purchased = false;
				break;
			case 25:
				SaveManager.Instance.item10Purchased = false;
				break;
			case 26:
				SaveManager.Instance.item11Purchased = false;
				break;
			default:
				Debug.Log ("Tried to bribe with something not in the sell item list with identifier " + itemIdentifier.ToString ());
				break;
			}
			//float tempPrice = this.GetComponent<MarketLib> ().GetPrice (itemIdentifier);
			//SaveManager.Instance.CurrentOwnedTickets += (int)Mathf.Round (tempPrice / 3);
			//MainGameUI.instance.UpdateTicketCountUI (SaveManager.Instance.CurrentOwnedTickets);
			gameObject.GetComponent<FurnitureManager> ().UpdateRoom ();
		} else {
			//transactionError (itemIdentifier);
		}

	}
}
