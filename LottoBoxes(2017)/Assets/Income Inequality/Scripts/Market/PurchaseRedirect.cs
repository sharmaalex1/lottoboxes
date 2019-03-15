using UnityEngine;
using System.Collections;

public class PurchaseRedirect : MonoBehaviour {

	public GameObject callingItem;

	public void callBackToItem()
	{
		callingItem.GetComponent<PurchaseButton> ().commitTransaction ();
	}
}
