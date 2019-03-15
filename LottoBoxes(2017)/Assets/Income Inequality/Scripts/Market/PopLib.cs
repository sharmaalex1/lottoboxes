using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PopLib : MonoBehaviour
{

	public GameObject marketTemplate;
	public GameObject MarketManager;
	public int numberOfItems;

	int row;
	int column;

    [Header("Non-tutorial items that need to be turned off in the shop")]
    public List<GameObject> nonTutorialFurniture;

    public List<GameObject> activeFurniture;

    void Start()
	{
		row = 1;
		column = 1;
		RectTransform itemArea = this.GetComponent<RectTransform>() as RectTransform;
		itemArea.pivot = new Vector2(0.5f, 1);

		numberOfItems = MarketManager.GetComponent<MarketLib>().numberOfItems;

		Vector3 nextLocation = new Vector3(-25, ((36*(numberOfItems-1))/4)-9, 0);

		for (int i = 0; i < numberOfItems; i++)
		{
			GameObject marketEntry = (GameObject)Instantiate(marketTemplate, nextLocation, Quaternion.identity);
			marketEntry.transform.SetParent(transform, false);
			marketEntry.GetComponent<MarketLibraryUIChanger>().itemIdentifier = MarketManager.GetComponent<MarketLib>().itemIdentifiers[i];
			Debug.Log("Made a market library entry with identifier: " + MarketManager.GetComponent<MarketLib>().itemIdentifiers[i].ToString());

			if (column == 1) {
				nextLocation = new Vector3 (nextLocation.x + 50, nextLocation.y, 0);
				column = 2;
				itemArea.sizeDelta = new Vector2 (100, itemArea.sizeDelta.y);
			}
			else if (column == 2) {
				nextLocation = new Vector3 (nextLocation.x - 50, nextLocation.y - 36, 0);
				column = 1;
				row++;
				itemArea.sizeDelta = new Vector2 (100, itemArea.sizeDelta.y + 36);
			}


			marketEntry.name = MarketManager.GetComponent<MarketLib>().GetTitle(MarketManager.GetComponent<MarketLib>().itemIdentifiers[i]);
			marketEntry.SetActive(true);
			MarketManager.GetComponent<MarketLib>().entryArrayPopulation(marketEntry, i);
		}

        // If that player hasn't completed the tutorial,
        // make all other buttons except for the tutorial item
        // non-interactable.
        if(!SaveManager.Instance.CompletedFurnitureTutorial)
        {
            Debug.Log("Attempting to get item data");
            Debug.Log(this.gameObject.transform.childCount);
            for (int i = 1; i < this.gameObject.transform.childCount; i++)
            {
                nonTutorialFurniture.Add(this.gameObject.transform.GetChild(i).gameObject);
            }

            foreach (GameObject furniturePiece in nonTutorialFurniture)
            {
                furniturePiece.GetComponent<Button>().interactable = false;
            }

            // since icons are added dynamically, need to change scale of some of them at runtime so that they don't look weird
            nonTutorialFurniture[2].transform.GetChild(0).transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(-.006f, .008f);

            nonTutorialFurniture[25].transform.GetChild(0).transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(-.006f, .009f);
        }
        else
        {
            for (int i = 1; i < this.gameObject.transform.childCount; i++)
            {
                activeFurniture.Add(this.gameObject.transform.GetChild(i).gameObject);
            }

            // since icons are added dynamically, need to change scale of some of them at runtime so that they don't look weird

            activeFurniture[2].transform.GetChild(0).transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(-.006f, .008f);

            activeFurniture[25].transform.GetChild(0).transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(-.006f, .009f);
        }

	}

    private void OnEnable()
    {
        for (int i = 1; i < this.gameObject.transform.childCount; i++)
        {
            activeFurniture.Add(this.gameObject.transform.GetChild(i).gameObject);
        }

        // since icons are added dynamically, need to change scale of some of them at runtime so that they don't look weird

        activeFurniture[2].transform.GetChild(0).transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(-.006f, .008f);

        activeFurniture[25].transform.GetChild(0).transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(-.006f, .009f);

        StartCoroutine(WaitToSetObjectScale());
    }

    /// <summary>
    /// Used specifically to modify object icon scale for the furniture store
    /// Unity doesn't like it when you try to do that immediately, 
    /// so you have to wait a short time before doing so.
    /// </summary>
    private IEnumerator WaitToSetObjectScale()
    {
        yield return new WaitForSeconds(0.2f);

        activeFurniture[2].transform.GetChild(0).transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(-.006f, .008f);

        activeFurniture[25].transform.GetChild(0).transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(-.006f, .009f);
    }

}