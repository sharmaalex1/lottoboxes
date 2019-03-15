using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FurnitureTutorialHelper : MonoBehaviour, IPointerDownHandler
{
    private bool wasTappedFurniture = false;
    private bool canTapFurniture = false;

    public void OnPointerDown(PointerEventData data)
    {
        if(!SaveManager.Instance.CompletedFurnitureTutorial)
        {
            if (!wasTappedFurniture && canTapFurniture)
            {
                wasTappedFurniture = true;
            }
        }
    }

    public void ContinueFurnitureTutorial()
    {
        Debug.Log("Furniture tutorial finished?" + SaveManager.Instance.CompletedFurnitureTutorial);
        if(!SaveManager.Instance.CompletedFurnitureTutorial)
        {
            Debug.Log("Trying to send message to end wait cycle");
            MainGameTutorial.Instance.ContinueTutorial(EnableClickDetectShopButton);
        }
    }

    //Call Back function for the Main Game Tutorial to use
    private void EnableClickDetectShopButton()
    {
        canTapFurniture = true;
    }
}
