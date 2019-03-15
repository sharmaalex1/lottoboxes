using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class FurnitureManager : MonoBehaviour
{

    [Header("Furniture Items -- Toggled on or off")]
    public GameObject Stand;
    //public GameObject Bookshelf; Currently no place for this
    public GameObject TV;
    public GameObject Aquarium;
    public GameObject Table;
    public GameObject Rug;
    public GameObject Lamp;
	public GameObject Desk;
	public GameObject fishBowl;
	public GameObject dogBed;
	public GameObject blinds;
	public GameObject punchingBag;
    [Header("Room Elements -- Images change")]
    public GameObject Window;
    public GameObject Floor;
    public GameObject Wall;
    public GameObject OriginalWall;
    [Header("Source Images for Room Elements")]
    public Sprite fixedWindow;
    public Sprite brokenWindow;
    //public Sprite newWindow;
    public Sprite wallVariant01;
    public Sprite wallVariant02;
    public Sprite wallVariant03;
	public Sprite wallVariant04;
	public Sprite wallVariant05;
	public Sprite wallVariant06;
	public Sprite wallVariant07;
	public Sprite defaultWall;
    public Sprite floorVariant01;
    public Sprite floorVariant02;
    public Sprite floorVariant03;
	public Sprite floorVariant04;
	public Sprite floorVariant05;
	public Sprite floorVariant06;
	public Sprite defaultFloor;


    void Awake()
    {
        UpdateRoom();
    }

    public void UpdateRoom()
    {
        #region Furniture Toggles
        if (SaveManager.Instance.item01Purchased)
            Stand.SetActive(true);
        else
            Stand.SetActive(false);

        if (SaveManager.Instance.item02Purchased)
            Rug.SetActive(true);
        else
            Rug.SetActive(false);

        if (SaveManager.Instance.item03Purchased)
            Lamp.SetActive(true);
        else
            Lamp.SetActive(false);

        if (SaveManager.Instance.item04Purchased)
            Table.SetActive(true);
        else
            Table.SetActive(false);

        if (SaveManager.Instance.item05Purchased)
            TV.SetActive(true);
        else
            TV.SetActive(false);

        if (SaveManager.Instance.item06Purchased)
            Aquarium.SetActive(true);
        else
            Aquarium.SetActive(false);
		
		if (SaveManager.Instance.item07Purchased)
			Desk.SetActive(true);
		else
			Desk.SetActive(false);

		if (SaveManager.Instance.item08Purchased)
			fishBowl.SetActive(true);
		else
			fishBowl.SetActive(false);

		if (SaveManager.Instance.item09Purchased)
			dogBed.SetActive(true);
		else
			dogBed.SetActive(false);

		if (SaveManager.Instance.item10Purchased)
			blinds.SetActive(true);
		else
			blinds.SetActive(false);

		/*if (SaveManager.Instance.item11Purchased)
			teaSet.SetActive(true);
		else
			teaSet.SetActive(false);*/

		if(!SaveManager.Instance.item06Purchased && !SaveManager.Instance.item05Purchased && !SaveManager.Instance.item07Purchased)
			SaveManager.Instance.itemOnStand = false;
			
			
        #endregion

        #region Room Element Changes
        if(SaveManager.Instance.isWallFixed)
        {
            OriginalWall.SetActive(false);
            Wall.SetActive(true);
        }

		if(!SaveManager.Instance.isWallFixed)
		{
			OriginalWall.SetActive(true);
			Wall.SetActive(false);
		}

        if (SaveManager.Instance.isWindowFixed)
            Window.GetComponent<Image>().sprite = fixedWindow;
        else
            Window.GetComponent<Image>().sprite = brokenWindow;

		if(SaveManager.Instance.floor01bought)
			Floor.GetComponent<Image>().sprite = floorVariant01;
		else if(SaveManager.Instance.floor02bought)
			Floor.GetComponent<Image>().sprite = floorVariant02;
		else if(SaveManager.Instance.floor03bought)
			Floor.GetComponent<Image>().sprite = floorVariant03;
		else if(SaveManager.Instance.floor04bought)
			Floor.GetComponent<Image>().sprite = floorVariant04;
		else if(SaveManager.Instance.floor05bought)
			Floor.GetComponent<Image>().sprite = floorVariant05;
		else if(SaveManager.Instance.floor06bought)
			Floor.GetComponent<Image>().sprite = floorVariant06;
		else
			Floor.GetComponent<Image>().sprite = defaultFloor;

		if(SaveManager.Instance.wall01bought)
			Wall.GetComponent<Image>().sprite = wallVariant01;
		else if(SaveManager.Instance.wall02bought)
			Wall.GetComponent<Image>().sprite = wallVariant02;
		else if(SaveManager.Instance.wall03bought)
			Wall.GetComponent<Image>().sprite = wallVariant03;
		else if(SaveManager.Instance.wall04bought)
			Wall.GetComponent<Image>().sprite = wallVariant04;
		else if(SaveManager.Instance.wall05bought)
			Wall.GetComponent<Image>().sprite = wallVariant05;
		else if(SaveManager.Instance.wall06bought)
			Wall.GetComponent<Image>().sprite = wallVariant06;
		else if(SaveManager.Instance.wall07bought)
			Wall.GetComponent<Image>().sprite = wallVariant07;
		else
			Wall.GetComponent<Image>().sprite = defaultWall;
        
        #endregion

    }

    // Update is called once per frame
    void Update()
    {

    }
}
