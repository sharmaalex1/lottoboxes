using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisasterDamageManager : MonoBehaviour {

	public GameObject disasterManager;
	public GameObject damageUI;
	public GameObject damageUIText;
	public GameObject furnitureManagerObject;

	FurnitureManager furnitureManager;

	int damageRoll;

	void Start()
	{
		furnitureManager = furnitureManagerObject.GetComponent<FurnitureManager> ();
	}

	public void rollForDamage()
	{
		Debug.Log ("Rolling for damage...");
		int itemDeterminant;
		damageRoll = (int)Random.Range (1, 9);
		Debug.Log ("Rolled a: " + damageRoll.ToString ());
		if (damageRoll == 7 || damageRoll == 4 || damageRoll == 2) {
			itemDeterminant = Random.Range (0, 2);
			Debug.Log ("Picking outcome: " + itemDeterminant.ToString ());
			if(itemDeterminant == 0)
			damageFurniture (1, 0);
			else if (itemDeterminant == 1)
				damageFurniture (1, 1);
			else if (itemDeterminant == 2)
				damageFurniture (0, 1);
		}
	}

	void damageFurniture(int damageWindow, int damageWall)
	{
		bool wallDamaged = false;
		bool windowDamaged = false;
		if (damageWindow == 1) {
			Debug.Log("Window Damaged");
			SaveManager.Instance.isWindowFixed = false;
			windowDamaged = true;
		}
		if (damageWall == 1) {
			Debug.Log ("Wall Damaged");
			SaveManager.Instance.isWallFixed = false;
			SaveManager.Instance.wall01bought = false;
			SaveManager.Instance.wall02bought = false;
			SaveManager.Instance.wall03bought = false;
			SaveManager.Instance.wall04bought = false;
			SaveManager.Instance.wall05bought = false;
			SaveManager.Instance.wall06bought = false;
			SaveManager.Instance.wall07bought = false;
			wallDamaged = true;
		}
		damageUIToggle (windowDamaged, wallDamaged);
		furnitureManager.UpdateRoom ();
	}

	void damageUIToggle(bool window, bool wall)
	{
		
		if (window) {
			damageUIText.GetComponent<Text> ().text = "Your windows were damaged in the disaster!";
		}
		if (wall) {
			damageUIText.GetComponent<Text> ().text = "Your walls were damaged in the disaster!";
		}
		if (window && wall) {
			damageUIText.GetComponent<Text> ().text = "Both your windows and walls were damaged in the disaster!";
		}
		damageUI.SetActive (true);
	}
}
