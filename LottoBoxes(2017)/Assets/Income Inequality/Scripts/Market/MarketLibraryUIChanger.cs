using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MarketLibraryUIChanger : MonoBehaviour
{

	public int itemIdentifier;
	GameObject MarketManager;
	/*public GameObject itemTitle;
	public GameObject itemDescription;
	public GameObject icon;
	public GameObject background;
	public GameObject button;*/

	public GameObject itemBackdrop;
	public GameObject itemIcon;
	public Text itemPrice;

	Sprite thisIcon;

	// Use this for initialization
	void Start()
	{

		//itemIcon = icon.GetComponent<Image>().sprite;
		MarketManager = GameObject.Find("MarketManager");

		UpdateMainUI();

	}

	public void UpdateMainUI()
	{
		itemPrice.text = MarketManager.GetComponent<MarketLib> ().GetPrice (itemIdentifier).ToString();
		itemIcon.GetComponent<Image> ().sprite = MarketManager.GetComponent<MarketLib> ().GetIcon (itemIdentifier);

		if (MarketManager.GetComponent<MarketLib> ().GetBoughtStatus (itemIdentifier)) {
			itemBackdrop.GetComponent<Image> ().color = Color.gray;
			itemPrice.GetComponent<Text> ().color = Color.gray;
			itemIcon.GetComponent<Image> ().color = Color.gray;
		} else {
			itemIcon.GetComponent<Image> ().color = Color.white;
			itemBackdrop.GetComponent<Image>().color = Color.white;

			Debug.Log (MarketManager.GetComponent<MarketLib> ().GetPrice (itemIdentifier).ToString ());
			if (MarketManager.GetComponent<MarketLib> ().GetPrice (itemIdentifier) > SaveManager.Instance.CurrentOwnedTickets)
				itemPrice.GetComponent<Text> ().color = Color.red;
			else
				itemPrice.GetComponent<Text> ().color = Color.green;
		}
	}

	/*public void UpdatePopUpUI()
	{
		if (MarketManager.GetComponent<MarketLib>().GetBoughtStatus(itemIdentifier))
		{
			itemDescription.GetComponent<Text>().text = MarketManager.GetComponent<MarketLib>().GetDescription(itemIdentifier);
			itemTitle.GetComponent<Text>().text = "\"" + MarketManager.GetComponent<MarketLib>().GetTitle(itemIdentifier) + "\"";
			background.GetComponent<Image>().color = Color.white;
			itemTitle.GetComponent<Text>().color = new Color(.5625f, 1, .6f);
			itemDescription.GetComponent<Text>().color = Color.white;
			icon.GetComponent<Image> ().color = Color.white;
		}
		else
		{
			itemDescription.GetComponent<Text>().text = "Not yet bought...";
			itemTitle.GetComponent<Text>().text = "Buy";
			thisIcon = MarketManager.GetComponent<MarketLib>().defaultIcon;
			icon.GetComponent<Image>().color = Color.gray;
			background.GetComponent<Image>().color = Color.gray;
			itemTitle.GetComponent<Text>().color = Color.gray;
			itemDescription.GetComponent<Text>().color = Color.gray;
		}
	}*/
}