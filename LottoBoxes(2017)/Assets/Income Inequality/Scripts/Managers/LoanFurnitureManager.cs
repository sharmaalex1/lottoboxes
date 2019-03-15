using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoanFurnitureManager : MonoBehaviour {

	public GameObject marketManager;
	public GameObject sharkBribeUI;
	public Text sharkSpeech;
	public Text selectedItem;
	string selectedItemImport;
	public int[] acceptedItemIds;

	int itemID;

	MarketLib marketLibrary;
	MarketManager marketManageScript;

	// Use this for initialization
	void Start () {
		marketLibrary = marketManager.GetComponent<MarketLib> ();
		marketManageScript = marketManager.GetComponent<MarketManager> ();
	}

	public void determineItem()
	{
		int selectedItemID = acceptedItemIds[Random.Range (0, acceptedItemIds.Length-1)];

		if(marketLibrary.GetBoughtStatus(selectedItemID))
		{
			if ((marketLibrary.GetTitle (selectedItemID) == "TV Stand" || marketLibrary.GetTitle (selectedItemID) == "Desk" ) && !marketLibrary.isStandOccupied ()) 
			{
				determineItem ();
			} 
			else
				commitItem (selectedItemID);
		}
		else determineItem();

		Debug.Log("Selected furniture item: " + selectedItemImport);
	}

	void commitItem(int id)
	{
		selectedItemImport = marketLibrary.GetTitle (id);
		Debug.Log("Selected furniture item: " + selectedItemImport);
		updateUI ();
		itemID = id;
	}

	void updateUI()
	{
		selectedItem.text = selectedItemImport;
		sharkSpeech.text = selectedItemImport;
		sharkBribeUI.SetActive (true);
	}

	public void acceptOffer()
	{
		marketManageScript.bribeItem (itemID);
	}
}
