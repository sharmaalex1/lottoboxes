using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class MarketLib : MonoBehaviour
{
    public Image defaultIcon;
    public GameObject template;
    public int numberOfItems;


    public int[] itemIdentifiers;

    public Image[] itemIcons;
    public GameObject[] libraryEntries;

    void Awake()
    {
        numberOfItems = itemIdentifiers.GetLength(0);
        Debug.Log("[Market]The array for identifiers is: " + itemIdentifiers.GetLength(0).ToString() + " entries long.");
        Debug.Log("[Market]The number of items is: " + numberOfItems);
        Array.Sort(itemIdentifiers);
        libraryEntries = new GameObject[numberOfItems];
        fillIconsWithDefault();
    }

    public void entryArrayPopulation(GameObject entryToAdd, int location)
    {
        libraryEntries[location] = entryToAdd;
    }

    public void updateEntryUI()
    {
        Debug.Log("Trying to update Market UI...");
        for (int i = 0; i < numberOfItems; i++)
        {
            libraryEntries[i].GetComponent<MarketLibraryUIChanger>().UpdateMainUI();
            //Debug.Log ("Updating UI for market: " + libraryEntries [i].GetComponent<MarketLibraryUIChanger> ().itemTitle.ToString() + " with identifier: " + libraryEntries [i].GetComponent<MarketLibraryUIChanger> ().itemIdentifier);
        }
        Debug.Log("UI Updated");
    }

    void fillIconsWithDefault()
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            if (itemIcons[i] == null)
                itemIcons[i] = defaultIcon;
        }
    }

    public Sprite GetIcon(int identifier)
    {
        if (identifier > 0)
            return itemIcons[identifier - 1].GetComponent<Image>().sprite;
        else
            return
                defaultIcon.GetComponent<Image>().sprite;
    }

    #region The Library
    public String GetTitle(int identifier)
    {
        switch (identifier)
        {
            case 1:
                return "TV Stand";
            //break;

            case 2:
                return "Carpet";
            //break;

            case 3:
                return "Lamp";
            //break;

            case 4:
                return "Coffee Table";
            //break;

            case 5:
                return "TV";
            //break;

            case 6:
                return "Aquarium";
            //break;
            case 7:
                return "Fixed Window";
            case 8:
                return "Fixed Wall";
		case 9:
			return "Blue Polka Dot Wallpaper";
		case 10:
			return "Pink Polka Dot Wallpaper";
		case 11:
			return "Neutral Wallpaper";
		case 12:
			return "Blue Flooring";
		case 13:
			return "Dark Harwood Flooring";
		case 14:
			return "Light Hardwood Flooring";
		case 15:
			return "Desk";
		case 16:
			return "Fish Bowl";
		case 17:
			return "Tan Wall";
		case 18:
			return "Blue Striped Wall";
		case 19:
			return "Blue Mouse Wall";
		case 20:
			return "Yellow Mouse Wall";
		case 21:
			return "Blue Hardwood Flooring";
		case 22:
			return "Grey Checkered Floor";
		case 23:
			return "White Checkered Floor";
		case 24:
			return "Dog Bed";
		case 25:
			return "Blinds";
		case 26:
			return "Punching Bag";

            default:
                return "Name here";
                //break;
		}
    }

    public String GetDescription(int identifier)
    {
        switch (identifier)
        {
            case 1:
                return "A Simple Stand.";
            //break;

            case 2:
                return "A Simple Rug.";
            //break;

            case 3:
                return "A Simple Lamp.";
            //break;

            case 4:
                return "A Coffee Table.";
            //break;

            case 5:
                return "A TV.";
            //break;

            case 6:
                return "A Shark Tank.";
            //break;
            case 7:
                return "A Brand New Window, 100% Less Broken!";
            case 8:
                return "Some new plaster, a roll of wallpaper, and the wall will be good as new!";
		case 15:
			return "A sturdy desk.";
		case 16:
			return "A fish bowl.";
		case 24:
			return "A bed for a dog you may one day have!";
		case 25:
			return "Blinds to hold back the sun.";
		case 26:
			return "A rough 'n' tough punching bag.";


            default:
                return "Description here";
                //break;
        }
    }

    public bool GetBoughtStatus(int identifier)
    {
        switch (identifier)
        {
            case 1:
                return SaveManager.Instance.item01Purchased;

            case 2:
                return SaveManager.Instance.item02Purchased;

            case 3:
                return SaveManager.Instance.item03Purchased;

            case 4:
                return SaveManager.Instance.item04Purchased;

            case 5:
                return SaveManager.Instance.item05Purchased;

            case 6:
                return SaveManager.Instance.item06Purchased;

            case 7:
                return SaveManager.Instance.isWindowFixed;

            case 8:
                return SaveManager.Instance.isWallFixed;
		case 9:
			return SaveManager.Instance.wall01bought;
		case 10:
			return SaveManager.Instance.wall02bought;
		case 11:
			return SaveManager.Instance.wall03bought;
		case 12:
			return SaveManager.Instance.floor01bought;
		case 13:
			return SaveManager.Instance.floor02bought;
		case 14:
			return SaveManager.Instance.floor03bought;
		case 15:
			return SaveManager.Instance.item07Purchased;
		case 16:
			return SaveManager.Instance.item08Purchased;
		case 17:
			return SaveManager.Instance.wall04bought;
		case 18:
			return SaveManager.Instance.wall05bought;
		case 19:
			return SaveManager.Instance.wall06bought;
		case 20:
			return SaveManager.Instance.wall07bought;
		case 21:
			return SaveManager.Instance.floor04bought;
		case 22:
			return SaveManager.Instance.floor05bought;
		case 23:
			return SaveManager.Instance.floor06bought;
		case 24:
			return SaveManager.Instance.item09Purchased;
		case 25:
			return SaveManager.Instance.item10Purchased;
		case 26:
			return SaveManager.Instance.item11Purchased;

            default:
                return true;
        }
    }

    public int GetPrice(int identifier)
    {
        switch (identifier)
        {
            case 1:
                return 1;
                //break;

            case 2:
                return 3;
                //break;

            case 3:
                return 2;
                //break;

            case 4:
                return 4;
                //break;

            case 5:
                return 6;
                //break;

            case 6:
                return 5;
            //break;

            case 7:
                return 3;

            case 8:
                return 3;
		case 9:
			return 3;
		case 10:
			return 3;
		case 11:
			return 3;
		case 12:
			return 3;
		case 13:
			return 3;
		case 14:
			return 3;
		case 15:
			return 2;
		case 16:
			return 1;
		case 17:
			return 3;
		case 18:
			return 3;
		case 19:
			return 3;
		case 20:
			return 3;
		case 21:
			return 3;
		case 22:
			return 3;
		case 23:
			return 3;
		case 24:
			return 3;
		case 25:
			return 3;
		case 26:
			return 4;

            default:
                return -1;
                //break;
        }
    }

    public int GetSellValue(int identifier)
    {
		int temp;
        switch (identifier)
        {
		default:
			temp = (int)Mathf.Round (GetPrice (identifier) / 3);
			if (temp >= 1)
				return temp;
			else
				return 1;
                //break;
        }
    }

    public bool CanBePurchased(int identifier)
    {
		switch (identifier) {
		case 1:
			if (!GetBoughtStatus (15))
				return true;
			else
				return false;
		case 5:
		case 6:
		case 16:
			if (!GetBoughtStatus (1) && !GetBoughtStatus(15)) 
				return false;
			 else
				return true;
		case 9:
		case 10:
		case 11:
			if (!GetBoughtStatus (8))
				return false;
			else
				return true;
		case 15:
			if (!GetBoughtStatus (1))
				return true;
			else
				return false;
		default:
			return true;
                
		}
    }

	public bool CanBeSold(int identifier)
	{
		switch(identifier)
		{
		case 1:
		case 15:
			{
				if (!SaveManager.Instance.itemOnStand)
					return true;
				else
					return false;		
			}
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
			return false;
		default:
			return true;
		}
	}

	public bool itemNeedsStand(int identifier)
	{
		switch (identifier) {
		case 5:
		case 6:
		case 16:
			return true;
		default:
			return false;
		}
	}

    public bool isStandOccupied()
    {
        return SaveManager.Instance.itemOnStand;
    }
    #endregion
}
